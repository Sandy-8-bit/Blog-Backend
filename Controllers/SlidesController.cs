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
}