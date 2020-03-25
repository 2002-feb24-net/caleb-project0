using Microsoft.Data.SqlClient;
using P0Library.Model;
using System;
using System.Linq;

namespace P0Store
{
    class Program
    {
        static void Main(string[] args)
        {
            DbSetup();
            Console.WriteLine("Are you a returning customer? (y/n)");
            string initResponse = Console.ReadLine();
            //continues to repeat as long as input is not correct
            //do
            //{
            int custId = 1;
                if (initResponse == "y" || initResponse == "Y")
                {
                //calls login method, login returns customer id for makeorder to put into new order item
                custId = Login();
                }
                else if (initResponse == "n" || initResponse == "N")
                {
                    Console.WriteLine("Would you like to create an account? (y/n)");
                    initResponse = Console.ReadLine();
                    if (initResponse == "y" || initResponse == "Y")
                    {
                        //calls create account method
                        CreateAccount();
                    }
                    //will continue to CallMethods() / 'storepage' even w/o creating account, so no need to use else if initResponse == "n"
                    else
                    {
                        Console.WriteLine("Invalid input. Please enter y/n");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter y/n");
                }
            //} while (initResponse != "y" || initResponse != "Y" || initResponse != "n" || initResponse != "N");
            // *************  user is now on a 'store page'
            bool repeat = true;
            //while loop to repeat so long as input does not match correct options, calls all critical store methods
            while (repeat == true)
            {
                CallMethods(custId);
                //prompts user for another cycle
                Console.WriteLine("Would you like to do something else? (y/n)");
                try
                {
                    string repeatInput = Console.ReadLine();
                    if (repeatInput == "y" || repeatInput == "Y")
                    {
                        repeat = true;
                    }
                    //user declines repeat cycle
                    else if (repeatInput == "n" || repeatInput == "N")
                    {
                        repeat = false;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Invalid input. Please enter y/n");
                }
            }
            Console.WriteLine("Thank you for shopping!");
            Console.ReadKey();
            Environment.Exit(0);
        }                                      //***********END MAIN***********************
        //************CALLS THE VARIOUS METHODS***********************************
        public static void CallMethods(int customerId)
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
            }
            else if (input == "2")
            {
                //login return customer id for makeorder to put into new order item
                MakeOrder(customerId);
            }
            else if (input == "3")
            {
                SearchCustomers();
            }
            else if (input == "4")
            {
                StoreOrderHistory();
            }
            else if (input == "5")
            {
                CustomerOrderHistory();
            }
            else
            {
                Console.WriteLine("Invalid input, please choose one of the following according to the displayed numbers:\n");
            }
        }
        //login to existing account********************************************************************************************************
        public static int Login()
        {
            using (var context = new Project0DbContext())
            {
                var pwdCustId = -1;
                //while the username and password are not found to match any pair in the database, repeat WHILE LOOP DIDN'T SEEM TO REALIZE THAT pwdCustId was changing, and kept looping despite pwdCustId > 0 message displaying
                while (pwdCustId < 0)
                {
                    Console.WriteLine("Username: ");
                    string usernameInput = Console.ReadLine();
                    //check to see if/where username input is in the database  -- gets single result
                    var usernameMatch = context.Customers.Where(u => u.Username == usernameInput).Single().Username;
                    Console.WriteLine("Password: ");
                    string passwordInput = Console.ReadLine();
                    //gets password in database
                    var passwordMatch = context.Customers.First(p => p.Password == passwordInput).Password;

                    //get customer id for both username and password and confirm that they match
                    pwdCustId = context.Customers.First(i => i.Password == passwordMatch && i.Username == usernameMatch).Id;
                    //confirm username matches with password
                    //if pwdCustId is found in database, the Id will be greater than 0, and SHOULD break out of while loop
                    if (pwdCustId >= 0)
                    {
                        Console.WriteLine("Thank you, and welcome.");
                    }
                    else
                    {
                        Console.WriteLine("That username and password does not match our records, please try again.");
                    }
                }
                return pwdCustId;
            }
        }
        //create new acccount********************************************************************************************************
        public static string CreateAccount()
        {
            //call to database
            using (var context = new Project0DbContext())
            {
                string usernameInput = "";
                bool newUsername = false;
                //while loop allows for repeated username attempts
                while (newUsername == false)
                {
                    //get username input
                    Console.WriteLine("Username: ");
                    usernameInput = Console.ReadLine();
                    //tries to find a match for username input already in the database
                    try
                    {
                        var user = context.Customers.Where(u => u.Username == usernameInput).Single().Username;
                        //if a user is found
                        if (user != null)
                        {
                            //indicates that username is already taken
                            Console.WriteLine("That username is already taken, please try again.");
                            //continues username while loop
                            newUsername = false;
                        }
                    }
                    //if input username is not found in the database, it will throw an InvalidOperationException because user will be null
                    catch (InvalidOperationException)
                    {
                        //breaks out of username while loop
                        newUsername = true;
                    }
                }
                //get input password
                Console.WriteLine("Password: ");
                string passwordInput = Console.ReadLine();
                //need to get id to add number to so primary key doesn't default to 0
                var idCount = context.Customers.Count()+1; // need to add 1 because c# index starts at 0, and sql index starts at 1
                //puts input usr & pwd into new customer item
                var customer = new Customers
                {
                    //adds 1 to total count of customers to get new primary key for new customer
                    Id = idCount++,
                    Username = usernameInput,
                    Password = passwordInput
                };
                //add new username and password to a new customer in the database
                context.Customers.Add(customer);
                //saves changes to database
                context.SaveChanges();
                Console.WriteLine("Welcome, " + usernameInput + "! Your account has been created.");

                //returns username for greeting
                return usernameInput;
            }
        }
        //list all products********************************************************************************************************
        public static void ListProducts()
        {
            using (var context = new Project0DbContext())
            {
                //gets list of products from database
                var productList = context.Products;
                //prints list of product names and prices to console
                foreach(var product in productList)
                {
                    //formatted to display price in currency format
                    Console.WriteLine(product.Name + "  " + String.Format("{0:C}", product.Price));
                }
            }
        }
        //make an order********************************************************************************************************
        public static void MakeOrder(int customerId)
        {
            using (var context = new Project0DbContext())
            {
                //(ASK FOR STORE TO ORDER FROM FIRST -IF HAVE THE TIME-, THEN A/F ORDERING REDUCE THAT STORE'S STOCK OF ITEM(S)) *****
                Console.WriteLine("Which store would you like to order from?");
                //gets list of stores from database
                var storeList = context.Stores;
                //prints list of store names and prices to console as numbered options
                foreach (var store in storeList)
                {
                    //formatted to display price in currency format
                    Console.WriteLine("[" + store.Id + "] " + store.Address);
                }
                //gets store of choice from user
                int storeChoice = Convert.ToInt32(Console.ReadLine());
                //after choosing store, can then choose items to order
                bool continueOrder;
                decimal orderCost = 0;
                int orderSelect = 0;
                do
                {
                    Console.WriteLine("Select product to order: ");
                    //gets list of products from database
                    var productList = context.Products;
                    //prints list of product names and prices to console as numbered options
                    foreach (var product in productList)
                    {
                        //formatted to display price in currency format
                        Console.WriteLine("[" + product.Id + "] " + product.Name + "  " + String.Format("{0:C}", product.Price));
                    }
                    //user selects item to add to order - try catch
                    try
                    {
                        orderSelect = Convert.ToInt32(Console.ReadLine());
                        //get price of products according to input id with linq
                        Products productPrice = productList.First(p => p.Id == orderSelect)/*.Price*/;
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
                Console.WriteLine("The total cost of your order is " + String.Format("{0:C}", orderCost));
                //create new order
                var neworder = new Orders
                {
                    Id = context.Orders.Count() + 1,
                    ProductId = orderSelect,
                    CustomerId = customerId,
                    StoreId = storeChoice,
                    Price = orderCost,
                    Time = DateTime.Now
                };
                //add order to customer
                context.Orders.Add(neworder);
                //save changes to database
                context.SaveChanges();
            }
        }
        //search customers by name********************************************************************************************************
        public static void SearchCustomers()
        {
            using (var context = new Project0DbContext())
            {
                //get search
                Console.WriteLine("Enter a name to search for: ");
                string custSearchInput = Console.ReadLine();
                //check search against the database
                try
                {
                    var user = context.Customers.Where(u => u.Username == custSearchInput).Single().Username;
                    //if a user is found
                    if (user != null)
                    {
                        //indicates that username is in the database
                        Console.WriteLine("That username is present!");
                    }
                }
                //if input username is not found in the database, it will throw an InvalidOperationException because user will be null
                catch (InvalidOperationException)
                {
                    //indicates username not found
                    Console.WriteLine("There was no similar username found, try again?");
                }
            }
        }
        //order history by store********************************************************************************************************
        public static void StoreOrderHistory()
        {
            using (var context = new Project0DbContext())
            {
                Console.WriteLine("Select a store to search: ");
                //gets list of stores from database
                var storeList = context.Stores;
                //prints list of stores to console
                foreach (var store in storeList)
                {
                    Console.WriteLine("[" + store.Id + "]  " + store.Address);
                }
                //gets store of choice from user
                int storeChoice = Convert.ToInt32(Console.ReadLine());
                //selects db info according to input store
                var storeOrderList = context.Orders.Where(s => s.StoreId == storeChoice);
                //using foreach instead of switch case to display order details by store
                foreach (var order in storeOrderList)
                {
                    Console.WriteLine("Customer ID-" + order.CustomerId + ", Product ID-" + order.ProductId + ", Price-" + order.Price);
                }
            }
        }
        //view order history of a customer********************************************************************************************************
        public static void CustomerOrderHistory()
        {
            using (var context = new Project0DbContext())
            {
                Console.WriteLine("Select a customer to view their order history: ");
                //gets list of customers from database
                var customerList = context.Customers;
                //prints list customers to console
                foreach (var customer in customerList)
                {
                    Console.WriteLine("[" + customer.Id + "]  " + customer.Username);
                }
                //gets customer of choice from user
                int customerChoice = Convert.ToInt32(Console.ReadLine());
                //selects db info according to input customer
                var orderList = context.Orders.Where(s => s.CustomerId == customerChoice);
                //using foreach instead of switch case to display order details by customer
                foreach (var order in orderList)
                {
                    Console.WriteLine("Customer ID-" + order.CustomerId + 
                                        ", Product ID-" + order.ProductId + 
                                        ", Store ID-" + order.StoreId +
                                        ", Price-" + order.Price);
                }
            }
        }
        //setup database if empty********************************************************************************************************
        public static void DbSetup()
        {
            // connect to the database
            using (var context = new Project0DbContext())
            {
                // if there are no stores, then add the initial data.
                if (!context.Stores.Any())
                {
                    var store1 = new Stores
                    {
                        Id = 1,
                        Address = "123 First St, Dallas, TX",
                        //need to create inventory (made of multiple items) based on products ****************************
                        Stock = 111
                    };
                    var store2 = new Stores
                    {
                        Id = 2,
                        Address = "223 2nd St, Dallas, TX",
                        //need to create inventory (made of multiple items) based on products ****************************
                        Stock = 112
                    };
                    var store3 = new Stores
                    {
                        Id = 3,
                        Address = "333 Third St, Dallas, TX",
                        //need to create inventory (made of multiple items) based on products ****************************
                        Stock = 113
                    };
                    // add created store
                    context.Stores.Add(store1);
                    context.Stores.Add(store2);
                    context.Stores.Add(store3);
                    // to apply the changes to the database
                    context.SaveChanges();
                }
                // if there are no persons, then add the initial data.
                if (!context.Products.Any())
                {
                    var product1 = new Products
                    {
                        Id = 1,
                        Name = "Book about Something",
                        Price = 10
                    };
                    var product2 = new Products
                    {
                        Id = 2,
                        Name = "Book about Nothing",
                        Price = 11
                    };
                    var product3 = new Products
                    {
                        Id = 3,
                        Name = "Book about...",
                        Price = 12
                    };
                    // sets new products to be added
                    context.Products.Add(product1);
                    context.Products.Add(product2);
                    context.Products.Add(product3);
                    // to apply the changes to the database
                    context.SaveChanges();
                }
                // if there are no persons, then add the initial data.
                if (!context.Customers.Any())
                {
                    var customer1 = new Customers
                    {
                        Id = 1,
                        Username = "username1",
                        Password = "password1",
                        // should probably dump address and state, or create separate table for addresses, check against project0 req
                        Address = "123 Main St",
                        City = "Dallas"
                    };
                    var customer2 = new Customers
                    {
                        Id = 2,
                        Username = "username2",
                        Password = "password2",
                        // should probably dump address and state, or create separate table for addresses, check against project0 req
                        Address = "223 Main St",
                        City = "Dallas"
                    };
                    // this doesn't modify the database YET.
                    context.Customers.Add(customer1);
                    context.Customers.Add(customer2);
                    // to apply the changes that you've "prepped" on this context instance:
                    context.SaveChanges();
                }
                // if there are no persons, then add the initial data.
                if (!context.Orders.Any())
                {
                    var order1 = new Orders
                    {
                        Id = 1,
                        ProductId = 2,
                        CustomerId = 1,
                        StoreId = 1,
                        Price = 11,
                        Time = DateTime.Now
                    };
                    var order2 = new Orders
                    {
                        Id = 2,
                        ProductId = 3,
                        CustomerId = 2,
                        StoreId = 3,
                        Price = 12,
                        Time = DateTime.Now
                    };
                    var order3 = new Orders
                    {
                        Id = 3,
                        ProductId = 1,
                        CustomerId = 2,
                        StoreId = 2,
                        Price = 10,
                        Time = DateTime.Now
                    };
                    // this doesn't modify the database YET.
                    context.Orders.Add(order1);
                    context.Orders.Add(order2);
                    context.Orders.Add(order3);
                    // to apply the changes that you've "prepped" on this context instance:
                    context.SaveChanges();
                }
                /*
                // regardless, modify Fred's name
                // first, load fred from the database (First is a LINQ method)
                var fred = context.Persons.First(p => p.Name.StartsWith("Fred"));
                Console.WriteLine(fred.Name);
                fred.Name += "+";
                context.SaveChanges();*/
            }
        }
    }
}
