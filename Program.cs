using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using CityInfoAPI.Services;
using CityInfoAPI.Db;
using CityInfoAPI.Models;

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

// Registering mail service to the container with transient lifetime
#if DEBUG
builder.Services.AddTransient<IMailService, LocalMailService>();
#else
builder.Services.AddTransient<IMailService, LocalMailService>();
#endif

// builder.Services.AddSingleton<CitiesDataStore>();

builder.Services.AddDbContext<CityInfoContext>(opts => opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// registering the repository with the scope lifetime
builder.Services.AddScoped<ICityInfoRepository, CityInfoRepository>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddAuthentication("Bearer").AddJwtBearer(opts => {
    opts.TokenValidationParameters = new() {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Authentication:Issuer"],
        ValidAudience = builder.Configuration["Authentication:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Authentication:SecretForKey"]))
    };
});

builder.Services.AddAuthorization(opts => {
    opts.AddPolicy("MustBeFromNairobi", policy => {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("city", "Nairobi");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpRedirection();

app.UseRouting();

// authentication middleware
app.UseAuthentication();

app.UseAuthorization();

// app.MapControllers();
app.UseEndpoints(endpoints => {
    endpoints.MapControllers();
});

app.Run();

