namespace SwiftDrop.ViewModels
{
    public class MapMarkerDto
    {
        public int OrderId { get; set; }
        public decimal Lat { get; set; }
        public decimal Lng { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "Pickup" or "Delivery"
    }
}