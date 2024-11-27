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
        SELECT a.accommodation_name, a.price_per_night, a.city, a.ratings
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
                    string hotelName = reader.GetString(0);
                    decimal price = reader.GetDecimal(1);
                    string cityResult = reader.GetString(2);
                    decimal rating = reader.GetDecimal(3);

                    availableRooms.Add($"Hotel: {hotelName}, Price: {price}, City: {cityResult}, Rating: {rating}");
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
            // SQL-frågan som anropar vyn
            string query = "SELECT * FROM bokningsInfo";

            await using (var cmd = _holidaymaker.CreateCommand(query))
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                Console.WriteLine("Bookings:");
                while (await reader.ReadAsync())
                {
                    // Hämta alla relevanta kolumner från vyn
                    Console.WriteLine($"Booking ID: {reader["booking_id"]}");
                    Console.WriteLine($"Customer Name: {reader["customer_name"]}");
                    Console.WriteLine($"Customer Email: {reader["customer_email"]}");
                    Console.WriteLine($"Customer Phone: {reader["customer_phone"]}");
                    Console.WriteLine($"Accommodation Name: {reader["accommodation_name"]}");
                    Console.WriteLine($"City: {reader["city"]}, Country: {reader["country"]}");
                    Console.WriteLine($"Room Type: {reader["room_type"]}, Beds: {reader["beds"]}, Total Persons: {reader["total_persons"]}");
                    Console.WriteLine($"Start Date: {reader["start_date"]}, End Date: {reader["end_date"]}");
                    Console.WriteLine($"Extra Bed: {reader["extra_bed"]}, Half Board: {reader["half_board"]}, Full Board: {reader["full_board"]}");
                
                    // Kontrollera om gruppmedlemmar finns
                    if (reader["group_member_id"] != DBNull.Value && reader["group_member_name"] != DBNull.Value)
                    {
                        Console.WriteLine($"Group Member ID: {reader["group_member_id"]}, Name: {reader["group_member_name"]}");
                    }
                    Console.WriteLine(new string('-', 50)); // Separator mellan bokningar
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

 