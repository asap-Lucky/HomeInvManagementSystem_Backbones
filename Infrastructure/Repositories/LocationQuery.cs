using Application.Interfaces.Queries;
using Infrastructure.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class LocationQuery : ILocationQuery
    {
        // Injections
        private readonly HomeinvsystemContext _context;
        private readonly ILogger<LocationQuery> _logger;

        public LocationQuery(HomeinvsystemContext context, ILogger<LocationQuery> logger) 
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<LocationDTO>> GetAllLocationsAsync()
        {
            try
            {
                var locations = await _context.Locations.ToListAsync();

                var outDTOs = locations.Select(location => new LocationDTO
                {
                    Id = location.Id,
                    Name = location.Name
                }).ToList();

                return outDTOs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all locations");
                throw;
            }
        }
    }
}
