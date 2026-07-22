using Implementation.Models;
using Implementation.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register seed data for DI to work during live run.
// The tests bypass this by constructing the service directly with their own data.
builder.Services.AddSingleton<IEnumerable<RecyclingFacility>>(new List<RecyclingFacility>());

// Register the service implementation
builder.Services.AddTransient<IGetNearbyRecyclingFacilitiesService, GetNearbyRecyclingFacilitiesService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();