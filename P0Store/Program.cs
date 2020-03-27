using Microsoft.Data.SqlClient;
using P0Library.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace P0Store
{
    class Program
    {
        //***********BEGIN MAIN***********************
        static void Main(string[] args)
        {
            //calls method to add tables if none already exist in the database
            DbSetup();
            //bool for input validation
            bool valid;
            //string for user inputs
            string initResponse;
            //initialization of cust id for swapping into makeorder from login and createaccount
            int custId = 1;

            //do while is input validation that calls to the yes/no validation method
            do
            {
                Console.WriteLine("Are you a returning customer? (y/n)");
                initResponse = Console.ReadLine();
                //checks that input is valid
                valid = ValidYN(initResponse);
            } while (valid != true);


            //do while is input validation that calls to the yes/no validation method
            do
            {
                //if cust says they are a returning customer
                if (initResponse == "y" || initResponse == "Y")
                {
                    //calls login method, login returns customer id for makeorder to put into new order item
                    custId = Login();
                }
                //if cust says they are not a returning customer
                else if (initResponse == "n" || initResponse == "N")
                {
                    do
                    {
                        Console.WriteLine("Would you like to create an account? (y/n)");
                        initResponse = Console.ReadLine();
                        //input validation
                        valid = ValidYN(initResponse);
                        if (initResponse == "y" || initResponse == "Y")
                        {
                            //calls create account method
                            custId = CreateAccount();
                        }
                    } while (valid != true);
                }
            } while (valid != true);

            //puts customer into 'storepage' loop wherein they view all method call options until they opt to quit
            CallMethods(custId);
        }
        //***********END MAIN***********************

        //************CALLS THE VARIOUS METHODS***********************************
        public static void CallMethods(int customerId)
        {
            Console.WriteLine("Would you like to: " +
                    "\n[1] View all products" +
                    "\n[2] Make an order (requires account)" +
                    "\n[3] Search Customers by Name" +
                    "\n[4] View Order History by Store" +
                    "\n[5] View Order History By Customer" +
                    "\n[6] Create a New Account" +
                    "\n[7] Login to a Different Account" +
                    "\n[8] Exit Application");
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
            else if (input == "6")
            {
                CreateAccount();
            }
            else if (input == "7")
            {
                Login();
            }
            //exits application
            else if (input == "8")
            {
                Console.WriteLine("Thank you for using Basic Books!");
                Console.ReadKey();
                Environment.Exit(0);
            }
            else
            {
                Console.WriteLine("Invalid input, please choose one of the displayed numbers.\n");
            }
            //repeats option to call until customer chooses to exit
            CallMethods(customerId);
        }
        //login to existing account********************************************************************************************************
        public static int Login()
        {
            using (var context = new Project0DbContext())
            {
                var custId = -1;
                //while the username and password are not found to match any pair in the database, repeat WHILE LOOP DIDN'T SEEM TO REALIZE THAT pwdCustId was changing, and kept looping despite pwdCustId > 0 message displaying
                while (custId < 0)
                {
                    bool matches;
                    string usernameMatch = "";
                    string passwordMatch = "";
                    string validUsernameInput = "";
                    //loop repeats until valid username match is found
                    do
                    {
                        Console.WriteLine("Username: ");
                        string usernameInput = Console.ReadLine();
                        //uses premade method to validate username input
                        validUsernameInput = ValidateString(usernameInput);
                        //username exception handling
                        try
                        {
                            //check to see if/where username input is in the database  -- gets single result
                            usernameMatch = context.Customers.Where(u => u.Username == validUsernameInput).Single().Username;
                            matches = true;
                        }
                        catch (InvalidOperationException)
                        {
                            Console.WriteLine("We could not find a match for that username, try again.");
                            matches = false;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            matches = false;
                        }
                    } while (matches == false);
                    //loop repeats until valid pwd match is found
                    do
                    {
                        Console.WriteLine("Password: ");
                        string passwordInput = Console.ReadLine();
                        //uses premade method to validate passoword input
                        string validPasswordInput = ValidateString(passwordInput);
                        //password exception handling
                        try
                        {
                            //gets password in database to compare
                            passwordMatch = context.Customers.First(p => p.Password == validPasswordInput).Password;
                            matches = true;
                        }
                        catch(InvalidOperationException)
                        {
                            Console.WriteLine("We could not find a match for that password, try again.");
                            matches = false;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            matches = false;
                        }
                    } while (matches == false);


                    //get customer id for both username and password and confirm that they match
                    custId = context.Customers.First(i => i.Password == passwordMatch && i.Username == usernameMatch).Id;
                    //confirm username matches with password
                    //if pwdCustId is found in database, the Id will be greater than 0, and SHOULD break out of while loop
                    if (custId >= 0)
                    {
                        //get name for greeting
                        string fnameGreet = context.Customers.Where(u => u.Username == validUsernameInput).Single().FirstName;
                        Console.WriteLine($"Thank you, {fnameGreet}, and welcome.");
                    }
                    else
                    {
                        Console.WriteLine("That username and password does not match our records, please try again.");
                    }
                }
                return custId;
            }
        }
        //create new acccount********************************************************************************************************
        public static int CreateAccount()
        {
            //call to database
            using (var context = new Project0DbContext())
            {
                string usernameInput = "";
                string validatedUsernameInput = "";
                string validatedPasswordInput = "";
                string validatedfnameInput = "";
                string validatedlnameInput = "";
                bool newUsername = false;
                //while loop allows for repeated username attempts
                while (newUsername == false)
                {
                    //get username input
                    Console.WriteLine("Username: ");
                    usernameInput = Console.ReadLine();
                    //input validation for create account username
                    validatedUsernameInput = ValidateString(usernameInput);
                    //tries to find a match for username input already in the database
                    try
                    {
                        var user = context.Customers.Where(u => u.Username == validatedUsernameInput).Single().Username;
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
                    //any other issues with exceptions will be handled here, and send the while loop back up top
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                bool fillsuccess;
                //initializes new customer object before filling it within the do while loop
                var customer = new Customers();
                //do while loop to ensure repeated attempts at inputting information
                do
                {
                    //get input password
                    Console.WriteLine("Password: ");
                    string passwordInput = Console.ReadLine();
                    //input validation for create account password
                    validatedPasswordInput = ValidateString(passwordInput);
                    //get firstname input
                    Console.WriteLine("First Name: ");
                    string fnameInput = Console.ReadLine();
                    //input validation for create account password
                    validatedfnameInput = ValidateString(fnameInput);
                    //get lastname input
                    Console.WriteLine("Last Name: ");
                    string lnameInput = Console.ReadLine();
                    //input validation for create account password
                    validatedlnameInput = ValidateString(lnameInput);
                    try
                    {
                        //need to get id to add number to so primary key doesn't default to 0
                        var idCount = context.Customers.Count() + 1; // need to add 1 because c# index starts at 0, and sql index starts at 1
                        //puts input usr, pwd, fname, lname into new customer item
                        customer = new Customers
                        {
                            Id = idCount,
                            Username = validatedUsernameInput,
                            Password = validatedPasswordInput,
                            FirstName = validatedfnameInput,
                            LastName = validatedlnameInput
                        };
                        //confirms passed without any exceptions
                        fillsuccess = true;
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                        fillsuccess = false;
                    }
                    
                } while (fillsuccess == false);

                //exception handling for adding customer to server
                try
                {
                    //adds a new customer in the database
                    context.Customers.Add(customer);
                    //saves changes to database
                    context.SaveChanges();
                    Console.WriteLine("Welcome, " + validatedfnameInput + "! Your account has been created.");
                }
                catch(Exception e)
                {
                    Console.WriteLine("Error, Account Creation Failed: " + e.Message);
                    Console.ReadKey();
                }

                //get generated customer id
                int custId = customer.Id;
                //returns name for greeting
                return custId;
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
                Console.WriteLine();
            }
        }
        //make an order********************************************************************************************************
        public static void MakeOrder(int customerId)
        {
            using (var context = new Project0DbContext())
            {
                bool storeexception;
                int storeChoice = 1;
                do
                {
                    try
                    {
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
                        storeChoice = Convert.ToInt32(Console.ReadLine());
                        //input must keep within range of store list
                        //gets count of stores from database
                        var storeCount = context.Stores.Count();
                        //if number input is greater than number of stores, or 0 or less: returns bad input and loops
                        if (storeChoice > storeCount || storeChoice <= 0)
                        {
                            //throws custom exception to pull out of try before displaying writelines
                            throw new System.ArgumentException("Parameter cannot be less than or greater than number of listed stores", "storeChoice");
                        }
                        //finishes try w/o any exceptions will break out of do while loop
                        storeexception = false;
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message + " Please enter a valid number");
                        storeexception = true;
                    }
                } while (storeexception == true);
                //after choosing store, can then choose items to order

                bool continueOrder;
                //item for each order's cost
                decimal thisOrderItemCost = 0;
                //item to gather the total cost of an instance of orders
                decimal orderCost = 0;
                int orderSelect = 0;
                int quantity = 0;
                string displayOrder = "";
                //adds to id when creating new order item, required b/c sql starts at 1 index, c# starts at 0
                int numOrdersToAdd = 1;
                //create list to put multiple order objects into
                List<Orders> OrderList = new List<Orders>();
                //create list with which to modify multiple inventory objects  UNABLE TO PUT INVENTORY ITEMS INTO LIST AND UPDATE
                List<Inventory> InventoryList = new List<Inventory>();
                do
                {
                    Console.WriteLine("Select product to order: ");
                    //gets list of products from database
                    var productList = context.Products;
                    //prints list of product names and prices to console as numbered options
                    foreach (var product in productList)
                    {
                        //shows product id, name, and formatted to display price in currency format
                        Console.WriteLine("[" + product.Id + "] " + product.Name + "  " + String.Format("{0:C}", product.Price));
                    }
                    //user selects item to add to order - try catch
                    try
                    {
                        //user selects product id
                        orderSelect = Convert.ToInt32(Console.ReadLine());
                        //get price of product according to input id with linq
                        Products productPrice = productList.First(p => p.Id == orderSelect)/*.Price*/;
                        decimal chosenProductPrice = productPrice.Price;

                        //query quantity of item selected (do while ensures no orders greater than 10 in quantity)
                        do
                        {
                            //exception handling for quantity of items selected
                            try
                            {
                                Console.WriteLine("And how many of those would you like?");
                                quantity = Convert.ToInt32(Console.ReadLine());
                            }
                            catch(Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                            //prevents quantity greater than 10 or a negative quantity
                            if (quantity > 10 || quantity < 0)
                            {
                                Console.WriteLine("May not buy more than 10 or less than 0.");
                            }
                        } while (quantity > 10 || quantity < 0);

                        //get name of product according to input id with linq
                        string productName = productList.First(p => p.Id == orderSelect).Name;
                        //gets cost of just this order item
                        thisOrderItemCost = chosenProductPrice * quantity;
                        //adds this order item to a string for displaying the order
                        displayOrder += productName + " - Quantity: " + quantity + " - Cost: " + String.Format("{0:C}", thisOrderItemCost) + "\n";
                        //add price to variable to add to order item and keeps adding each loop for total
                        orderCost += chosenProductPrice * quantity;
                    }
                    //throws exception with invalid input
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Console.WriteLine("Invalid input, please try ordering again.");
                    }

                    //create new order item and adds to OrderList each loop
                    OrderList.Add(new Orders
                    {
                        //need to add 2 to count because sql counts from 1 and c# counts from 0
                        Id = context.Orders.Count() + numOrdersToAdd,
                        ProductId = orderSelect,
                        CustomerId = customerId,
                        StoreId = storeChoice,
                        Price = thisOrderItemCost,
                        OrderTime = DateTime.Now,
                        Quantity = quantity
                    });
                    //adds to count variable to ensure valid/unique new id
                    numOrdersToAdd++;

                    //exception handling for inventory database update
                    try
                    {
                        //remove quantities of each item selected from given store
                        //gets inventory object that matches currently selected store and product 1 and subtracts quantity from it
                        var inventoryQuantity = context.Inventory.Where(s => s.StoreId == storeChoice && s.ProductId == orderSelect).Single();
                        //subtracts quantity selected from Inventory objects quantity
                        inventoryQuantity.Quantity -= quantity;
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

                    bool validatedContinueOrderInput;
                    string continueOrderInput = "";
                    //do while continues as long as input response to repeat further orders is invalid
                    do
                    {
                        //checks if user would like to add more items to the order
                        Console.WriteLine("Would you like to continue ordering (y)? Or submit your order (n)?");
                        continueOrderInput = Console.ReadLine();
                        validatedContinueOrderInput = ValidYN(continueOrderInput);
                    }
                    while (validatedContinueOrderInput == false);
                    //decides whether to go through full order loop again, based on validated continueOrderInput
                    if (continueOrderInput == "y" || continueOrderInput == "Y")
                    {
                        continueOrder = true;
                    }
                    else
                    {
                        continueOrder = false;
                    }
                    //breaks out of full ordering loop when user is finished
                } while (continueOrder == true);

                // shows customer their cart
                //console writeline string that appends the names of products and gets a quantity for each
                Console.WriteLine("Your order:\n" + displayOrder);
                //display the total cost of the order
                Console.WriteLine("The total cost of your order is " + String.Format("{0:C}", orderCost) + "\n");


                //add order item to commit for each item in orderlist
                foreach(var order in OrderList)
                {
                    context.Orders.AddRange(order);
                }
                //save changes to database
                context.SaveChanges();
            }
        }
        //search customers by name********************************************************************************************************
        public static void SearchCustomers()
        {
            using (var context = new Project0DbContext())
            {
                bool repeat = true;
                bool valid;
                string tryagain = "";
                //do while repeats if user misses but wants to keep searching
                do
                {
                    //get search
                    Console.WriteLine("Enter a (first) name to search for: ");
                    string fnameSearchInput = Console.ReadLine();
                    string validatedfnameSearchInput = ValidateString(fnameSearchInput);

                    //check search against the database
                    try
                    {
                        var userfname = context.Customers.Where(u => u.FirstName == validatedfnameSearchInput).Single().FirstName;
                        var userlname = context.Customers.Where(u => u.FirstName == validatedfnameSearchInput).Single().LastName;
                        var username = context.Customers.Where(u => u.FirstName == validatedfnameSearchInput).Single().Username;
                        //if a user is found
                        if (userfname != null)
                        {
                            //indicates that username is in the database
                            Console.WriteLine($"That user is present in the database! {userfname} {userlname}'s username is {username}.\n");
                            //do while handles input validation - ensures user input is either y/n
                            do
                            {
                                //prompts user if they want to search again
                                Console.WriteLine("Would you like to search for another user? (y/n)");
                                tryagain = Console.ReadLine();
                                valid = ValidYN(tryagain);
                            } while (valid == false);
                            //user chooses to try again or not logic
                            if (tryagain == "y" || tryagain == "Y")
                            {
                                repeat = true;
                            }
                            else if (tryagain == "n" || tryagain == "N")
                            {
                                repeat = false;
                            }
                        }
                    }
                    //if input username is not found in the database, it will throw an InvalidOperationException because user will be null
                    catch (InvalidOperationException)
                    {
                        //do while handles input validation - ensures user input is either y/n
                        do
                        {
                            //indicates username not found
                            Console.WriteLine("There was no similar username found, try again? (y/n)");
                            tryagain = Console.ReadLine();
                            valid = ValidYN(tryagain);
                        } while (valid == false);
                        //user chooses to try again or not logic
                        if (tryagain == "y" || tryagain == "Y")
                        {
                            repeat = true;
                        }
                        else if (tryagain == "n" || tryagain == "N")
                        {
                            repeat = false;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        repeat = true;
                    }
                } while (repeat == true);
            }
        }
        //order history by store********************************************************************************************************
        public static void StoreOrderHistory()
        {
            using (var context = new Project0DbContext())
            {
                bool valid;
                int storeChoice = 0;
                //do while will repeat if user input or db call causes an exception
                do
                {
                    Console.WriteLine("Select a store to search: ");
                    //gets store of choice from user with exception handling
                    try
                    {
                        //gets list of stores from database
                        var storeList = context.Stores;
                        //prints list of stores to console
                        foreach (var store in storeList)
                        {
                            Console.WriteLine("[" + store.Id + "]  " + store.Address);
                        }
                        storeChoice = Convert.ToInt32(Console.ReadLine());
                        //selects db order info according to input store
                        var storeOrderList = context.Orders.Where(s => s.StoreId == storeChoice);
                        //input must keep within range of store list
                        //gets count of stores from database
                        var storeCount = context.Stores.Count();
                        //if number input is greater than number of stores, or 0 or less: returns bad input and loops
                        if (storeChoice > storeCount || storeChoice <= 0)
                        {
                            //throws custom exception to pull out of try before displaying writelines
                            throw new System.ArgumentException("Parameter cannot be less than or greater than number of listed stores", "storeChoice");
                        }
                        Console.WriteLine("This store's order history: ");
                        //using foreach instead of switch case to display order details by store
                        foreach (var order in storeOrderList)
                        {
                            Console.WriteLine("Customer ID-" + order.CustomerId +
                                                ", Product ID-" + order.ProductId +
                                                ", Quantity bought-" + order.Quantity +
                                                ", Price-" + order.Price +
                                                ", Time=" + order.OrderTime);
                        }
                        //selects db inventory info according to input store
                        var storeInventory = context.Inventory.Where(s => s.StoreId == storeChoice);
                        //display inventory of selected store
                        Console.WriteLine("\nThis store's current stock: ");
                        foreach (var productInventory in storeInventory)
                        {
                            Console.WriteLine("Product ID: " + productInventory.ProductId + "  Current Stock: " + productInventory.Quantity);
                        }
                        Console.WriteLine();
                        //going all the way through try w/o exception will break out of do while loop
                        valid = true;
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message + "  Please enter a valid number.");
                        //a caught exception will cause the do while loop to repeat
                        valid = false;
                    }
                } while (valid == false);
            }
        }
        //view order history of a customer*********************************************************************************************
        public static void CustomerOrderHistory()
        {
            using (var context = new Project0DbContext())
            {
                bool exeptionthrown;
                //do while repeats if an exception is thrown
                do
                {
                    try
                    {
                        //prompt user for customer choice
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
                        //input must keep within range of store list
                        //gets count of stores from database
                        var customerCount = context.Customers.Count();
                        //if number input is greater than number of customers, or 0 or less: returns bad input and loops
                        if (customerChoice > customerCount || customerChoice <= 0)
                        {
                            //throws custom exception to pull out of try before displaying writelines
                            throw new System.ArgumentException("Parameter cannot be less than or greater than number of listed customers", "customerChoice");
                        }
                        //gets number of orders of customer
                        var custOrderCount = orderList.Count();
                        //advises that there are no recorded orders for given customer if none found
                        if(custOrderCount < 1)
                        {
                            Console.WriteLine("This customer doesn't have any orders yet.");
                        }
                        //using foreach instead of switch case to display order details by customer
                        foreach (var order in orderList)
                        {
                            Console.WriteLine("Customer ID-" + order.CustomerId +
                                                ", Product ID-" + order.ProductId +
                                                ", Store ID-" + order.StoreId +
                                                ", Price-" + order.Price +
                                                ", Time-" + order.OrderTime);
                        }
                        //finishes try w/ no exceptions thrown, will finish loop
                        exeptionthrown = false;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message + "  Please input correct number.");
                        exeptionthrown = true;
                    }
                }
                while (exeptionthrown == true);
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
                        Address = "123 1st St, Dallas, TX",
                        //need to create inventory (made of multiple items) based on products ****************************
                        //Store stock removed due to Inventory Quantity
                        //Stock = 111
                    };
                    var store2 = new Stores
                    {
                        Id = 2,
                        Address = "223 2nd St, Dallas, TX",
                        //need to create inventory (made of multiple items) based on products ****************************
                        //Store stock removed due to Inventory Quantity
                        //Stock = 212
                    };
                    var store3 = new Stores
                    {
                        Id = 3,
                        Address = "333 3rd St, Dallas, TX",
                        //need to create inventory (made of multiple items) based on products ****************************
                        //Store stock removed due to Inventory Quantity
                        //Stock = 313
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
                        Price = Convert.ToDecimal(11.95)
                    };
                    var product2 = new Products
                    {
                        Id = 2,
                        Name = "Book about Nothing",
                        Price = Convert.ToDecimal(22.95)
                    };
                    var product3 = new Products
                    {
                        Id = 3,
                        Name = "Book about Everything",
                        Price = Convert.ToDecimal(39.95)
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
                        FirstName = "Amy",
                        LastName = "Arrigeti"
                    };
                    var customer2 = new Customers
                    {
                        Id = 2,
                        Username = "username2",
                        Password = "password2",
                        FirstName = "Bob",
                        LastName = "Builder"
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
                        Price = 12,
                        OrderTime = DateTime.Now,
                        Quantity = 1
                    };
                    var order2 = new Orders
                    {
                        Id = 2,
                        ProductId = 3,
                        CustomerId = 2,
                        StoreId = 3,
                        Price = 12,
                        OrderTime = DateTime.Now,
                        Quantity = 1
                    };
                    var order3 = new Orders
                    {
                        Id = 3,
                        ProductId = 1,
                        CustomerId = 2,
                        StoreId = 2,
                        Price = 64,
                        OrderTime = DateTime.Now,
                        Quantity = 3
                    };
                    // this doesn't modify the database YET.
                    context.Orders.Add(order1);
                    context.Orders.Add(order2);
                    context.Orders.Add(order3);
                    // to apply the changes that you've "prepped" on this context instance:
                    context.SaveChanges();
                }
                // if there are no persons, then add the initial data.
                if (!context.Inventory.Any())
                {
                    var inventory1 = new Inventory
                    {
                        Id = 1,
                        ProductId = 1,
                        StoreId = 1,
                        Quantity = 111
                    };
                    var inventory2 = new Inventory
                    {
                        Id = 2,
                        ProductId = 2,
                        StoreId = 1,
                        Quantity = 224
                    };
                    var inventory3 = new Inventory
                    {
                        Id = 3,
                        ProductId = 3,
                        StoreId = 1,
                        Quantity = 336
                    };
                    var inventory4 = new Inventory
                    {
                        Id = 4,
                        ProductId = 1,
                        StoreId = 2,
                        Quantity = 112
                    };
                    var inventory5 = new Inventory
                    {
                        Id = 5,
                        ProductId = 2,
                        StoreId = 2,
                        Quantity = 224
                    };
                    var inventory6 = new Inventory
                    {
                        Id = 6,
                        ProductId = 3,
                        StoreId = 2,
                        Quantity = 336
                    };
                    var inventory7 = new Inventory
                    {
                        Id = 7,
                        ProductId = 1,
                        StoreId = 3,
                        Quantity = 113
                    };
                    var inventory8 = new Inventory
                    {
                        Id = 8,
                        ProductId = 2,
                        StoreId = 3,
                        Quantity = 224
                    };
                    var inventory9 = new Inventory
                    {
                        Id = 9,
                        ProductId = 3,
                        StoreId = 3,
                        Quantity = 336
                    };
                    // this doesn't modify the database YET.
                    context.Inventory.Add(inventory1);
                    context.Inventory.Add(inventory2);
                    context.Inventory.Add(inventory3);
                    context.Inventory.Add(inventory4);
                    context.Inventory.Add(inventory5);
                    context.Inventory.Add(inventory6);
                    context.Inventory.Add(inventory7);
                    context.Inventory.Add(inventory8);
                    context.Inventory.Add(inventory9);
                    // to apply the changes that you've "prepped" on this context instance:
                    context.SaveChanges();
                }
            }
        }
        //method to validate yes/no inputs (returns false if invalid)*****************************************************************************
        public static bool ValidYN(string input)
        {
            if(input == "y" || input == "Y")
            {
                return true;
            }
            else if(input == "n" || input == "N")
            {
                return true;
            }
            else
            {
                Console.WriteLine("Invalid input. Please return (y/n).");
                return false;
            }
        }
        //method to validate strings (returns validated string)******************************************************************
        public static string ValidateString(string input)
        {
            string validatedString;
            //ensures input string is not blank, null, or more than 35 characters
            if (input == "" || input.Length > 35 || input == null)
            {
                Console.WriteLine("Input must not be blank or longer than 35 characters.");
                Console.WriteLine("Please enter a valid input: ");
                input = Console.ReadLine();
            }

            /*//ensures name does not include numbers
            foreach (char item in input)
            {
                if (char.IsDigit(item))
                {
                    Console.Write("Digits Are NotAllowed....\n");
                    Console.Write("Please Enter Correct Name: ");
                    input = Console.ReadLine();
                }
            }*/

            validatedString = input;
            return validatedString;
        }
    }
}
