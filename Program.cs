using AngleSharp;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Serilog;
using ZeonService.Data;
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

var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("../Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var dataSource = new NpgsqlDataSourceBuilder(config.GetConnectionString("DefaultConnection"))
    .EnableDynamicJson()  
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
builder.Services.AddScoped<IFileGetter<Guid, (byte[], string)>, ImageGetter>();

builder.Services.AddDbContext<ZeonDbContext>(options => options.UseNpgsql(dataSource));
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseSerilog(logger);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.MapControllers();

app.Run();
