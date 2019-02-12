using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksCatalog.Models
{
    public class SoldProduct
    {
        public int Id { get; set; }
        public string Product_name { get; set; }
        public string Manufacturer { get; set; }
        public decimal Product_price { get; set; }
        public int Product_count { get; set; }
        public int Category_id { get; set; }
        public DateTime Sale_date { get; set; }
        public int Client_id { get; set; }
    }
}
