using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.DataLayer.Entities
{
    public class Expense
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string Title { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; }


        public User User { get; set; } = null!;
        public Category Category { get; set; } = null!;
    }
}
