using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Configuration
{
    // Configuration options for Home Inventory Management API.
    // TODO: Spread this out if more options come in the future for more specific uses. Right now its only for the OFF API.
    public class HomeInvOptions
    {
        public string? OFFApiUrl { get; set; }

        public string? OFFAuthToken { get; set; }
    }
}
