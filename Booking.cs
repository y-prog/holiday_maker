namespace MenuWithDatabase;

public class Booking
{
    
    public  int customerID { get; set; } // Detta är CustomerId i databasen
    public int AccommodationId { get; set; } // Detta är RoomId i databasen
    public DateTime StartDateTime { get; set; } // Detta är StartDate i databasen
    public DateTime EndDateTime { get; set; }   // Detta är EndDate i databasen
    public Boolean extraBed { get; set; } // Detta är ExtraBed i databasen
    public Boolean halfBoard { get; set; } // Detta är HalfBoard i databasen
    public Boolean fullBoard { get; set; } // Detta är FullBoard i databasen
    // Du kan också lägga till andra egenskaper, t.ex. datum för bokningen, användarens namn, osv.
}