using Npgsql;
using System;
using System.Threading.Tasks;

namespace MenuWithDatabase
{
    public class Actions
    {
        private readonly Database _database;

        public Actions(Database database)
        {
            _database = database;
        }

        public async Task InsertNewCustomerAsync(string firstName, string lastName, string email, string phoneNumber, DateTime dateOfBirth)
        {
            string query = "INSERT INTO customers (first_name, last_name, email, phone_number, date_of_birth) " +
                           "VALUES (@FirstName, @LastName, @Email, @PhoneNumber, @DateOfBirth)";
            

            try
            {
                using var conn = _database.Connection().CreateConnection();
                await conn.OpenAsync();

                using var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("FirstName", firstName);
                cmd.Parameters.AddWithValue("LastName", lastName);
                cmd.Parameters.AddWithValue("Email", email);
                cmd.Parameters.AddWithValue("PhoneNumber", phoneNumber);
                cmd.Parameters.AddWithValue("DateOfBirth", dateOfBirth);

                await cmd.ExecuteNonQueryAsync();

                Console.WriteLine("Customer inserted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while inserting customer: {ex.Message}");
            }
        }
        
        
       public async Task DeleteCustomer()
{
    // Ask the user to input the email
    Console.WriteLine("Enter the email of the customer to delete:");
    string emailToDelete = Console.ReadLine()?.Trim().ToLower(); // Use .Trim() to avoid extra spaces

    // Define the query to get the customer details by email
    string queryGetCustomer = "SELECT first_name, last_name, email, phone_number, date_of_birth FROM customers WHERE email = @Email";

    try
    {
        // Open a separate connection to the database for the SELECT query
        using var connForSelect = _database.Connection().CreateConnection();
        await connForSelect.OpenAsync();

        // Prepare and execute the query to get the customer details
        using var cmdForSelect = new NpgsqlCommand(queryGetCustomer, connForSelect);
        cmdForSelect.Parameters.AddWithValue("Email", emailToDelete);

        // Execute the query and read the data
        using var reader = await cmdForSelect.ExecuteReaderAsync();

        // If no data is found
        if (!reader.HasRows)
        {
            Console.WriteLine($"No customer found with email '{emailToDelete}'.");
            return;
        }

        // Read and display the customer data
        reader.Read(); // Move to the first result
        string firstName = reader.GetString(0);
        string lastName = reader.GetString(1);
        string email = reader.GetString(2);
        string phoneNumber = reader.GetString(3);
        DateTime dateOfBirth = reader.GetDateTime(4);

        Console.WriteLine("\nCustomer details:");
        Console.WriteLine($"Name: {firstName} {lastName}");
        Console.WriteLine($"Email: {email}");
        Console.WriteLine($"Phone: {phoneNumber}");
        Console.WriteLine($"Date of Birth: {dateOfBirth.ToShortDateString()}");

        // Ask for confirmation
        Console.WriteLine("\nAre you sure you want to delete this customer? (Y/N)");

        string confirmation = Console.ReadLine()?.Trim().ToLower();

        // Check if user confirmed with 'y'
        if (confirmation == "y")
        {
            // Now that the customer is confirmed, we can proceed with deleting them
            string queryDelete = "DELETE FROM customers WHERE email = @Email";

            // Use a different connection for the DELETE query to avoid conflicts
            using var connForDelete = _database.Connection().CreateConnection();
            await connForDelete.OpenAsync();

            using var cmdForDelete = new NpgsqlCommand(queryDelete, connForDelete);
            cmdForDelete.Parameters.AddWithValue("Email", emailToDelete);
            
            int rowsAffected = await cmdForDelete.ExecuteNonQueryAsync();

            if (rowsAffected > 0)
            {
                Console.WriteLine($"Customer with email '{emailToDelete}' deleted successfully.");
            }
            else
            {
                Console.WriteLine($"Failed to delete customer with email '{emailToDelete}'.");
            }
        }
        else
        {
            Console.WriteLine("Deletion cancelled.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error while deleting customer: {ex.Message}");
    }
}
        
        

    }
}