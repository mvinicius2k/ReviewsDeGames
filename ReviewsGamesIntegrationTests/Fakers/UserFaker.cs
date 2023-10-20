using Bogus;
using ReviewsDeGames.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewsGamesIntegrationTests.Fakers
{
    public class UserFaker : Faker<UserRegisterDto>
    {
        public UserFaker()
        {
            RuleFor(u => u.Email, f => f.Internet.Email());
            RuleFor(u => u.UserName, f => f.Internet.UserName());
            RuleFor(u => u.AvatarUrl, f => f.Internet.Avatar());

        }
    }
}
