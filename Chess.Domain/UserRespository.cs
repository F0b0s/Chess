using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess.DataAccess.Contracts;
using Chess.DataAccess.Contracts.Dto;
using Chess.Domain.Builders;

namespace Chess.Domain
{
    public class UsersRepository : IUsersRepository
    {
        private readonly IUserDataAccess _userDataAccess;

        public UsersRepository(IUserDataAccess userDataAccess)
        {
            _userDataAccess = userDataAccess;
        }

        public async Task<bool> ValidateUser(string name, string password)
        {
            var user = await _userDataAccess.GetUserByLoginAsync(name);

            return user != null && user.Password == password;
        }

        public async Task<User> GetUserByAppLogin(string userLogin, string password)
        {
            var user = await _userDataAccess.GetUserByLoginAsync(userLogin);

            if (string.Equals(user.Password, password))
                return user.CreateAccountEntity();

            return null;
        }

        public User GetExternalUserByLogin(string provider, string userLogin)
        {
            var user = _userDataAccess.GetUserByExtenalLogin(userLogin, provider);

            return user?.CreateAccountEntity();
        }

        public bool ValidateExternalUser(string externalUserId, string externalLoginType)
        {
            var user = _userDataAccess.GetUserByExtenalLogin(externalUserId, externalLoginType);

            return user != null;
        }

        public UserDto GetUserByExtenalLogin(string externalLogin)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CreateUser(string login, string password)
        {
            try
            {
                await _userDataAccess.AddNewUser(new NewUserCommandDto
                {
                    ExternalLoginId = string.Empty,
                    Login = login,
                    FirstName = string.Empty,
                    LastName = string.Empty,
                    LoginType = string.Empty,
                    Password = password
                });
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<User> CreateExternalUser(string externalUserId, string firstName, string lastName, string photoRec, string externalLoginType, string email)
        {
            var newUser = await _userDataAccess.AddNewUser(new NewUserCommandDto
            {
                ExternalLoginId = externalUserId,
                LoginType = externalLoginType,
                FirstName = firstName,
                LastName = lastName,
                Login = null    ,
                Password = null,
                PhotoRec = photoRec,
                Email = email
            });

            return _userDataAccess.GetUserByExtenalLogin(externalUserId, externalLoginType).CreateAccountEntity();
        }

        public bool ValidateExternalUser(string externalUserId, string firstName, string lastName, string photoRec, string externalLoginType)
        {
            var externalUser = _userDataAccess.GetUserByExtenalLogin(externalUserId, externalLoginType);
            if (externalUser != null)
            {
                _userDataAccess.UpdateExternalLoginValues(externalUser, firstName, lastName, photoRec);
                return true;
            }

            return false;
        }
    }

    public interface IUsersRepository
    {
        Task<bool> ValidateUser(string name, string password);

        Task<User> GetUserByAppLogin(string userLogin, string password);

        User GetExternalUserByLogin(string provider, string userLogin);

        UserDto GetUserByExtenalLogin(string externalLogin);

        Task<bool> CreateUser(string login, string password);

        Task<User> CreateExternalUser(string externalUserId, string firstName, string lastName, string photoRec, string externalLoginType, string email);
        bool ValidateExternalUser(string externalUserId, string firstName, string lastName, string photoRec, string externalLoginType);
    }
}
