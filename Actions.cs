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
 
    public async void listCity()
    {
        // Fråga användaren om vilken stad de vill söka på
        Console.Write("Enter city name: ");
        string city = Console.ReadLine();

        // Förbered SQL-kommandot för att hämta städer från vyn "LedigaRum" där city matchar
        await using (var cmd = _holidaymaker.CreateCommand("SELECT city FROM  ledigaRum WHERE city = city"))
        {
            // Lägg till parameter för city
            cmd.Parameters.AddWithValue("@city", city);

            // Kör kommandot och hämta resultatet
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                // Läs varje rad från resultatet
                while (await reader.ReadAsync())
                {
                    // Skriv ut endast city
                    Console.WriteLine($"city: {reader.GetString(0)}");
                }
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
}
 