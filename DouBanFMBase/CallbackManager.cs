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
        public static void CallBackTrigger(int action,bool isSuccess)
        {
            switch (action)
            {
                case (int)DbFMCommonData.CallbackType.Login:
                    //判断在哪个page调用login接口
                    if (DbFMCommonData.MainPageLoaded)
                    {
                        Mainpage.UserLoginSuccess(isSuccess);
                        App.ViewModel.LoginSuccess = DbFMCommonData.loginSuccess;
                    }
                    else
                    {
                        Startpage.LoginResult(isSuccess);
                    }
                    break;
                case (int)DbFMCommonData.CallbackType.LoadedData:
                    if (DbFMCommonData.MainPageLoaded)
                    {
                        if (DbFMCommonData.DownLoadSuccess)
                        {
                            Mainpage.DataContextLoaded();
                        }
                        else
                        {
                            Mainpage.DataContextLoadedFail();
                        }
                    }
                    break;
                case (int)DbFMCommonData.CallbackType.LoadSongBack:
                    if (isSuccess)
                    {
                        Mainpage.GetSongSuccess(0);
                    }
                    else
                    {
                        Mainpage.GetSongFail();
                    }
                    break;
                    
                default:
                    break;
            }
        }
    }
}
