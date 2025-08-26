using Massora.Business.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Massora.Business.Services
{
    public interface IDashboardService
    {
        Task<DashboardStatsDto> GetStatsForUserAsync(string userId);
    }
}
