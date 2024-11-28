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

        public async Task ShowMenu()
        {
            while (true)
            {
                Console.WriteLine("Choose option:");
                Console.WriteLine("1. Create Booking");
                Console.WriteLine("9. Quit");

                var choice = Console.ReadLine();
                if (choice == "1")
                {
                    await CreateBookingFlow();
                }
                else if (choice == "9")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid option. Please try again.");
                }
            }
        }

        private async Task CreateBookingFlow()
        {
            Console.WriteLine("Enter city (leave blank for all cities):");
            var city = Console.ReadLine();

            DateTime startDate;
            while (true)
            {
                Console.WriteLine("Enter the start date for the booking (YYYY-MM-DD):");
                var startDateInput = Console.ReadLine();
                if (DateTime.TryParse(startDateInput, out startDate))
                {
                    break;
                }
                Console.WriteLine("Invalid date format. Please try again.");
            }

            DateTime endDate;
            while (true)
            {
                Console.WriteLine("Enter the end date for the booking (YYYY-MM-DD):");
                var endDateInput = Console.ReadLine();
                if (DateTime.TryParse(endDateInput, out endDate))
                {
                    break;
                }
                Console.WriteLine("Invalid date format. Please try again.");
            }

            Console.WriteLine("Enter max price per night (leave blank for no limit):");
            var maxPriceInput = Console.ReadLine();
            decimal? maxPricePerNight = string.IsNullOrEmpty(maxPriceInput) ? (decimal?)null : decimal.Parse(maxPriceInput);

            Console.WriteLine("Enter max distance to beach (leave blank for no limit):");
            var maxDistanceToBeachInput = Console.ReadLine();
            int? maxDistanceToBeach = string.IsNullOrEmpty(maxDistanceToBeachInput) ? (int?)null : int.Parse(maxDistanceToBeachInput);

            Console.WriteLine("Enter max distance to city center (leave blank for no limit):");
            var maxDistanceToCityCenterInput = Console.ReadLine();
            int? maxDistanceToCityCenter = string.IsNullOrEmpty(maxDistanceToCityCenterInput) ? (int?)null : int.Parse(maxDistanceToCityCenterInput);

            Console.WriteLine("Do you want a pool? (y/n):");
            var hasPool = Console.ReadLine().ToLower() == "y";

            Console.WriteLine("Do you want evening entertainment? (y/n):");
            var hasEveningEntertainment = Console.ReadLine().ToLower() == "y";

            Console.WriteLine("Do you want a kids' club? (y/n):");
            var hasKidsClub = Console.ReadLine().ToLower() == "y";

            var accommodations = await _actions.GetAvailableAccommodations(city, startDate, endDate, maxPricePerNight, maxDistanceToBeach, maxDistanceToCityCenter, hasPool, hasEveningEntertainment, hasKidsClub);

            if (accommodations.Count == 0)
            {
                Console.WriteLine("No accommodations found matching the criteria.");
                return;
            }

            for (int i = 0; i < accommodations.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {accommodations[i].Name} - {accommodations[i].PricePerNight} per night");
            }

            Console.WriteLine("Select an accommodation by index:");
            int index = int.Parse(Console.ReadLine()) - 1;
            var selectedAccommodation = accommodations[index];

            Console.WriteLine("Enter your first name:");
            var firstName = Console.ReadLine();

            Console.WriteLine("Enter your last name:");
            var lastName = Console.ReadLine();

            Console.WriteLine("Enter your phone number:");
            var phoneNumber = Console.ReadLine();

            Console.WriteLine("Enter your date of birth (YYYY-MM-DD):");
            var dateOfBirth = DateTime.Parse(Console.ReadLine());

            int customerId = await _actions.CreateCustomer(firstName, lastName, phoneNumber, dateOfBirth);

            Console.WriteLine("Do you want an extra bed? (y/n):");
            var extraBed = Console.ReadLine().ToLower() == "y";

            Console.WriteLine("Do you want half board? (y/n):");
            var halfBoard = Console.ReadLine().ToLower() == "y";

            Console.WriteLine("Do you want full board? (y/n):");
            var fullBoard = Console.ReadLine().ToLower() == "y";

            await _actions.CreateBooking(customerId, selectedAccommodation.Id, startDate, endDate, extraBed, halfBoard, fullBoard);
        }
    }
}