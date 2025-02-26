using Backend.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

public class SlideService
{
    private readonly IMongoCollection<Slide> _slidesCollection;

    public SlideService(IOptions<MongoDbSettings> mongoSettings)
    {
        var mongoClient = new MongoClient(mongoSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(mongoSettings.Value.DatabaseName);
        _slidesCollection = mongoDatabase.GetCollection<Slide>(mongoSettings.Value.SlidesCollectionName);
    }

    // Get all slides
    public async Task<List<Slide>> GetSlidesAsync() =>
        await _slidesCollection.Find(_ => true).ToListAsync();

    // Get a single slide by ID
    public async Task<Slide?> GetSlideByIdAsync(string id) =>
        await _slidesCollection.Find(s => s.Id == id).FirstOrDefaultAsync();

    // Get slides by category
    public async Task<List<Slide>> GetSlidesByCategoryAsync(string category) =>
        await _slidesCollection.Find(s => s.Category == category).ToListAsync();

    // Create a new slide
    public async Task CreateSlideAsync(Slide slide) =>
        await _slidesCollection.InsertOneAsync(slide);
}