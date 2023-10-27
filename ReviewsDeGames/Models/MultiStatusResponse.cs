namespace ReviewsDeGames.Models
{
    public class MultiStatusResponse<T>
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public T? Result { get; set; }


    }
}
