var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS policies
builder.Services.AddCors(options =>
{
    options.AddPolicy("customPolicy", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
    
    // Example of a more restrictive policy
    options.AddPolicy("restrictedPolicy", builder =>
    {
        builder.WithOrigins("https://example.com", "https://localhost:5001")
               .WithMethods("GET", "POST", "PUT", "DELETE")
               .WithHeaders("Content-Type", "Authorization");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

// Add CORS middleware - must be before MapReverseProxy
app.UseCors();

app.UseWebSockets(); // Needed for ws/wss proxy
app.MapReverseProxy();
app.MapControllers();

app.Run();
