namespace ReviewsDeGames.Models
{
    public class UserVote : IModel<(string user, int post)>
    {
        public string UserId { get; set; }
        public int PostId { get; set; }
        public int Value { get; set; }

        public User User { get; set; }
        public Post Post { get; set; }

        public (string user, int post) GetId()
            => (UserId, PostId);

        public void SetId((string user, int post) value)
        {
            UserId = value.user;
            PostId = value.post;
        }
    }

    public record UserVoteRequestDto
    {
        public int PostId { get; init; }
        public int Value { get; init; }
    }
}
