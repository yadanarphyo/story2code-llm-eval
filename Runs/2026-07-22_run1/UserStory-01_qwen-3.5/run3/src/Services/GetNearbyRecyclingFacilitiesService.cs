using System;
using System.Collections.Generic;
using System.Linq;
using Implementation.Models;

namespace Implementation.Services
{
    public class GetNearbyRecyclingFacilitiesService : IGetNearbyRecyclingFacilitiesService
    {
        private readonly IEnumerable<RecyclingFacility> _facilities;

        public GetNearbyRecyclingFacilitiesService(IEnumerable<RecyclingFacility> facilities)
        {
            _facilities = facilities ?? throw new ArgumentNullException(nameof(facilities));
        }

        public IEnumerable<RecyclingFacility> GetNearbyRecyclingFacilities(string zipCode)
        {
            if (string.IsNullOrWhiteSpace(zipCode))
            {
                throw new ArgumentException("Zip code cannot be null, empty, or whitespace.", nameof(zipCode));
            }

            var results = _facilities
                .Where(f => f.ZipCode == zipCode)
                .ToList();

            return results;
        }
    }
}