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
            builder.EntitySet<Image>(ImagesController.Route);
            builder.EntitySet<Post>(PostsController.Route);
            builder.EntitySet<UserVote>(UserVotesController.Route);
            return builder.GetEdmModel();
        }
    }

   
}
