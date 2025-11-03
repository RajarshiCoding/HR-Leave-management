namespace HRManagementBackend.Models
{
    public class Holiday
    {
        public int HolidayId { get; set; }        // Primary key
        public required string Title { get; set; }         // Holiday name
        public string? Description { get; set; }  // Optional description
        public DateOnly Date { get; set; }        // Date of the holiday
        public DateOnly CreatedAt { get; set; }   // Record creation timestamp
    }

    public class HolidayUpdateDto
    {
        // public int? HolidayId { get; set; }        // Primary key
        public string? Title { get; set; }         // Holiday name
        public string? Description { get; set; }  // Optional description
        // public DateTime? Date { get; set; }        // Date of the holiday
        // public DateTime? CreatedAt { get; set; }   // Record creation timestamp
        
    }
}
