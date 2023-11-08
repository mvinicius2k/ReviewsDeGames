using Bogus;
using ReviewsDeGames.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewsGamesTests.Fakers
{
    public class PostFaker : Faker<PostRequestDto>
    {
        public const string BadTitleRule = "badTitle";
        public const string BadFeaturedImageRule = "badFeatured";
        public PostFaker()
        {

            RuleFor(p => p.Title, f => f.Lorem.Sentence(5));
            RuleFor(p => p.Text, f => f.Lorem.Text());

            RuleSet(BadTitleRule, (r) => r.RuleFor(p => p.Title, f => f.Lorem.Paragraph(4)));
            RuleSet(BadFeaturedImageRule, (r) => r.RuleFor(p => p.Title, f => f.Lorem.Paragraph(4)));
        }

    }

    
}
