using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace up.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public string SaleNumber { get; set; }
        public int EmployeeId { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMethod { get; set; }

        public string EmployeeName { get; set; }

    }
}
