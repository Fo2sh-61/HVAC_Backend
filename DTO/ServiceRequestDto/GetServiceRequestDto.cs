namespace Backend.DTO.ServiceRequestDto
{
    public class GetServiceRequestDto
    {
        public string CustomerId { get; set; }
        public Guid EngineerId { get; set; }
        public Guid ServiceId { get; set; }
        public int AcCount { get; set; }
        public DateTime? PreferedDateTime { get; set; }
        public string Status { get; set; }
        public double EstimatedPrice { get; set; }
        public double FinalPrice { get; set; }
        public string Notes { get; set; }
        public DateTime CompletedAt { get; set; }
    }
}
