public class Accommodation
{
    public int Id { get; set; }
    public int SizeId { get; set; }
    public string Name { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public decimal PricePerNight { get; set; }
    public int DistanceToBeach { get; set; }
    public int DistanceToCityCenter { get; set; }
    public bool HasPool { get; set; }
    public bool HasEveningEntertainment { get; set; }
    public bool HasKidsClub { get; set; }
    public bool HasRestaurants { get; set; }
    public decimal Ratings { get; set; }
}