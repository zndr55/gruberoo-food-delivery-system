using System;
using System.Collections.Generic;

namespace GruberooApp
{
    // A menu that belongs to a restaurant (one restaurant can have several menus)
    public class Menu
    {
        private string menuId;
        private string menuName;
        private List<FoodItem> foodItems;

        public string MenuId
        {
            get { return menuId; }
            set { menuId = value; }
        }

        public string MenuName
        {
            get { return menuName; }
            set { menuName = value; }
        }

        public List<FoodItem> FoodItems
        {
            get { return foodItems; }
            set { foodItems = value; }
        }

        public Menu()
        {
            foodItems = new List<FoodItem>();
        }

        public Menu(string id, string name)
        {
            menuId = id;
            menuName = name;
            foodItems = new List<FoodItem>();
        }

        public void AddFoodItem(FoodItem item)
        {
            foodItems.Add(item);
        }

        public bool RemoveFoodItem(FoodItem item)
        {
            return foodItems.Remove(item);
        }

        public void DisplayFoodItems()
        {
            Console.WriteLine("\n--- " + menuName + " ---");
            Console.WriteLine(string.Format("  {0,-25} {1,-35} {2,8}", "Name", "Description", "Price"));
            Console.WriteLine("  " + new string('-', 70));

            if (foodItems.Count == 0)
            {
                Console.WriteLine("  (No items in this menu)");
            }
            else
            {
                for (int i = 0; i < foodItems.Count; i++)
                {
                    Console.WriteLine("  " + (i + 1) + ". " + foodItems[i].ToString());
                }
            }
        }

        public override string ToString()
        {
            return "[" + menuId + "] " + menuName + " (" + foodItems.Count + " items)";
        }
    }
}