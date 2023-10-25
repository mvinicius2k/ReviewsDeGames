namespace ReviewsDeGames.Models
{
    public interface IModel<TId>
    {
        public TId GetId();
        public TId SetId(TId value);
    }
}
