using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess.DataAccess.Contracts.Dto;

namespace Chess.DataAccess.Contracts
{
    public interface IUserDataAccess
    {
        Task<UserDto> GetUserByLoginAsync(string login);

        UserDto GetUserByExtenalLogin(string externalKey, string externalLoginType);

        Task<long> AddNewUser(NewUserCommandDto newUserCommandDto);

        void UpdateExternalLoginValues(UserDto userDto, string firstName, string lastName, string photoRec);
    }
}
