using ReviewsDeGames.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewsGamesIntegrationTests.Helpers
{
    public static class EndPoints
    {
        public readonly static string UserRegister = string.Join('/', UsersController.Route, UsersController.ActionRegister);
        public readonly static string UserGetAll = string.Join('/', UsersController.Route, UsersController.ActionGet);
    }
}
