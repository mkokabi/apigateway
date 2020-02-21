using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CatalogService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoriesController : ControllerBase
    {
        private static readonly Category[] Categories = new[]
        {
            new Category {Key = "automative", Label="Automative"},
            new Category {Key = "book", Label="Books"},
            new Category {Key = "fashion", Label="Clothing, Shoes & Accessories"},
            new Category {Key = "fashion-womens", Label="Women", ParentCategory = "fashion"},
            new Category {Key = "fashion-mens", Label="Men", ParentCategory = "fashion"},
            new Category {Key = "fashion-baby", Label="Baby", ParentCategory = "fashion"},
            new Category {Key = "electronic", Label="Electronic"},
            new Category {Key = "electronic-mobile", Label="Mobile", ParentCategory = "electronic"},
            new Category {Key = "electronic-mobile-accessories", Label="Accessories", ParentCategory = "electronic-mobile"},
            new Category {Key = "electronic-computer", Label="Computer", ParentCategory = "electronic"},
        };

        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ILogger<CategoriesController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CategoryViewModel>> Get()
        {
            var dictionary = new Dictionary<string, CategoryViewModel>();

            foreach (var item in Categories)
            {
                if (item.ParentCategory == null)
                {
                    dictionary.Add(item.Key, new CategoryViewModel { Key = item.Key, Label = item.Label });
                }
                else
                {
                    var parent = FindParent(item.ParentCategory, dictionary);
                    if (parent.Children == null)
                    {
                        parent.Children = new List<CategoryViewModel>();
                    }
                    parent.Children.Add(new CategoryViewModel { Key = item.Key, Label = item.Label });
                }
            }
            return dictionary.Values.ToArray();
        }

        private CategoryViewModel FindParent(string parentCategory, Dictionary<string, CategoryViewModel> dictionary)
        {
            if (dictionary.TryGetValue(parentCategory, out CategoryViewModel categoryViewModel))
            {
                return categoryViewModel;
            }
            return FindParent(parentCategory, 
                dictionary.FirstOrDefault(item => parentCategory.StartsWith(item.Key))
                .Value.Children
                .ToDictionary(item => item.Key, item => item)
            );
        }
    }
}
