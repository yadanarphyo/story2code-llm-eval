using Implementation.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<IGetNearbyRecyclingFacilitiesService, GetNearbyRecyclingFacilitiesService>();

var app = builder.Build();

app.MapControllers();

app.Run();