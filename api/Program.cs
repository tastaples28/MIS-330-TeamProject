using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using CampusReHome.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Use snake_case for JSON property names to match frontend expectations
        options.JsonSerializerOptions.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
    });
builder.Services.AddSingleton<DatabaseService>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    return new DatabaseService(configuration);
});

// Configure CORS for API access
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Enable CORS
app.UseCors("AllowAll");


// Serve static files from Client directory (parent folder)
var clientPath = Path.Combine(builder.Environment.ContentRootPath, "..", "Client");
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(clientPath),
    RequestPath = ""
});

// Helper function to serve HTML files
static async Task ServeHtmlFile(HttpContext context, string fileName, string contentRootPath)
{
    var filePath = Path.Combine(contentRootPath, "..", "Client", fileName);
    if (File.Exists(filePath))
    {
        context.Response.ContentType = "text/html";
        await context.Response.SendFileAsync(filePath);
    }
    else
    {
        context.Response.StatusCode = 404;
        await context.Response.WriteAsync("File not found");
    }
}

var contentRoot = builder.Environment.ContentRootPath;

// IMPORTANT: Register routes in this order for proper matching

// 1. Default route - serve index.html
app.MapGet("/", async (HttpContext context) => await ServeHtmlFile(context, "index.html", contentRoot));

// 2. Specific HTML page routes (must come before catch-all and controllers)
app.MapGet("/customers", async (HttpContext context) => await ServeHtmlFile(context, "customers.html", contentRoot));
app.MapGet("/furniture", async (HttpContext context) => await ServeHtmlFile(context, "furniture.html", contentRoot));
app.MapGet("/employees", async (HttpContext context) => await ServeHtmlFile(context, "employees.html", contentRoot));
app.MapGet("/transactions", async (HttpContext context) => await ServeHtmlFile(context, "transactions.html", contentRoot));

// 3. API controllers (for /api/* routes)
app.MapControllers();

// 4. Catch-all HTML routes for sub-pages (edit, new, etc.) - MUST be last
app.MapGet("/customers/{*path}", async (HttpContext context) => await ServeHtmlFile(context, "customers.html", contentRoot));
app.MapGet("/furniture/{*path}", async (HttpContext context) => await ServeHtmlFile(context, "furniture.html", contentRoot));
app.MapGet("/employees/{*path}", async (HttpContext context) => await ServeHtmlFile(context, "employees.html", contentRoot));
app.MapGet("/transactions/{*path}", async (HttpContext context) => await ServeHtmlFile(context, "transactions.html", contentRoot));

app.Run();

// Snake case naming policy for JSON
public class SnakeCaseNamingPolicy : System.Text.Json.JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;
        
        var result = new System.Text.StringBuilder();
        result.Append(char.ToLowerInvariant(name[0]));
        
        for (int i = 1; i < name.Length; i++)
        {
            if (char.IsUpper(name[i]))
            {
                result.Append('_');
                result.Append(char.ToLowerInvariant(name[i]));
            }
            else
            {
                result.Append(name[i]);
            }
        }
        
        return result.ToString();
    }
}

