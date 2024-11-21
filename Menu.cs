namespace MenuWithDatabase;

public class Menu
{
    Actions _actions;
    public Menu(Actions actions)
    {
        _actions = actions;
        PrintMenu();
    }

    private void PrintMenu()
    {
        Console.WriteLine("Choose option");
        Console.WriteLine("1. List all");
        Console.WriteLine("2. Show one");
        Console.WriteLine("3. Add one");
        Console.WriteLine("4. Update one");
        Console.WriteLine("5. Delete one");
        Console.WriteLine("9. Quit");
        AskUser();
    }

    private async void AskUser()
    {
        var response = Console.ReadLine();
        if (response is not null)
        {
            string? id; // define for multiple use below
            
            switch (response)
            {
                case("1"):
                    Console.WriteLine("Listing all");
                    _actions.ListAll();
                    break;
                case("2"):
                    Console.WriteLine("Enter id to show details about one");
                    id = Console.ReadLine();
                    if (id is not null)
                    { 
                        _actions.ShowOne(id);
                    }
                    break;
                case("3"):
                    Console.WriteLine("Enter name (required)");
                    var name = Console.ReadLine(); // required
                    Console.WriteLine("Enter slogan");
                    var slogan = Console.ReadLine(); // not required
                    if (name is not null)
                    {
                        _actions.AddOne(name, slogan);
                    }
                    break;
                case("4"):
                    Console.WriteLine("Enter id to update one");
                    id = Console.ReadLine();
                    if (id is not null)
                    { 
                        _actions.UpdateOne(id);
                    }
                    break;
                case("5"):
                    Console.WriteLine("Enter id to delete one");
                    id = Console.ReadLine();
                    if (id is not null)
                    { 
                        _actions.DeleteOne(id);
                    }
                    break;
                case("9"):
                    Console.WriteLine("Quitting");
                    Environment.Exit(0);
                    break;
            }

            PrintMenu();
        }
        
    }
    
}