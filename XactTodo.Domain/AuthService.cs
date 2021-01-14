using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XactTodo.Domain.AggregatesModel.IdentityAggregate;
using XactTodo.Domain.AggregatesModel.UserAggregate;
using XactTodo.Domain.Exceptions;
using XactTodo.Security;
using XactTodo.Security.Session;

namespace XactTodo.Domain
{
    public class AuthService : IAuthService
    {
        public const int TOKEN_EXPIRATION = 3600 * 24 * 7; //seconds 设为7天才过期，客户端不必自动刷新Token
        private readonly ILogger log;
        private readonly IClaimsSession session;
        private readonly IUserRepository userRepository;
        private readonly IIdentityRepository identityRepository;

        public AuthService(ILogger<AuthService> _log,
            IClaimsSession session,
            IUserRepository userRepository,
            SeedWork.IRepository<User> userRepository2,
            SeedWork.IRepository<User, int> userRepository3,
            IIdentityRepository identityRepository)
        {
            this.log = _log;
            this.session = session;
            this.userRepository = userRepository;
            this.identityRepository = identityRepository;
        }

        public Identity Login(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException(nameof(userName));
            userName = userName.Trim();
            var user = userRepository.Find(p => p.UserName == userName || p.Email == userName).SingleOrDefault();
            if (user == null)
            {
                throw new UserNotFoundException(userName);
            }
            if (string.IsNullOrEmpty(user.Password))//如果当前账号未设置密码
            {
                user.ChangePassword("", password);  //则将本次输入的密码作为该用户的密码
                userRepository.Update(user);
            }
            else if (!user.ValidatePassword(password))
            {
                throw new PasswordInvalidException();
            }
            var token = Token.NewToken(TOKEN_EXPIRATION);
            var identity = new Identity
            {
                UserId = user.Id,
                UserName = user.UserName,
                Nickname = user.DisplayName,
                AccessToken = token.AccessToken,
                RefreshToken = token.RefreshToken,
                IssueTime = token.IssueTime,
                ExpiresIn = token.ExpiresIn
            };
            identityRepository.Add(identity);
            identityRepository.UnitOfWork.SaveChangesAsync().Wait();
            return identity;
        }

        public void Logout()
        {
            if (!session.UserId.HasValue)
                return;
            var identity = identityRepository.Find(p => p.AccessToken == session.AccessToken).FirstOrDefault();
            if (identity != null)
            {
                identity.Invalid = true;
                identityRepository.Update(identity);
            }
        }

        public Token RefreshToken(string refreshToken)
        {
            var accessToken = session?.AccessToken;
            var existed = identityRepository.Find(p => p.RefreshToken == refreshToken).FirstOrDefault();
            if (existed == null)
            {
                log.LogWarning($"Identity not found via refresh_token:{refreshToken}");
                throw new Exception($"[RefreshToken] is invalid! {refreshToken}");
            }
            if(!existed.AccessToken.Equals(accessToken, StringComparison.InvariantCultureIgnoreCase))
            {
                log.LogWarning($"access_tokon({accessToken}) & refresh_token({refreshToken}) is not matched!");
                throw new Exception($"[AccessToken] is invalid! {accessToken}");
            }
            //添加新的身份证明信息
            var token = Token.NewToken(TOKEN_EXPIRATION);
            var identity = new Identity
            {
                UserId = existed.UserId,
                UserName = existed.UserName,
                Nickname = existed.Nickname,
                AccessToken = token.AccessToken,
                RefreshToken = token.RefreshToken,
                IssueTime = token.IssueTime,
                ExpiresIn = token.ExpiresIn
            };
            identityRepository.Add(identity);
            //然后将旧的身份证明信息设置为30秒后过期，以免部分堵塞在路上的请求被拒绝
            existed.ExpiresIn = (int)(DateTime.Now.AddSeconds(30) - existed.IssueTime).TotalSeconds;
            identityRepository.Update(existed);
            //更新数据库
            identityRepository.UnitOfWork.SaveChangesAsync().Wait();
            return token;
        }

        public Identity ValidateToken(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
                return null;
            var identity = identityRepository.Find(p => !p.Invalid && p.AccessToken == accessToken).FirstOrDefault();
            if (identity == null)
            {
                throw new Exception("access_token is invalid!");
            }
            if (identity.IssueTime.AddSeconds(identity.ExpiresIn) < DateTime.Now)
            {
                throw new Exception("Token is expired!");
            }
            identity.RefreshToken = null; //不返回刷新令牌，以免泄漏。即：刷新令牌只能通过登录接口获取到
            return identity;
        }

        public User CurrentLoginUser
        {
            get
            {
                return userRepository.FindById(session.UserId ?? -1);
            }
        }

    }
}
