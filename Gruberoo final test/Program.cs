

using System;
using System.Collections.Generic;
using System.IO;

namespace GruberooApp
{
    class Program
    {
        // Status constants for the prog
        private const string STATUS_PENDING = "Pending";
        private const string STATUS_PREPARING = "Preparing";
        private const string STATUS_DELIVERED = "Delivered";
        private const string STATUS_CANCELLED = "Cancelled";
        private const string STATUS_REJECTED = "Rejected";

        // Payment constants for prog
        private const string PAY_CREDIT_CARD = "CC";
        private const string PAY_PAYPAL = "PP";
        private const string PAY_CASH = "CD";

        // Collections 
        private List<Restaurant> restaurants = new List<Restaurant>();
        private List<Customer> customers = new List<Customer>();
        private Stack<Order> refundStack = new Stack<Order>();
        private int nextOrderId = 1001;

        // Counters
        private int restaurantCount = 0;
        private int foodItemCount = 0;
        private int customerCount = 0;
        private int orderCount = 0;

        // File paths
        private string dataDirectory = "";
        private string restaurantsFile;
        private string foodItemsFile;
        private string specialOffersFile;
        private string customersFile;
        private string ordersFile;
        private string queueFile;
        private string stackFile;

        public Program()
        {
            // Check if Data folder exists, use it orelse just use current directory
            if (Directory.Exists("Data"))
            {
                dataDirectory = "Data";
            }
            else
            {
                dataDirectory = "";
            }

            // Build file paths
            if (dataDirectory == "")
            {
                restaurantsFile = "restaurants.csv";
                foodItemsFile = "fooditems.csv";
                specialOffersFile = "specialoffers.csv";
                customersFile = "customers.csv";
                ordersFile = "orders.csv";
                queueFile = "queue.csv";
                stackFile = "stack.csv";
            }
            else
            {
                restaurantsFile = dataDirectory + "/restaurants.csv";
                foodItemsFile = dataDirectory + "/fooditems.csv";
                specialOffersFile = dataDirectory + "/specialoffers.csv";
                customersFile = dataDirectory + "/customers.csv";
                ordersFile = dataDirectory + "/orders.csv";
                queueFile = dataDirectory + "/queue.csv";
                stackFile = dataDirectory + "/stack.csv";
            }
        }

        static void Main(string[] args)
        {
            Program app = new Program();
            app.Run();
        }

        void Run()
        {
            Console.WriteLine("Welcome to the Gruberoo Food Delivery System");

            if (Directory.Exists("Data"))
            {
                Console.WriteLine("Data folder found!");
                string[] files = Directory.GetFiles("Data");
                Console.WriteLine("Files in Data folder:");
                for (int i = 0; i < files.Length; i = i + 1)
                {
                    Console.WriteLine("  - " + files[i]);
                }
            }
            else
            {
            }
            Console.WriteLine("");
            Console.WriteLine("");

            //Basic UI
            try
            {
                LoadRestaurantsAndFoodItems();
                LoadCustomersAndOrders();

                Console.WriteLine(restaurantCount + " restaurants loaded!");
                Console.WriteLine(foodItemCount + " food items loaded!");
                Console.WriteLine(customerCount + " customers loaded!");
                Console.WriteLine(orderCount + " orders loaded!");
                Console.WriteLine("");
                Console.ReadKey();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading data: " + ex.Message);
                Console.WriteLine("");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("");
            bool running = true;
            while (running)
            {
                Console.Clear();
                DisplayMainMenu();
                string choice = Console.ReadLine();

                //Main menu
                try
                {

                    if (choice == "1")
                    {
                        ListAllRestaurantsAndMenu();
                    }
                    else if (choice == "2")
                    {
                        ListAllOrders();
                    }
                    else if (choice == "3")
                    {
                        CreateNewOrder();
                    }
                    else if (choice == "4")
                    {
                        ProcessOrder();
                    }
                    else if (choice == "5")
                    {
                        ModifyOrder();
                    }
                    else if (choice == "6")
                    {
                        DeleteOrder();
                    }
                    else if (choice == "7")
                    {
                        BulkProcessOrders();
                    }
                    else if (choice == "8")
                    {
                        DisplayTotalOrderAmount();
                    }
                    else if (choice == "9")
                    {
                        RateRestaurant();
                    }
                    else if (choice == "0")
                    {
                        SaveAndExit();
                        running = false;
                    }
                    else
                    {
                        Console.WriteLine("Invalid option! Press any key to continue...");
                        Console.ReadKey();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("");
                    Console.WriteLine("Error: " + ex.Message);
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
        }
        //menu ui
        void DisplayMainMenu()
        {
            Console.WriteLine("===== Gruberoo Food Delivery System =====");
            Console.WriteLine("1. List all restaurants and menu items");
            Console.WriteLine("2. List all orders");
            Console.WriteLine("3. Create a new order");
            Console.WriteLine("4. Process an order");
            Console.WriteLine("5. Modify an existing order");
            Console.WriteLine("6. Delete an existing order");
            Console.WriteLine("7. Bulk process orders (Advanced)");
            Console.WriteLine("8. Display total order amount (Advanced)");
            Console.WriteLine("9. Rate a restaurant (Advanced)");
            Console.WriteLine("0. Exit");
            Console.WriteLine("=========================================");
            Console.Write("Enter your choice: ");
        }
        //data loader
        void LoadRestaurantsAndFoodItems()
        {
            restaurants.Clear();
            restaurantCount = 0;
            foodItemCount = 0;

            if (!File.Exists(restaurantsFile))
            {
                throw new FileNotFoundException("Restaurants file not found");
            }

            StreamReader sr = new StreamReader(restaurantsFile);
            string header = sr.ReadLine();
            string line = sr.ReadLine();

            while (line != null)
            {
                line = line.Trim();

                if (line.Length > 0)
                {
                    string[] parts = line.Split(',');
                    if (parts.Length >= 3)
                    {

                        Restaurant r = new Restaurant(parts[0].Trim(), parts[1].Trim(), parts[2].Trim());
                        restaurants.Add(r);
                        restaurantCount = restaurantCount + 1;
                    }
                }
                line = sr.ReadLine();
            }
            sr.Close();

            if (File.Exists(foodItemsFile))
            {
                StreamReader srFood = new StreamReader(foodItemsFile);
                header = srFood.ReadLine();
                line = srFood.ReadLine();

                while (line != null)
                {

                    line = line.Trim();

                    if (line.Length > 0)
                    {
                        string[] parts = line.Split(',');
                        if (parts.Length >= 4)
                        {

                            string restId = parts[0].Trim();
                            string itemName = parts[1].Trim();
                            string description = parts[2].Trim();
                            double price = double.Parse(parts[3].Trim());

                            Restaurant restaurant = null;
                            for (int i = 0; i < restaurants.Count; i = i + 1)
                            {
                                if (restaurants[i].RestaurantId == restId)
                                {
                                    restaurant = restaurants[i];
                                    break;
                                }
                            }

                            if (restaurant != null)
                            {
                                if (restaurant.Menus.Count == 0)
                                {
                                    Menu defaultMenu = new Menu("M001", "Main Menu");
                                    restaurant.AddMenu(defaultMenu);
                                }

                                FoodItem item = new FoodItem(itemName, description, price);
                                restaurant.Menus[0].AddFoodItem(item);
                                foodItemCount = foodItemCount + 1;
                            }
                        }
                    }
                    line = srFood.ReadLine();
                }
                srFood.Close();
            }

            if (File.Exists(specialOffersFile))
            {
                StreamReader srOffer = new StreamReader(specialOffersFile);
                header = srOffer.ReadLine();
                line = srOffer.ReadLine();

                while (line != null)
                {
                    line = line.Trim();

                    if (line.Length > 0)
                    {
                        string[] parts = line.Split(',');
                        if (parts.Length >= 4)
                        {

                            string restName = parts[0].Trim();

                            Restaurant restaurant = null;
                            for (int i = 0; i < restaurants.Count; i = i + 1)
                            {
                                if (restaurants[i].RestaurantName == restName || restaurants[i].RestaurantId == restName)
                                {
                                    restaurant = restaurants[i];
                                    break;
                                }
                            }

                            if (restaurant != null)
                            {
                                double discount = 0;
                                if (parts[3].Trim() != "-")
                                {
                                    discount = double.Parse(parts[3].Trim());
                                }
                                SpecialOffer offer = new SpecialOffer(parts[1].Trim(), parts[2].Trim(), discount);
                                restaurant.SpecialOffers.Add(offer);
                            }
                        }
                    }
                    line = srOffer.ReadLine();
                }
                srOffer.Close();
            }
        }

        void LoadCustomersAndOrders()
        {
            customers.Clear();
            customerCount = 0;
            orderCount = 0;

            if (!File.Exists(customersFile))
            {
                throw new FileNotFoundException("Customers file not found");
            }

            StreamReader sr = new StreamReader(customersFile);
            string header = sr.ReadLine();
            string line = sr.ReadLine();

            while (line != null)
            {

                line = line.Trim();

                if (line.Length > 0)
                {
                    string[] parts = line.Split(',');
                    if (parts.Length >= 2)
                    {

                        Customer c = new Customer(parts[0].Trim(), parts[1].Trim());
                        customers.Add(c);
                        customerCount = customerCount + 1;
                    }
                }
                line = sr.ReadLine();
            }
            sr.Close();

            if (File.Exists(ordersFile))
            {
                StreamReader srOrder = new StreamReader(ordersFile);
                header = srOrder.ReadLine();
                line = srOrder.ReadLine();

                while (line != null)
                {

                    line = line.Trim();

                    if (line.Length > 0)
                    {
                        List<string> parts = ParseCsvLine(line);
                        if (parts.Count >= 10)
                        {
                            ParseOrderLine(parts);
                            orderCount = orderCount + 1;
                        }
                    }
                    line = srOrder.ReadLine();
                }
                srOrder.Close();
            }
        }

        List<string> ParseCsvLine(string line)
        {
            List<string> result = new List<string>();
            bool inQuotes = false;
            string currentField = "";

            for (int i = 0; i < line.Length; i = i + 1)
            {
                char c = line[i];

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(currentField);
                    currentField = "";
                }
                else
                {
                    currentField = currentField + c;
                }
            }
            result.Add(currentField);

            return result;
        }

        void ParseOrderLine(List<string> parts)
        {
            int orderId = int.Parse(parts[0]);
            string customerEmail = parts[1];
            string restaurantId = parts[2];

            string[] dateParts = parts[3].Split('/');
            int day = int.Parse(dateParts[0]);
            int month = int.Parse(dateParts[1]);
            int year = int.Parse(dateParts[2]);

            string[] timeParts = parts[4].Split(':');
            int hour = int.Parse(timeParts[0]);
            int minute = int.Parse(timeParts[1]);

            DateTime deliveryDateTime = new DateTime(year, month, day, hour, minute, 0);

            string address = parts[5].Replace(";", ",");

            string[] dateTimeParts = parts[6].Split(' ');
            string[] createdDateParts = dateTimeParts[0].Split('/');
            string[] createdTimeParts = dateTimeParts[1].Split(':');
            int createdDay = int.Parse(createdDateParts[0]);
            int createdMonth = int.Parse(createdDateParts[1]);
            int createdYear = int.Parse(createdDateParts[2]);
            int createdHour = int.Parse(createdTimeParts[0]);
            int createdMinute = int.Parse(createdTimeParts[1]);
            DateTime createdDateTime = new DateTime(createdYear, createdMonth, createdDay, createdHour, createdMinute, 0);

            double totalAmount = double.Parse(parts[7]);
            string status = parts[8];

            string itemsStr = parts[9].Replace("\"", "");
            string offersStr = "";


            if (parts.Count > 10)
            {
                offersStr = parts[10].Replace("\"", "");
            }

            Customer customer = null;
            for (int i = 0; i < customers.Count; i = i + 1)
            {
                if (customers[i].Email == customerEmail)
                {
                    customer = customers[i];
                    break;
                }
            }

            Restaurant restaurant = null;
            for (int i = 0; i < restaurants.Count; i = i + 1)
            {
                if (restaurants[i].RestaurantId == restaurantId)
                {
                    restaurant = restaurants[i];
                    break;
                }
            }

            if (customer != null && restaurant != null)
            {
                Order order = new Order();
                order.OrderId = orderId;
                order.Customer = customer;
                order.Restaurant = restaurant;
                order.DeliveryDateTime = deliveryDateTime;
                order.DeliveryAddress = address;
                order.OrderDateTime = createdDateTime;
                order.OrderTotal = totalAmount;
                order.OrderStatus = status;

                if (!string.IsNullOrEmpty(itemsStr))
                {
                    string[] itemPairs = itemsStr.Split('|');
                    for (int i = 0; i < itemPairs.Length; i = i + 1)
                    {
                        string[] itemParts = itemPairs[i].Split(',');
                        if (itemParts.Length >= 2)
                        {

                            string itemName = itemParts[0].Trim();
                            int qty = int.Parse(itemParts[1].Trim());

                            FoodItem foodItem = null;
                            for (int m = 0; m < restaurant.Menus.Count; m = m + 1)
                            {
                                for (int f = 0; f < restaurant.Menus[m].FoodItems.Count; f = f + 1)
                                {
                                    if (restaurant.Menus[m].FoodItems[f].ItemName == itemName)
                                    {
                                        foodItem = restaurant.Menus[m].FoodItems[f];
                                        break;
                                    }
                                }
                                if (foodItem != null) break;
                            }

                            if (foodItem != null)
                            {
                                OrderedFoodItem orderedItem = new OrderedFoodItem(foodItem, qty);
                                order.OrderedItems.Add(orderedItem);
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(offersStr))
                {
                    string[] offerCodes = offersStr.Split('|');
                    for (int i = 0; i < offerCodes.Length; i = i + 1)
                    {
                        string cleanCode = offerCodes[i].Trim();
                        for (int s = 0; s < restaurant.SpecialOffers.Count; s = s + 1)
                        {
                            if (restaurant.SpecialOffers[s].OfferCode == cleanCode)
                            {
                                order.SpecialOffers.Add(restaurant.SpecialOffers[s]);
                                break;
                            }
                        }
                    }
                }

                customer.AddOrder(order);

                if (status == STATUS_PENDING || status == STATUS_PREPARING)
                {
                    restaurant.OrderQueue.Enqueue(order);
                }
                else if (status == STATUS_CANCELLED || status == STATUS_REJECTED)
                {
                    refundStack.Push(order);
                }

                if (orderId >= nextOrderId)
                {
                    nextOrderId = orderId + 1;
                }
            }
        }
        //restaurant listing
        void ListAllRestaurantsAndMenu()
        {
            Console.WriteLine("");
            Console.WriteLine("All Restaurants and Menu Items");
            Console.WriteLine("==============================");

            for (int r = 0; r < restaurants.Count; r = r + 1)
            {
                Restaurant restaurant = restaurants[r];


                Console.WriteLine("Restaurant: " + restaurant.RestaurantName + " (" + restaurant.RestaurantId + ")");


                if (restaurant.Menus.Count > 0)
                {
                    Menu menu = restaurant.Menus[0];
                    for (int f = 0; f < menu.FoodItems.Count; f = f + 1)
                    {
                        FoodItem item = menu.FoodItems[f];
                        Console.WriteLine("  - " + item.ItemName + ": " + item.ItemDesc + " - $" + item.ItemPrice.ToString("F2"));
                    }
                }

                Console.WriteLine("");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        void ListAllOrders()
        {
            Console.WriteLine("");
            Console.WriteLine("========== ALL ORDERS ==========");
            Console.WriteLine(string.Format("{0,-10} {1,-20} {2,-20} {3,-20} {4,-10} {5}",
                "Order ID", "Customer", "Restaurant", "Delivery Date/Time", "Amount", "Status"));
            Console.WriteLine(new string('-', 100));


            List<Order> allOrders = new List<Order>();
            for (int c = 0; c < customers.Count; c = c + 1)
            {
                Customer customer = customers[c];
                for (int o = 0; o < customer.Orders.Count; o = o + 1)
                {
                    allOrders.Add(customer.Orders[o]);
                }
            }


            for (int i = 0; i < allOrders.Count - 1; i = i + 1)
            {
                for (int j = 0; j < allOrders.Count - i - 1; j = j + 1)
                {
                    if (allOrders[j].OrderId > allOrders[j + 1].OrderId)
                    {

                        Order temp = allOrders[j];
                        allOrders[j] = allOrders[j + 1];
                        allOrders[j + 1] = temp;
                    }
                }
            }


            for (int i = 0; i < allOrders.Count; i = i + 1)
            {
                Order order = allOrders[i];
                Console.WriteLine(string.Format("{0,-10} {1,-20} {2,-20} {3,-20} ${4,-9:F2} {5}",
                    order.OrderId,
                    order.Customer.CustomerName,
                    order.Restaurant.RestaurantName,
                    order.DeliveryDateTime.ToString("dd/MM/yyyy HH:mm"),
                    order.OrderTotal,
                    order.OrderStatus));
            }

            Console.WriteLine("");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        void CreateNewOrder()
        {
            Console.WriteLine("");
            Console.WriteLine("========== CREATE NEW ORDER ==========");
            Console.WriteLine("");

            Console.Write("Enter Customer Email: ");
            string email = Console.ReadLine();

            Customer customer = null;
            for (int i = 0; i < customers.Count; i = i + 1)
            {
                if (customers[i].Email == email)
                {
                    customer = customers[i];
                    break;
                }
            }

            if (customer == null)
            {
                Console.WriteLine("Customer not found!");
                Console.ReadKey();
                return;
            }

            Console.Write("Enter Restaurant ID: ");
            string restId = Console.ReadLine();

            Restaurant restaurant = null;
            string restIdInput = restId.Trim();

            for (int i = 0; i < restaurants.Count; i = i + 1)
            {
                if (restaurants[i].RestaurantId.Trim().ToUpper() == restIdInput.ToUpper())
                {
                    restaurant = restaurants[i];
                    break;
                }
            }

            if (restaurant == null)
            {
                Console.WriteLine("Restaurant not found!");
                Console.ReadKey();
                return;
            }

            if (restaurant.Menus.Count == 0 || restaurant.Menus[0].FoodItems.Count == 0)
            {
                Console.WriteLine("This restaurant has no menu items!");
                Console.ReadKey();
                return;
            }

            Console.Write("Enter Delivery Date (dd/MM/yyyy): ");
            string dateInput = Console.ReadLine();
            DateTime deliveryDate;

            try
            {
                string[] dateParts = dateInput.Split('/');
                int day = int.Parse(dateParts[0]);
                int month = int.Parse(dateParts[1]);
                int year = int.Parse(dateParts[2]);
                deliveryDate = new DateTime(year, month, day);
            }
            catch
            {
                Console.WriteLine("Invalid date format! Use dd/MM/yyyy");
                Console.ReadKey();
                return;
            }

            Console.Write("Enter Delivery Time (HH:mm): ");
            string timeInput = Console.ReadLine();
            DateTime deliveryTime;

            try
            {
                string[] timeParts = timeInput.Split(':');
                int hour = int.Parse(timeParts[0]);
                int minute = int.Parse(timeParts[1]);
                deliveryTime = new DateTime(1, 1, 1, hour, minute, 0);
            }
            catch
            {
                Console.WriteLine("Invalid time format! Use HH:mm");
                Console.ReadKey();
                return;
            }

            DateTime deliveryDateTime = new DateTime(deliveryDate.Year, deliveryDate.Month, deliveryDate.Day,
                                                      deliveryTime.Hour, deliveryTime.Minute, 0);

            Console.Write("Enter Delivery Address: ");
            string address = Console.ReadLine();

            Console.WriteLine("");
            Console.WriteLine("--- Available Items ---");
            Menu menu = restaurant.Menus[0];
            for (int i = 0; i < menu.FoodItems.Count; i = i + 1)
            {
                Console.WriteLine((i + 1) + ". " + menu.FoodItems[i].ToString());
            }

            Order order = new Order();
            order.Customer = customer;
            order.Restaurant = restaurant;
            order.DeliveryDateTime = deliveryDateTime;
            order.DeliveryAddress = address;

            while (true)
            {
                Console.Write("");
                Console.Write("Select item number (0 to finish): ");
                string itemInput = Console.ReadLine();
                int itemNum;

                try
                {
                    itemNum = int.Parse(itemInput);
                }
                catch
                {
                    Console.WriteLine("Invalid selection!");
                    continue;
                }

                if (itemNum < 0 || itemNum > menu.FoodItems.Count)
                {
                    Console.WriteLine("Invalid selection!");
                    continue;
                }
                if (itemNum == 0) break;

                Console.Write("Enter quantity: ");
                string qtyInput = Console.ReadLine();
                int qty;

                try
                {
                    qty = int.Parse(qtyInput);
                }
                catch
                {
                    Console.WriteLine("Invalid quantity!");
                    continue;
                }

                if (qty <= 0)
                {
                    Console.WriteLine("Invalid quantity!");
                    continue;
                }



                FoodItem selectedItem = menu.FoodItems[itemNum - 1];
                OrderedFoodItem orderedItem = new OrderedFoodItem(selectedItem, qty);
                order.AddOrderedFoodItem(orderedItem);

                Console.WriteLine("Added " + qty + "x " + selectedItem.ItemName);
            }

            if (order.OrderedItems.Count == 0)
            {
                Console.WriteLine("No items selected! Order cancelled.");
                Console.ReadKey();
                return;
            }

            Console.Write("Add special request? [Y/N]: ");
            string specialReqChoice = Console.ReadLine();
            if (specialReqChoice.ToUpper() == "Y")
            {
                Console.Write("Enter special request: ");
                string specialRequest = Console.ReadLine();

            }

            order.CalculateOrderTotal();


            double subtotalWithoutDelivery = order.OrderTotal - order.DELIVERY_FEE;
            Console.WriteLine("");
            Console.WriteLine("Subtotal: $" + subtotalWithoutDelivery.ToString("F2") + " + $" + order.DELIVERY_FEE.ToString("F2") + " (delivery) = $" + order.OrderTotal.ToString("F2"));

            if (restaurant.SpecialOffers.Count > 0)
            {
                Console.Write("Apply special offer code? [Y/N]: ");
                string offerChoice = Console.ReadLine();
                if (offerChoice.ToUpper() == "Y")
                {
                    Console.WriteLine("Available offers:");
                    for (int i = 0; i < restaurant.SpecialOffers.Count; i = i + 1)
                    {
                        Console.WriteLine("  " + restaurant.SpecialOffers[i].ToString());
                    }
                    Console.Write("Enter offer code: ");
                    string code = Console.ReadLine().ToUpper();

                    SpecialOffer selectedOffer = null;
                    for (int i = 0; i < restaurant.SpecialOffers.Count; i = i + 1)
                    {
                        if (restaurant.SpecialOffers[i].OfferCode == code)
                        {
                            selectedOffer = restaurant.SpecialOffers[i];
                            break;
                        }
                    }

                    if (selectedOffer != null)
                    {
                        order.AddSpecialOffer(selectedOffer);
                        subtotalWithoutDelivery = order.OrderTotal - order.DELIVERY_FEE;
                        Console.WriteLine("Offer applied! New total: $" + subtotalWithoutDelivery.ToString("F2") + " + $" + order.DELIVERY_FEE.ToString("F2") + " (delivery) = $" + order.OrderTotal.ToString("F2"));
                    }
                    else
                    {
                        Console.WriteLine("Invalid offer code!");
                    }
                }
            }

            Console.Write("Proceed to payment? [Y/N]: ");
            string payChoice = Console.ReadLine();
            if (payChoice.ToUpper() != "Y")
            {
                Console.WriteLine("Order cancelled.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Payment methods: [CC] Credit Card, [PP] PayPal, [CD] Cash on Delivery");
            Console.Write("Enter payment method: ");
            string payMethod = Console.ReadLine().ToUpper();

            if (payMethod != PAY_CREDIT_CARD && payMethod != PAY_PAYPAL && payMethod != PAY_CASH)
            {
                Console.WriteLine("Invalid payment method!");
                Console.ReadKey();
                return;
            }

            order.OrderPaymentMethod = payMethod;
            order.OrderPaid = true;
            order.OrderStatus = STATUS_PENDING;
            order.OrderId = nextOrderId;
            nextOrderId = nextOrderId + 1;

            customer.AddOrder(order);
            restaurant.OrderQueue.Enqueue(order);

            StreamWriter writer = new StreamWriter(ordersFile, true);
            writer.WriteLine(order.ToCsvString());
            writer.Close();

            Console.WriteLine("");
            Console.WriteLine("Order " + order.OrderId + " created successfully!");
            Console.WriteLine("Total Amount: $" + order.OrderTotal.ToString("F2"));
            Console.WriteLine("Status: " + order.OrderStatus);

            Console.ReadKey();
        }

        void ApplySpecialOfferToOrder(Order order, Restaurant restaurant)
        {
            if (restaurant.SpecialOffers.Count == 0)
            {
                Console.WriteLine("No special offers available for this restaurant.");
                return;
            }

            Console.Write("Apply special offer code? [Y/N]: ");
            string offerChoice = Console.ReadLine();
            if (offerChoice.ToUpper() != "Y")
            {
                return;
            }

            Console.WriteLine("Available offers:");
            for (int i = 0; i < restaurant.SpecialOffers.Count; i = i + 1)
            {
                Console.WriteLine(" " + restaurant.SpecialOffers[i].ToString());
            }

            Console.Write("Enter offer code: ");
            string code = Console.ReadLine();
            if (code == null)
            {
                Console.WriteLine("Invalid offer code!");
                return;
            }
            code = code.ToUpper();

            SpecialOffer selectedOffer = null;
            for (int i = 0; i < restaurant.SpecialOffers.Count; i = i + 1)
            {
                if (restaurant.SpecialOffers[i].OfferCode.ToUpper() == code)
                {
                    selectedOffer = restaurant.SpecialOffers[i];
                    break;
                }
            }

            if (selectedOffer != null)
            {
                order.AddSpecialOffer(selectedOffer);
                Console.WriteLine("Offer applied! New total: " + order.OrderTotal.ToString("C"));
            }
            else
            {
                Console.WriteLine("Invalid offer code!");
            }
        }

        void ProcessOrder()
        {
            Console.WriteLine("");
            Console.WriteLine("Process Order");
            Console.WriteLine("=============");
            Console.WriteLine("Enter Restaurant ID: ");
            string restId = Console.ReadLine();

            Restaurant restaurant = null;
            string restIdInput = restId.Trim();

            for (int i = 0; i < restaurants.Count; i = i + 1)
            {
                if (restaurants[i].RestaurantId.Trim().ToUpper() == restIdInput.ToUpper())
                {
                    restaurant = restaurants[i];
                    break;
                }
            }

            if (restaurant == null)
            {
                Console.WriteLine("Restaurant not found!");
                Console.ReadKey();
                return;
            }

            if (restaurant.OrderQueue.Count == 0)
            {
                Console.WriteLine("No orders in queue!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("");
            Queue<Order> tempQueue = new Queue<Order>();

            while (restaurant.OrderQueue.Count > 0)
            {
                Order order = restaurant.OrderQueue.Dequeue();


                Console.WriteLine("Order " + order.OrderId + ":");
                Console.WriteLine("Customer: " + order.Customer.CustomerName);
                Console.WriteLine("Ordered Items:");


                for (int i = 0; i < order.OrderedItems.Count; i = i + 1)
                {
                    OrderedFoodItem item = order.OrderedItems[i];
                    Console.WriteLine((i + 1) + ". " + item.ItemName + " - " + item.QtyOrdered);
                }

                Console.WriteLine("Delivery date/time: " + order.DeliveryDateTime.ToString("dd/MM/yyyy  HH:mm"));
                Console.WriteLine("Total Amount:  $" + order.OrderTotal.ToString("F2"));
                Console.WriteLine("Order Status: " + order.OrderStatus);
                Console.WriteLine("");

                if (order.OrderStatus == STATUS_CANCELLED)
                {
                    Console.WriteLine("[SKIPPED - Already Cancelled]");
                    Console.WriteLine("");
                    tempQueue.Enqueue(order);
                    continue;
                }

                Console.Write("[C]onfirm / [R]eject / [S]kip / [D]eliver: ");
                string action = Console.ReadLine().ToUpper();
                Console.WriteLine("");

                if (action == "C")
                {
                    if (order.OrderStatus == STATUS_PENDING)
                    {
                        order.OrderStatus = STATUS_PREPARING;
                        Console.WriteLine("Order " + order.OrderId + " confirmed. Status: " + order.OrderStatus);
                        tempQueue.Enqueue(order);
                    }
                    else
                    {
                        Console.WriteLine("Can only confirm Pending orders!");
                        tempQueue.Enqueue(order);
                    }
                }
                else if (action == "R")
                {
                    if (order.OrderStatus == STATUS_PENDING)
                    {
                        order.OrderStatus = STATUS_REJECTED;
                        refundStack.Push(order);
                        Console.WriteLine("Order " + order.OrderId + " rejected. Refund of $" + order.OrderTotal.ToString("F2") + " processed.");
                    }
                    else
                    {
                        Console.WriteLine("Can only reject Pending orders!");
                        tempQueue.Enqueue(order);
                    }
                }
                else if (action == "D")
                {
                    if (order.OrderStatus == STATUS_PREPARING)
                    {
                        order.OrderStatus = STATUS_DELIVERED;
                        Console.WriteLine("Order " + order.OrderId + " delivered. Status: " + order.OrderStatus);

                        Console.Write("Rate this restaurant (1-5, 0 to skip): ");
                        string rateInput = Console.ReadLine();
                        int rating;

                        try
                        {
                            rating = int.Parse(rateInput);
                        }
                        catch
                        {
                            rating = 0;
                        }

                        if (rating >= 1 && rating <= 5)
                        {
                            restaurant.AddRating(rating);
                            Console.WriteLine("Thank you! New rating: " + restaurant.AverageRating.ToString("F1") + "/5");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Can only deliver Preparing orders!");
                        tempQueue.Enqueue(order);
                    }
                }
                else if (action == "S")
                {
                    Console.WriteLine("Order " + order.OrderId + " skipped.");
                    tempQueue.Enqueue(order);
                }
                else
                {
                    Console.WriteLine("Invalid action. Order skipped.");
                    tempQueue.Enqueue(order);
                }
                Console.WriteLine("");
            }

            restaurant.OrderQueue = tempQueue;
            Console.WriteLine("Processing complete!");
            Console.ReadKey();
        }

        void ModifyOrder()
        {
            Console.WriteLine("");
            Console.WriteLine("========== MODIFY ORDER ==========");
            Console.WriteLine("");

            Console.Write("Enter Customer Email: ");
            string email = Console.ReadLine();

            Customer customer = null;
            for (int i = 0; i < customers.Count; i = i + 1)
            {
                if (customers[i].Email == email)
                {
                    customer = customers[i];
                    break;
                }
            }

            if (customer == null)
            {
                Console.WriteLine("Customer not found!");
                Console.ReadKey();
                return;
            }

            List<Order> pendingOrders = new List<Order>();
            for (int i = 0; i < customer.Orders.Count; i = i + 1)
            {
                if (customer.Orders[i].OrderStatus == STATUS_PENDING)
                {
                    pendingOrders.Add(customer.Orders[i]);
                }
            }

            if (pendingOrders.Count == 0)
            {
                Console.WriteLine("No pending orders to modify!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("");
            Console.WriteLine("Pending Orders:");
            for (int i = 0; i < pendingOrders.Count; i = i + 1)
            {
                Console.WriteLine("  " + pendingOrders[i].OrderId + ": " +
                                pendingOrders[i].Restaurant.RestaurantName + " - " +
                                pendingOrders[i].OrderTotal.ToString("C"));
            }

            Console.Write("Enter Order ID to modify: ");
            string orderInput = Console.ReadLine();
            int orderId;

            try
            {
                orderId = int.Parse(orderInput);
            }
            catch
            {
                Console.WriteLine("Invalid Order ID!");
                Console.ReadKey();
                return;
            }

            Order order = null;
            for (int i = 0; i < pendingOrders.Count; i = i + 1)
            {
                if (pendingOrders[i].OrderId == orderId)
                {
                    order = pendingOrders[i];
                    break;
                }
            }

            if (order == null)
            {
                Console.WriteLine("Order not found or not pending!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("");
            Console.WriteLine("Current Order Details:");
            Console.WriteLine("  Items:");
            for (int i = 0; i < order.OrderedItems.Count; i = i + 1)
            {
                Console.WriteLine("    " + order.OrderedItems[i].ToString());
            }
            Console.WriteLine("  Address: " + order.DeliveryAddress);
            Console.WriteLine("  Delivery Time: " + order.DeliveryDateTime.ToString("dd/MM/yyyy HH:mm"));
            Console.WriteLine("  Total: " + order.OrderTotal.ToString("C"));

            Console.WriteLine("");
            Console.WriteLine("What would you like to modify?");
            Console.WriteLine("1. Items");
            Console.WriteLine("2. Address");
            Console.WriteLine("3. Delivery Time");
            Console.Write("Choice: ");

            double oldTotal = order.OrderTotal;
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                ModifyOrderItems(order);
            }
            else if (choice == "2")
            {
                Console.Write("Enter new address: ");
                order.DeliveryAddress = Console.ReadLine();
                Console.WriteLine("");
                Console.WriteLine("Order modified successfully!");
                Console.WriteLine("New Address: " + order.DeliveryAddress);
                Console.ReadKey();
                return;
            }
            else if (choice == "3")
            {
                Console.Write("Enter new delivery date (dd/MM/yyyy): ");
                string newDateStr = Console.ReadLine();
                DateTime newDate;

                try
                {
                    string[] dateParts = newDateStr.Split('/');
                    int day = int.Parse(dateParts[0]);
                    int month = int.Parse(dateParts[1]);
                    int year = int.Parse(dateParts[2]);
                    newDate = new DateTime(year, month, day);
                }
                catch
                {
                    Console.WriteLine("Invalid date!");
                    Console.ReadKey();
                    return;
                }

                Console.Write("Enter new delivery time (HH:mm): ");
                string newTimeStr = Console.ReadLine();
                DateTime newTime;

                try
                {
                    string[] timeParts = newTimeStr.Split(':');
                    int hour = int.Parse(timeParts[0]);
                    int minute = int.Parse(timeParts[1]);
                    newTime = new DateTime(1, 1, 1, hour, minute, 0);
                }
                catch
                {
                    Console.WriteLine("Invalid time!");
                    Console.ReadKey();
                    return;
                }

                order.DeliveryDateTime = new DateTime(newDate.Year, newDate.Month, newDate.Day,
                                                       newTime.Hour, newTime.Minute, 0);
            }

            order.CalculateOrderTotal();

            if (order.OrderTotal > oldTotal)
            {
                double diff = order.OrderTotal - oldTotal;
                Console.WriteLine("");
                Console.WriteLine("Additional payment required: " + diff.ToString("C"));
                Console.Write("Pay now? [Y/N]: ");
                string payConfirm = Console.ReadLine();
                if (payConfirm.ToUpper() != "Y")
                {
                    Console.WriteLine("Modification cancelled!");
                    Console.ReadKey();
                    return;
                }
            }

            UpdateOrdersFile();
            Console.WriteLine("");
            Console.WriteLine("Order modified successfully!");
            Console.WriteLine("New Delivery Date/Time: " + order.DeliveryDateTime.ToString("dd/MM/yyyy HH:mm"));
            Console.WriteLine("New Total: " + order.OrderTotal.ToString("C"));
            Console.ReadKey();
        }

        void ModifyOrderItems(Order order)
        {
            Console.WriteLine("");
            Console.WriteLine("--- Current Items ---");
            for (int i = 0; i < order.OrderedItems.Count; i = i + 1)
            {
                Console.WriteLine((i + 1) + ". " + order.OrderedItems[i].ToString());
            }

            Console.WriteLine("");
            Console.WriteLine("1. Add item");
            Console.WriteLine("2. Remove item");
            Console.WriteLine("3. Change quantity");
            Console.Write("Choice: ");

            string choice = Console.ReadLine();

            if (choice == "1")
            {
                Menu menu = order.Restaurant.Menus[0];
                Console.WriteLine("");
                Console.WriteLine("--- Available Items ---");
                for (int i = 0; i < menu.FoodItems.Count; i = i + 1)
                {
                    Console.WriteLine((i + 1) + ". " + menu.FoodItems[i].ToString());
                }

                Console.Write("Select item: ");
                string addInput = Console.ReadLine();
                int addIdx;

                try
                {
                    addIdx = int.Parse(addInput);
                }
                catch
                {
                    Console.WriteLine("Invalid selection!");
                    return;
                }

                if (addIdx > 0 && addIdx <= menu.FoodItems.Count)
                {
                    Console.Write("Quantity: ");
                    string qtyInput = Console.ReadLine();
                    int qty;

                    try
                    {
                        qty = int.Parse(qtyInput);
                    }
                    catch
                    {
                        Console.WriteLine("Invalid quantity!");
                        return;
                    }

                    if (qty > 0)
                    {
                        FoodItem selectedItem = menu.FoodItems[addIdx - 1];
                        OrderedFoodItem newItem = new OrderedFoodItem(selectedItem, qty);
                        order.AddOrderedFoodItem(newItem);
                        Console.WriteLine("Item added successfully!");
                    }
                }
            }
            else if (choice == "2")
            {
                Console.Write("Enter item number to remove: ");
                string remInput = Console.ReadLine();
                int remIdx;

                try
                {
                    remIdx = int.Parse(remInput);
                }
                catch
                {
                    Console.WriteLine("Invalid number!");
                    return;
                }

                if (remIdx > 0 && remIdx <= order.OrderedItems.Count)
                {
                    order.OrderedItems.RemoveAt(remIdx - 1);
                    order.CalculateOrderTotal();
                    Console.WriteLine("Item removed successfully!");
                }
            }
            else if (choice == "3")
            {
                Console.Write("Enter item number to modify: ");
                string modInput = Console.ReadLine();
                int modIdx;

                try
                {
                    modIdx = int.Parse(modInput);
                }
                catch
                {
                    Console.WriteLine("Invalid number!");
                    return;
                }

                if (modIdx > 0 && modIdx <= order.OrderedItems.Count)
                {
                    Console.Write("Enter new quantity: ");
                    string qtyInput = Console.ReadLine();
                    int newQty;

                    try
                    {
                        newQty = int.Parse(qtyInput);
                    }
                    catch
                    {
                        Console.WriteLine("Invalid quantity!");
                        return;
                    }

                    if (newQty > 0)
                    {
                        order.OrderedItems[modIdx - 1].QtyOrdered = newQty;
                        order.CalculateOrderTotal();
                        Console.WriteLine("Quantity updated successfully!");
                    }
                }
            }
        }

        void DeleteOrder()
        {
            Console.WriteLine("");
            Console.WriteLine("========== DELETE/CANCEL ORDER ==========");
            Console.WriteLine("");

            Console.Write("Enter Customer Email: ");
            string email = Console.ReadLine();

            Customer customer = null;
            for (int i = 0; i < customers.Count; i = i + 1)
            {
                if (customers[i].Email == email)
                {
                    customer = customers[i];
                    break;
                }
            }

            if (customer == null)
            {
                Console.WriteLine("Customer not found!");
                Console.ReadKey();
                return;
            }

            List<Order> pendingOrders = new List<Order>();
            for (int i = 0; i < customer.Orders.Count; i = i + 1)
            {
                if (customer.Orders[i].OrderStatus == STATUS_PENDING)
                {
                    pendingOrders.Add(customer.Orders[i]);
                }
            }

            if (pendingOrders.Count == 0)
            {
                Console.WriteLine("No pending orders to delete!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("");
            Console.WriteLine("Pending Orders:");
            for (int i = 0; i < pendingOrders.Count; i = i + 1)
            {
                Console.WriteLine("  " + pendingOrders[i].OrderId + ": " +
                                pendingOrders[i].Restaurant.RestaurantName + " - " +
                                pendingOrders[i].OrderTotal.ToString("C"));
            }

            Console.Write("Enter Order ID to cancel: ");
            string idInput = Console.ReadLine();
            int orderId;

            try
            {
                orderId = int.Parse(idInput);
            }
            catch
            {
                Console.WriteLine("Invalid Order ID!");
                Console.ReadKey();
                return;
            }

            Order order = null;
            for (int i = 0; i < pendingOrders.Count; i = i + 1)
            {
                if (pendingOrders[i].OrderId == orderId)
                {
                    order = pendingOrders[i];
                    break;
                }
            }

            if (order == null)
            {
                Console.WriteLine("Order not found or not pending!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("");
            Console.WriteLine("Order Details:");
            Console.WriteLine("  Order ID: " + order.OrderId);
            Console.WriteLine("  Restaurant: " + order.Restaurant.RestaurantName);
            Console.WriteLine("  Amount: " + order.OrderTotal.ToString("C"));

            Console.Write("Confirm cancellation? [Y/N]: ");
            string confirm = Console.ReadLine();
            if (confirm.ToUpper() == "Y")
            {
                order.OrderStatus = STATUS_CANCELLED;
                refundStack.Push(order);
                UpdateOrdersFile();
                Console.WriteLine("");
                Console.WriteLine("Order cancelled successfully! Refund will be processed.");
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("Cancellation aborted.");
            }

            Console.ReadKey();
        }

        void BulkProcessOrders()
        {
            Console.WriteLine("");
            Console.WriteLine("========== BULK PROCESS ORDERS ==========");
            Console.WriteLine("");

            DateTime now = DateTime.Now;

            int totalProcessed = 0;
            int preparingCount = 0;
            int rejectedCount = 0;
            int totalOrders = 0;

            for (int i = 0; i < restaurants.Count; i = i + 1)
            {
                totalOrders = totalOrders + restaurants[i].OrderQueue.Count;
            }

            if (totalOrders == 0)
            {
                Console.WriteLine("No pending orders to process!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Total pending orders: " + totalOrders);
            Console.WriteLine("Processing...");
            Console.WriteLine("");

            for (int r = 0; r < restaurants.Count; r = r + 1)
            {
                Restaurant restaurant = restaurants[r];
                Queue<Order> tempQueue = new Queue<Order>();

                while (restaurant.OrderQueue.Count > 0)
                {
                    Order order = restaurant.OrderQueue.Dequeue();

                    if (order.OrderStatus == STATUS_PENDING)
                    {
                        totalProcessed = totalProcessed + 1;

                        TimeSpan diff = order.DeliveryDateTime - now;

                        if (diff.TotalHours < 1)
                        {
                            order.OrderStatus = STATUS_REJECTED;
                            refundStack.Push(order);
                            rejectedCount = rejectedCount + 1;
                            Console.WriteLine("Order " + order.OrderId + " - REJECTED (Delivery too soon)");
                        }
                        else
                        {
                            order.OrderStatus = STATUS_PREPARING;
                            preparingCount = preparingCount + 1;
                            Console.WriteLine("Order " + order.OrderId + " - CONFIRMED (Preparing)");
                            tempQueue.Enqueue(order);
                        }
                    }
                    else
                    {
                        tempQueue.Enqueue(order);
                    }
                }

                restaurant.OrderQueue = tempQueue;
            }

            Console.WriteLine("");
            Console.WriteLine("========== SUMMARY ==========");
            Console.WriteLine("Total orders processed: " + totalProcessed);
            Console.WriteLine("Preparing: " + preparingCount);
            Console.WriteLine("Rejected: " + rejectedCount);

            if (totalOrders > 0)
            {
                double percentage = ((double)totalProcessed / totalOrders) * 100;
                Console.WriteLine("Processing rate: " + percentage.ToString("F1") + "%");
            }

            Console.WriteLine("");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        void DisplayTotalOrderAmount()
        {
            Console.WriteLine("");
            Console.WriteLine("========== FINANCIAL SUMMARY ==========");
            Console.WriteLine("");

            double grandTotal = 0;
            double totalRefunds = 0;

            for (int r = 0; r < restaurants.Count; r = r + 1)
            {
                Restaurant restaurant = restaurants[r];
                Console.WriteLine("Restaurant: " + restaurant.RestaurantName);

                double restaurantTotal = 0;
                int deliveredCount = 0;

                for (int c = 0; c < customers.Count; c = c + 1)
                {
                    Customer customer = customers[c];
                    for (int o = 0; o < customer.Orders.Count; o = o + 1)
                    {
                        Order order = customer.Orders[o];
                        if (order.Restaurant.RestaurantId == restaurant.RestaurantId && order.OrderStatus == STATUS_DELIVERED)
                        {
                            restaurantTotal = restaurantTotal + (order.OrderTotal - order.DELIVERY_FEE);
                            deliveredCount = deliveredCount + 1;
                        }
                    }
                }

                double restaurantRefunds = 0;
                int refundCount = 0;

                for (int c = 0; c < customers.Count; c = c + 1)
                {
                    Customer customer = customers[c];
                    for (int o = 0; o < customer.Orders.Count; o = o + 1)
                    {
                        Order order = customer.Orders[o];
                        if (order.Restaurant.RestaurantId == restaurant.RestaurantId &&
                            (order.OrderStatus == STATUS_CANCELLED || order.OrderStatus == STATUS_REJECTED))
                        {
                            restaurantRefunds = restaurantRefunds + order.OrderTotal;
                            refundCount = refundCount + 1;
                        }
                    }
                }

                Console.WriteLine("  Successful Orders: " + deliveredCount);
                Console.WriteLine("  Total Sales (less delivery): " + restaurantTotal.ToString("C"));
                Console.WriteLine("  Refunded Orders: " + refundCount);
                Console.WriteLine("  Total Refunds: " + restaurantRefunds.ToString("C"));
                Console.WriteLine("");

                grandTotal = grandTotal + restaurantTotal;
                totalRefunds = totalRefunds + restaurantRefunds;
            }

            Console.WriteLine("========== OVERALL SUMMARY ==========");
            Console.WriteLine("Total Sales (all restaurants): " + grandTotal.ToString("C"));
            Console.WriteLine("Total Refunds: " + totalRefunds.ToString("C"));

            double gruberooEarnings = grandTotal * 0.30;
            Console.WriteLine("Gruberoo Commission (30%): " + gruberooEarnings.ToString("C"));
            Console.WriteLine("Net to Restaurants: " + (grandTotal - gruberooEarnings).ToString("C"));

            Console.WriteLine("");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        void RateRestaurant()
        {
            Console.WriteLine("");
            Console.WriteLine("========== RATE RESTAURANT ==========");
            Console.WriteLine("");

            Console.Write("Enter Restaurant ID: ");
            string restId = Console.ReadLine();

            Restaurant restaurant = null;
            string search = restId.Trim();

            for (int i = 0; i < restaurants.Count; i = i + 1)
            {

                if (restaurants[i].RestaurantId.Trim().ToUpper() == search.ToUpper() ||
                    restaurants[i].RestaurantName.Trim().ToUpper() == search.ToUpper())
                {
                    restaurant = restaurants[i];
                    break;
                }
            }


            if (restaurant == null)
            {
                Console.WriteLine("Restaurant not found!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Restaurant: " + restaurant.RestaurantName);
            if (restaurant.TotalRatings > 0)
            {
                Console.WriteLine("Current Rating: " + restaurant.AverageRating.ToString("F1") +
                                "/5 (" + restaurant.TotalRatings + " reviews)");
            }
            else
            {
                Console.WriteLine("No ratings yet.");
            }

            Console.Write("Enter your rating (1-5): ");
            string rateInput = Console.ReadLine();
            int rating;

            try
            {
                rating = int.Parse(rateInput);
            }
            catch
            {
                Console.WriteLine("Invalid rating! Must be between 1 and 5.");
                Console.ReadKey();
                return;
            }

            if (rating < 1 || rating > 5)
            {
                Console.WriteLine("Invalid rating! Must be between 1 and 5.");
                Console.ReadKey();
                return;
            }

            restaurant.AddRating(rating);
            Console.WriteLine("");
            Console.WriteLine("Thank you! New average rating: " + restaurant.AverageRating.ToString("F1") + "/5");

            Console.ReadKey();
        }

        void UpdateOrdersFile()
        {
            StreamWriter sw = new StreamWriter(ordersFile, false);
            sw.WriteLine("OrderId,CustomerEmail,RestaurantId,DeliveryDate,DeliveryTime,DeliveryAddress,CreatedDateTime,TotalAmount,Status,Items");

            for (int c = 0; c < customers.Count; c = c + 1)
            {
                Customer customer = customers[c];
                for (int o = 0; o < customer.Orders.Count; o = o + 1)
                {
                    sw.WriteLine(customer.Orders[o].ToCsvString());
                }
            }
            sw.Close();
        }

        void SaveAndExit()
        {
            Console.WriteLine("");
            Console.WriteLine("========== SAVING DATA ==========");
            Console.WriteLine("");

            try
            {
                StreamWriter sw = new StreamWriter(queueFile, false);
                sw.WriteLine("OrderId,CustomerEmail,RestaurantId,Status");
                for (int r = 0; r < restaurants.Count; r = r + 1)
                {
                    Restaurant restaurant = restaurants[r];
                    Queue<Order> tempQueue = new Queue<Order>();
                    while (restaurant.OrderQueue.Count > 0)
                    {
                        Order order = restaurant.OrderQueue.Dequeue();
                        tempQueue.Enqueue(order);
                        sw.WriteLine(order.OrderId + "," + order.Customer.Email + "," +
                                    order.Restaurant.RestaurantId + "," + order.OrderStatus);
                    }
                    restaurant.OrderQueue = tempQueue;
                }
                sw.Close();
                Console.WriteLine("Saved order queues to " + queueFile);

                sw = new StreamWriter(stackFile, false);
                sw.WriteLine("OrderId,CustomerEmail,RestaurantId,Status,Amount");

                List<Order> tempList = new List<Order>();
                while (refundStack.Count > 0)
                {
                    Order order = refundStack.Pop();
                    tempList.Add(order);
                    sw.WriteLine(order.OrderId + "," + order.Customer.Email + "," +
                                order.Restaurant.RestaurantId + "," + order.OrderStatus + "," +
                                order.OrderTotal.ToString("F2"));
                }

                for (int i = tempList.Count - 1; i >= 0; i = i - 1)
                {
                    refundStack.Push(tempList[i]);
                }
                sw.Close();
                Console.WriteLine("Saved refund stack to " + stackFile);

                UpdateOrdersFile();
                Console.WriteLine("Updated orders file");

                Console.WriteLine("");
                Console.WriteLine("Data saved successfully! Goodbye!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving data: " + ex.Message);
            }

            Console.WriteLine("");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}