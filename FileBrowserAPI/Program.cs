using FileBrowserAPI.Repository;
using System.IO.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("FileRepositoryConfig.json");

// Add services to the container.
builder.Services.AddTransient<IDataRepository, FileRepository>();
builder.Services.AddTransient<IFileSystem, FileSystem>();

builder.Services.AddControllers();
builder.WebHost.UseUrls("http://localhost:80");

var app = builder.Build();
// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();
app.Run();
