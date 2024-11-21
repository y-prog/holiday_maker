namespace MenuWithDatabase;

class Program
{
    
    static void Main(string[] args)
    {
        Database database = new();
        var db = database.Connection();
        var actions = new Actions(db);
        new Menu(actions);
    }
}