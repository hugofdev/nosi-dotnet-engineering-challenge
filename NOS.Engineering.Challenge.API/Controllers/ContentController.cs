using System.Net;
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
    public ContentController(IContentsManager manager)
    {
        _manager = manager;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetManyContents()
    {
        var contents = await _manager.GetManyContents().ConfigureAwait(false);

        if (!contents.Any())
            return NotFound();
        
        return Ok(contents);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetContent(Guid id)
    {
        var content = await _manager.GetContent(id).ConfigureAwait(false);

        if (content == null)
            return NotFound();
        
        return Ok(content);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateContent(
        [FromBody] ContentInput content
        )
    {
        var createdContent = await _manager.CreateContent(content.ToDto()).ConfigureAwait(false);

        return createdContent == null ? Problem() : Ok(createdContent);
    }
    
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateContent(
        Guid id,
        [FromBody] ContentInput content
        )
    {
        var updatedContent = await _manager.UpdateContent(id, content.ToDto()).ConfigureAwait(false);

        return updatedContent == null ? NotFound() : Ok(updatedContent);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteContent(
        Guid id
    )
    {
        var deletedId = await _manager.DeleteContent(id).ConfigureAwait(false);
        return Ok(deletedId);
    }
    
    [HttpPost("{id}/genre")]
    public async Task<IActionResult> AddGenres(
        Guid id,
        [FromBody] IEnumerable<string> genre
    )
    {
        var content = await _manager.GetContent(id).ConfigureAwait(false);
        if (content == null)
        {
            return NotFound();
        }

        var updatedGenres = content.GenreList.ToList().Concat(genre).Distinct();
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

        return Ok(updatedContent);
    }
    
    [HttpDelete("{id}/genre")]
    public async Task<IActionResult> RemoveGenres(
        Guid id,
        [FromBody] IEnumerable<string> genre
    )
    {
        var content = await _manager.GetContent(id).ConfigureAwait(false);
        if (content == null)
        {
            return NotFound();
        }

        var updatedGenres = content.GenreList.ToList().Except(genre);
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

        return Ok(updatedContent);
    }
}