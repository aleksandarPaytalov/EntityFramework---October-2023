namespace CarDealer.DTOs.Import
{
    public class CarDTO
    {
        public string Make { get; set; }

        public string Model { get; set; }

        public long TraveledDistance { get; set; }

        public int[] PartsId { get; set; }

    }
}
