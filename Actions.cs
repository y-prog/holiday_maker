using System;

using Npgsql;
namespace MenuWithDatabase;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class Actions
{
    private readonly NpgsqlDataSource _holidaymaker;

    public Actions(NpgsqlDataSource holidaymaker)
    {
        _holidaymaker = holidaymaker;
    }

    public async void ListAll()
    {
        await using (var cmd = _holidaymaker.CreateCommand("SELECT * FROM size"))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                Console.WriteLine($"id: {reader.GetInt32(0)} \t room_type: {reader.GetString(1)}");
            }
        }
    }

    public async void ShowOne(string id)
    {
        await using (var cmd = _holidaymaker.CreateCommand("SELECT * FROM customers WHERE id = $1"))
        {
            cmd.Parameters.AddWithValue(int.Parse(id));
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    Console.WriteLine($"id: {reader.GetInt32(0)} \t name: {reader.GetString(1)} \t email: {reader.GetString(2)}");
                }
            }
        }
    }

    public async void AddOne(string name, string? slogan)
    {
        await using (var cmd = _holidaymaker.CreateCommand("INSERT INTO items (name, slogan) VALUES ($1, $2)"))
        {
            cmd.Parameters.AddWithValue(name);
            cmd.Parameters.AddWithValue(slogan);
            await cmd.ExecuteNonQueryAsync();
        }
    }

    public async void UpdateOne(string id)
    {
        Console.WriteLine("Current entry:");
        ShowOne(id);
        Console.WriteLine("Enter updated name (required):");
        var name = Console.ReadLine();
        Console.WriteLine("Enter updated slogan:");
        var slogan = Console.ReadLine();
        if (name is not null)
        {
            await using (var cmd = _holidaymaker.CreateCommand("UPDATE items SET name = $2, slogan = $3 WHERE id = $1"))
            {
                cmd.Parameters.AddWithValue(int.Parse(id));
                cmd.Parameters.AddWithValue(name);
                cmd.Parameters.AddWithValue(slogan);
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }

    public async void DeleteOne(string id)
    {
        await using (var cmd = _holidaymaker.CreateCommand("DELETE FROM items WHERE id = $1"))
        {
            cmd.Parameters.AddWithValue(int.Parse(id));
            await cmd.ExecuteNonQueryAsync();
        }
    }

    public async Task<List<string>> SearchAvailableRooms(
    string city,
    decimal maxPrice,
    DateTime date,
    string roomType,
    int maxDistanceToBeach,
    int maxDistanceToCenter,
    bool hasPool,
    bool hasEntertainment,
    bool hasKidsClub,
    bool hasRestaurant,
    decimal minRating)
{
    List<string> availableRooms = new List<string>();

    string query = @"
        SELECT a.name, a.price_per_night, a.city, a.ratings 
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
          AND $10 BETWEEN a.date_from AND a.date_to
          AND a.room_type = $11";

    try
    {
        // Log query for debugging
        Console.WriteLine($"SQL Query: {query}");
        Console.WriteLine($"Parameters: city={city}, maxPrice={maxPrice}, date={date}, roomType={roomType}, maxDistanceToBeach={maxDistanceToBeach}, maxDistanceToCenter={maxDistanceToCenter}, hasPool={hasPool}, hasEntertainment={hasEntertainment}, hasKidsClub={hasKidsClub}, hasRestaurant={hasRestaurant}, minRating={minRating}");

        await using (var cmd = _holidaymaker.CreateCommand(query))
        {
            cmd.Parameters.AddWithValue(city);
            cmd.Parameters.AddWithValue(maxPrice);
            cmd.Parameters.AddWithValue(maxDistanceToBeach);
            cmd.Parameters.AddWithValue(maxDistanceToCenter);
            cmd.Parameters.AddWithValue(hasPool);
            cmd.Parameters.AddWithValue(hasEntertainment);
            cmd.Parameters.AddWithValue(hasKidsClub);
            cmd.Parameters.AddWithValue(hasRestaurant);
            cmd.Parameters.AddWithValue(minRating);
            cmd.Parameters.AddWithValue(date);
            cmd.Parameters.AddWithValue(roomType);

            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    availableRooms.Add($"Name: {reader.GetString(0)}, Price: {reader.GetDecimal(1)}, City: {reader.GetString(2)}, Rating: {reader.GetDecimal(3)}");
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }

    // Handle no results
    if (availableRooms.Count == 0)
    {
        Console.WriteLine("No available accommodations match your criteria.");
    }

    return availableRooms;
}

    public async Task CreateBooking(int customerId, int accommodationId, DateTime startDate, DateTime endDate, bool extraBed, bool halfBoard, bool fullBoard)
    {
        try
        {
            await using (var cmd = _holidaymaker.CreateCommand(@"
                INSERT INTO booking (customer_id, accommodation_id, start_date, end_date, extra_bed, half_board, full_board) 
                VALUES ($1, $2, $3, $4, $5, $6, $7)"))
            {
                cmd.Parameters.AddWithValue(customerId);
                cmd.Parameters.AddWithValue(accommodationId);
                cmd.Parameters.AddWithValue(startDate);
                cmd.Parameters.AddWithValue(endDate);
                cmd.Parameters.AddWithValue(extraBed);
                cmd.Parameters.AddWithValue(halfBoard);
                cmd.Parameters.AddWithValue(fullBoard);
                await cmd.ExecuteNonQueryAsync();
                Console.WriteLine("Booking created successfully.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating booking: {ex.Message}");
        }
    }

    public async Task<List<string>> GetSortedRooms(string sortBy)
    {
        List<string> sortedRooms = new List<string>();
        string query = sortBy == "price" 
            ? "SELECT name, price_per_night, ratings FROM filtreraRum ORDER BY price_per_night" 
            : "SELECT name, price_per_night, ratings FROM filtreraRum ORDER BY ratings DESC";

        try
        {
            await using (var cmd = _holidaymaker.CreateCommand(query))
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    sortedRooms.Add($"Name: {reader.GetString(0)}, Price: {reader.GetDecimal(1)}, Rating: {reader.GetDecimal(2)}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching sorted rooms: {ex.Message}");
        }

        return sortedRooms;
    }
    
    public async Task RunCustomQuery(string query)
    {
        try
        {
            Console.WriteLine($"Running Query: {query}");
            await using (var cmd = _holidaymaker.CreateCommand(query))
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                if (!reader.HasRows)
                {
                    Console.WriteLine("No data found.");
                    return;
                }

                // Skriv ut kolumnnamn
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    Console.Write($"{reader.GetName(i)}\t");
                }
                Console.WriteLine();

                // Skriv ut data
                while (await reader.ReadAsync())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Console.Write($"{reader[i]}\t");
                    }
                    Console.WriteLine();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error executing query: {ex.Message}");
        }
    }

    
    public async Task ListTable(string tableName)
    {
        try
        {
            string query = $"SELECT * FROM {tableName}";
            await RunCustomQuery(query);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching table {tableName}: {ex.Message}");
        }
    }
}
