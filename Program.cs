using Microsoft.EntityFrameworkCore;
using ZeonService.Data;
using ZeonService.Models;
using ZeonService.Parser.Interfaces;
using ZeonService.Parser.Parsers;
using ZeonService.Parser.Repositories;
using ZeonService.Parser.Services;
using ZeonService.Parser.Settings;

var builder = WebApplication.CreateBuilder(args);

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.Development.json")
    .AddEnvironmentVariables()
    .Build();

builder.Services.Configure<ZeonParserSettings>(config.GetSection(nameof(ZeonParserSettings)));

builder.Services.AddScoped<IHtmlLoader, HtmlLoader>();
builder.Services.AddScoped<IZeonParser, ZeonParser>();
builder.Services.AddScoped<IImageDownloader, ImageDownloader>();
builder.Services.AddScoped<IImageSaver, ImageToFileSaver>();
builder.Services.AddScoped<IDownloadAndSaveImageService, ZeonDownloadAndSaveImageService>();
builder.Services.AddScoped<IZeonProductParser, ZeonProductParser>();
builder.Services.AddScoped<IZeonCategoryParser, ZeonCategoryParser>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddDbContext<ZeonDbContext>(options => options.UseNpgsql(config.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
