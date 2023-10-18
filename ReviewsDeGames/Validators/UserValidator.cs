using FluentValidation;
using ReviewsDeGames.Helpers;
using ReviewsDeGames.Models;
using ReviewsDeGames.Repository;
using System.Resources;

namespace ReviewsDeGames.Validators
{
    public class UserValidator : AbstractValidator<UserRegisterDto>
    {
        public const string UpdateRuleSet = "update";
        private readonly IUserRepository _users;


        private bool IsUniqueOrSame(string nick)
        {
            var users = _users.GetQuery().FirstOrDefault();
            return users == null || nick == users.UserName;
        }

        public UserValidator(IUserRepository users)
        {
            _users = users;

            //Se for update, adicionar regra 
            RuleSet(UpdateRuleSet, () =>
            {
                RuleFor(u => u.UserName)
                      .Must(nick => IsUniqueOrSame(nick))
                      .WithMessage(u => Messages.Get(Message.UnavaliableUserName, u.UserName));
            });

            RuleFor(u => u.UserName)
               .NotEmpty()
               .MaximumLength(User.UsernameMaxLenght)
               .WithMessage(Messages.Get(Message.RangeLength, 1, User.UsernameMaxLenght))
               .Must(s => !s.Contains('@'))
               .WithMessage(Messages.Get(Message.InvalidCharacters));

            RuleFor(u => u.Email)
            .EmailAddress()
                .WithMessage(Messages.Get(Message.InvalidEmail));
        }

       
    }
}
