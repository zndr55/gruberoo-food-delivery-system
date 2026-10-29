using System;
using System.Collections.Generic;

namespace GruberooApp
{
    // A customer, and the orders they have placed
    public class Customer
    {
        private string emailAddress;
        private string customerName;
        private List<Order> orders;

        public string Email
        {
            get { return emailAddress; }
            set { emailAddress = value; }
        }

        public string CustomerName
        {
            get { return customerName; }
            set { customerName = value; }
        }

        public List<Order> Orders
        {
            get { return orders; }
            set { orders = value; }
        }

        public Customer()
        {
            orders = new List<Order>();
        }

        public Customer(string name, string email)
        {
            customerName = name;
            emailAddress = email;
            orders = new List<Order>();
        }

        public void AddOrder(Order order)
        {
            orders.Add(order);
            order.Customer = this;
        }

        public bool RemoveOrder(Order order)
        {
            return orders.Remove(order);
        }

        public override string ToString()
        {
            return customerName + " (" + emailAddress + ")";
        }
    }
}