﻿using FluentValidation;
using ReviewsDeGames.Helpers;
using ReviewsDeGames.Models;
using ReviewsDeGames.Repository;
using ReviewsDeGames.Services;
using System.Linq.Expressions;
using System.Resources;

namespace ReviewsDeGames.Validators
{
    public class UserValidator : AbstractValidator<UserRegisterDto>
    {
        public const string UpdateRuleSet = "update";
        public const string AddNewRuleSet = "new";
        private readonly IUserRepository _users;

        private bool IsUniqueOrSameUsername(string nick)
        {
            var normalizedNick = nick.ToUpper();
            var users = _users.GetQuery().FirstOrDefault(u => u.NormalizedUserName == normalizedNick);
            return users == null || nick.ToUpper() == users.NormalizedUserName;
        }
        private bool IsUniqueOrSameEmail(string email)
        {
            email = email.ToUpper();
            var users = _users.GetQuery().FirstOrDefault(u => u.NormalizedEmail == email);
            return users == null || email.ToUpper() == users.NormalizedEmail;
        }

       
        public UserValidator(IUserRepository users, IDescribesService describes, IImagesRepository images)
        {
            _users = users;

            //Se for update, adicionar regra 
            RuleSet(UpdateRuleSet, () =>
            {
                RuleFor(u => u.UserName)
                      .Must(nick => IsUniqueOrSameUsername(nick))
                      .WithMessage(u => describes.UnavaliableUserName(u.UserName));

                RuleFor(u => u.Email)
                    .Must(email => IsUniqueOrSameEmail(email))
                    .WithMessage(i => describes.UnavaliableEmail(i.Email));
            });
            RuleSet(AddNewRuleSet, () =>
            {
                RuleFor(u => u.UserName)
                    .Must(username => _users.GetQuery().Any(u => u.NormalizedUserName == username.ToUpper()) == false)
                    .WithMessage(username => describes.UnavaliableUserName(username));
                RuleFor(u => u.Email)
                    .Must(email => _users.GetQuery().Any(u => u.NormalizedUserName == email.ToUpper()) == false)
                    .WithMessage(username => describes.UnavaliableEmail(username));
            });



            RuleFor(u => u.UserName)
               .NotEmpty()
               .WithMessage(describes.NotEmpty())
               .MaximumLength(User.UsernameMaxLenght)
               .WithMessage(describes.MaxLength(User.UsernameMaxLenght))
               .Must(s => !s.Contains('@'))
               .WithMessage(describes.InvalidCharacters());

            RuleFor(u => u.Email)
            .EmailAddress()
                .WithMessage(describes.InvalidEmail());


            RuleFor(u => u.AvatarId)
                .MustAsync(async (avtId, _) => (await images.GetById(avtId.Value)) != null).When(u => u.AvatarId != null)
                .WithMessage(u => describes.EntityNotFound(nameof(Image), u.AvatarId));
                



        }

       
    }
}
