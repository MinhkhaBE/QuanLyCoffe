using QuanLyCafe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCafe.DAO
{
    class FoodCategoryDAO
    {
        private static FoodCategoryDAO instance;

        public static FoodCategoryDAO Instance
        {
            get { if (instance == null) instance = new FoodCategoryDAO(); return FoodCategoryDAO.instance; }
            private set { FoodCategoryDAO.instance = value; }
        }

        private FoodCategoryDAO() { }

        public List<FoodCategory> GetFoodByCategoryID(int id)
        {
            List<FoodCategory> list = new List<FoodCategory>();

            string query = "select * from FoodCategory where id = " + id;

            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach (DataRow item in data.Rows)
            {
                FoodCategory foodCategory = new FoodCategory(item);
                list.Add(foodCategory);
            }

            return list;
        }
        public DataTable GetListFoodCategory1()// new rows
        {
            return DataProvider.Instance.ExecuteQuery("SELECT id , name FROM dbo.FoodCategory");
        }
        public List<FoodCategory> GetListFoodCategory()
        {
            List<FoodCategory> list = new List<FoodCategory>();

            string query = "select * from FoodCategory";

            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach (DataRow item in data.Rows)
            {
                FoodCategory foodCategory = new FoodCategory(item);
                list.Add(foodCategory);
            }

            return list;
        }

       

        public bool InsertFoodCategory(string name)
        {
            string query = string.Format("INSERT dbo.FoodCategory ( name )VALUES  ( N'{0}')", name);
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }

        public bool UpdateFoodCategory(string name, int id)
        {
            string query = string.Format("UPDATE dbo.FoodCategory SET name = N'{0}' WHERE id = {1}", name, id);
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }

        public bool DeleteFoodCategory(int foodCategory)
        {
            string query = string.Format("Delete FoodCategory where id = {0}", foodCategory);
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }
    }
}
