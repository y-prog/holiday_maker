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
        Console.WriteLine("2. Show one");
        Console.WriteLine("3. Add one");
        Console.WriteLine("4. Update one");
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
                    Console.Write("Write City :  ");
                    string city = Console.ReadLine();
                    // Feature för filtrera på city Sigge
                    
                    
                    Console.Write("Price Per Night : ");
                    string pricePerNight = Console.ReadLine();
                    //Feature för filtrera på price per night Sebastian
                    
                    
                    Console.Write("Date : ");
                    string date = Console.ReadLine();
                    //Feature för filtrera på date Shaban
                    
                    
                    Console.Write("Room type : "); 
                    string roomType = Console.ReadLine();
                    //Feature för filtrera på room type Yani
                    
                    
                    Console.Write("Distance To Beach : ");
                    string distanceToBeach = Console.ReadLine();
                    //Feature för filtrera på distance to beach
                    
                    
                    Console.Write("Distance To Center : ");
                    string distanceToCenter = Console.ReadLine();
                    //Feature för filtrera på distance to center
                    
                    Console.Write("Has Pool : ");
                    string hasPool = Console.ReadLine();
                    //Feature för filtrera på has pool
                    
                    Console.Write("Has Entertainment : ");
                    string hasEntertainment = Console.ReadLine();
                    //Feature för filtrera på has entertainment
                    
                    Console.Write("Has Kids Club : ");
                    string hasKidsClub = Console.ReadLine();
                    //Feature för filtrera på has kids club
                    
                    Console.Write("Has Restaurant : ");
                    string hasRestaurant = Console.ReadLine();
                    //Feature för filtrera på has restaurant
                    
                    Console.Write("Rating : ");
                    string rating = Console.ReadLine();
                    //Feature för filtrera på rating
                    
                    break;
                
                
                case("2"):
                    Console.WriteLine("Back to the main menu");
                   //  FeatureBack to the main menu
                   
                   
                    break;
                case("3"):
                    Console.WriteLine("Enter name (required)");
                    var name = Console.ReadLine(); // required
                    Console.WriteLine("Enter slogan");
                    var slogan = Console.ReadLine(); // not required
                    if (name is not null)
                    {
                        _actions.AddOne(name, slogan);
                    }
                    break;
                case("4"):
                    Console.WriteLine("Enter id to update one");
                    id = Console.ReadLine();
                    if (id is not null)
                    { 
                        _actions.UpdateOne(id);
                    }
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