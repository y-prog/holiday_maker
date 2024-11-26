using System;

namespace MenuWithDatabase;



public class Menu
{
    Actions _actions;
    public Menu(Actions actions)
    {
        _actions = actions;
        PrintMenu();
    }

    private void PrintMenu()
    {
        Console.WriteLine("Choose option");
        Console.WriteLine("1. Create Booking");
        Console.WriteLine("2. Sort Ascending");
        Console.WriteLine("3. Sort Ratings");
        Console.WriteLine("4. Insert New Customer");
        Console.WriteLine("5. Delete one");
        Console.WriteLine("9. Quit");
        AskUser();
    }

    private async void AskUser()
    {
        var response = Console.ReadLine();
        if (response is not null)
        {
            string? id; // define for multiple use below
            
            switch (response)
            {
                case("1"):
                    Console.WriteLine("Listing all");
                    _actions.ListAll(); // test functions 
                    break;
                
                
                case("2"):
                    Console.WriteLine("Sort in ascending order by city and room type:");

                    // Prompt user for city and room type for filtering
                    Console.Write("Enter city name to search (or press Enter to skip): ");
                    string city = Console.ReadLine();

                    Console.Write("Enter room type (size_id) to search (or press Enter to skip): ");
                    string sizeIdInput = Console.ReadLine();
                    int? sizeId = string.IsNullOrWhiteSpace(sizeIdInput) ? (int?)null : int.Parse(sizeIdInput);

                    // Call the SortAfterPrice method with city and sizeId filters
                    _actions.SortAfterPrice();

                    break;

                
                case("3"):
                    Console.WriteLine("Sort by Ratings");
                    _actions.SortAfterRatings(); // by hotel name
                    break;
                    
                
                // Inside your menu logic (example case "4" for Registering a New User)
                case ("4"):
                    Console.WriteLine("Registering new customer...");
    
                    // Collect customer details
                    Console.Write("First Name: ");
                    string firstName = Console.ReadLine();
    
                    Console.Write("Last Name: ");
                    string lastName = Console.ReadLine();
    
                    Console.Write("Email: ");
                    string email = Console.ReadLine();
    
                    Console.Write("Phone Number: ");
                    string phoneNumber = Console.ReadLine();
    
                    Console.Write("Date of Birth (yyyy-mm-dd): ");
                    DateTime dateOfBirth = DateTime.Parse(Console.ReadLine());

                    // Call RegisterCustomerAsync to insert the new customer
                    await _actions.RegisterCustomerAsync(firstName, lastName, email, phoneNumber, dateOfBirth);
                    Console.WriteLine("Customer added");
                    break;

                
                case("5"):
                    Console.WriteLine("Enter id to delete one");
                    id = Console.ReadLine();
                    if (id is not null)
                    { 
                        _actions.DeleteOne(id);
                    }
                    break;
                case("9"):
                    Console.WriteLine("Quitting");
                    Environment.Exit(0);
                    break;
            }

            PrintMenu();
        }
        
    }
    
}