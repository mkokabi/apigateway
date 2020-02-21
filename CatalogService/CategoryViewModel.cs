using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CatalogService
{
    public class CategoryViewModel
    {
        [JsonPropertyName("id")]
        public string Key { get; set; }
        [JsonPropertyName("name")]
        public string Label { get; set; }
        public List<CategoryViewModel> Children { get; set; }
    }
}
