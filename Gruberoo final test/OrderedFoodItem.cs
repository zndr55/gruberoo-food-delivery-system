using System;
using System.Collections.Generic;

namespace GruberooApp
{
    public class OrderedFoodItem : FoodItem
    {
        private int qtyOrdered;
        private double subTotal;

        public int QtyOrdered
        {
            get { return qtyOrdered; }
            set { qtyOrdered = value; }
        }

        public double SubTotal
        {
            get { return subTotal; }
            set { subTotal = value; }
        }

        public OrderedFoodItem() : base()
        {
            qtyOrdered = 0;
            subTotal = 0;
        }

        public OrderedFoodItem(FoodItem item, int qty) : base(item.ItemName, item.ItemDesc, item.ItemPrice)
        {
            qtyOrdered = qty;
            Customise = item.Customise;
            CalculateSubtotal();
        }

        public void CalculateSubtotal()
        {
            subTotal = qtyOrdered * ItemPrice;
        }

        public override string ToString()
        {
            return string.Format("{0} x{1} = ${2:F2}", ItemName, qtyOrdered, subTotal);
        }
    }
}