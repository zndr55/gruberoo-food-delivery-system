using System;
using System.Collections.Generic;

namespace GruberooApp
{
    // An order placed by a customer
    public class Order
    {
        private int orderId;
        private DateTime orderDateTime;
        private double orderTotal;
        private string orderStatus;
        private DateTime deliveryDateTime;
        private string deliveryAddress;
        private string orderPaymentMethod;
        private bool orderPaid;
        private List<OrderedFoodItem> orderedItems;
        private List<SpecialOffer> specialOffers;
        private Customer customer;
        private Restaurant restaurant;

        public int OrderId
        {
            get { return orderId; }
            set { orderId = value; }
        }

        public DateTime OrderDateTime
        {
            get { return orderDateTime; }
            set { orderDateTime = value; }
        }

        public double OrderTotal
        {
            get { return orderTotal; }
            set { orderTotal = value; }
        }

        public string OrderStatus
        {
            get { return orderStatus; }
            set { orderStatus = value; }
        }

        public DateTime DeliveryDateTime
        {
            get { return deliveryDateTime; }
            set { deliveryDateTime = value; }
        }

        public string DeliveryAddress
        {
            get { return deliveryAddress; }
            set { deliveryAddress = value; }
        }

        public string OrderPaymentMethod
        {
            get { return orderPaymentMethod; }
            set { orderPaymentMethod = value; }
        }

        public bool OrderPaid
        {
            get { return orderPaid; }
            set { orderPaid = value; }
        }

        public List<OrderedFoodItem> OrderedItems
        {
            get { return orderedItems; }
            set { orderedItems = value; }
        }

        public List<SpecialOffer> SpecialOffers
        {
            get { return specialOffers; }
            set { specialOffers = value; }
        }

        public Customer Customer
        {
            get { return customer; }
            set { customer = value; }
        }

        public Restaurant Restaurant
        {
            get { return restaurant; }
            set { restaurant = value; }
        }

        public double DELIVERY_FEE = 5.00;
        public double GRUBEROO_COMMISSION = 0.30;

        public Order()
        {
            orderedItems = new List<OrderedFoodItem>();
            specialOffers = new List<SpecialOffer>();
            orderDateTime = DateTime.Now;
            orderStatus = "Pending";  // Default status as string
            orderPaid = false;
        }

        public void CalculateOrderTotal()
        {
            double subtotal = 0;
            foreach (var item in orderedItems)
            {
                item.CalculateSubtotal();
                subtotal += item.SubTotal;
            }

            orderTotal = subtotal + DELIVERY_FEE;

            foreach (var offer in specialOffers)
            {
                if (offer.Discount > 0)
                {
                    double discountAmount = orderTotal * (offer.Discount / 100.0);
                    orderTotal -= discountAmount;
                }
            }
        }

        public void AddOrderedFoodItem(OrderedFoodItem item)
        {
            orderedItems.Add(item);
            CalculateOrderTotal();
        }

        public bool RemoveOrderedFoodItem(OrderedFoodItem item)
        {
            bool removed = orderedItems.Remove(item);
            if (removed)
                CalculateOrderTotal();
            return removed;
        }

        public void AddSpecialOffer(SpecialOffer offer)
        {
            specialOffers.Add(offer);
            CalculateOrderTotal();
        }

        public string ToCsvString()
        {
            string items = "";
            for (int i = 0; i < orderedItems.Count; i++)
            {
                if (i > 0) items += "|";
                items += orderedItems[i].ItemName + "," + orderedItems[i].QtyOrdered;
            }

            string offers = "";
            for (int i = 0; i < specialOffers.Count; i++)
            {
                if (i > 0) offers += "|";
                offers += specialOffers[i].OfferCode;
            }

            return orderId + "," + customer.Email + "," + restaurant.RestaurantId + "," +
                   deliveryDateTime.ToString("dd/MM/yyyy") + "," + deliveryDateTime.ToString("HH:mm") + "," +
                   deliveryAddress.Replace(",", ";") + "," + orderDateTime.ToString("dd/MM/yyyy HH:mm") + "," +
                   orderTotal.ToString("F2") + "," + orderStatus + "," + items + "," + offers;
        }

        public override string ToString()
        {
            return "Order " + orderId + ": " + customer.CustomerName + " -> " + restaurant.RestaurantName + " | " +
                   "Delivery: " + deliveryDateTime.ToString("dd/MM/yyyy HH:mm") + " | " +
                   "Amount: $" + orderTotal.ToString("F2") + " | Status: " + orderStatus;
        }
    }
}