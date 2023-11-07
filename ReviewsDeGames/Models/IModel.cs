namespace ReviewsDeGames.Models
{
    public interface IModel<TId>
    {
        public TId GetId();
        public void SetId(TId value);
        public object[] GetIdKeys();
    }
}
