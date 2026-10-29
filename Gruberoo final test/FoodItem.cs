using System;
using System.Collections.Generic;

namespace GruberooApp
{
    public class FoodItem
    {
        private string itemName;
        private string itemDesc;
        private double itemPrice;
        private string customise;

        public string ItemName
        {
            get { return itemName; }
            set { itemName = value; }
        }

        public string ItemDesc
        {
            get { return itemDesc; }
            set { itemDesc = value; }
        }

        public double ItemPrice
        {
            get { return itemPrice; }
            set { itemPrice = value; }
        }

        public string Customise
        {
            get { return customise; }
            set { customise = value; }
        }

        public FoodItem()
        {
            customise = "None";
        }

        public FoodItem(string name, string desc, double price)
        {
            itemName = name;
            itemDesc = desc;
            itemPrice = price;
            customise = "None";
        }

        public override string ToString()
        {
            return string.Format("{0,-25} {1,-35} ${2,6:F2}", itemName, itemDesc, itemPrice);
        }
    }
}