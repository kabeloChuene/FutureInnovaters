using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SmartSeatAllocation.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Register business services
builder.Services.AddSingleton<AllocationService>();
builder.Services.AddSingleton<ISessionService>(sp => new SessionService(sp.GetRequiredService<AllocationService>()));
builder.Services.AddSingleton<IParticipantService>(sp => new ParticipantService(sp.GetRequiredService<AllocationService>()));
builder.Services.AddSingleton<IAllocationService>(sp => sp.GetRequiredService<AllocationService>());

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Smart Seat Allocation API",
        Version = "v1",
        Description = "API for managing participant allocation to training sessions",
        Contact = new OpenApiContact
        {
            Name = "Smart Seat Allocation Platform"
        }
    });
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Smart Seat Allocation API V1");
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();
