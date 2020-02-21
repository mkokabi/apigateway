using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogService
{
    public class Category
    {
        public string Key { get; set; }
        public string Label { get; set; }
        public string ParentCategory { get; set; }
    }
}
