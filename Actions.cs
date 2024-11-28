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

    }
}