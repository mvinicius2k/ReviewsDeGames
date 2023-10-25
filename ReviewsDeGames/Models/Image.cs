namespace ReviewsDeGames.Models
{
    public class Image : IModel<int>
    {
        public int Id { get; set; }
        public string OwnerId { get; set; }
        public DateTime UploadedAt { get; set; }

        public virtual User Owner { get; set; }

        public int GetId()
            => Id;

        public int SetId(int value)
            => Id = value;
    }
}
