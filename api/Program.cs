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
        // Use snake_case for JSON property names in RESPONSES only
        // For INPUT (deserialization), accept PascalCase directly
        options.JsonSerializerOptions.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
        // Allow case-insensitive property matching for deserialization
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        // Configure to handle both input formats
        options.JsonSerializerOptions.ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip;
        options.JsonSerializerOptions.AllowTrailingCommas = true;
    })
    .AddJsonOptions(options =>
    {
        // Create a separate options for input that doesn't use naming policy
        // This is a workaround - we'll handle input manually in controllers
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
app.MapGet("/login", async (HttpContext context) => await ServeHtmlFile(context, "login.html", contentRoot));
app.MapGet("/register", async (HttpContext context) => await ServeHtmlFile(context, "register.html", contentRoot));
app.MapGet("/profile", async (HttpContext context) => await ServeHtmlFile(context, "profile.html", contentRoot));
app.MapGet("/checkout", async (HttpContext context) => await ServeHtmlFile(context, "checkout.html", contentRoot));
app.MapGet("/employee-login", async (HttpContext context) => await ServeHtmlFile(context, "employee-login.html", contentRoot));
app.MapGet("/admin-login", async (HttpContext context) => await ServeHtmlFile(context, "employee-login.html", contentRoot)); // Redirect old URL
app.MapGet("/admin", async (HttpContext context) => await ServeHtmlFile(context, "admin.html", contentRoot));

// 3. API controllers (for /api/* routes)
app.MapControllers();

// 4. Catch-all HTML routes for sub-pages (edit, new, etc.) - MUST be last
app.MapGet("/customers/{*path}", async (HttpContext context) => await ServeHtmlFile(context, "customers.html", contentRoot));
app.MapGet("/furniture/{*path}", async (HttpContext context) => await ServeHtmlFile(context, "furniture.html", contentRoot));
app.MapGet("/employees/{*path}", async (HttpContext context) => await ServeHtmlFile(context, "employees.html", contentRoot));
app.MapGet("/transactions/{*path}", async (HttpContext context) => await ServeHtmlFile(context, "transactions.html", contentRoot));
app.MapGet("/login/{*path}", async (HttpContext context) => await ServeHtmlFile(context, "login.html", contentRoot));
app.MapGet("/register/{*path}", async (HttpContext context) => await ServeHtmlFile(context, "register.html", contentRoot));
app.MapGet("/profile/{*path}", async (HttpContext context) => await ServeHtmlFile(context, "profile.html", contentRoot));
app.MapGet("/checkout/{*path}", async (HttpContext context) => await ServeHtmlFile(context, "checkout.html", contentRoot));
app.MapGet("/employee-login/{*path}", async (HttpContext context) => await ServeHtmlFile(context, "employee-login.html", contentRoot));
app.MapGet("/admin-login/{*path}", async (HttpContext context) => await ServeHtmlFile(context, "employee-login.html", contentRoot)); // Redirect old URL
app.MapGet("/admin/{*path}", async (HttpContext context) => await ServeHtmlFile(context, "admin.html", contentRoot));

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

