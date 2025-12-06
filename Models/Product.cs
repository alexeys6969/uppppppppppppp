using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace up.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Article { get; set; }
        public string NameProduct { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public decimal Price { get; set; }
        public int QuantityStock { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

    }
}
