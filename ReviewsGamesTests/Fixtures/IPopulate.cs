namespace ReviewsGamesTests.Fixtures
{
    public interface IPopulate
    {
        public Task<HttpResponseMessage> Populate(HttpClient http);
    }
}
