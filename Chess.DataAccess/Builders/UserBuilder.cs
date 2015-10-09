using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess.Data;
using Chess.DataAccess.Contracts.Dto;

namespace Chess.DataAccess.Builders
{
    public static class UserBuilder
    {
        public static UserDto CreateUserDto(this User user, IEnumerable<Login> logins)
        {
            var userDto = user.CreateUserDto();
            userDto.Logins = logins.Select(CreateLoginDto).ToArray();

            return userDto;
        }

        public static UserDto CreateUserDto(this User user)
        {
            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Login = user.Login,
                Password = user.Password,
                PhotoRec = user.PhotoRec
            };
        }

        public static LoginDto CreateLoginDto(this Login login)
        {
            return new LoginDto
            {
                ExternalUserId = login.ExternalUserId,
                LoginType = login.LoginType
            };
        }
    }
}
