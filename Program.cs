using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Serilog;
using CityInfoAPI.Services;
using CityInfoAPI.Db;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/cityinfoapi.txt", rollingInterval: RollingInterval.Hour)
    .CreateLogger();


var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// Add services to the container.

// builder.Services.AddControllers(opts => {
//     // Allow different formatting of data
//     opts.ReturnHttpNotAcceptable = true;
// }).AddXmlDataContractSerializerFormatters(); // Add Xml input and output formatters

builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<FileExtensionContentTypeProvider>();

// Registering mail service to the container
#if DEBUG
builder.Services.AddTransient<IMailService, LocalMailService>();
#else
builder.Services.AddTransient<IMailService, LocalMailService>();
#endif
// builder.Configuration.AddEnvironmentVariables();
// builder.Services.AddDbContext<CityInfoContext>(opts => opts.UseSqlServer(builder.Configuration.GetValue<string>("CityInfoDBConnectionStr")));
builder.Services.AddDbContext<CityInfoContext>(opts => opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpRedirection();

app.UseRouting();

app.UseAuthorization();

// app.MapControllers();
app.UseEndpoints(endpoints => {
    endpoints.MapControllers();
});

app.Run();

