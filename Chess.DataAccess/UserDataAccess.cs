using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess.Data;
using Chess.DataAccess.Builders;
using Chess.DataAccess.Contracts;
using Chess.DataAccess.Contracts.Dto;

namespace Chess.DataAccess
{
    public class UserDataAccess : IUserDataAccess
    {
        public async Task<UserDto> GetUserByLoginAsync(string login)
        {
            using (var context = new ChessDbContext())
            {
                User user = await context.Users.Include("Logins").FirstOrDefaultAsync(i => i.Login == login);

                if (user == null)
                    return null;

                return user.CreateUserDto(user.Logins);
            }
        }

        public UserDto GetUserByExtenalLogin(string externalKey, string externalLoginType)
        {
            using (var context = new ChessDbContext())
            {
                Login user = context.Logins.FirstOrDefault(i => i.ExternalUserId == externalKey && i.LoginType == externalLoginType);

                if (user == null)
                    return null;

                return user.User.CreateUserDto(user.User.Logins);
            }
        }

        public async Task<long> AddNewUser(NewUserCommandDto newUserCommandDto)
        {
            using (var context = new ChessDbContext())
            {
                var newUser = new User
                {
                    FirstName = newUserCommandDto.FirstName,
                    LastName = newUserCommandDto.LastName,
                    PhotoRec = newUserCommandDto.PhotoRec,
                    Email = newUserCommandDto.Email
                };

                if (!string.IsNullOrWhiteSpace(newUserCommandDto.ExternalLoginId))
                {
                    var login = new Login
                    {
                        LoginType = newUserCommandDto.LoginType,
                        ExternalUserId = newUserCommandDto.ExternalLoginId,
                        User = newUser
                    };

                    newUser.Login = newUserCommandDto.Email;

                    context.Logins.Add(login);
                }
                else
                {
                    newUser.Login = newUserCommandDto.Login;
                    newUser.Password = newUserCommandDto.Password;
                }

                context.Users.Add(newUser);

                await context.SaveChangesAsync();

                return context.Logins.First(i => i.LoginType == newUserCommandDto.LoginType && i.ExternalUserId == newUserCommandDto.ExternalLoginId).Id;
            }
        }

        public void UpdateExternalLoginValues(UserDto userDto, string firstName, string lastName, string photoRec)
        {
            using (var context = new ChessDbContext())
            {
                var userEntity = new User { Id = userDto.Id };
                var entity = context.Users.Attach(userEntity);

                var wasModified = false;

                if (userDto.PhotoRec != photoRec)
                {
                    entity.PhotoRec = photoRec;
                    //context.Entry(entity).Property((item) => item.PhotoRec).IsModified = true;
                    wasModified = true;
                }

                if (userDto.FirstName != firstName)
                {
                    entity.FirstName = photoRec;
                    //context.Entry(entity).Property((item) => item.FirstName).IsModified = true;
                    wasModified = true;
                }

                if (userDto.LastName != lastName)
                {
                    entity.LastName = lastName;
                    //context.Entry(entity).Property((item) => item.LastName).IsModified = true;
                }

                if (wasModified)
                    context.SaveChanges();
            }
        }
    }
}
