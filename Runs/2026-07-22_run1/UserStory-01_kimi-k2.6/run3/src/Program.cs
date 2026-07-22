using Implementation.Models;
using Implementation.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IEnumerable<RecyclingFacility>>(_ => new List<RecyclingFacility>());
builder.Services.AddScoped<IGetNearbyRecyclingFacilitiesService, GetNearbyRecyclingFacilitiesService>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();