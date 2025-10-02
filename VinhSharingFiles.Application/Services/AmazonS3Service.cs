using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using VinhSharingFiles.Application.Interfaces;
using VinhSharingFiles.Domain.DTOs;
using VinhSharingFiles.Domain.Entities;
using VinhSharingFiles.Domain.SysVariables;

namespace VinhSharingFiles.Application.Services
{
    public class AmazonS3Service : ICloudService
    {
        private IConfiguration _configuration;
        private AwsConfiguration _awsConfig;
        private readonly IFileSharingRepository _fileRepository;

        // Or configure with specific region and credentials
        private IAmazonS3 _s3Client;

        public AmazonS3Service(IConfiguration configuration, IAmazonS3 s3Client, IFileSharingRepository fileRepository) 
        {
            _configuration = configuration;
            _s3Client = s3Client;
            _awsConfig = new AwsConfiguration
            {
                AWSAccessKey = _configuration.GetSection("AWS")["AccessKey"],
                AWSSecretKey = _configuration.GetSection("AWS")["SecretKey"],
                AWSRegion = _configuration.GetSection("AWS")["Region"],
                AWSBucketName = _configuration.GetSection("AWS")["BucketName"]
            };

            _fileRepository = fileRepository;
        }

        public async Task<FileObjectDto> DownloadFileAsync(int fileId)
        {
            // Logic to get the file name from your database using the fileId
            var fileRecord = await _fileRepository.GetFileByIdAsync(fileId) ?? throw new Exception("File not found in the database.");

            // Use display name if available, otherwise use the original file name
            string fileName = fileRecord.DisplayName ?? fileRecord.FileName;

            try
            {                
                // Specify your bucket name and the key (file name)
                GetObjectRequest request = new()
                {
                    BucketName = _awsConfig.AWSBucketName,
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
                    ContentType = response.Headers["Content-Type"],
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

            //Verify AWS Credentials && contentLength
            var credentials = new BasicAWSCredentials(_awsConfig.AWSAccessKey, _awsConfig.AWSSecretKey);
            var config = new AmazonS3Config
            {
                RegionEndpoint = Amazon.RegionEndpoint.APSoutheast1 // Set your region
            };
            using var client = new AmazonS3Client(credentials, config);
            await using var newMemoryStream = new MemoryStream();
            file.CopyTo(newMemoryStream);

            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = newMemoryStream,
                Key = file.FileName,
                BucketName = _awsConfig.AWSBucketName, // "your-bucket-name"
                CannedACL = S3CannedACL.PublicRead
            };

            var fileTransferUtility = new TransferUtility(client);
            await fileTransferUtility.UploadAsync(uploadRequest);

            // Save file info to database
            FileSharing fileInfo = new()
            {
                Id = 0,
                UserId = userId,
                DisplayName = file.FileName,
                FileType = file.ContentType,
                FilePath = file.FileName,
                FileName = file.FileName,
                FileSize = file.Length,
                CreatedAt = DateTime.UtcNow,
                AutoDelete = deleteAfterDownload ?? false
            };
                       
            var fileId = await _fileRepository.AddFileAsync(fileInfo);

            // Return the file ID or any other relevant information
            return fileId;
        }

        public Task<int> UploadTextFileAsync(int userId, string textData, bool? deleteAfterAccessed)
        {
            ///Create a text file and upload
            throw new NotImplementedException();
        }
    }
}
