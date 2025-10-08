using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Text;
using VinhSharingFiles.Application.Interfaces;
using VinhSharingFiles.Domain.SysVariables;

namespace VinhSharingFiles.Application.Services;

public class AmazonS3Service : IExternalService
{
    private readonly AwsConfiguration _awsConfig;
    // Or configure with specific region and credentials
    private readonly IAmazonS3 _s3Client;

    public AmazonS3Service(IConfiguration configuration)
    {
        _awsConfig = new AwsConfiguration
        {
            AccessKey = configuration.GetSection("AWS")["AccessKey"],
            SecretKey = configuration.GetSection("AWS")["SecretKey"],
            Region = configuration.GetSection("AWS")["Region"],
            BucketName = configuration.GetSection("AWS")["BucketName"]
        };
        //Verify AWS Credentials && contentLength
        var credentials = new BasicAWSCredentials(_awsConfig.AccessKey, _awsConfig.SecretKey);
        var config = new AmazonS3Config
        {
            RegionEndpoint = Amazon.RegionEndpoint.APSoutheast1 // Set your region
        };
        _s3Client = new AmazonS3Client(credentials, config);
    }

    public async Task DeleteFileAsync(string fileName)
    {
        //Call S3 to delete the file
        var deleteObjectRequest = new DeleteObjectRequest
        {
            BucketName = _awsConfig.BucketName,
            Key = fileName
        };

        await _s3Client.DeleteObjectAsync(deleteObjectRequest);
    }

    public async Task<Stream> DownloadFileAsync(string fileName)
    {        
        var request = new GetObjectRequest
        {
            BucketName = _awsConfig.BucketName,
            Key = fileName
        };

        using GetObjectResponse response = await _s3Client.GetObjectAsync(request);
        if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
        {
            var memoryStream = new MemoryStream();
            await response.ResponseStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0; // Reset stream position for reading
            return memoryStream;
        }
        else
        {
            throw new AmazonS3Exception($"Error downloading file: {response.HttpStatusCode}");
        }
    }

    public async Task<string> MultipartUploadAsync(string fileName, IFormFile file, Dictionary<string, string> customMetadata)
    {
        var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, _awsConfig.BucketName);
        if (!bucketExists)
        {
            await CreatingBucket();
        }

        // 1. Initiate the multipart upload
        InitiateMultipartUploadRequest initiateRequest = new()
        {
            BucketName = _awsConfig.BucketName, // "your-bucket-name"
            Key = fileName,
        };

        InitiateMultipartUploadResponse? initiateResponse = null;
        try
        {
            initiateResponse = await _s3Client.InitiateMultipartUploadAsync(initiateRequest);
            string uploadId = initiateResponse.UploadId;
            Console.WriteLine($"Initiated multipart upload with ID: {uploadId}");

            // 3. Upload the individual parts
            var uploadParts = await UploadPartsAsync(uploadId, file.FileName);

            // 4. Complete the multipart upload
            await CompleteUploadAsync(uploadId, file.FileName, uploadParts);

            Console.WriteLine("Multipart upload completed successfully.");
        }
        catch (AmazonS3Exception e)
        {
            Console.WriteLine($"S3 error: {e.Message}");
            if (initiateResponse != null)
            {
                // In case of an error, abort the upload to clean up resources
                await AbortUploadAsync(initiateResponse.UploadId, file.FileName);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Unknown error: {e.Message}");
        }

        return fileName;
    }

    public async Task<string> UploadFileAsync(string fileName, IFormFile file, Dictionary<string, string> customMetadata)
    {
        var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, _awsConfig.BucketName);
        if (!bucketExists)
        {
            await CreatingBucket();
        }

        await using var newMemoryStream = new MemoryStream();
        file.CopyTo(newMemoryStream);

        var uploadRequest = new TransferUtilityUploadRequest
        {
            InputStream = newMemoryStream,
            Key = fileName,
            BucketName = _awsConfig.BucketName, // "your-bucket-name"
            CannedACL = S3CannedACL.PublicRead
        };

        foreach (var entry in customMetadata)
        {
            uploadRequest.Metadata.Add(entry.Key, entry.Value);
        }

        var fileTransferUtility = new TransferUtility(_s3Client);
        await fileTransferUtility.UploadAsync(uploadRequest);

        return fileName;
    }

    public async Task<string> UploadTextFileAsync(string fileName, string contentToStore, Dictionary<string, string> customMetadata)
    {
        try
        {
            byte[] contentAsBytes = Encoding.UTF8.GetBytes(contentToStore ?? "");
            using MemoryStream ms = new(contentAsBytes);
            PutObjectRequest putRequest = new()
            {
                BucketName = _awsConfig.BucketName, // "your-bucket-name"
                Key = fileName,
                InputStream = ms
            };

            foreach (var entry in customMetadata)
            {
                putRequest.Metadata.Add(entry.Key, entry.Value);
            }

            PutObjectResponse response = await _s3Client.PutObjectAsync(putRequest);
            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new ArgumentException($"Failed to upload file '{fileName}' to bucket '{_awsConfig.BucketName}'. Status code: {response.HttpStatusCode}");
            }

            Console.WriteLine($"Successfully uploaded string to S3 object: s3://{_awsConfig.BucketName}/{fileName}");
        }
        catch (AmazonS3Exception e)
        {
            Console.WriteLine($"Error encountered on server. Message:'{e.Message}' when writing an object");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Unknown error encountered on client. Message:'{e.Message}' when writing an object");
        }

        return fileName;
    }

    private async Task CreatingBucket()
    {
        try
        {
            // Optionally, set other properties, e.g., CannedACL = S3CannedACL.Private
            var putBucketRequest = new PutBucketRequest
            {
                BucketName = _awsConfig.BucketName,
                //CannedACL = S3CannedACL.Private, // Set the access control list
                BucketRegion = _awsConfig.Region // Explicitly set the region
            };

            PutBucketResponse response = await _s3Client.PutBucketAsync(putBucketRequest);
            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new ArgumentException($"Failed to create bucket '{_awsConfig.BucketName}'. Status code: {response.HttpStatusCode}");
            }

            Console.WriteLine($"Bucket '{_awsConfig.BucketName}' created successfully.");
        }
        catch (AmazonS3Exception e)
        {
            Console.WriteLine($"Error creating bucket: {e.Message}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"An unexpected error occurred: {e.Message}");
        }
    }

    private const string FilePath = "path/to/your/large-file.bin";

    private async Task<PartETag[]> UploadPartsAsync(string uploadId, string fileName)
    {
        // 5 MB (minimum part size is 5 MB, except for the last part)
        const long partSize = FileVariables.PART_SIZE_UPLOADING_IN_MB * 1024 * 1024;

        List<PartETag> uploadParts = [];
        long filePosition = 0;
        int partNumber = 1;

        using (var fileStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
        {
            while (filePosition < fileStream.Length)
            {
                long bytesToRead = Math.Min(partSize, fileStream.Length - filePosition);
                var uploadPartRequest = new UploadPartRequest
                {
                    BucketName = _awsConfig.BucketName, // "your-bucket-name"
                    Key = fileName,
                    UploadId = uploadId,
                    PartNumber = partNumber,
                    FilePosition = filePosition,
                    InputStream = fileStream,
                    PartSize = bytesToRead
                };

                var uploadPartResponse = await _s3Client.UploadPartAsync(uploadPartRequest);
                uploadParts.Add(new PartETag(partNumber, uploadPartResponse.ETag));

                Console.WriteLine($"Uploaded part {partNumber}. ETag: {uploadPartResponse.ETag}");
                filePosition += bytesToRead;
                partNumber++;
            }
        }
        return [.. uploadParts];
    }

    private async Task CompleteUploadAsync(string uploadId, string fileName, PartETag[] partTags)
    {
        var completeRequest = new CompleteMultipartUploadRequest
        {
            BucketName = _awsConfig.BucketName, // "your-bucket-name"
            Key = fileName,
            UploadId = uploadId,
            PartETags = [.. partTags]
        };

        await _s3Client.CompleteMultipartUploadAsync(completeRequest);
    }

    private async Task AbortUploadAsync(string uploadId, string fileName)
    {
        var abortRequest = new AbortMultipartUploadRequest
        {
            BucketName = _awsConfig.BucketName, // "your-bucket-name"
            Key = fileName,
            UploadId = uploadId
        };

        await _s3Client.AbortMultipartUploadAsync(abortRequest);
        Console.WriteLine($"Aborted multipart upload with ID: {uploadId}");
    }
}
