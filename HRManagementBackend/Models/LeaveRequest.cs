namespace HRManagementBackend.Models
{
    public class LeaveRequest
    {
        public int RequestId { get; set; }       // Primary key
        public int EmpId { get; set; }           // Employee who applied
        public string? Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }  // Leave start date
        public DateTime EndDate { get; set; }    // Leave end date
        public int NoOfDays { get; set; }        // Total leave days (calculated)
        public string? Reason { get; set; }      // Reason for leave
        public required string Status { get; set; }       // Pending / Approved / Rejected
        public string? HrNote { get; set; }      // HR comments
        public DateTime AppliedOn { get; set; }  // Request creation timestamp
        public DateTime? ReviewedOn { get; set; } // HR review timestamp
    }
    // DTO for updating leave status
    public class LeaveStatusUpdateDto
    {
        public string? Status { get; set; }   // "Approved" or "Rejected"
        public string? HrNote { get; set; }  // Optional note
    }
}
