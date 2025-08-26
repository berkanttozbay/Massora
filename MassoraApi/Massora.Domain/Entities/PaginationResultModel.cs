using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Massora.Domain.Entities
{
    public class PaginationResultModel<T> where T : class
    {
        public int TotalPages { get; set; } = 0;
        public int TotalCount { get; set; } = 0;
        public List<T> Items { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }

    }
}
