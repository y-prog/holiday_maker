using System;
using System.Threading.Tasks;
using Npgsql;

namespace MenuWithDatabase
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var database = new Database();
            var connection = database.Connection();

            var actions = new Actions(connection);
            var menu = new Menu(actions);

            await menu.ShowMenu();
        }
    }
}