using System;
using Npgsql;
namespace MenuWithDatabase;

public class Actions
{
    NpgsqlDataSource _holidaymaker;
    public Actions(NpgsqlDataSource holidaymaker)
    {
        _holidaymaker =holidaymaker;
    }
    public async void ListAll()
    {
        await using (var cmd = _holidaymaker.CreateCommand("SELECT * FROM size"))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                Console.WriteLine($"id: {reader.GetInt32(0)} \t room_type1:{reader.GetString(1)}");
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
                    Console.WriteLine($"id: {reader.GetInt32(0)} \t name: {reader.GetString(1)} \t slogan: {reader.GetString(2)}");
                }
            }
        }
    }
    public async void AddOne(string name, string? slogan)
    {
        // Insert data
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
        Console.WriteLine("Enter updated name (required)");
        var name = Console.ReadLine(); // required
        Console.WriteLine("Enter updated slogan");
        var slogan = Console.ReadLine(); // not required
        if (name is not null)
        {
            // Update data
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
        // Delete data
        await using (var cmd = _holidaymaker.CreateCommand("DELETE FROM items WHERE id = $1"))
        {
            cmd.Parameters.AddWithValue(int.Parse(id));
            await cmd.ExecuteNonQueryAsync();
        }
    }
    
    public async void SortAfterPrice()
    {
        // Collect search parameters from the user
        Console.Write("Enter city name to search (or press Enter to skip): ");
        string city = Console.ReadLine()?.Trim();
    
        Console.Write("Enter room type (size_id) to search (or press Enter to skip): ");
        string sizeIdInput = Console.ReadLine()?.Trim();
        int? sizeId = string.IsNullOrWhiteSpace(sizeIdInput) ? (int?)null : int.Parse(sizeIdInput);

        // Build the SQL query dynamically based on the user input
        string query = "SELECT id, name, price_per_night, city, size_id FROM accommodation WHERE 1=1";

        // Add conditions to the query based on user input
        if (!string.IsNullOrWhiteSpace(city))
        {
            query += " AND city ILIKE @City"; // Case-insensitive match for city
        }
        if (sizeId.HasValue)
        {
            query += " AND size_id = @SizeId"; // Filter by size_id (room type)
        }

        query += " ORDER BY price_per_night ASC"; // Sorting by price

        // Execute the query with parameters
        await using (var cmd = _holidaymaker.CreateCommand(query))
        {
            // Add parameters to prevent SQL injection
            if (!string.IsNullOrWhiteSpace(city))
            {
                cmd.Parameters.AddWithValue("@City", "%" + city + "%"); // Use wildcards for partial matches
            }
            if (sizeId.HasValue)
            {
                cmd.Parameters.AddWithValue("@SizeId", sizeId.Value);
            }

            await using var reader = await cmd.ExecuteReaderAsync();
            if (!reader.HasRows)
            {
                Console.WriteLine("No results found.");
                return;
            }

            // Print the sorted results
            while (await reader.ReadAsync())
            {
                Console.WriteLine($"ID: {reader.GetInt32(0)}, Name: {reader.GetString(1)}, Price per Night: {reader.GetDecimal(2)}");
            }
        }
    }

    public async void SortAfterRatings()
    {
        const string query = @"
        SELECT name, ratings 
        FROM accommodation 
        GROUP BY name, ratings 
        ORDER BY ratings DESC";

        await using (var cmd = _holidaymaker.CreateCommand(query))
        {
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                // Print the sorted results
                Console.WriteLine($"name: {reader.GetString(0)}, ratings: {reader.GetDecimal(1)}");
            }
        }
    }
    
    public async Task RegisterCustomerAsync(string firstName, string lastName, string email, string phoneNumber, DateTime dateOfBirth)
    {
        const string query = @"
        INSERT INTO customers (first_name, last_name, email, phone_number, date_of_birth)
        VALUES (@FirstName, @LastName, @Email, @PhoneNumber, @DateOfBirth)";

        try
        {
            // Prepare the command with the parameters
            await using (var cmd = _holidaymaker.CreateCommand(query))
            {
                // Add the values from the parameters to the command
                cmd.Parameters.AddWithValue("@FirstName", firstName);
                cmd.Parameters.AddWithValue("@LastName", lastName);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                cmd.Parameters.AddWithValue("@DateOfBirth", dateOfBirth);

                // Execute the command (this will insert the user into the database)
                await cmd.ExecuteNonQueryAsync();

                Console.WriteLine("Customer registered successfully.");
            }
        }
        catch (Exception ex)
        {
            // Handle any errors that might occur during insertion
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    
    
    


}
