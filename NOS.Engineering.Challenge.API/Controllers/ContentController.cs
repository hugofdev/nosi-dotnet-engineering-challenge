using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NOS.Engineering.Challenge.API.Extensions;
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
    private readonly IMemoryCache _cache;

    public ContentController(
        IContentsManager manager,
        ILogger<ContentController> logger,
        IMemoryCache cache)
    {
        _manager = manager;
        _logger = logger;
        _cache = cache;
    }

    [Obsolete("This endpoint is obsolete. Please use the new GetFilteredContents endpoint instead.")]
    [HttpGet]
    public async Task<IActionResult> GetManyContents(bool bypassCaching = false)
    {
        _logger.LogInformation($"[GET] {nameof(GetManyContents)}: " +
            $"Attempting to retrieve all movies.");

        var cacheKey = CachingExtensions.GetCacheKey(new List<string> { "Movies", "All" });
        IEnumerable<Content?> contents;

        try
        {
            if (!_cache.TryGetValue(cacheKey, out contents))
            {
                contents = await _manager.GetManyContents().ConfigureAwait(false);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(15));
                if (!bypassCaching) _cache.Set(cacheKey, contents, cacheEntryOptions);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"[GET] {nameof(GetManyContents)}: An unexpected error has occurred.");
            return StatusCode(500);
        }

        if (!contents.Any())
        {
            _logger.LogInformation($"[GET] {nameof(GetManyContents)}: No movies could be found.");
            return NotFound();
        }

        _logger.LogInformation($"[GET] {nameof(GetManyContents)}: " +
            $"Successfully retrieved a total of {contents.Count()} movies.");
        return Ok(contents);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetContent(Guid id, bool bypassCaching = false)
    {
        _logger.LogInformation($"[GET] {nameof(GetContent)}: " +
            $"Attempting to retrive movie with ID {id}.");

        var cacheKey = CachingExtensions.GetCacheKey(new List<string> { "Movies", id.ToString() });
        Content? content;

        try
        {
            if (!_cache.TryGetValue(cacheKey, out content))
            {
                content = await _manager.GetContent(id).ConfigureAwait(false);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(15));
                if (!bypassCaching) _cache.Set(cacheKey, content, cacheEntryOptions);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"[GET] {nameof(GetContent)}: An unexpected error has occurred.");
            return StatusCode(500);
        }

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

            // When any content is added, updated or deleted, the cache with all movies must be reset
            var allMoviesCacheKey = CachingExtensions.GetCacheKey(new List<string> { "Movies", "All" });
            _cache.Remove(allMoviesCacheKey);

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

            // If content is updated, cache from that content becomes outdated, so it should be removed
            var cacheKey = CachingExtensions.GetCacheKey(new List<string> { "Movies", id.ToString() });
            _cache.Remove(cacheKey);

            // When any content is added, updated or deleted, the cache with all movies must be reset
            var allMoviesCacheKey = CachingExtensions.GetCacheKey(new List<string> { "Movies", "All" });
            _cache.Remove(allMoviesCacheKey);

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

            // If content is deleted, cache from that content should be removed
            var cacheKey = CachingExtensions.GetCacheKey(new List<string> { "Movies", id.ToString() });
            _cache.Remove(cacheKey);

            // When any content is added, updated or deleted, the cache with all movies must be reset
            var allMoviesCacheKey = CachingExtensions.GetCacheKey(new List<string> { "Movies", "All" });
            _cache.Remove(allMoviesCacheKey);

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

            // If content is updated, cache from that content becomes outdated, so it should be removed
            var cacheKey = CachingExtensions.GetCacheKey(new List<string> { "Movies", id.ToString() });
            _cache.Remove(cacheKey);

            // When any content is added, updated or deleted, the cache with all movies must be reset
            var allMoviesCacheKey = CachingExtensions.GetCacheKey(new List<string> { "Movies", "All" });
            _cache.Remove(allMoviesCacheKey);

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

            // If content is updated, cache from that content becomes outdated, so it should be removed
            var cacheKey = CachingExtensions.GetCacheKey(new List<string> { "Movies", id.ToString() });
            _cache.Remove(cacheKey);

            // When any content is added, updated or deleted, the cache with all movies must be reset
            var allMoviesCacheKey = CachingExtensions.GetCacheKey(new List<string> { "Movies", "All" });
            _cache.Remove(allMoviesCacheKey);

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

    [HttpGet("filter")]
    public async Task<IActionResult> GetFilteredContents(
        string title = "",
        string genre = "",
        bool bypassCaching = false
    )
    {
        _logger.LogInformation($"[GET] {nameof(GetFilteredContents)}: " +
            $"Attempting to retrieve list of filtered movies.");

        var cacheKey = CachingExtensions.GetCacheKey(new List<string> { "Movies", "Filter" , $"{title}", $"{genre}" });
        IEnumerable<Content?> searchMatches;

        try
        {
            if (!_cache.TryGetValue(cacheKey, out searchMatches))
            {
                var allContents = await _manager.GetManyContents().ConfigureAwait(false);

                if (!allContents.Any())
                {
                    _logger.LogInformation($"[GET] {nameof(GetFilteredContents)}: No movies could be found.");
                    return NotFound();
                }

                if (string.IsNullOrEmpty(title) && string.IsNullOrEmpty(genre))
                {
                    searchMatches = allContents;
                }
                else if (string.IsNullOrEmpty(title))
                {
                    searchMatches = allContents.Where(content => content.GenreList.Contains(genre));
                }
                else if (string.IsNullOrEmpty(genre))
                {
                    searchMatches = allContents.Where(content => content.Title.ToLower().Contains(title.ToLower()));
                }
                else
                {
                    searchMatches = allContents
                      .Where(content => content.Title.ToLower().Contains(title.ToLower()))
                      .Where(content => content.GenreList.Contains(genre));
                }

                if (!searchMatches.Any())
                {
                    _logger.LogInformation($"[GET] {nameof(GetFilteredContents)}: No content matched the search filters.");
                    return NotFound();
                }

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(15));
                if (!bypassCaching) _cache.Set(cacheKey, searchMatches, cacheEntryOptions);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"[GET] {nameof(GetFilteredContents)}: An unexpected error has occurred.");
            return StatusCode(500);
        }

        _logger.LogInformation($"[GET] {nameof(GetFilteredContents)}: " +
            $"Successfully retrieved a total of {searchMatches.Count()} movies.");
        return Ok(searchMatches);
    }
}