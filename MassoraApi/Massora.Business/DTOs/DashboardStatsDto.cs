using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Massora.Business.DTOs
{
    public class DashboardStatsDto
    {
        public int VehicleCount { get; set; }
        public int DriverCount { get; set; }
        public int PartnerCompanyCount { get; set; }
        public int ActiveWorkHistoryCount { get; set; } // Örnek ek bir bilgi
    }
}
