using Backend.Common;

namespace Backend.Models
{
    public class ServiceRequest : BaseEntity
    {
        public Guid CustomerId { get; set; }
        public Guid EngineerId { get; set; }
        public Guid ServiceId { get; set; }
        public int AcCount { get; set; }
        public DateTime? PreferedDateTime { get; set; }
        public string Status { get; set; } = "Not Started";
        public double EstimatedPrice { get; set; }
        public double FinalPrice { get; set; }
        public string? Notes { get; set; }
        public DateTime? CompletedAt { get; set; }

    }
} 
