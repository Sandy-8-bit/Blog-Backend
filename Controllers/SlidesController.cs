using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/slider")]
[ApiController]
public class SlideController : ControllerBase
{
    private readonly SlideService _slideService;

    public SlideController(SlideService slideService)
    {
        _slideService = slideService;
    }

    // Get all slides
    [HttpGet]
    public async Task<ActionResult<List<Slide>>> GetSlides()
    {
        var slides = await _slideService.GetSlidesAsync();
        return slides ?? new List<Slide>();
    }

    // Get a single slide by ID
    [HttpGet("{id}")]
    public async Task<ActionResult<Slide>> GetSlideById(string id)
    {
        var slide = await _slideService.GetSlideByIdAsync(id);
        return slide is null ? NotFound() : Ok(slide);
    }

    // Get slides by category
    [HttpGet("category/{category}")]
    public async Task<ActionResult<List<Slide>>> GetSlidesByCategory(string category)
    {
        var slides = await _slideService.GetSlidesByCategoryAsync(category);
        return slides ?? new List<Slide>();
    }

    // Create a new slide
    [HttpPost]
    public async Task<ActionResult<Slide>> CreateSlide([FromBody] Slide slide)
    {
        await _slideService.CreateSlideAsync(slide);
        return CreatedAtAction(nameof(GetSlideById), new { id = slide.Id }, slide);
    }

    // Like a slide
    [HttpPost("{id}/like")]
    public async Task<IActionResult> LikeSlide(string id, [FromBody] LikeRequest request)
    {
        try
        {
            var success = await _slideService.LikeSlideAsync(id, request.Username!);
            return success ? Ok() : BadRequest("Error processing like/unlike request.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in LikeSlide API: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    // Unlike a slide
    [HttpPost("{id}/unlike")]
    public async Task<IActionResult> UnlikeSlide(string id, [FromBody] LikeRequest request)
    {
        var success = await _slideService.UnlikeSlideAsync(id, request.Username!);
        return success ? Ok() : BadRequest("User has not liked this slide.");
    }

    // Add a comment to a slide
    [HttpPost("{id}/comment")]
    public async Task<IActionResult> CommentSlide(string id, [FromBody] CommentRequest request)
    {
        var success = await _slideService.AddCommentAsync(id, request.Username!, request.Comment!);
        return success ? Ok() : BadRequest("Failed to add comment.");
    }

    // Update slide details
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSlide(string id, [FromBody] Slide slide)
    {
        if (id != slide.Id) return BadRequest("Slide ID mismatch.");
        var success = await _slideService.UpdateSlideAsync(slide);
        return success ? Ok() : NotFound();
    }

    // Delete a slide
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSlide(string id)
    {
        var success = await _slideService.DeleteSlideAsync(id);
        return success ? Ok() : NotFound();
    }
}
