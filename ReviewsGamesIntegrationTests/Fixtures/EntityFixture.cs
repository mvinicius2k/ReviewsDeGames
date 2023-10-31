using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ReviewsGamesTests.Fixtures
{
    public interface IPopulate
    {
        public Task<HttpResponseMessage> Populate(HttpClient http);
    }

    public class EntityFixture<TData>
    {
        public HashSet<TData> Dataset { get; private set; } = new HashSet<TData>();

        public async Task<List<HttpResponseMessage>> Populate(IPopulate populate, int count, HttpClient http)
        {
            var result = new List<HttpResponseMessage>(count);
            for (int i = 0; i < result.Count; i++)
            {
                var response = await populate.Populate(http);
                result.Add(await populate.Populate(http));
                if (!response.IsSuccessStatusCode)
                    continue;
                var data = await response.Content.ReadFromJsonAsync<TData>();
                if(data != null)
                    Dataset.Add(data);
            }
            return result;
        }

        public TData PickRandom() => new Faker().PickRandom(Dataset.ToArray());

    }
}
