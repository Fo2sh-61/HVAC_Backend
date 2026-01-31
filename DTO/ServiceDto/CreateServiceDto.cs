using Backend.Models;

namespace Backend.DTO.EngineerDto
{
    public class CreateServiceDto
    {
        public string Code { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public double BasePrice { get; set; }
        public bool IsActive { get; set; } = true;
    }
}