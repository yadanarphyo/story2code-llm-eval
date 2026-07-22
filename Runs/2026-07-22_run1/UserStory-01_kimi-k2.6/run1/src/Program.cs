using Implementation.Models;
using Implementation.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var seedFacilities = new List<RecyclingFacility>
{
    new RecyclingFacility
    {
        FacilityId = 1,
        Name = "Downtown Recycling Center",
        Address = "123 Main St",
        City = "Springfield",
        State = "IL",
        ZipCode = "62701",
        DistanceInMiles = 1.2,
        PhoneNumber = "555-0101"
    },
    new RecyclingFacility
    {
        FacilityId = 2,
        Name = "Westside Drop-Off",
        Address = "456 Oak Ave",
        City = "Springfield",
        State = "IL",
        ZipCode = "62704",
        DistanceInMiles = 3.5,
        PhoneNumber = "555-0102"
    }
};

builder.Services.AddSingleton<IEnumerable<RecyclingFacility>>(seedFacilities);
builder.Services.AddScoped<IGetNearbyRecyclingFacilitiesService, GetNearbyRecyclingFacilitiesService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();