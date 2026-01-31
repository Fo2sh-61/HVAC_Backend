namespace Backend.DTO.ReviewDto
{
    public class CreateReviewDto
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
