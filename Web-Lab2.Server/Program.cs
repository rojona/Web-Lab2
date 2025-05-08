using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Web_Lab2.Data;
using Web_Lab2.Models;
using Web_Lab2.Repos;
using Web_Lab2.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IProductRep, ProductRep>();
builder.Services.AddScoped<ICustomerRep, CustomerRep>();
builder.Services.AddScoped<IOrderRep, OrderRep>();
builder.Services.AddScoped<IOrderItemRep, OrderItemRep>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient",
        policy => { 
            policy.WithOrigins("http://localhost:5000")
                .AllowAnyMethod()
                .AllowAnyHeader(); 
        });
});

builder.WebHost.UseWebRoot("wwwroot");
builder.Environment.EnvironmentName = "Development";
builder.WebHost.UseUrls("http://localhost:5231", "https://localhost:5232");

var app = builder.Build();

Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");
Console.WriteLine($"WebRootPath: {app.Environment.WebRootPath}");
Console.WriteLine($"ContentRootPath: {app.Environment.ContentRootPath}");

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseWebAssemblyDebugging();
    Console.WriteLine("Development mode is enabled");
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowBlazorClient");

app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        logger.LogInformation($"Checking for database updates...");

        context.Database.Migrate();

        logger.LogInformation($"Checking for existing users...");

        if (!context.Users.Any())
        {
            logger.LogInformation($"No users found. Creating default users...");

            context.Users.Add(new User
            {
                Username = "admin",
                Email = "admin@admin.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin"),
                Roles = new List<string> { "Admin" }
            });

            context.Users.Add(new User
            {
                Username = "user",
                Email = "user@user.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("user"),
                Roles = new List<string> { "User" }
            });

            await context.SaveChangesAsync();
            logger.LogInformation($"Created new users...");
        }
        else
        {
            logger.LogInformation($"Users already exists...");
        }
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while checking for updates...");
    }
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    var users = context.Users.ToList();
    logger.LogInformation($"Database contains {users.Count} users:");
    
    foreach (var user in users)
    {
        logger.LogInformation($"User: {user.Username}, Email: {user.Email}, Roles: {string.Join(",", user.Roles)}");
    }
}

Console.WriteLine($"Server is listening on URLs: {string.Join(", ", app.Urls)}");

app.Run();