using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace up.Models
{
    public class Returns
    {
        public int Id { get; set; }
        public string ReturnNumber { get; set; }
        public int SaleId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime ReturnDate { get; set; }
        public string Reason { get; set; }
        public double TotalRefund { get; set; }
    }
}
