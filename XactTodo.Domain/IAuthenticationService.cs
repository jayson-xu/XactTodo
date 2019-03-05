using System;
using System.Collections.Generic;
using System.Text;
using XactTodo.Domain.AggregatesModel.IdentityAggregate;
using XactTodo.Domain.SeedWork;

namespace XactTodo.Domain
{
    public interface IAuthenticationService: IService
    {
        Identity Login(string userName, string password);

        void Logout();

        Token RefreshToken(string refreshToken);

        Identity ValidateToken(string accessToken);

    }
}
