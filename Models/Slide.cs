using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

public class Slide
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public List<string>? Likes { get; set; } = new(); // Stores usernames who liked the post
    public List<Comment>? Comments { get; set; } = new(); // Stores comments
}

public class Comment
{
    public string? Username { get; set; } = string.Empty;
    public string? Text { get; set; } = string.Empty;
    public DateTime? Date { get; set; }
}


public class LikeRequest
{
    public string? Username { get; set; } = string.Empty;
}

public class CommentRequest
{
    public string? Username { get; set; } = string.Empty;
    public string? Comment { get; set; } = string.Empty;
}
