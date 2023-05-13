using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using Warehouse.Data;

namespace Warehouse.Models
{
    public class ProductManager
    {
        public static float warehouseSum(WarehouseContext context)
        {
            var products = context.NewProduct.ToList();
            float sum = 0;
            foreach (var product in products)
                sum += product.quantity * product.price;
            return sum;
        }

    }
}
