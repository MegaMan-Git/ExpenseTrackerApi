using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Application.Filters
{
    public class ExpenseQueryString : QueryStringParameters
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate {  get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
    }
}
