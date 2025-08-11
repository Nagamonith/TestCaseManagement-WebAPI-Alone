using Microsoft.EntityFrameworkCore;
using TestCaseManagement.Api.Extensions;
using TestCaseManagement.Api.Middleware;
using TestCaseManagement.Api.Utilities;
using TestCaseManagement.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services
    .AddDatabase(builder.Configuration)
    .AddAutoMapper(typeof(Program))  // Ensure you have AutoMapper package installed
    .AddRepositories()
    .AddServices()
    .AddValidators()
    .AddUtilities();

// Register DbInitializer
builder.Services.AddTransient<DbInitializer>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database Context
// Add this with your other services
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ CORS setup so frontend can call API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontendClients", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200",    // Angular dev server
                "http://localhost:4948")    // Your other frontend origin
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Initialize the database
    using var scope = app.Services.CreateScope();
    var dbInitializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
    await dbInitializer.InitializeAsync();
}

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();

// ✅ Enable CORS - must be after UseHttpsRedirection and before UseAuthorization
app.UseCors("AllowFrontendClients");

app.UseAuthorization();

app.MapControllers();

app.Run();