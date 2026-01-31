using Backend.Models;

namespace Backend.DTO.EngineerDto
{
    public class CreateEngineerDto
    {
        public string FullName { get; set; }
        public string UserId { get; set; }
        public string Specialization { get; set; }
        public int YearsOfExperiencer { get; set; }
    }
}
