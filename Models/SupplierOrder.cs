using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace up.Models
{
    public class SupplierOrder
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public DateTime OrderDate {  get; set; }
        public string StatusOrder { get; set; }
        public decimal TotalCost { get; set; }
    }
}
