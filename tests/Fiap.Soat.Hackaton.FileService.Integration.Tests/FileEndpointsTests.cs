using System.Net;
using System.Net.Http.Headers;

namespace Fiap.Soat.Hackaton.FileService.Integration.Tests;

public sealed class FileEndpointsTests
{
    [Fact]
    public async Task UploadFile_ReturnsCreated_AndPublishesDomainMessage()
    {
        using var factory = new FileServiceApiFactory();
        var client = factory.CreateClient();

        using var content = CreateMultipartFileContent("sample.pdf", "application/pdf", "test pdf content");

        var response = await client.PostAsync("/files/upload", content);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(1, factory.FileStorage.UploadCalls);
        Assert.Single(factory.FileRepository.InsertedFiles);
        Assert.Single(factory.MessagePublisher.PublishedMessages);

        var publishedMessage = factory.MessagePublisher.PublishedMessages[0];
        Assert.StartsWith("file.uploaded.", publishedMessage.RoutingKey, StringComparison.Ordinal);

        var uploadedFile = factory.FileStorage.LastUploadedFile;
        Assert.NotNull(uploadedFile);
        Assert.Equal("sample.pdf", uploadedFile!.FileName);
        Assert.Equal("application/pdf", uploadedFile.ContentType);
        Assert.Equal(System.Text.Encoding.UTF8.GetBytes("test pdf content"), uploadedFile.Content);
    }

    [Fact]
    public async Task UploadFile_WithUnsupportedContentType_ReturnsBadRequest_AndSkipsDependencies()
    {
        using var factory = new FileServiceApiFactory();
        var client = factory.CreateClient();

        using var content = CreateMultipartFileContent("sample.txt", "text/plain", "plain text");

        var response = await client.PostAsync("/files/upload", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(0, factory.FileStorage.UploadCalls);
        Assert.Empty(factory.FileRepository.InsertedFiles);
        Assert.Empty(factory.MessagePublisher.PublishedMessages);
    }

    private static MultipartFormDataContent CreateMultipartFileContent(string fileName, string contentType, string fileContent)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(fileContent);
        var streamContent = new StreamContent(new MemoryStream(bytes));
        streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);

        var formData = new MultipartFormDataContent();
        formData.Add(streamContent, "file", fileName);
        return formData;
    }
}
