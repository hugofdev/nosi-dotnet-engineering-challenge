using Microsoft.AspNetCore.Mvc;
using NOS.Engineering.Challenge.API.Models;
using NOS.Engineering.Challenge.Managers;
using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ContentController : Controller
{
    private readonly IContentsManager _manager;
    private readonly ILogger<ContentController> _logger;

    public ContentController(
        IContentsManager manager,
        ILogger<ContentController> logger)
    {
        _manager = manager;
        _logger = logger;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetManyContents()
    {
        _logger.LogInformation($"[GET] {nameof(GetManyContents)}: " +
            $"Attempting to retrieve all movies.");

        try
        {
            var contents = await _manager.GetManyContents().ConfigureAwait(false);
            if (!contents.Any())
            {
                _logger.LogInformation($"[GET] {nameof(GetManyContents)}: No movies could be found.");
                return NotFound();
            }

            _logger.LogInformation($"[GET] {nameof(GetManyContents)}: " +
                $"Successfully retrieved a total of {contents.Count()} movies.");
            return Ok(contents);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"[GET] {nameof(GetManyContents)}: An unexpected error has occurred.");
            return StatusCode(500);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetContent(Guid id)
    {
        _logger.LogInformation($"[GET] {nameof(GetContent)}: " +
            $"Attempting to retrive movie with ID {id}.");

        try
        {
            var content = await _manager.GetContent(id).ConfigureAwait(false);
            if (content == null)
            {
                _logger.LogInformation($"[GET] {nameof(GetContent)}: " +
                    $"Unable to find movie with ID '{id}'.");
                return NotFound();
            }

            _logger.LogInformation($"[GET] {nameof(GetContent)}: " +
                $"Successfully retrieved movie with ID '{id}'.");
            return Ok(content);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"[GET] {nameof(GetContent)}: An unexpected error has occurred.");
            return StatusCode(500);
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateContent(
        [FromBody] ContentInput content
        )
    {
        _logger.LogInformation($"[POST] {nameof(CreateContent)}: " +
            $"Attempting to create movie with Title {content.Title}.", content);

        try
        {
            var createdContent = await _manager.CreateContent(content.ToDto()).ConfigureAwait(false);
            if (createdContent == null)
            {
                _logger.LogWarning($"[POST] {nameof(CreateContent)}: " +
                    $"Failed to create movie with Title '{content.Title}'.");
                return Problem();
            }

            _logger.LogInformation($"[POST] {nameof(CreateContent)}: " +
                $"Successfully created movie with Title '{content.Title}'.");
            return Ok(createdContent);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"[POST] {nameof(CreateContent)}: An unexpected error has occurred.");
            return StatusCode(500);
        }
    }
    
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateContent(
        Guid id,
        [FromBody] ContentInput content
        )
    {
        _logger.LogInformation($"[PATCH] {nameof(UpdateContent)}: " +
            $"Attempting to update information from movie with ID {id}.", content);

        try
        {
            var updatedContent = await _manager.UpdateContent(id, content.ToDto()).ConfigureAwait(false);
            if (updatedContent == null)
            {
                _logger.LogInformation($"[PATCH] {nameof(UpdateContent)}: " +
                    $"Failed to update content for movie with ID '{id}' because it could not be found.");
                return NotFound();
            }

            _logger.LogInformation($"[PATCH] {nameof(UpdateContent)}: " +
                $"Successfully updated content for movie with ID '{id}'.");
            return Ok(updatedContent);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"[PATCH] {nameof(UpdateContent)}: An unexpected error has occurred.");
            return StatusCode(500);
        }
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteContent(
        Guid id
    )
    {
        _logger.LogInformation($"[DELETE] {nameof(DeleteContent)}: " +
            $"Attempting to delete movie with ID {id}.");

        try
        {
            var deletedId = await _manager.DeleteContent(id).ConfigureAwait(false);
            if (deletedId == Guid.Empty)
            {
                _logger.LogInformation($"[DELETE] {nameof(DeleteContent)}: " +
                    $"Failed to deleted movie with ID '{id}' because it could not be found.");
                return NotFound();
            }

            _logger.LogInformation($"[DELETE] {nameof(DeleteContent)}: " +
                $"Successfully deleted movie with ID '{id}'.");
            return Ok(deletedId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"[DELETE] {nameof(DeleteContent)}: An unexpected error has occurred.");
            return StatusCode(500);
        }
    }
    
    [HttpPost("{id}/genre")]
    public async Task<IActionResult> AddGenres(
        Guid id,
        [FromBody] IEnumerable<string> genre
    )
    {
        _logger.LogInformation($"[POST] {nameof(AddGenres)}: " +
            $"Attempting to add genres to movie with ID {id}.", genre);
        try
        {
            var content = await _manager.GetContent(id).ConfigureAwait(false);
            if (content == null)
            {
                _logger.LogInformation($"[POST] {nameof(AddGenres)}: " +
                    $"Failed to update genres for movie with ID '{id}' because it was not found.");
                return NotFound();
            }

            var newGenres = genre.Except(content.GenreList);
            if (newGenres.Count() == 0)
            {
                _logger.LogInformation($"[POST] {nameof(AddGenres)}: " +
                    $"No new genres were added to movie with ID {id}.");
                return Ok(content);
            }

            var updatedGenres = content.GenreList.Concat(genre).Distinct();
            var updatedContentDto = new ContentDto
            (
                title: null,
                subTitle: null,
                description: null,
                imageUrl: null,
                duration: null,
                startTime: null,
                endTime: null,
                genreList: updatedGenres
            );

            var updatedContent = await _manager.UpdateContent(id, updatedContentDto).ConfigureAwait(false);

            _logger.LogInformation($"[POST] {nameof(AddGenres)}: " +
                $"Successfully added [{string.Join(", ", newGenres)}] to movie with ID {id}.");
            return Ok(updatedContent);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"[POST] {nameof(AddGenres)}: An unexpected error has occurred.");
            return StatusCode(500);
        }
    }
    
    [HttpDelete("{id}/genre")]
    public async Task<IActionResult> RemoveGenres(
        Guid id,
        [FromBody] IEnumerable<string> genre
    )
    {
        _logger.LogInformation($"[DELETE] {nameof(RemoveGenres)}: " +
            $"Attempting to remove genres from movie with ID {id}.", genre);

        try
        {
            var content = await _manager.GetContent(id).ConfigureAwait(false);
            if (content == null)
            {
                _logger.LogInformation($"[DELETE] {nameof(RemoveGenres)}:" +
                    $"Failed to update genres for movie with ID '{id}' because it was not found.");
                return NotFound();
            }

            var removedGenres = content.GenreList.Where(item => genre.Contains(item));
            if (removedGenres.Count() == 0)
            {
                _logger.LogInformation($"[DELETE] {nameof(RemoveGenres)}: " +
                    $"No existing genres were removed from movie with ID {id}.");
                return Ok(content);
            }

            var updatedGenres = content.GenreList.Except(genre);
            var updatedContentDto = new ContentDto
            (
                title: null,
                subTitle: null,
                description: null,
                imageUrl: null,
                duration: null,
                startTime: null,
                endTime: null,
                genreList: updatedGenres
            );

            var updatedContent = await _manager.UpdateContent(id, updatedContentDto).ConfigureAwait(false);

            _logger.LogInformation($"[DELETE] {nameof(RemoveGenres)}: " +
                $"Successfully removed [{string.Join(", ", removedGenres)}] from movie with ID {id}.");
            return Ok(updatedContent);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"[DELETE] {nameof(RemoveGenres)}: An unexpected error has occurred.");
            return StatusCode(500);
        }
    }
}