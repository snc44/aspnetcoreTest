using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BooksCatalog.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Product_name { get; set; }
        public string Manufacturer { get; set; }
        public decimal Product_price { get; set; }
        public int Product_count { get; set; }
        public int Category_id { get; set; }
    }
}
