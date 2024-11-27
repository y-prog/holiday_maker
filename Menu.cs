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
    if (!decimal.TryParse(Console.ReadLine(), out decimal maxPrice))
    {
        Console.WriteLine("Invalid price input.");
        return;
    }

    Console.Write("Enter Date Range (YYYY-MM-DD YYYY-MM-DD): ");
    string dateInput = Console.ReadLine();
    string[] dateRange = dateInput.Split(' ');

    if (dateRange.Length != 2 ||
        !DateTime.TryParse(dateRange[0], out DateTime startDate) ||
        !DateTime.TryParse(dateRange[1], out DateTime endDate))
    {
        Console.WriteLine("Invalid date input. Please enter dates in the format: YYYY-MM-DD YYYY-MM-DD.");
        return;
    }

    Console.Write("Room Type: ");
    string roomType = Console.ReadLine() ?? "";

    Console.Write("Enter Maximum Distance to Beach: ");
    if (!int.TryParse(Console.ReadLine(), out int maxDistanceToBeach))
    {
        Console.WriteLine("Invalid distance input.");
        return;
    }

    Console.Write("Enter Maximum Distance to Center: ");
    if (!int.TryParse(Console.ReadLine(), out int maxDistanceToCenter))
    {
        Console.WriteLine("Invalid distance input.");
        return;
    }

    Console.Write("Has Pool (true/false): ");
    if (!bool.TryParse(Console.ReadLine(), out bool hasPool))
    {
        Console.WriteLine("Invalid input for pool.");
        return;
    }

    Console.Write("Has Entertainment (true/false): ");
    if (!bool.TryParse(Console.ReadLine(), out bool hasEntertainment))
    {
        Console.WriteLine("Invalid input for entertainment.");
        return;
    }

    Console.Write("Has Kids Club (true/false): ");
    if (!bool.TryParse(Console.ReadLine(), out bool hasKidsClub))
    {
        Console.WriteLine("Invalid input for kids club.");
        return;
    }

    Console.Write("Has Restaurant (true/false): ");
    if (!bool.TryParse(Console.ReadLine(), out bool hasRestaurant))
    {
        Console.WriteLine("Invalid input for restaurant.");
        return;
    }

    Console.Write("Minimum Rating: ");
    if (!decimal.TryParse(Console.ReadLine(), out decimal minRating))
    {
        Console.WriteLine("Invalid rating input.");
        return;
    }

    // Dynamisk SQL-fråga
    string query = @"
        SELECT a.accommodation_name, a.price_per_night, a.city, a.ratings
        FROM ledigaRum a
        WHERE a.city = $1
          AND a.price_per_night <= $2
          AND a.distance_to_beach <= $3
          AND a.distance_to_city_center <= $4
          AND a.has_pool = $5
          AND a.has_evening_entertainment = $6
          AND a.has_kids_club = $7
          AND a.has_restaurants = $8
          AND a.ratings >= $9
          AND a.date_from <= $10
          AND a.date_to >= $11
          AND a.room_type = $12";

    if (!string.IsNullOrEmpty(hotelname))
    {
        query += " AND a.accommodation_name ILIKE $13";
    }

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
        Console.Write("Booking ID: ");
        string? bookingId = Console.ReadLine();
        if (string.IsNullOrEmpty(bookingId))
        {
            Console.WriteLine("Invalid Booking ID.");
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

        await _actions.UpdateBooking(bookingId, newStartDate, newEndDate, newExtraBed, newHalfBoard, newFullBoard);
    }


    private async Task DeleteBooking()
    {
        Console.Write("Enter Booking ID: ");
        string? bookingId = Console.ReadLine();
        if (string.IsNullOrEmpty(bookingId))
        {
            Console.WriteLine("Invalid Booking ID.");
            return;
        }
        await _actions.DeleteBooking(bookingId);
    }
}