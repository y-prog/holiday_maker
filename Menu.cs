using System;
using System.Threading.Tasks;

namespace MenuWithDatabase
{
    public class Menu
    {
        private readonly Actions _actions;

        public Menu(Actions actions)
        {
            _actions = actions;
        }

        public async Task ShowMenuAsync()
        {
            while (true)
            {
                Console.WriteLine("\nChoose an option:");
                Console.WriteLine("1. Create Booking");
                Console.WriteLine("2. Sort Ascending");
                Console.WriteLine("3. Sort Ratings");
                Console.WriteLine("4. Insert New Customer");
                Console.WriteLine("5. Delete One");
                Console.WriteLine("6. Display Customers");
                Console.WriteLine("9. Quit");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        // Logic for Create Booking
                        break;

                    case "2":
                        // Logic for Sort Ascending
                        break;

                    case "3":
                        // Logic for Sort Ratings
                        break;

                    case "4":
                        // Insert New Customer
                        await InsertNewCustomerAsync();
                        break;

                    case "5":
                        // Delete One
                        break;

                    case "6":
                        // Display Customers
                        break;

                    case "9":
                        Console.WriteLine("Exiting...");
                        return;

                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        private async Task InsertNewCustomerAsync()
        {
            Console.WriteLine("Registering new customer...");

            Console.Write("First Name: ");
            string firstName = Console.ReadLine();

            Console.Write("Last Name: ");
            string lastName = Console.ReadLine();

            Console.Write("Email: ");
            string email = Console.ReadLine();

            Console.Write("Phone Number: ");
            string phoneNumber = Console.ReadLine();

            Console.Write("Date of Birth (yyyy-mm-dd): ");
            DateTime dateOfBirth = DateTime.Parse(Console.ReadLine());

            // Call InsertNewCustomerAsync from Actions class
            await _actions.InsertNewCustomerAsync(firstName, lastName, email, phoneNumber, dateOfBirth);
        }
    }
}
