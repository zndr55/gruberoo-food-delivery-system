# Gruberoo Food Delivery System

A C# console app for my Programming 2 module (Jan to Feb 2026), done as a team of 2.

It handles restaurants, menus, food items, customers, special offers and orders using classes (`OrderedFoodItem` inherits from `FoodItem`), plus a queue for orders waiting to be processed and a stack for refunds.

Menu options:

1. List restaurants and menu items
2. List all orders
3. Create an order
4. Process an order
5. Modify an order
6. Delete an order
7. Bulk process orders
8. Display total order amount
9. Rate a restaurant

## Running it

You need the .NET 8 SDK. The CSV files from the module (`restaurants`, `fooditems`, `specialoffers`, `customers`, `orders`) are not included. Put them in the `Gruberoo final test` folder, then run `dotnet run` from there.
