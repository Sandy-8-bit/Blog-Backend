using Backend.Models;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Configure MongoDB Settings
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDB"));

// Register MongoDB Client
var mongoSettings = builder.Configuration.GetSection("MongoDB").Get<MongoDbSettings>();
var mongoClient = new MongoClient(mongoSettings!.ConnectionString);
var mongoDatabase = mongoClient.GetDatabase(mongoSettings.DatabaseName);

builder.Services.AddSingleton<IMongoClient>(mongoClient);
builder.Services.AddSingleton<IMongoDatabase>(mongoDatabase);
builder.Services.AddScoped<SupabaseAuthService>();

// Register SlideService
builder.Services.AddSingleton<SlideService>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins("https://localhost:7037") // Your Blazor frontend URL
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // Allow cookies & authentication headers
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Apply CORS before authorization and controllers
app.UseCors("AllowBlazorClient");

app.UseAuthorization();
app.MapControllers();

app.Run();
