using System;

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
        Console.WriteLine("1. Create Booking");
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

                case ("1"):
                     _actions.listCity(); 


                    /*await _actions.dateStart(); // start date   
                    await _actions.dateEnd(); // end date
                    await _actions.RoomType(); // list room types
                    await _actions.priceNight(); // price per night
                    await _actions.distanceBeach(); // distance to beach
                    await _actions.distanceCenter(); // distance to center
                    await _actions.hasPool(); // has pool   
                    await _actions.hasEntertaiment(); // has entertainment
                    await _actions.hasKidsClub(); // has kids club
                    await _actions.hasRestaurant(); // has restaurant
                    await _actions.rating(); // rating
                    await _actions.extraBed(); // extra bed
                    await _actions.fullBoard(); // want fullboard?
                    await _actions.halfBoard(); // want halfboard?
                    await _actions.ShowResults(); // show result */
                    
                    break;




                    PrintMenu();
            }

        }

    }
}