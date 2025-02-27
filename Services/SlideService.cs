using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;

public class SlideService
{
    private readonly IMongoCollection<Slide> _slides;

    public SlideService(IMongoDatabase database)
    {
        _slides = database.GetCollection<Slide>("Slides");
    }

    // Fetch all slides
    public async Task<List<Slide>> GetSlidesAsync()
    {
        try
        {
            var slides = await _slides.Find(_ => true).ToListAsync();
            if (slides == null || slides.Count == 0)
            {
                Console.WriteLine("No slides found in the database.");
            }
            return slides!;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching slides: {ex.Message}");
            return new List<Slide>();
        }
    }


    // Fetch slide by ID
    public async Task<Slide?> GetSlideByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId)) return null;
        return await _slides.Find(s => s.Id == objectId.ToString()).FirstOrDefaultAsync();
    }

    // Fetch slides by category
    public async Task<List<Slide>> GetSlidesByCategoryAsync(string category) =>
        await _slides.Find(s => s.Category == category).ToListAsync();

    // Create a new slide
    public async Task CreateSlideAsync(Slide slide)
    {
        if (string.IsNullOrWhiteSpace(slide.Title) || string.IsNullOrWhiteSpace(slide.ImageUrl))
            throw new ArgumentException("Title and ImageUrl are required.");

        slide.Id ??= ObjectId.GenerateNewId().ToString();
        slide.Likes = new List<string>();  // Initialize likes list
        slide.Comments = new List<Comment>(); // Initialize comments list

        await _slides.InsertOneAsync(slide);
    }

    // Update an existing slide
    public async Task<bool> UpdateSlideAsync(Slide slide)
    {
        var result = await _slides.ReplaceOneAsync(s => s.Id == slide.Id, slide);
        return result.ModifiedCount > 0;
    }

    // Delete a slide
    public async Task<bool> DeleteSlideAsync(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId)) return false;
        var result = await _slides.DeleteOneAsync(s => s.Id == objectId.ToString());
        return result.DeletedCount > 0;
    }

    // Like a slide (Ensure a user can only like once)
    public async Task<bool> LikeSlideAsync(string slideId, string username)
    {
        var slide = await GetSlideByIdAsync(slideId);
        if (slide == null) return false;

        // Prevent duplicate likes
        if (slide.Likes!.Contains(username)) return false;

        slide.Likes.Add(username);
        var update = Builders<Slide>.Update.Set(s => s.Likes, slide.Likes);
        var result = await _slides.UpdateOneAsync(s => s.Id == slide.Id, update);

        return result.ModifiedCount > 0;
    }

    // Unlike a slide (optional)
    public async Task<bool> UnlikeSlideAsync(string slideId, string username)
    {
        var slide = await GetSlideByIdAsync(slideId);
        if (slide == null) return false;

        if (!slide.Likes!.Contains(username)) return false;

        slide.Likes.Remove(username);
        var update = Builders<Slide>.Update.Set(s => s.Likes, slide.Likes);
        var result = await _slides.UpdateOneAsync(s => s.Id == slide.Id, update);

        return result.ModifiedCount > 0;
    }

    // Add a comment to a slide
    public async Task<bool> AddCommentAsync(string slideId, string username, string commentText)
    {
        var slide = await GetSlideByIdAsync(slideId);
        if (slide == null) return false;

        var comment = new Comment
        {
            Username = username,
            Text = commentText,
            Date = DateTime.UtcNow
        };

        slide.Comments!.Add(comment);
        var update = Builders<Slide>.Update.Set(s => s.Comments, slide.Comments);
        var result = await _slides.UpdateOneAsync(s => s.Id == slide.Id, update);

        return result.ModifiedCount > 0;
    }
}
