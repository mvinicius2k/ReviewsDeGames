using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using ReviewsDeGames.Controllers;
using ReviewsDeGames.Models;
using System.Data;

namespace ReviewsDeGames
{
    public class OData
    {
        public static IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder = new();
            builder.EntitySet<User>(UsersController.Route).EntityType.Ignore(u => u.PasswordHash);
            //builder.EntitySet<Role>(RolesController.Route);
            //builder.EntitySet<Board>(BoardsController.Route);
            return builder.GetEdmModel();
        }
    }
}
