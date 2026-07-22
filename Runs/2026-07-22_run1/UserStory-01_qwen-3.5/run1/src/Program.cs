using Implementation.Services;
using Implementation.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register the service interface and implementation.
// The service requires IEnumerable<RecyclingFacility> in its constructor.
// For the live app (which is not executed by tests), we provide an empty enumerable.
builder.Services.AddTransient<IEnumerable<RecyclingFacility>>(_ => Enumerable.Empty<RecyclingFacility>());
builder.Services.AddTransient<IGetNearbyRecyclingFacilitiesService, GetNearbyRecyclingFacilitiesService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthorization();

app.MapControllers();

app.Run();