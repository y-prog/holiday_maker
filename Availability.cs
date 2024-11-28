public class Availability
{
    public int Id { get; set; }
    public int AccommodationId { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public bool IsAvailable { get; set; }
}