using Warehouse.Data;

namespace Warehouse.Models
{
    public class SingletonSum
    {
        private SingletonSum() { }
        private static SingletonSum _instance = null;
        private static float wSum = 0;
        private static List<float> cSum = new List<float>();
        private static List<string> cIDs = new List<string>();
        public static SingletonSum GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SingletonSum();
            }
            return _instance;
        }
        public void setSums(WarehouseContext context)
        {
            wSum = ProductManager.warehouseSum(context);
            cSum = CategoryManager.categoriesSum(context);
            cIDs = CategoryManager.getCategoriesIDs(context);
        }
        public float getWarehouseSum()
        {
            return wSum;
        }
        public List<float> getCategoriesSum()
        {
            return cSum;
        }
        public List<string> getCategoriesIDs()
        {
            return cIDs;
        }
    }
}
