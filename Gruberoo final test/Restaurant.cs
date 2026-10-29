using System;
using System.Collections.Generic;

namespace GruberooApp
{
    // A restaurant with its menus and the orders it receives
    public class Restaurant
    {
        private string restaurantId;
        private string restaurantName;
        private string restaurantEmail;
        private List<Menu> menus;
        private List<SpecialOffer> specialOffers;
        private Queue<Order> orderQueue;
        private double averageRating;
        private int totalRatings;

        public string RestaurantId
        {
            get { return restaurantId; }
            set { restaurantId = value; }
        }

        public string RestaurantName
        {
            get { return restaurantName; }
            set { restaurantName = value; }
        }

        public string RestaurantEmail
        {
            get { return restaurantEmail; }
            set { restaurantEmail = value; }
        }

        public List<Menu> Menus
        {
            get { return menus; }
            set { menus = value; }
        }

        public List<SpecialOffer> SpecialOffers
        {
            get { return specialOffers; }
            set { specialOffers = value; }
        }

        public Queue<Order> OrderQueue
        {
            get { return orderQueue; }
            set { orderQueue = value; }
        }

        public double AverageRating
        {
            get { return averageRating; }
            set { averageRating = value; }
        }

        public int TotalRatings
        {
            get { return totalRatings; }
            set { totalRatings = value; }
        }

        public Restaurant()
        {
            menus = new List<Menu>();
            specialOffers = new List<SpecialOffer>();
            orderQueue = new Queue<Order>();
            averageRating = 0;
            totalRatings = 0;
        }

        public Restaurant(string id, string name, string email)
        {
            restaurantId = id;
            restaurantName = name;
            restaurantEmail = email;
            menus = new List<Menu>();
            specialOffers = new List<SpecialOffer>();
            orderQueue = new Queue<Order>();
            averageRating = 0;
            totalRatings = 0;
        }

        public void AddMenu(Menu menu)
        {
            menus.Add(menu);
        }

        public bool RemoveMenu(Menu menu)
        {
            return menus.Remove(menu);
        }

        public void DisplayMenus()
        {
            Console.WriteLine("\nMenus for " + restaurantName + ":");
            if (menus.Count == 0)
            {
                Console.WriteLine("  (No menus available)");
            }
            else
            {
                foreach (var menu in menus)
                {
                    Console.WriteLine("  " + menu.ToString());
                }
            }
        }

        public void DisplaySpecialOffers()
        {
            Console.WriteLine("\nSpecial Offers for " + restaurantName + ":");
            if (specialOffers.Count == 0)
            {
                Console.WriteLine("  (No special offers available)");
            }
            else
            {
                foreach (var offer in specialOffers)
                {
                    Console.WriteLine("  " + offer.ToString());
                }
            }
        }

        public void AddRating(int rating)
        {
            if (rating >= 1 && rating <= 5)
            {
                double totalScore = (averageRating * totalRatings) + rating;
                totalRatings++;
                averageRating = totalScore / totalRatings;
            }
        }

        public override string ToString()
        {
            string ratingInfo = totalRatings > 0 ? " | Rating: " + averageRating.ToString("F1") + "/5 (" + totalRatings + " reviews)" : "";
            return "[" + restaurantId + "] " + restaurantName + " - " + restaurantEmail + ratingInfo;
        }
    }
}