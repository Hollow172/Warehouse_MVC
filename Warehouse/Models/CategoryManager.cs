using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using Warehouse.Data;

namespace Warehouse.Models
{
    public class CategoryManager
    {
        public static List<string> getCategoriesIDs(WarehouseContext context)
        {
            var categories = context.Category.ToList();
            var categoriesIDs = new List<string>();
            foreach (var category in categories)
            {
                categoriesIDs.Add(category.name);
            }
            return categoriesIDs;
        }

        public static List<float> categoriesSum(WarehouseContext context)
        {
            var categories = context.Category.ToList();
            var products = context.NewProduct.ToList();
            var catSum = new List<float>();
            foreach (var category in categories)
            {
                float sum = 0;
                foreach (var product in products)
                {
                    if (product.categoryId == category.id)
                    {
                        sum += product.quantity * product.price;
                    }
                }
                catSum.Add(sum);
            }
            return catSum;
        }

    }
}
