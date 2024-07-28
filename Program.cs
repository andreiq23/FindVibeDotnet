using AngleSharp;
using AngleSharp.Dom;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/songs", async (string? search) =>
{
    var url = "https://muzkan.net/?q=";
    search = search!.Replace(" ", "+");
    using HttpClient client = new HttpClient();
    var response = await client.GetStringAsync(url + search);

    IBrowsingContext context = BrowsingContext.New(Configuration.Default);
    IDocument document = await context.OpenAsync(req => req.Content(response));
    var wrapper = document.QuerySelector(".files__wrapper");

    var songs = new List<Song>();
    foreach (var song in wrapper!.Children)
    {
        var artist = song?.QuerySelector("h4")?.Text();
        var title = song?.QuerySelector("h5")?.Text();
        var image = song?.QuerySelector("img")?.GetAttribute("data-src");
        var link = song?.QuerySelector("div[mp3source]")?.Attributes["mp3source"]?.Value;
        var newSong = new Song()
        {
            Id = Guid.NewGuid().ToString(),
            Artist = artist,
            Title = title,
            Image = image,
            Link = link
        };
        songs.Add(newSong);
    }

    return songs;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

