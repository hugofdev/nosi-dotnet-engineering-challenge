using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using NOS.Engineering.Challenge.API.Controllers;
using NOS.Engineering.Challenge.API.Tests.Mock;
using NOS.Engineering.Challenge.Managers;
using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.API.Tests.Controllers;

public class ContentControllerTests
{
    private Mock<IContentsManager> _mockManager;
    private Mock<ILogger<ContentController>> _mockLogger;
    private Mock<IMemoryCache> _mockCache;
    private ContentController _controller;

    public ContentControllerTests()
    {
        _mockManager = new Mock<IContentsManager>();
        _mockLogger = new Mock<ILogger<ContentController>>();
        _mockCache = new Mock<IMemoryCache>();
        _controller = new ContentController(_mockManager.Object, _mockLogger.Object, _mockCache.Object);
    }

    [Fact]
    public async Task GetManyContents_ReturnsOkResult()
    {
        var mockResponse = MockData.MockGetManyContentsResponse;
        _mockManager.Setup(manager => manager.GetManyContents()).ReturnsAsync(mockResponse);

        var result = await _controller.GetManyContents(true);

        Assert.IsType<OkObjectResult>(result);
        var contentResult = (result as OkObjectResult)?.Value as IEnumerable<Content>;
        Assert.NotNull(contentResult);
        Assert.NotEmpty(contentResult);
        Assert.Equal(mockResponse, contentResult);
    }

    [Fact]
    public async Task GetContent_ReturnsOkResult()
    {
        var mockId = Guid.NewGuid();
        var mockResponse = MockData.MockGetContentResponse(mockId);
        _mockManager.Setup(manager => manager.GetContent(mockId)).ReturnsAsync(mockResponse);

        var result = await _controller.GetContent(mockId, true);

        Assert.IsType<OkObjectResult>(result);
        var contentResult = (result as OkObjectResult)?.Value as Content;
        Assert.NotNull(contentResult);
        Assert.Equal(mockId, contentResult.Id);
    }

    [Fact]
    public async Task CreateContent_ReturnsOkResult()
    {
        var mockRequest = MockData.MockCreateContentRequest;
        var mockResponse = MockData.MockCreateContentResponse;
        _mockManager.Setup(manager => manager.CreateContent(It.IsAny<ContentDto>())).ReturnsAsync(mockResponse);

        var result = await _controller.CreateContent(mockRequest);

        Assert.IsType<OkObjectResult>(result);
        var createdResult = (result as OkObjectResult)?.Value as Content;
        Assert.NotNull(createdResult);
        Assert.Equal(mockResponse.Title, createdResult.Title);
    }

    [Fact]
    public async Task UpdateContent_ReturnsOkResult()
    {
        var mockId = Guid.NewGuid();
        var originalContent = MockData.MockUpdateContentOriginal(mockId);
        var mockRequest = MockData.MockUpdateContentRequest;
        var updatedContent = MockData.MockUpdateContentUpdated(mockId);
        _mockManager.Setup(manager => manager.UpdateContent(mockId, It.IsAny<ContentDto>())).ReturnsAsync(updatedContent);

        var result = await _controller.UpdateContent(mockId, mockRequest);

        Assert.IsType<OkObjectResult>(result);
        var updatedResult = (result as OkObjectResult)?.Value as Content;
        Assert.NotNull(updatedResult); // Check for not null response
        Assert.Equal(mockId, updatedResult.Id); // Check if correct content was updated
        Assert.Equal(originalContent.Title, updatedResult.Title); // Check if value is unchanged
        Assert.Equal(mockRequest.Description, updatedResult.Description); // Check if value has changed
        Assert.Equal(mockRequest.ImageUrl, updatedResult.ImageUrl); // Check if value has changed
    }

    [Fact]
    public async Task DeleteContent_ReturnsOkResult()
    {
        var mockId = Guid.NewGuid();
        _mockManager.Setup(manager => manager.DeleteContent(mockId)).ReturnsAsync(mockId);

        var result = await _controller.DeleteContent(mockId);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var idResult = result as OkObjectResult;
        Assert.NotNull(idResult);
        Assert.Equal(mockId, (Guid)idResult.Value);

    }
    
    [Fact]
    public async Task AddGenres_ReturnsOkResult()
    {
        var mockId = Guid.NewGuid();
        var originalContent = MockData.MockAddGenresOriginal(mockId);
        var mockRequest = MockData.MockAddGenresRequest;
        var updatedContent = MockData.MockAddGenresUpdated(mockId);
        _mockManager.Setup(manager => manager.GetContent(mockId)).ReturnsAsync(originalContent);
        _mockManager.Setup(manager => manager.UpdateContent(It.IsAny<Guid>(), It.IsAny<ContentDto>())).ReturnsAsync(updatedContent);

        var result = await _controller.AddGenres(mockId, mockRequest);

        Assert.IsType<OkObjectResult>(result);
        var updatedResult = (result as OkObjectResult)?.Value as Content;
        Assert.NotNull(updatedResult); // Check for not null response
        Assert.Equal(mockId, updatedResult.Id); // Check if correct content was updated
        var expectedGenres = originalContent.GenreList.Concat(mockRequest).Distinct();
        Assert.Equal(expectedGenres, updatedContent.GenreList); // Chek if genres are what is expected
    }

    [Fact]
    public async Task RemoveGenres_ReturnsOkResult()
    {
        var mockId = Guid.NewGuid();
        var originalContent = MockData.MockRemoveGenresOriginal(mockId);
        var mockRequest = MockData.MockRemoveGenresRequest;
        var updatedContent = MockData.MockRemoveGenresUpdated(mockId);
        _mockManager.Setup(manager => manager.GetContent(mockId)).ReturnsAsync(originalContent);
        _mockManager.Setup(manager => manager.UpdateContent(It.IsAny<Guid>(), It.IsAny<ContentDto>())).ReturnsAsync(updatedContent);

        var result = await _controller.RemoveGenres(mockId, mockRequest);

        Assert.IsType<OkObjectResult>(result);
        var updatedResult = (result as OkObjectResult)?.Value as Content;
        Assert.NotNull(updatedResult); // Check for not null response
        Assert.Equal(mockId, updatedResult.Id); // Check if correct content was updated
        var expectedGenres = originalContent.GenreList.Except(mockRequest);
        Assert.Equal(expectedGenres, updatedContent.GenreList); // Chek if genres are what is expected
    }

    [Fact]
    public async Task GetFilteredContents_ReturnsOkResult()
    {
        var responseId = Guid.NewGuid();

        var allContents = MockData.MockGetFilteredContentsAllContents(responseId);
        var titleFilter = MockData.MockGetFilteredContentsTitleFilter;
        var genreFilter = MockData.MockGetFilteredContentsGenreFilter;
        var expectedResponse = MockData.MockGetFilteredContentsResponse(responseId);
        _mockManager.Setup(manager => manager.GetManyContents()).ReturnsAsync(allContents);

        var result = await _controller.GetFilteredContents(titleFilter, genreFilter, true);

        Assert.IsType<OkObjectResult>(result);
        var contentResult = (result as OkObjectResult)?.Value as IEnumerable<Content?>;
        Assert.NotNull(contentResult);
        Assert.NotEmpty(contentResult);
        Assert.Equal(expectedResponse.ElementAt(0).Id, contentResult.ToList().ElementAt(0).Id);
    }
}