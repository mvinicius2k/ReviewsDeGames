using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewsGamesIntegrationTests.Fixtures
{
    public class WebFactory : IDisposable
    {
        public WebApplicationFactory<Program> Instance;

        public WebFactory()
        {
            Instance = new WebApplicationFactory<Program>();
            Instance.ClientOptions.BaseAddress = new Uri("https://localhost:8000/");
        }

        public void Dispose()
            => Instance.Dispose();
    }
}
