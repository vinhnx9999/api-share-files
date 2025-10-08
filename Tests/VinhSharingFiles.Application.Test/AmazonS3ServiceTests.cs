using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using Moq;
using VinhSharingFiles.Application.Services;

namespace VinhSharingFiles.Application.Test;

public class AmazonS3ServiceTests
{
    // Implement tests for AmazonS3Service here
    private readonly AmazonS3Service _service;
    private readonly Mock<IAmazonS3> _mockS3Client;

    public AmazonS3ServiceTests()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            { "MySettingKey", "InMemoryValue"},
            { "AWS:AccessKey", "AccessKey"},
            { "AWS:SecretKey", "SecretKey"},
            { "AWS:Region", "Region"},
            { "AWS:BucketName", "BucketName"}
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        // Initialize the service with necessary dependencies
        _service = new AmazonS3Service(configuration);
        _mockS3Client = new Mock<IAmazonS3>();
    }

    [Fact]
    public async Task DownloadFile_ReturnsItemAsync()
    {
        // Arrange
        string fileName = "testfile.txt";
        string bucketName = "test-bucket";
        string expectedContent = "Hello, S3!";

        _mockS3Client.Setup(s => s.GetObjectAsync(It.IsAny<GetObjectRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetObjectResponse
            {
                BucketName = bucketName,
                Key = fileName,
                ResponseStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(expectedContent)),
                HttpStatusCode = System.Net.HttpStatusCode.OK
            });

        // Act
        //await _service.DownloadFileAsync(fileName);

        // Assert
        //_mockS3Client.Verify(s => s.PutObjectAsync(
        //        It.Is<PutObjectRequest>(req =>
        //            req.BucketName == bucketName &&
        //            req.Key == fileName),
        //        It.IsAny<CancellationToken>()), Times.Once);
    }
}
