using P0Library.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace P0Store
{
    class StorePage
    {
        //MAIN CONFLICTS, moved relevant methods and info to Program.cs
        static void Main(string[] args)
        {
            bool validinput = false;
            //while loop to repeat so long as input does not match correct options
            while(validinput == false)
            {
                Console.WriteLine("Would you like to: " +
                    "\n[1] View all products" +
                    "\n[2] Make an order" +
                    "\n[3] Search Customers" +
                    "\n[4] View Order History by Store" +
                    "\n[5] View Order History By Customer");
                string input = Console.ReadLine();
                if (input == "1")
                {
                    ListProducts();
                    validinput = true;
                }
                else if (input == "2")
                {
                    MakeOrder();
                    validinput = true;
                }
                else if (input == "3")
                {
                    SearchCustomers();
                    validinput = true;
                }
                else if (input == "4")
                {
                    StoreOrderHistory();
                    validinput = true;
                }
                else if (input == "5")
                {
                    CustomerOrderHistory();
                    validinput = true;
                }
                else
                {
                    Console.WriteLine("Invalid input, please choose one of the following according to the displayed numbers:\n");
                    validinput = false;
                }
            }
            Console.WriteLine("Thank you for shopping!");
            Console.ReadKey();
            Environment.Exit(0);
        }
        //list all products
        public static void ListProducts()
        {
            using (var context = new Project0DbContext())
            {
                //gets list of products from database       //HOW TO GRAB NAMES AND THEN DISPLAY THEM
                var productList = context.Products.Find().Name ;
                //prints list of products to console  ***  NEED TO TEST TO ENSURE THIS DISPLAYS PROPERLY OR IF MODIFICATIONS ARE NEEDED ***
                Console.WriteLine(productList);
            }
        }
        //make an order
        public static void MakeOrder()
        {
            using (var context = new Project0DbContext())
            {
                //(ASK FOR STORE TO ORDER FROM FIRST -IF HAVE THE TIME-, THEN A/F ORDERING REDUCE THAT STORE'S STOCK OF ITEM(S)) *****
                bool continueOrder;
                decimal orderCost = 0;
                do
                {
                    Console.WriteLine("Select product to order: ");
                    //get list of products
                    var products = context.Products.ToList();
                    //display products as numbered options
                    foreach (var product in products)
                    {
                        int optionCount = 1;
                        Console.WriteLine("[" + optionCount + "] " + product);
                    }
                    //user selects item to add to order - try catch
                    try
                    {
                        int orderSelect = Convert.ToInt32(Console.ReadLine());
                        //get price of products according to input id with linq
                        Products productPrice = products.First(p => p.Id == orderSelect)/*.Price*/;
                        decimal chosenProductPrice = productPrice.Price;
                        //add price to variable for order summary display INSTEAD NOT using switch case
                        orderCost += chosenProductPrice;
                    }
                    //throws exception with invalid input
                    catch (Exception)
                    {
                        Console.WriteLine("Invalid input, please try ordering again.");
                    }
                    //checks if user would like to add more items to the order
                    Console.WriteLine("Would you like to continue ordering? (y/n)");
                    string continueOrderInput = Console.ReadLine();
                    if (continueOrderInput == "y" || continueOrderInput == "Y")
                    {
                        continueOrder = true;
                    }
                    else
                    {
                        continueOrder = false;
                    }
                    //breaks out of ordering loop when user is finished
                } while (continueOrder == true);
            }
        }
        //search customers by name
        public static void SearchCustomers()
        {
            using (var context = new Project0DbContext())
            {
                //get search
                Console.WriteLine("Enter a name to search for: ");
                string custSearchInput = Console.ReadLine();

            }
        }
    }
}