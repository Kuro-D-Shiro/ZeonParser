using ZeonService.Controllers;
using ZeonService.Parser;
using ZeonService.Parser.Interfaces;
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
