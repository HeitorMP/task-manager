using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Secret key used to create and validate the token
var secretKey = Environment.GetEnvironmentVariable("SECRET_KEY");


if (string.IsNullOrEmpty(secretKey))
{
    throw new Exception("SECRET_KEY is not set inside the container.");
}

var key = Encoding.ASCII.GetBytes(secretKey);
// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // JWT configuration in Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insert the JWT token in the field below. Example: Bearer YOUR_TOKEN_HERE"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddAuthorization();

// Configure JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        // Adiciona o nome do serviço backend
        policy.WithOrigins("http://localhost", "http://localhost:80", "http://backend:5170")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddSingleton<UserService>();
var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowAll"); // Frontend interaction

app.UseHttpsRedirection();
app.UseAuthentication(); // Enable JWT authentication
app.UseAuthorization();  // Enable authorization

// Simulate an in-memory database
var tasksDb = new ConcurrentDictionary<int, TaskItem>();
var idCounter = 1;

// Login endpoint to generate the token
app.MapPost("/auth/login", (LoginRequest request, UserService userService) =>
{
    Console.WriteLine("aqui");
    Console.WriteLine(request);
    if (userService.ValidateUser(request.Username, request.Password))
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, request.Username),
            new Claim(ClaimTypes.Role, "User") // You can adjust as needed
        };

        var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: "http://localhost",
            audience: "http://localhost",
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: credentials
        );

        return Results.Ok(new { Token = new JwtSecurityTokenHandler().WriteToken(token) });
    }
    return Results.Unauthorized();
})
.WithName("Login")
.WithOpenApi();

// Register
app.MapPost("/auth/register", async (HttpContext context, UserService userService) =>
{
    // Force reading the request body as JSON
    var user = await context.Request.ReadFromJsonAsync<User>();

    Console.WriteLine($"Email: {user.Email}");
    Console.WriteLine($"Password: {user.Password}");
    Console.WriteLine($"ConfirmPassword: {user.ConfirmPassword}");

    var result = userService.RegisterUser(user.Email, user.Password, user.ConfirmPassword);
    
    return result == "User successfully registered!" ? Results.Ok(new { message = result}) : Results.BadRequest(new { message = result});
})
.WithName("Register")
.WithOpenApi();

// GET /tasks - Returns all tasks for the authenticated user
app.MapGet("/tasks", (HttpContext httpContext, string? status) =>
{
    var username = httpContext.User.Identity?.Name;
    if (string.IsNullOrEmpty(username))
        return Results.Unauthorized();

    // Filter tasks based on the status, if provided
    var tasks = string.IsNullOrEmpty(status)
        ? tasksDb.Values.ToList()
        : tasksDb.Values.Where(t => t.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();

    if (tasks.Count == 0)
        return Results.NotFound($"No tasks found with status: {status}");

    return Results.Ok(new {tasks = tasks});
})
.WithName("GetFilteredTasks")
.WithOpenApi()
.RequireAuthorization();

// GET /tasks/{id} - Returns a specific task for the authenticated user
app.MapGet("/tasks/{id:int}", (int id, HttpContext httpContext) =>
{
    var username = httpContext.User.Identity?.Name;
    if (string.IsNullOrEmpty(username))
        return Results.Unauthorized();

    if (tasksDb.TryGetValue(id, out var task))
    {
        return Results.Ok(task);
    }
    return Results.NotFound($"Task with ID {id} not found.");
})
.WithName("GetTaskById")
.WithOpenApi()
.RequireAuthorization();

app.MapPost("/tasks", (TaskInput input, HttpContext httpContext) =>
{
    var username = httpContext.User.Identity?.Name;
    if (string.IsNullOrEmpty(username))
        return Results.Unauthorized();

    var newTask = new TaskItem
    {
        Id = idCounter++,
        Title = input.Title,
        Description = input.Description,
        Status = input.Status
    };

    tasksDb[newTask.Id] = newTask;
    return Results.Created($"/tasks/{newTask.Id}", newTask);
});


app.MapPut("/tasks/{id:int}", (int id, TaskInput input, HttpContext httpContext) =>
{
    var username = httpContext.User.Identity?.Name;
    if (string.IsNullOrEmpty(username))
        return Results.Unauthorized();

    if (tasksDb.TryGetValue(id, out var existingTask))
    {
        existingTask.Title = input.Title;
        existingTask.Description = input.Description;
        existingTask.Status = input.Status;
        return Results.Ok(existingTask);
    }
    return Results.NotFound($"Task with ID {id} not found.");
})
.WithName("UpdateTask")
.WithOpenApi()
.RequireAuthorization();


app.MapDelete("/tasks/{id:int}", (int id, HttpContext httpContext) =>
{
    var username = httpContext.User.Identity?.Name;
    if (string.IsNullOrEmpty(username))
        return Results.Unauthorized();

    if (tasksDb.TryRemove(id, out _))
    {
        Console.WriteLine($"Task with ID {id} removed successfully.");
        Console.WriteLine("Current tasks: " + string.Join(", ", tasksDb.Keys));
        return Results.NoContent();
    }
    return Results.NotFound($"Task with ID {id} not found.");
})
.WithName("DeleteTask")
.WithOpenApi()
.RequireAuthorization();


app.Run();

// Models
record TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending"; // Pending, In Progress, Completed
}

record TaskInput(string Title, string Description, string Status);
record LoginRequest(string Username, string Password);

