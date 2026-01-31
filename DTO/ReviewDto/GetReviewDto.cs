namespace Backend.DTO.ReveiwDto
{
    public class GetReviewDto
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
