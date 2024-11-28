using System;
using System.Threading.Tasks;

namespace MenuWithDatabase
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Initialize the Database and Actions objects
            var database = new Database();
            var actions = new Actions(database);

            // Initialize the menu and show it
            var menu = new Menu(actions);
            await menu.ShowMenuAsync();
        }
    }
}