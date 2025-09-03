using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestCaseManagement.Api.Extensions;
using TestCaseManagementService.Middleware;
using TestCaseManagement.Data;

var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
if (!Directory.Exists(wwwrootPath))
{
    Directory.CreateDirectory(wwwrootPath);
}

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

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
        options.InvalidModelStateResponseFactory = context =>
        {
            return new BadRequestObjectResult(new
            {
                success = false,
                message = "Validation failed",
                errors = context.ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
            });
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database Context
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

// ✅ Swagger should be available in both Debug & Release
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TestCaseManagement API v1");
    c.RoutePrefix = "swagger"; // access via /swagger
});

// ✅ Run DB initializer only in development (optional for prod)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbInitializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
    await dbInitializer.InitializeAsync();
}

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();
app.UseCors("AllowFrontendClients");

app.UseAuthorization();

app.MapControllers();

app.Run();
