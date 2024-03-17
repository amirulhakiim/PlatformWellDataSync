// Located under PlatformWellDataSync/Models

namespace PlatformWellDataSync.Models
{
    public class PlatformData
    {
        public int Id { get; set; }
        public string? UniqueName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<WellData> Well { get; set; } = new List<WellData>();
    }

    public class WellData
    {
        public int Id { get; set; }
        public int PlatformId { get; set; }
        public string? UniqueName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
