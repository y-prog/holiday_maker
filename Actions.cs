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
    string? hotelname,
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

    // Ny SQL-query med namngivna parametrar
    string query = @"
    SELECT a.accommodation_id, a.accommodation_name, a.price_per_night, a.city, a.ratings
    FROM ledigaRum a
    WHERE a.city = @city
      AND a.price_per_night <= @maxPrice
      AND a.distance_to_beach <= @maxDistanceToBeach
      AND a.distance_to_city_center <= @maxDistanceToCenter
      AND a.has_pool = @hasPool
      AND a.has_evening_entertainment = @hasEntertainment
      AND a.has_kids_club = @hasKidsClub
      AND a.has_restaurants = @hasRestaurant
      AND a.ratings >= @minRating
      AND a.date_from <= @startDate
      AND a.date_to >= @endDate
      AND a.room_type = @roomType";


    // Om hotelname är angivet, lägg till filter
    if (!string.IsNullOrEmpty(hotelname))
    {
        query += " AND a.accommodation_name ILIKE @hotelName";
    }

    try
    {
        await using (var cmd = _holidaymaker.CreateCommand(query))
        {
            // Bind parametrarna till SQL-kommandot
            cmd.Parameters.Add(new NpgsqlParameter("@city", NpgsqlTypes.NpgsqlDbType.Text) { Value = city });
            cmd.Parameters.Add(new NpgsqlParameter("@maxPrice", NpgsqlTypes.NpgsqlDbType.Numeric) { Value = maxPrice });
            cmd.Parameters.Add(new NpgsqlParameter("@maxDistanceToBeach", NpgsqlTypes.NpgsqlDbType.Integer) { Value = maxDistanceToBeach });
            cmd.Parameters.Add(new NpgsqlParameter("@maxDistanceToCenter", NpgsqlTypes.NpgsqlDbType.Integer) { Value = maxDistanceToCenter });
            cmd.Parameters.Add(new NpgsqlParameter("@hasPool", NpgsqlTypes.NpgsqlDbType.Boolean) { Value = hasPool });
            cmd.Parameters.Add(new NpgsqlParameter("@hasEntertainment", NpgsqlTypes.NpgsqlDbType.Boolean) { Value = hasEntertainment });
            cmd.Parameters.Add(new NpgsqlParameter("@hasKidsClub", NpgsqlTypes.NpgsqlDbType.Boolean) { Value = hasKidsClub });
            cmd.Parameters.Add(new NpgsqlParameter("@hasRestaurant", NpgsqlTypes.NpgsqlDbType.Boolean) { Value = hasRestaurant });
            cmd.Parameters.Add(new NpgsqlParameter("@minRating", NpgsqlTypes.NpgsqlDbType.Numeric) { Value = minRating });
            cmd.Parameters.Add(new NpgsqlParameter("@startDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = startDate });
            cmd.Parameters.Add(new NpgsqlParameter("@endDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = endDate });
            cmd.Parameters.Add(new NpgsqlParameter("@roomType", NpgsqlTypes.NpgsqlDbType.Text) { Value = roomType });

            if (!string.IsNullOrEmpty(hotelname))
            {
                cmd.Parameters.Add(new NpgsqlParameter("@hotelName", NpgsqlTypes.NpgsqlDbType.Text) { Value = $"%{hotelname}%" });
            }

            Console.WriteLine("Executing SQL Query:");
            Console.WriteLine(cmd.CommandText);

            foreach (NpgsqlParameter param in cmd.Parameters)
            {
                Console.WriteLine($"Param: {param.ParameterName}, Value: {param.Value}, Type: {param.NpgsqlDbType}");
            }

            // Utför frågan
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int accommodationId = reader.GetInt32(0);  // Get the accommodation_id (first column)
                    string hotelName = reader.GetString(1);     // Get the accommodation_name (second column)
                    decimal price = reader.GetDecimal(2);       // Get the price_per_night (third column)
                    string cityResult = reader.GetString(3);    // Get the city (fourth column)
                    decimal rating = reader.GetDecimal(4);      // Get the ratings (fifth column)

                    availableRooms.Add($"ID: {accommodationId}, Hotel: {hotelName}, Price: {price}, City: {cityResult}, Rating: {rating}");
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
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

 public async Task UpdateBooking(string? bookingId, string? email, string? newStartDate, string? newEndDate, string? newExtraBed, string? newHalfBoard, string? newFullBoard)
{
    try
    {
        // Kontrollera att antingen bookingId eller email är angivet
        if (string.IsNullOrEmpty(bookingId) && string.IsNullOrEmpty(email))
        {
            Console.WriteLine("Either Booking ID or Email must be provided.");
            return;
        }

        // Förbered datum och bool-värden
        DateTime? parsedStartDate = null;
        DateTime? parsedEndDate = null;

        if (!string.IsNullOrEmpty(newStartDate) && DateTime.TryParse(newStartDate, out DateTime startDate))
        {
            parsedStartDate = startDate;
        }

        if (!string.IsNullOrEmpty(newEndDate) && DateTime.TryParse(newEndDate, out DateTime endDate))
        {
            parsedEndDate = endDate;
        }

        // SQL för att hantera både Booking ID och Email
        string sqlQuery = !string.IsNullOrEmpty(bookingId)
            ? @"
                UPDATE booking 
                SET start_date = COALESCE($2, start_date),
                    end_date = COALESCE($3, end_date),
                    extra_bed = COALESCE($4, extra_bed),
                    half_board = COALESCE($5, half_board),
                    full_board = COALESCE($6, full_board)
                WHERE id = $1"
            : @"
                UPDATE booking 
                SET start_date = COALESCE($2, start_date),
                    end_date = COALESCE($3, end_date),
                    extra_bed = COALESCE($4, extra_bed),
                    half_board = COALESCE($5, half_board),
                    full_board = COALESCE($6, full_board)
                WHERE customer_id = (
                    SELECT id FROM customers WHERE email = $1
                )";

        await using (var cmd = _holidaymaker.CreateCommand(sqlQuery))
        {
            // Lägg till parametrar
            cmd.Parameters.AddWithValue(!string.IsNullOrEmpty(bookingId) ? int.Parse(bookingId) : (object)email);

            // Använd parsed values eller DBNull
            cmd.Parameters.AddWithValue(parsedStartDate.HasValue ? (object)parsedStartDate.Value : DBNull.Value);
            cmd.Parameters.AddWithValue(parsedEndDate.HasValue ? (object)parsedEndDate.Value : DBNull.Value);
            cmd.Parameters.AddWithValue(string.IsNullOrEmpty(newExtraBed) ? (object)DBNull.Value : bool.Parse(newExtraBed));
            cmd.Parameters.AddWithValue(string.IsNullOrEmpty(newHalfBoard) ? (object)DBNull.Value : bool.Parse(newHalfBoard));
            cmd.Parameters.AddWithValue(string.IsNullOrEmpty(newFullBoard) ? (object)DBNull.Value : bool.Parse(newFullBoard));

            // Kör kommandot
            await cmd.ExecuteNonQueryAsync();
            Console.WriteLine("Booking updated successfully.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error updating booking: {ex.Message}");
    }
}


public async Task DisplayAndDeleteBooking(string identifier)
{
    try
    {
        string query;
        bool isId = int.TryParse(identifier, out int bookingId);

        if (isId)
        {
            
            query = @"
                SELECT booking.id, booking.start_date, booking.end_date, customers.email 
                FROM booking
                INNER JOIN customers ON booking.customer_id = customers.id
                WHERE booking.id = $1";
        }
        else
        {
            
            query = @"
                SELECT booking.id, booking.start_date, booking.end_date, customers.email 
                FROM booking
                INNER JOIN customers ON booking.customer_id = customers.id
                WHERE customers.email = $1";
        }

        await using (var cmd = _holidaymaker.CreateCommand(query))
        {
            if (isId)
            {
                
                cmd.Parameters.AddWithValue(bookingId);
            }
            else
            {
                
                cmd.Parameters.AddWithValue(identifier);
            }

            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    
                    Console.WriteLine($"Booking ID: {reader["id"]}");
                    Console.WriteLine($"Start Date: {reader["start_date"]}");
                    Console.WriteLine($"End Date: {reader["end_date"]}");
                    Console.WriteLine($"Email: {reader["email"]}");

                    Console.Write("Do you want to delete this booking? (yes/no): ");
                    string? input = Console.ReadLine()?.ToLower();

                    if (input == "yes")
                    {
                        
                        await DeleteBookingById((int)reader["id"]);
                    }
                    else
                    {
                        Console.WriteLine("Deletion canceled.");
                    }
                }
                else
                {
                    Console.WriteLine("No booking found with the given identifier.");
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error retrieving or deleting booking: {ex.Message}");
    }
}

private async Task DeleteBookingById(int bookingId)
{
    try
    {
        string query = "DELETE FROM booking WHERE id = $1";

        await using (var cmd = _holidaymaker.CreateCommand(query))
        {
            cmd.Parameters.AddWithValue(bookingId);
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

 