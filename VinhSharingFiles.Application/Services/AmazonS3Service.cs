using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Text;
using VinhSharingFiles.Application.Interfaces;
using VinhSharingFiles.Domain.DTOs;
using VinhSharingFiles.Domain.Entities;
using VinhSharingFiles.Domain.SysVariables;

namespace VinhSharingFiles.Application.Services;

public class AmazonS3Service : ICloudService
{
    private readonly IConfiguration _configuration;
    private readonly AwsConfiguration _awsConfig;
    private readonly IFileSharingRepository _fileRepository;
    private readonly int MAX_LENGTH_STORE_TEXT_IN_DB = 2500;
    // Or configure with specific region and credentials
    private readonly IAmazonS3 _s3Client;

    public AmazonS3Service(IConfiguration configuration, IFileSharingRepository fileRepository) 
    {
        _configuration = configuration;

        _awsConfig = new AwsConfiguration
        {
            AccessKey = _configuration.GetSection("AWS")["AccessKey"],
            SecretKey = _configuration.GetSection("AWS")["SecretKey"],
            Region = _configuration.GetSection("AWS")["Region"],
            BucketName = _configuration.GetSection("AWS")["BucketName"]
        };

        _fileRepository = fileRepository;
        //Verify AWS Credentials && contentLength
        var credentials = new BasicAWSCredentials(_awsConfig.AccessKey, _awsConfig.SecretKey);
        var config = new AmazonS3Config
        {
            RegionEndpoint = Amazon.RegionEndpoint.APSoutheast1 // Set your region
        };
        _s3Client = new AmazonS3Client(credentials, config);
    }

    public async Task<IEnumerable<FileDto>> GetAllFilesByUserIdAsync(int userId)
    {
        var data = await _fileRepository.GetAllFiles(userId);
        return data.Select(x => new FileDto
        {
            Id = x.Id,
            FileSize = x.FileSize,
            FileName = x.FileName,
            FilePath = x.FilePath,
            ContentType = x.FileType,
            Description = x.Description,
            CreatedAt = x.CreatedAt,
        });
    }

    public async Task<FileObjectDto> PreviewFileAsync(int fileId)
    {
        // Logic to get the file name from your database using the fileId
        var fileRecord = await _fileRepository.GetFileByIdAsync(fileId) ?? throw new Exception("File not found in the database.");

        if (fileRecord.FileName == "STORE_TEXT_IN_DB")
        {
            return new FileObjectDto
            {                    
                ContentType = "STORE_TEXT_IN_DB",
                FileId = fileId,
                Name = "STORE_TEXT_IN_DB", //This is the display file name
                Description = fileRecord.Description // This is the text stored
            };
        }

        // Use display name if available, otherwise use the original file name
        string fileName = fileRecord.DisplayName ?? fileRecord.FileName;

        try
        {
            var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, _awsConfig.BucketName);
            if (!bucketExists)
            {
                await CreatingBucket();
            }

            // Fetch the file from S3
            var s3Object = await _s3Client.GetObjectAsync(_awsConfig.BucketName, fileRecord.FileName);
            return new FileObjectDto
            {
                ContentType = fileRecord.FileType ?? s3Object.Headers.ContentType,
                FileId = fileId,
                Name = fileName, //This is the display file name
                Data = s3Object.ResponseStream // This is the stream containing the file data
            };
        }
        catch (AmazonS3Exception e)
        {
            // Handle S3 specific exceptions (e.g., file not found, access denied)
            Console.WriteLine($"Error getting object: {e.Message}");
            throw;
        }
        catch (Exception e)
        {
            // Handle other general exceptions
            Console.WriteLine($"Error: {e.Message}");
            throw;
        }
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

    public async Task<FileObjectDto> DownloadFileAsync(int fileId)
    {
        // Logic to get the file name from your database using the fileId
        var fileRecord = await _fileRepository.GetFileByIdAsync(fileId) ?? throw new Exception("File not found in the database.");
        if (fileRecord.FileName == "STORE_TEXT_IN_DB")
        {
            return new FileObjectDto
            {
                ContentType = "STORE_TEXT_IN_DB",
                FileId = fileId,
                Name = "STORE_TEXT_IN_DB", //This is the display file name
                Description = fileRecord.Description // This is the text stored
            };
        }

        // Use display name if available, otherwise use the original file name
        string fileName = fileRecord.DisplayName ?? fileRecord.FileName;

        try
        {                
            // Specify your bucket name and the key (file name)
            GetObjectRequest request = new()
            {
                BucketName = _awsConfig.BucketName,
                Key = fileRecord.FileName //This is the original file name
            };

            GetObjectResponse response = await _s3Client.GetObjectAsync(request);

            if (fileRecord.AutoDelete)
            {
                // If the file is marked as delete after download, remove it from the database after fetching
                await _fileRepository.DeleteFileByIdAsync(fileId);
            }

            return new FileObjectDto
            {
                ContentType = fileRecord.FileType ?? response.Headers["Content-Type"],
                FileId = fileId,
                Name = fileName, //This is the display file name
                Data = response.ResponseStream // This is the stream containing the file data
            };
        }
        catch (AmazonS3Exception e)
        {
            // Handle S3 specific exceptions (e.g., file not found, access denied)
            Console.WriteLine($"Error getting object: {e.Message}");
            throw;
        }
        catch (Exception e)
        {
            // Handle other general exceptions
            Console.WriteLine($"Error: {e.Message}");
            throw;
        }
    }

    public async Task<int> UploadFileAsync(int userId, IFormFile file, bool? deleteAfterDownload)
    {
        ////Add metadata to file 
        //string newDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

        //// Initiate the upload.
        //InitiateMultipartUploadResponse initResponse = await s3Client.InitiateMultipartUploadAsync(initiateRequest);
        //int uploadmb = 5;

        //// Upload parts.
        //long contentLength = new FileInfo(zippath).Length;
        //long partSize = uploadmb * (long)Math.Pow(2, 20); // 5 MB  
        
        await using var newMemoryStream = new MemoryStream();
        file.CopyTo(newMemoryStream);

        var uploadRequest = new TransferUtilityUploadRequest
        {
            InputStream = newMemoryStream,
            Key = file.FileName,
            BucketName = _awsConfig.BucketName, // "your-bucket-name"
            CannedACL = S3CannedACL.PublicRead
        };

        var fileTransferUtility = new TransferUtility(_s3Client);
        await fileTransferUtility.UploadAsync(uploadRequest);

        return await SaveFileToDatabase(userId, file.FileName, file.ContentType, file.Length, deleteAfterDownload);            
    }

    //Create a text file and upload
    public async Task<int> UploadTextFileAsync(int userId, string contentToStore, bool? deleteAfterAccessed)
    {
        if ((contentToStore ?? "").Length < MAX_LENGTH_STORE_TEXT_IN_DB)
        {
            return await SaveTextToDatabase(userId, contentToStore, deleteAfterAccessed);
        }

        try
        {         
            byte[] contentAsBytes = Encoding.UTF8.GetBytes(contentToStore ?? "");
            string fileName = $"data_{userId:X2}_{DateTime.UtcNow.Ticks:X2}.txt";
            using MemoryStream ms = new(contentAsBytes);
            PutObjectRequest putRequest = new()
            {
                BucketName = _awsConfig.BucketName, // "your-bucket-name"
                Key = fileName,
                InputStream = ms
            };

            PutObjectResponse response = await _s3Client.PutObjectAsync(putRequest);
            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new ArgumentException($"Failed to upload file '{fileName}' to bucket '{_awsConfig.BucketName}'. Status code: {response.HttpStatusCode}");
            }

            Console.WriteLine($"Successfully uploaded string to S3 object: s3://{_awsConfig.BucketName}/{fileName}");

            // You can optionally set the content type, e.g., "text/plain".
            return await SaveFileToDatabase(userId, fileName, "text/plain", 0, deleteAfterAccessed);
        }
        catch (AmazonS3Exception e)
        {
            Console.WriteLine($"Error encountered on server. Message:'{e.Message}' when writing an object");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Unknown error encountered on client. Message:'{e.Message}' when writing an object");
        }

        return 0;
    }

    private async Task<int> SaveTextToDatabase(int userId, string? contentToStore, bool? deleteAfterAccessed)
    {
        // Save file info to database
        FileSharing fileInfo = new()
        {
            Id = 0,
            UserId = userId,
            DisplayName = "STORE_TEXT_IN_DB",
            FileName = "STORE_TEXT_IN_DB",
            FileType = "",
            FilePath = "",
            FileSize = 0,
            Description = contentToStore,
            CreatedAt = DateTime.UtcNow,
            AutoDelete = deleteAfterAccessed ?? false
        };

        var fileId = await _fileRepository.AddFileAsync(fileInfo);

        // Return the file ID or any other relevant information
        return fileId;
    }

    private async Task<int> SaveFileToDatabase(int userId, string fileName, 
        string contentType, long fileSize, bool? deleteAfterDownload)
    {
        // Save file info to database
        FileSharing fileInfo = new()
        {
            Id = 0,
            UserId = userId,
            DisplayName = fileName,
            FileType = contentType,
            FilePath = fileName,
            FileName = fileName,
            FileSize = fileSize,
            CreatedAt = DateTime.UtcNow,
            AutoDelete = deleteAfterDownload ?? false
        };

        var fileId = await _fileRepository.AddFileAsync(fileInfo);

        // Return the file ID or any other relevant information
        return fileId;
    }

}
