using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksCatalog.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Category_name { get; set; }
        public int Products_count { get; set; }
    }
}
