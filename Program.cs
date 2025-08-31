using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Serilog;
using ZeonService.Data;
using ZeonService.Middleware;
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

builder.Services.AddDbContextFactory<ZeonDbContext>(options => options.UseNpgsql(dataSource));
builder.Services.AddControllers();
builder.Services.AddHttpClient("ZeonClient" ,client =>
{
    client.Timeout = TimeSpan.FromMinutes(config.GetSection("ZeonParserSettings").GetValue<int>("TimeoutMinutes"));
    client.DefaultRequestHeaders.Add("User-Agent", config.GetSection("ZeonParserSettings")["User-Agent"]);
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();
builder.Services.AddHangfire(c => c
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(options =>
        options.UseNpgsqlConnection(config.GetConnectionString("DefaultConnection")),
        new PostgreSqlStorageOptions
        {
            InvisibilityTimeout = TimeSpan.FromMinutes(90)
        }));
builder.Services.AddHangfireServer();

builder.Host.UseSerilog(logger);

var app = builder.Build();

app.UseMiddleware<ExceptionToJsonMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        DashboardTitle = "My Jobs Dashboard",
        StatsPollingInterval = 5000,
        AppPath = "/" 
    });
}

app.Lifetime.ApplicationStarted.Register(() => 
    RecurringJob.AddOrUpdate<ZeonParser>(
        "full-parse",
        zp => zp.Parse(),
        Cron.Daily(2),
         new RecurringJobOptions { TimeZone = TimeZoneInfo.Local })
);

app.UseSerilogRequestLogging();

app.MapControllers();

app.Run();
