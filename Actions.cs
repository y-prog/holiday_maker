using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;

namespace MenuWithDatabase
{
    public class Actions
    {
        private readonly NpgsqlDataSource _holidaymaker;

        public Actions(NpgsqlDataSource holidaymaker)
        {
            _holidaymaker = holidaymaker;
        }

        public async Task<List<Accommodation>> GetAvailableAccommodations(
            string city, DateTime? startDate, DateTime? endDate, decimal? maxPrice,
            int? maxDistanceToBeach, int? maxDistanceToCity, bool? hasPool,
            bool? hasEveningEntertainment, bool? hasKidsClub)
        {
            var accommodations = new List<Accommodation>();

            var query = @"
                SELECT a.id, a.name, a.price_per_night, a.distance_to_beach, a.distance_to_city_center,
                       a.has_pool, a.has_evening_entertainment, a.has_kids_club, a.has_restaurants, a.ratings
                FROM Accommodation a
                JOIN Availability av ON a.id = av.accommodation_id
                WHERE av.is_available = TRUE";

            var parameters = new List<object>();

            if (!string.IsNullOrEmpty(city))
            {
                query += " AND a.city = @city";
                parameters.Add(city);
            }

            if (startDate.HasValue)
            {
                query += " AND av.date_from <= @startDate";
                parameters.Add(startDate.Value);
            }

            if (endDate.HasValue)
            {
                query += " AND av.date_to >= @endDate";
                parameters.Add(endDate.Value);
            }

            if (maxPrice.HasValue)
            {
                query += " AND a.price_per_night <= @maxPrice";
                parameters.Add(maxPrice.Value);
            }

            if (maxDistanceToBeach.HasValue)
            {
                query += " AND a.distance_to_beach <= @maxDistanceToBeach";
                parameters.Add(maxDistanceToBeach.Value);
            }

            if (maxDistanceToCity.HasValue)
            {
                query += " AND a.distance_to_city_center <= @maxDistanceToCity";
                parameters.Add(maxDistanceToCity.Value);
            }

            if (hasPool.HasValue)
            {
                query += " AND a.has_pool = @hasPool";
                parameters.Add(hasPool.Value);
            }

            if (hasEveningEntertainment.HasValue)
            {
                query += " AND a.has_evening_entertainment = @hasEveningEntertainment";
                parameters.Add(hasEveningEntertainment.Value);
            }

            if (hasKidsClub.HasValue)
            {
                query += " AND a.has_kids_club = @hasKidsClub";
                parameters.Add(hasKidsClub.Value);
            }

            await using (var cmd = _holidaymaker.CreateCommand(query))
            {
                if (parameters.Count > 0)
                {
                    var parameterNames = new[]
                    {
                        "@city", "@startDate", "@endDate", "@maxPrice", "@maxDistanceToBeach", "@maxDistanceToCity",
                        "@hasPool", "@hasEveningEntertainment", "@hasKidsClub"
                    };
                    for (int i = 0; i < parameters.Count; i++)
                    {
                        cmd.Parameters.AddWithValue(parameterNames[i], parameters[i]);
                    }
                }

                await using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        accommodations.Add(new Accommodation
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            PricePerNight = reader.GetDecimal(2),
                            DistanceToBeach = reader.GetInt32(3),
                            DistanceToCityCenter = reader.GetInt32(4),
                            HasPool = reader.GetBoolean(5),
                            HasEveningEntertainment = reader.GetBoolean(6),
                            HasKidsClub = reader.GetBoolean(7),
                            HasRestaurants = reader.GetBoolean(8),
                            Ratings = reader.GetDecimal(9)
                        });
                    }
                }
            }

            return accommodations;
        }

        public async Task<int> CreateCustomer(string firstName, string lastName, string phoneNumber, DateTime dateOfBirth)
        {
            int newCustomerId = 0;

            await using (var cmd = _holidaymaker.CreateCommand(@"
                INSERT INTO Customers (first_name, last_name, phone_number, date_of_birth)
                VALUES ($1, $2, $3, $4) RETURNING id"))
            {
                cmd.Parameters.AddWithValue(firstName);
                cmd.Parameters.AddWithValue(lastName);
                cmd.Parameters.AddWithValue(phoneNumber);
                cmd.Parameters.AddWithValue(dateOfBirth);

                var result = await cmd.ExecuteScalarAsync();
                if (result != null)
                {
                    newCustomerId = Convert.ToInt32(result);
                }
            }

            return newCustomerId;
        }

        public async Task CreateBooking(int customerId, int accommodationId, DateTime startDate, DateTime endDate, bool extraBed, bool halfBoard, bool fullBoard)
        {
            await using (var cmd = _holidaymaker.CreateCommand(@"
                INSERT INTO Booking (customer_id, accommodation_id, start_date, end_date, extra_bed, half_board, full_board)
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
            }

            Console.WriteLine("Booking successfully created.");
        }
    }
}