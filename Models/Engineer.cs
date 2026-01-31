using Backend.Common;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;
using static Azure.Core.HttpHeader;

namespace Backend.Models
{
    public class Engineer : BaseEntity
    {
        public string Specialization { get; set; }
        public int YearsOfExperiencer { get; set; }
        public bool? IsActive { get; set; } = true;
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }   
    }
} 
