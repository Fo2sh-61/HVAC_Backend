namespace Backend.DTO.ServiceRequestDto
{
    public class CreateServiceRequestDto
    { 
        public Guid ServiceId { get; set; }
        public int AcCount { get; set; }
        public DateTime? PreferedDateTime { get; set; }
        public string? Notes { get; set; } 
    }
}
