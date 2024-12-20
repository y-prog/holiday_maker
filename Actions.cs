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
    decimal? maxPrice,          
    DateTime? startDate,        
    DateTime? endDate,          
    string? roomType,           
    int? maxDistanceToBeach,    
    int? maxDistanceToCenter,   
    bool? hasPool,              
    bool? hasEntertainment,     
    bool? hasKidsClub,          
    bool? hasRestaurant,        
    decimal? minRating)         
{
    List<string> availableRooms = new List<string>();

    
    string query = @"
        SELECT a.accommodation_id, a.accommodation_name, a.price_per_night, a.city, a.ratings, 
               a.date_from, a.date_to
        FROM ledigaRum a
        WHERE 1 = 1"; 

    
    if (!string.IsNullOrEmpty(city)) query += " AND a.city = @city";
    if (maxPrice.HasValue) query += " AND a.price_per_night <= @maxPrice";
    if (maxDistanceToBeach.HasValue) query += " AND a.distance_to_beach <= @maxDistanceToBeach";
    if (maxDistanceToCenter.HasValue) query += " AND a.distance_to_city_center <= @maxDistanceToCenter";
    if (hasPool.HasValue) query += " AND a.has_pool = @hasPool";
    if (hasEntertainment.HasValue) query += " AND a.has_evening_entertainment = @hasEntertainment";
    if (hasKidsClub.HasValue) query += " AND a.has_kids_club = @hasKidsClub";
    if (hasRestaurant.HasValue) query += " AND a.has_restaurants = @hasRestaurant";
    if (minRating.HasValue) query += " AND a.ratings >= @minRating";
    if (startDate.HasValue) query += " AND a.date_from <= @startDate";
    if (endDate.HasValue) query += " AND a.date_to >= @endDate";
    if (!string.IsNullOrEmpty(roomType)) query += " AND a.room_type = @roomType";
    if (!string.IsNullOrEmpty(hotelname)) query += " AND a.accommodation_name ILIKE @hotelName";

    try
    {
        await using (var cmd = _holidaymaker.CreateCommand(query))
        {
            
            if (!string.IsNullOrEmpty(city)) cmd.Parameters.Add(new NpgsqlParameter("@city", NpgsqlTypes.NpgsqlDbType.Text) { Value = city });
            if (maxPrice.HasValue) cmd.Parameters.Add(new NpgsqlParameter("@maxPrice", NpgsqlTypes.NpgsqlDbType.Numeric) { Value = maxPrice.Value });
            if (maxDistanceToBeach.HasValue) cmd.Parameters.Add(new NpgsqlParameter("@maxDistanceToBeach", NpgsqlTypes.NpgsqlDbType.Integer) { Value = maxDistanceToBeach.Value });
            if (maxDistanceToCenter.HasValue) cmd.Parameters.Add(new NpgsqlParameter("@maxDistanceToCenter", NpgsqlTypes.NpgsqlDbType.Integer) { Value = maxDistanceToCenter.Value });
            if (hasPool.HasValue) cmd.Parameters.Add(new NpgsqlParameter("@hasPool", NpgsqlTypes.NpgsqlDbType.Boolean) { Value = hasPool.Value });
            if (hasEntertainment.HasValue) cmd.Parameters.Add(new NpgsqlParameter("@hasEntertainment", NpgsqlTypes.NpgsqlDbType.Boolean) { Value = hasEntertainment.Value });
            if (hasKidsClub.HasValue) cmd.Parameters.Add(new NpgsqlParameter("@hasKidsClub", NpgsqlTypes.NpgsqlDbType.Boolean) { Value = hasKidsClub.Value });
            if (hasRestaurant.HasValue) cmd.Parameters.Add(new NpgsqlParameter("@hasRestaurant", NpgsqlTypes.NpgsqlDbType.Boolean) { Value = hasRestaurant.Value });
            if (minRating.HasValue) cmd.Parameters.Add(new NpgsqlParameter("@minRating", NpgsqlTypes.NpgsqlDbType.Numeric) { Value = minRating.Value });
            if (startDate.HasValue) cmd.Parameters.Add(new NpgsqlParameter("@startDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = startDate.Value });
            if (endDate.HasValue) cmd.Parameters.Add(new NpgsqlParameter("@endDate", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = endDate.Value });
            if (!string.IsNullOrEmpty(roomType)) cmd.Parameters.Add(new NpgsqlParameter("@roomType", NpgsqlTypes.NpgsqlDbType.Text) { Value = roomType });
            if (!string.IsNullOrEmpty(hotelname)) cmd.Parameters.Add(new NpgsqlParameter("@hotelName", NpgsqlTypes.NpgsqlDbType.Text) { Value = $"%{hotelname}%" });

            Console.WriteLine("Executing SQL Query:");
            Console.WriteLine(cmd.CommandText);

            foreach (NpgsqlParameter param in cmd.Parameters)
            {
                Console.WriteLine($"Param: {param.ParameterName}, Value: {param.Value}, Type: {param.NpgsqlDbType}");
            }

            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int accommodationId = reader.GetInt32(0);  
                    string hotelName = reader.GetString(1);     
                    decimal price = reader.GetDecimal(2);       
                    string cityResult = reader.GetString(3);    
                    decimal rating = reader.GetDecimal(4);      
                    DateTime dateFrom = reader.GetDateTime(5);  
                    DateTime dateTo = reader.GetDateTime(6);    

                    
                    availableRooms.Add($"ID: {accommodationId}, Hotel: {hotelName}, Price: {price}, City: {cityResult}, Rating: {rating}, Available From: {dateFrom.ToShortDateString()} To: {dateTo.ToShortDateString()}");
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
    await using (var conn = _holidaymaker.CreateConnection())
    {
        await conn.OpenAsync();

        
        await using (var transaction = await conn.BeginTransactionAsync())
        {
            try
            {
                
                await using (var checkCmd = conn.CreateCommand())
                {
                    checkCmd.CommandText = @"
                        SELECT id
                        FROM Availability
                        WHERE accommodation_id = @AccommodationId
                          AND date_from <= @StartDate
                          AND date_to >= @EndDate
                        FOR UPDATE"; 
                    checkCmd.Transaction = transaction;

                    checkCmd.Parameters.AddWithValue("AccommodationId", accommodationId);
                    checkCmd.Parameters.AddWithValue("StartDate", startDate);
                    checkCmd.Parameters.AddWithValue("EndDate", endDate);

                    var availabilityId = await checkCmd.ExecuteScalarAsync();
                    if (availabilityId == null)
                    {
                        Console.WriteLine("No availability found for the selected dates.");
                        return;
                    }
                }

                
                await using (var insertCmd = conn.CreateCommand())
                {
                    insertCmd.CommandText = @"
                        INSERT INTO booking (customer_id, accommodation_id, start_date, end_date, extra_bed, half_board, full_board)
                        VALUES (@CustomerId, @AccommodationId, @StartDate, @EndDate, @ExtraBed, @HalfBoard, @FullBoard)
                        ON CONFLICT DO NOTHING";
                    insertCmd.Transaction = transaction;

                    insertCmd.Parameters.AddWithValue("CustomerId", customerId);
                    insertCmd.Parameters.AddWithValue("AccommodationId", accommodationId);
                    insertCmd.Parameters.AddWithValue("StartDate", startDate);
                    insertCmd.Parameters.AddWithValue("EndDate", endDate);
                    insertCmd.Parameters.AddWithValue("ExtraBed", extraBed);
                    insertCmd.Parameters.AddWithValue("HalfBoard", halfBoard);
                    insertCmd.Parameters.AddWithValue("FullBoard", fullBoard);

                    int rowsAffected = await insertCmd.ExecuteNonQueryAsync();

                    if (rowsAffected == 0)
                    {
                        Console.WriteLine("Booking already exists.");
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Booking created successfully.");
                    }
                }

                
                await using (var deleteCmd = conn.CreateCommand())
                {
                    deleteCmd.CommandText = @"
                        DELETE FROM Availability
                        WHERE accommodation_id = @AccommodationId
                          AND date_from <= @StartDate
                          AND date_to >= @EndDate";
                    deleteCmd.Transaction = transaction;

                    deleteCmd.Parameters.AddWithValue("AccommodationId", accommodationId);
                    deleteCmd.Parameters.AddWithValue("StartDate", startDate);
                    deleteCmd.Parameters.AddWithValue("EndDate", endDate);

                    int deleteRows = await deleteCmd.ExecuteNonQueryAsync();

                    if (deleteRows == 0)
                    {
                        Console.WriteLine("No availability was deleted. Check the availability dates.");
                    }
                    else
                    {
                        Console.WriteLine("Availability deleted successfully.");
                    }
                }

                
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                
                await transaction.RollbackAsync();
                Console.WriteLine($"Error creating booking: {ex.Message}");
            }
        }
    }
}








    public async Task<List<string>> GetSortedRooms(string sortBy)
    {
        List<string> sortedRooms = new List<string>();

        
        if (sortBy != "price" && sortBy != "rating")
        {
            Console.WriteLine("Invalid sort option. Please choose 'price' or 'rating'.");
            return sortedRooms;
        }

        
        string query = sortBy == "price"
            ? "SELECT city, price_per_night, ratings FROM filtreraRum ORDER BY price_per_night"
            : "SELECT city, price_per_night, ratings FROM filtreraRum ORDER BY ratings DESC";

        try
        {
            await using (var cmd = _holidaymaker.CreateCommand(query))
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    sortedRooms.Add($"City: {reader.GetString(0)}, Price: {reader.GetDecimal(1)}, Rating: {reader.GetDecimal(2)}");
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
        
        if (string.IsNullOrEmpty(bookingId) && string.IsNullOrEmpty(email))
        {
            Console.WriteLine("Either Booking ID or Email must be provided.");
            return;
        }

        
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
            
            cmd.Parameters.AddWithValue(!string.IsNullOrEmpty(bookingId) ? int.Parse(bookingId) : (object)email);

            
            cmd.Parameters.AddWithValue(parsedStartDate.HasValue ? (object)parsedStartDate.Value : DBNull.Value);
            cmd.Parameters.AddWithValue(parsedEndDate.HasValue ? (object)parsedEndDate.Value : DBNull.Value);
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



public async Task ListBookings(string filterValue)
{
    try
    {
        
        string query = @"
            SELECT * 
            FROM bokningsInfo
            WHERE customer_id::TEXT = @FilterValue OR customer_email = @FilterValue";

        await using (var cmd = _holidaymaker.CreateCommand(query))
        {
            
            cmd.Parameters.AddWithValue("FilterValue", filterValue);

            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    Console.WriteLine($"Booking ID: {reader["booking_id"]}, Customer Name: {reader["customer_name"]}, Email: {reader["customer_email"]}, Accommodation: {reader["accommodation_name"]}, Start Date: {reader["start_date"]}, End Date: {reader["end_date"]}");
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error listing bookings: {ex.Message}");
    }
}


    
    public async Task InsertNewCustomerAsync(string firstName, string lastName, string email, string phoneNumber, DateTime dateOfBirth)
    {
        string query = @"
        INSERT INTO customers (first_name, last_name, email, phone_number, date_of_birth) 
        VALUES (@FirstName, @LastName, @Email, @PhoneNumber, @DateOfBirth)";

        try
        {
            await using (var cmd = _holidaymaker.CreateCommand(query))
            {
                
                cmd.Parameters.Add(new NpgsqlParameter("@FirstName", NpgsqlTypes.NpgsqlDbType.Text) { Value = firstName });
                cmd.Parameters.Add(new NpgsqlParameter("@LastName", NpgsqlTypes.NpgsqlDbType.Text) { Value = lastName });
                cmd.Parameters.Add(new NpgsqlParameter("@Email", NpgsqlTypes.NpgsqlDbType.Text) { Value = email });
                cmd.Parameters.Add(new NpgsqlParameter("@PhoneNumber", NpgsqlTypes.NpgsqlDbType.Text) { Value = phoneNumber });
                cmd.Parameters.Add(new NpgsqlParameter("@DateOfBirth", NpgsqlTypes.NpgsqlDbType.Date) { Value = dateOfBirth });

                Console.WriteLine("Executing SQL Query:");
                Console.WriteLine(cmd.CommandText);

                foreach (NpgsqlParameter param in cmd.Parameters)
                {
                    Console.WriteLine($"Param: {param.ParameterName}, Value: {param.Value}, Type: {param.NpgsqlDbType}");
                }

                
                await cmd.ExecuteNonQueryAsync();
                Console.WriteLine("Customer inserted successfully.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while inserting customer: {ex.Message}");
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

 