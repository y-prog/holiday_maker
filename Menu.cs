using System;

namespace MenuWithDatabase;

public class Menu
{
    private readonly Actions _actions;

    public Menu(Actions actions)
    {
        _actions = actions;
        PrintMenu().Wait();
    }

    private async Task PrintMenu()
    {
        Console.WriteLine("\nChoose an option:");
        Console.WriteLine("1. Search Available Rooms");
        Console.WriteLine("2. Create Booking");
        Console.WriteLine("3. Sort Rooms");
        Console.WriteLine("4. Update Booking");
        Console.WriteLine("5. Delete Booking");
        Console.WriteLine("6. List Bookings");
        Console.WriteLine("98. List Table");
        Console.WriteLine("9. Quit");
        await AskUser();
    }

    private async Task AskUser()
    {
        string? response = Console.ReadLine();

        switch (response)
        {
            case "1":
                await SearchAvailableRooms();
                break;
            case "2":
                await CreateBooking();
                break;
            case "3":
                await SortRooms();
                break;
            case "4":
                await UpdateBooking();
                break;
            case "5":
                await DeleteBooking();
                break;
            case "6":
                await _actions.ListBookings();
                break;
            case "98":
                Console.Write("Enter table name: ");
                string? tableName = Console.ReadLine();
                if (!string.IsNullOrEmpty(tableName))
                    await _actions.ListTable(tableName);
                break;
            case "9":
                Console.WriteLine("Exiting program.");
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("Invalid option. Please try again.");
                break;
        }

        await PrintMenu();
    }

private async Task SearchAvailableRooms()
{
    Console.Write("Enter City: ");
    string city = Console.ReadLine() ?? "";

    Console.Write("Enter Hotel Name (or leave blank for any): ");
    string hotelname = Console.ReadLine();

    Console.Write("Enter Maximum Price: ");
    string maxPriceInput = Console.ReadLine();
    decimal? maxPrice = string.IsNullOrEmpty(maxPriceInput) ? (decimal?)null : decimal.Parse(maxPriceInput);

    Console.Write("Enter Date Range (YYYY-MM-DD YYYY-MM-DD): ");
    string dateInput = Console.ReadLine();
    string[] dateRange = dateInput.Split(' ');

    DateTime? startDate = null;
    DateTime? endDate = null;
    if (dateRange.Length == 2 &&
        DateTime.TryParse(dateRange[0], out DateTime tempStartDate) &&
        DateTime.TryParse(dateRange[1], out DateTime tempEndDate))
    {
        startDate = tempStartDate;
        endDate = tempEndDate;
    }

    Console.Write("Room Type (or leave blank for any): ");
    string roomType = Console.ReadLine();

    Console.Write("Enter Maximum Distance to Beach: ");
    string maxDistanceToBeachInput = Console.ReadLine();
    int? maxDistanceToBeach = string.IsNullOrEmpty(maxDistanceToBeachInput) ? (int?)null : int.Parse(maxDistanceToBeachInput);

    Console.Write("Enter Maximum Distance to Center: ");
    string maxDistanceToCenterInput = Console.ReadLine();
    int? maxDistanceToCenter = string.IsNullOrEmpty(maxDistanceToCenterInput) ? (int?)null : int.Parse(maxDistanceToCenterInput);

    Console.Write("Has Pool (true/false): ");
    string hasPoolInput = Console.ReadLine();
    bool? hasPool = string.IsNullOrEmpty(hasPoolInput) ? (bool?)null : bool.Parse(hasPoolInput);

    Console.Write("Has Entertainment (true/false): ");
    string hasEntertainmentInput = Console.ReadLine();
    bool? hasEntertainment = string.IsNullOrEmpty(hasEntertainmentInput) ? (bool?)null : bool.Parse(hasEntertainmentInput);

    Console.Write("Has Kids Club (true/false): ");
    string hasKidsClubInput = Console.ReadLine();
    bool? hasKidsClub = string.IsNullOrEmpty(hasKidsClubInput) ? (bool?)null : bool.Parse(hasKidsClubInput);

    Console.Write("Has Restaurant (true/false): ");
    string hasRestaurantInput = Console.ReadLine();
    bool? hasRestaurant = string.IsNullOrEmpty(hasRestaurantInput) ? (bool?)null : bool.Parse(hasRestaurantInput);

    Console.Write("Minimum Rating: ");
    string minRatingInput = Console.ReadLine();
    decimal? minRating = string.IsNullOrEmpty(minRatingInput) ? (decimal?)null : decimal.Parse(minRatingInput);

    var availableRooms = await _actions.SearchAvailableRooms(
        city, hotelname, maxPrice, startDate, endDate, roomType,
        maxDistanceToBeach, maxDistanceToCenter, hasPool,
        hasEntertainment, hasKidsClub, hasRestaurant, minRating);

    Console.WriteLine("\nAvailable Accommodations:");
    if (availableRooms.Count > 0)
    {
        availableRooms.ForEach(Console.WriteLine);
    }
    else
    {
        Console.WriteLine("No available accommodations match your criteria.");
    }
}


    private async Task CreateBooking()
    {
        Console.Write("Customer ID: ");
        if (!int.TryParse(Console.ReadLine(), out int customerId))
        {
            Console.WriteLine("Invalid Customer ID.");
            return;
        }

        Console.Write("Accommodation ID: ");
        if (!int.TryParse(Console.ReadLine(), out int accommodationId))
        {
            Console.WriteLine("Invalid Accommodation ID.");
            return;
        }

        Console.Write("Start Date (YYYY-MM-DD): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime startDate))
        {
            Console.WriteLine("Invalid Start Date.");
            return;
        }

        Console.Write("End Date (YYYY-MM-DD): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime endDate))
        {
            Console.WriteLine("Invalid End Date.");
            return;
        }

        Console.Write("Extra Bed (true/false): ");
        if (!bool.TryParse(Console.ReadLine(), out bool extraBed))
        {
            Console.WriteLine("Invalid Extra Bed input.");
            return;
        }

        Console.Write("Half Board (true/false): ");
        if (!bool.TryParse(Console.ReadLine(), out bool halfBoard))
        {
            Console.WriteLine("Invalid Half Board input.");
            return;
        }

        Console.Write("Full Board (true/false): ");
        if (!bool.TryParse(Console.ReadLine(), out bool fullBoard))
        {
            Console.WriteLine("Invalid Full Board input.");
            return;
        }

        await _actions.CreateBooking(customerId, accommodationId, startDate, endDate, extraBed, halfBoard, fullBoard);
    }

    private async Task SortRooms()
    {
        Console.Write("Sort by (price/rating): ");
        string? sortBy = Console.ReadLine()?.ToLower();
        if (sortBy != "price" && sortBy != "rating")
        {
            Console.WriteLine("Invalid sort option.");
            return;
        }

        var sortedRooms = await _actions.GetSortedRooms(sortBy);
        Console.WriteLine("\nSorted Accommodations:");
        sortedRooms.ForEach(Console.WriteLine);
    }

    private async Task UpdateBooking()
    {
        Console.Write("Booking ID (leave empty if using Email): ");
        string? bookingId = Console.ReadLine();

        Console.Write("Email (leave empty if using Booking ID): ");
        string? email = Console.ReadLine();

        if (string.IsNullOrEmpty(bookingId) && string.IsNullOrEmpty(email))
        {
            Console.WriteLine("You must provide either a Booking ID or an Email.");
            return;
        }

        Console.Write("New Start Date (YYYY-MM-DD): ");
        string? newStartDate = Console.ReadLine();

        Console.Write("New End Date (YYYY-MM-DD): ");
        string? newEndDate = Console.ReadLine();

        Console.Write("New Extra Bed (true/false, leave empty to keep current): ");
        string? newExtraBed = Console.ReadLine();

        Console.Write("New Half Board (true/false, leave empty to keep current): ");
        string? newHalfBoard = Console.ReadLine();

        Console.Write("New Full Board (true/false, leave empty to keep current): ");
        string? newFullBoard = Console.ReadLine();

        await _actions.UpdateBooking(bookingId, email, newStartDate, newEndDate, newExtraBed, newHalfBoard, newFullBoard);
    }



    private async Task DeleteBooking()
    {
        Console.Write("Enter Booking ID or Email: ");
        string? identifier = Console.ReadLine();
        if (string.IsNullOrEmpty(identifier))
        {
            Console.WriteLine("Invalid input. Please enter a Booking ID or Email.");
            return;
        }
        await _actions.DisplayAndDeleteBooking(identifier);
    }

}