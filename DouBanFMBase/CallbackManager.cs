using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DouBanFMBase
{
    public class CallbackManager
    {
        public static MainPage Mainpage;
        public static StartPage Startpage;
        public static void CallBackTrigger(int action,string msg)
        {
            switch (action)
            {
                case (int)DbFMCommonData.CallbackType.Login:
                    Startpage.LoginResult(msg);
                    break;
                case (int)DbFMCommonData.CallbackType.LoadedData:
                    if (DbFMCommonData.MainPageLoaded)
                    {
                        Mainpage.DataContextLoaded();
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
