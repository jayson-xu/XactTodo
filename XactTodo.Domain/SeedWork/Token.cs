using System;
using System.Collections.Generic;
using System.Text;

namespace XactTodo.Domain
{
    /// <summary>
    /// 登录令牌
    /// </summary>
    public class Token
    {
        /// <summary>
        /// 访问令牌
        /// </summary>
        [Newtonsoft.Json.JsonProperty("access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// 令牌类型(默认为Bearer)
        /// </summary>
        [Newtonsoft.Json.JsonProperty("token_type")]
        public string TokenType { get; set; } = "bearer";

        /// <summary>
        /// 令牌有效时长(秒)
        /// </summary>
        [Newtonsoft.Json.JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        /// <summary>
        /// 令牌颁发时间
        /// </summary>
        [Newtonsoft.Json.JsonProperty("issue_time")]
        public DateTime IssueTime { get; set; }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        [Newtonsoft.Json.JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        /// <summary>
        /// 令牌失效时间
        /// </summary>
        [Newtonsoft.Json.JsonProperty("expire_time")]
        public DateTime ExpireTime
        {
            get { return IssueTime.AddSeconds(ExpiresIn); }
        }

        /// <summary>
        /// 无参构造函数(用于json序列化/反序列化)
        /// </summary>
        public Token()
        {
            this.IssueTime = DateTime.Now;
        }

        /// <summary>
        /// 创建一个新的令牌
        /// </summary>
        /// <param name="access_token">访问令牌</param>
        /// <param name="refresh_token">刷新令牌</param>
        /// <param name="expires_in">令牌有效时间(秒)</param>
        public Token(string access_token, string refresh_token, int expires_in) : this()
        {
            this.AccessToken = access_token;
            this.RefreshToken = refresh_token;
            this.ExpiresIn = expires_in;
            this.IssueTime = DateTime.Now;
        }

        public static Token NewToken(int expires_in)
        {
            var accessToken = Guid.NewGuid().ToString("N");
            var refreshToken = Guid.NewGuid().ToString("N");
            var token = new Token(accessToken, refreshToken, expires_in);
            return token;
        }
    }

}
