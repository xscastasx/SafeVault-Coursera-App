using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add OpenAPI
builder.Services.AddOpenApi();

// Add authentication + authorization
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // In a real app, you'd configure token validation here.
        options.TokenValidationParameters.ValidateAudience = false;
        options.TokenValidationParameters.ValidateIssuer = false;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Enable OpenAPI UI
app.MapOpenApi();

// Enable authentication + authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

// Protected admin route group
var adminGroup = app.MapGroup("/admin")
    .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });

adminGroup.MapGet("/settings", () => "Admin settings");

app.Run();
