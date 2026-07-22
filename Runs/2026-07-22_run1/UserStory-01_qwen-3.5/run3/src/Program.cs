using Implementation.Models;
using Implementation.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register the service implementation.
builder.Services.AddTransient<IGetNearbyRecyclingFacilitiesService, GetNearbyRecyclingFacilitiesService>();

// Register the data dependency required by the service constructor.
// For the live application, we provide an empty list. 
// The unit tests bypass DI and instantiate the service directly with test data.
builder.Services.AddSingleton<IEnumerable<RecyclingFacility>>(new List<RecyclingFacility>());

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthorization();

app.MapControllers();

app.Run();