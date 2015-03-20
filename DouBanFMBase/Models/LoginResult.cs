using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DouBanFMBase
{
    internal class LoginResult
    {
        /// <summary>
        /// 返回code
        /// </summary>
        public int r { get; set; }
        /// <summary>
        /// r为1时值有效，错误信息
        /// </summary>
        public string err { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public string user_id { get; set; }
        /// <summary>
        /// 登录成功token
        /// </summary>
        public string token { get; set; }
        /// <summary>
        /// 登录成功expire
        /// </summary>
        public string expire { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        public string user_name { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string email { get; set; }
    }
}
