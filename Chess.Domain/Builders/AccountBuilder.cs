using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess.DataAccess.Contracts.Dto;

namespace Chess.Domain.Builders
{
    public static class AccountBuilder
    {
        public static User CreateAccountEntity(this UserDto userDto)
        {
            return new User
            {
                Id = userDto.Id,
                Login = userDto.Login,
                Email = userDto.Email,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                PhotoRec = userDto.PhotoRec
            };
        }

        public static User CreateAccountEntityWithExternalLogins(this UserDto userDto)
        {
            var user = userDto.CreateAccountEntity();

            user.ExternalLogins = userDto.Logins.Select(CreateExternalLogin);
            return user;
        }

        private static ExternalLoginInfo CreateExternalLogin(LoginDto loginDto)
        {
            return new ExternalLoginInfo
            {
                ExternalUserId = loginDto.ExternalUserId,
                ProviderKey = loginDto.LoginType.ToString(),
            };
        }
    }
}