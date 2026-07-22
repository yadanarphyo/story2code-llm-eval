using Implementation.Data;
using Implementation.Models;
using Implementation.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register seed data for the live application
builder.Services.AddSingleton<IEnumerable<RecyclingFacility>>(SeedData.GetFacilities());

// Register the service
builder.Services.AddSingleton<IGetNearbyRecyclingFacilitiesService, GetNearbyRecyclingFacilitiesService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();