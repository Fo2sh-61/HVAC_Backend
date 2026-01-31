using Backend.Common;
using Backend.Models;

public class Review:BaseEntity
{
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; }
}