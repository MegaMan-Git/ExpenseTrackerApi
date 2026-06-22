using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Application.Filters
{
    public class QueryStringParameters
    {
        private int _pageSize = 10;
        private const int _maxPageSize = 20;
        private string _sortDirection = "asc";

        public int PageNumber {  get; set; } = 1;
        public string SortBy { get; set; } = string.Empty; 

        public int PageSize {
            get
            {
                return _pageSize;
            }
            set
            {
                if(value > 0)
                _pageSize = Math.Min(_maxPageSize, value);
            }
        
        }

        public string SortDirection
        {
            get
            {
                return _sortDirection;
            }
            set
            {
                if (value?.ToLower() == "desc")
                    _sortDirection = "desc";
                else
                    _sortDirection = "asc";
            }
        }

    }

}
