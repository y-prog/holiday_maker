using System;

namespace MenuWithDatabase;



using System;

public class Menu
{
    private readonly Actions _actions;

    public Menu(Actions actions)
    {
        _actions = actions;
        PrintMenu();
    }

    private void PrintMenu()
    {
        Console.WriteLine("Choose an option:");
        Console.WriteLine("1. Search Available Rooms");
        Console.WriteLine("2. Create Booking");
        Console.WriteLine("3. Sort Rooms");
        Console.WriteLine("98. debugg");
        Console.WriteLine("9. Quit");
        AskUser();
    }

    private async void AskUser()
    {
        var response = Console.ReadLine();
        if (response is not null)
        {
            switch (response)
            {
                case "1": // Search Available Rooms
                    Console.Write("City: ");
                    string city = Console.ReadLine();
                    Console.Write("Price Per Night: ");
                    decimal maxPrice = decimal.Parse(Console.ReadLine());
                    Console.Write("Date (YYYY-MM-DD): ");
                    DateTime date = DateTime.Parse(Console.ReadLine());
                    Console.Write("Room type: ");
                    string roomType = Console.ReadLine();
                    Console.Write("Max Distance To Beach: ");
                    int maxDistanceToBeach = int.Parse(Console.ReadLine());
                    Console.Write("Max Distance To Center: ");
                    int maxDistanceToCenter = int.Parse(Console.ReadLine());
                    Console.Write("Has Pool (true/false): ");
                    bool hasPool = bool.Parse(Console.ReadLine());
                    Console.Write("Has Entertainment (true/false): ");
                    bool hasEntertainment = bool.Parse(Console.ReadLine());
                    Console.Write("Has Kids Club (true/false): ");
                    bool hasKidsClub = bool.Parse(Console.ReadLine());
                    Console.Write("Has Restaurant (true/false): ");
                    bool hasRestaurant = bool.Parse(Console.ReadLine());
                    Console.Write("Minimum Rating: ");
                    decimal minRating = decimal.Parse(Console.ReadLine());

                    var availableRooms = await _actions.SearchAvailableRooms(city, maxPrice, date, roomType, maxDistanceToBeach, maxDistanceToCenter, hasPool, hasEntertainment, hasKidsClub, hasRestaurant, minRating);

                    Console.WriteLine("\nAvailable Accommodations:");
                    availableRooms.ForEach(Console.WriteLine);
                    break;

                case "2": // Create Booking
                    Console.Write("Customer ID: ");
                    int customerId = int.Parse(Console.ReadLine());
                    Console.Write("Accommodation ID: ");
                    int accommodationId = int.Parse(Console.ReadLine());
                    Console.Write("Start Date (YYYY-MM-DD): ");
                    DateTime startDate = DateTime.Parse(Console.ReadLine());
                    Console.Write("End Date (YYYY-MM-DD): ");
                    DateTime endDate = DateTime.Parse(Console.ReadLine());
                    Console.Write("Extra Bed (true/false): ");
                    bool extraBed = bool.Parse(Console.ReadLine());
                    Console.Write("Half Board (true/false): ");
                    bool halfBoard = bool.Parse(Console.ReadLine());
                    Console.Write("Full Board (true/false): ");
                    bool fullBoard = bool.Parse(Console.ReadLine());

                    await _actions.CreateBooking(customerId, accommodationId, startDate, endDate, extraBed, halfBoard, fullBoard);
                    break;

                case "3": // Sort Rooms
                    Console.Write("Sort by (price/rating): ");
                    string sortBy = Console.ReadLine();
                    var sortedRooms = await _actions.GetSortedRooms(sortBy);

                    Console.WriteLine("\nSorted Accommodations:");
                    sortedRooms.ForEach(Console.WriteLine);
                    break;

                case "9": // Quit
                    Console.WriteLine("Exiting program.");
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
                
                case "98": // List table content
                    Console.Write("Enter table name: ");
                    string tableName = Console.ReadLine();
                    await _actions.ListTable(tableName);
                    break;

            }

            PrintMenu();
        }
    }
}
