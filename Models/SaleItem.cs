using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace up.Models
{
    public class SaleItem
    {
        public int Id {  get; set; }
        public int sale_id { get; set; }
        public string sale_number { get; set; }
        public int product_id { get; set; }
        public string product_name { get; set; }
        public int quantity { get; set; }
        public decimal price { get; set; }
    }
}
