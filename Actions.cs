using System;
 
using Npgsql;
namespace MenuWithDatabase;
 
 
using Npgsql;

public class Actions
{
    private readonly NpgsqlDataSource _holidaymaker;

    public Actions(NpgsqlDataSource holidaymaker)
    {
        _holidaymaker = holidaymaker;
    }

   public async Task<List<string>> SearchAvailableRooms(
    string city,
    string? name,
    decimal maxPrice,
    DateTime startDate,
    DateTime endDate,
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
          AND ($2 IS NULL OR a.name ILIKE $2)
          AND a.price_per_night <= $3
          AND a.distance_to_beach <= $4
          AND a.distance_to_city_center <= $5
          AND a.has_pool = $6
          AND a.has_evening_entertainment = $7
          AND a.has_kids_club = $8
          AND a.has_restaurants = $9
          AND a.ratings >= $10
          AND a.date_from <= $11
          AND a.date_to >= $12
          AND a.room_type = $13";

    try
    {
        Console.WriteLine($"SQL Query: {query}");
        Console.WriteLine($"Parameters: city={city}, name={name ?? "Any"}, maxPrice={maxPrice}, startDate={startDate}, endDate={endDate}, roomType={roomType}, maxDistanceToBeach={maxDistanceToBeach}, maxDistanceToCenter={maxDistanceToCenter}, hasPool={hasPool}, hasEntertainment={hasEntertainment}, hasKidsClub={hasKidsClub}, hasRestaurant={hasRestaurant}, minRating={minRating}");

        await using (var cmd = _holidaymaker.CreateCommand(query))
        {
            cmd.Parameters.AddWithValue(city);
            cmd.Parameters.AddWithValue(string.IsNullOrEmpty(name) ? DBNull.Value : $"%{name}%"); // Filtrera om `name` är angivet
            cmd.Parameters.AddWithValue(maxPrice);
            cmd.Parameters.AddWithValue(maxDistanceToBeach);
            cmd.Parameters.AddWithValue(maxDistanceToCenter);
            cmd.Parameters.AddWithValue(hasPool);
            cmd.Parameters.AddWithValue(hasEntertainment);
            cmd.Parameters.AddWithValue(hasKidsClub);
            cmd.Parameters.AddWithValue(hasRestaurant);
            cmd.Parameters.AddWithValue(minRating);
            cmd.Parameters.AddWithValue(startDate);
            cmd.Parameters.AddWithValue(endDate);
            cmd.Parameters.AddWithValue(roomType);

            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    string hotelName = reader.GetString(0); // TEXT/VARCHAR
                    decimal price = reader.GetDecimal(1); // NUMERIC/DECIMAL
                    string cityResult = reader.GetString(2); // TEXT/VARCHAR
                    decimal rating = reader.GetDecimal(3); // NUMERIC/DECIMAL

                    availableRooms.Add($"Hotel: {hotelName}, Price: {price}, City: {cityResult}, Rating: {rating}");
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }

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

    public async Task UpdateBooking(string bookingId, string? newStartDate, string? newEndDate, string? newExtraBed, string? newHalfBoard, string? newFullBoard)
    {
        try
        {
            await using (var cmd = _holidaymaker.CreateCommand(@"
                UPDATE booking 
                SET start_date = COALESCE($2, start_date),
                    end_date = COALESCE($3, end_date),
                    extra_bed = COALESCE($4, extra_bed),
                    half_board = COALESCE($5, half_board),
                    full_board = COALESCE($6, full_board)
                WHERE id = $1"))
            {
                cmd.Parameters.AddWithValue(int.Parse(bookingId));
                cmd.Parameters.AddWithValue(newStartDate);
                cmd.Parameters.AddWithValue(newEndDate);
                cmd.Parameters.AddWithValue(string.IsNullOrEmpty(newExtraBed) ? (object)DBNull.Value : bool.Parse(newExtraBed));
                cmd.Parameters.AddWithValue(string.IsNullOrEmpty(newHalfBoard) ? (object)DBNull.Value : bool.Parse(newHalfBoard));
                cmd.Parameters.AddWithValue(string.IsNullOrEmpty(newFullBoard) ? (object)DBNull.Value : bool.Parse(newFullBoard));
                await cmd.ExecuteNonQueryAsync();
                Console.WriteLine("Booking updated successfully.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating booking: {ex.Message}");
        }
    }

    public async Task DeleteBooking(string bookingId)
    {
        try
        {
            await using (var cmd = _holidaymaker.CreateCommand("DELETE FROM booking WHERE id = $1"))
            {
                cmd.Parameters.AddWithValue(int.Parse(bookingId));
                await cmd.ExecuteNonQueryAsync();
                Console.WriteLine("Booking deleted successfully.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting booking: {ex.Message}");
        }
    }

    public async Task ListBookings()
    {
        try
        {
            await using (var cmd = _holidaymaker.CreateCommand("SELECT * FROM booking"))
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    Console.WriteLine($"ID: {reader["id"]}, Customer ID: {reader["customer_id"]}, Accommodation ID: {reader["accommodation_id"]}, Start Date: {reader["start_date"]}, End Date: {reader["end_date"]}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error listing bookings: {ex.Message}");
        }
    }

    public async Task ListTable(string tableName)
    {
        try
        {
            string query = $"SELECT * FROM {tableName}";
            await using (var cmd = _holidaymaker.CreateCommand(query))
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    Console.Write($"{reader.GetName(i)}\t");
                }
                Console.WriteLine();

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
            Console.WriteLine($"Error fetching table {tableName}: {ex.Message}");
        }
    }
}

 