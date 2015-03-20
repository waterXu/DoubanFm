using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DouBanFMBase
{
   public class TextConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            string retValue = "";
            bool boolVlaue = (bool)value;
            if (boolVlaue)
            {
                if (parameter!=null && parameter.ToString().Equals("NickName"))
                {
                    retValue = DbFMCommonData.NickName??(DbFMCommonData.NickName="请先登录");
                }
                else
                {
                    retValue = "更换账号";
                }
            }
            else
            {
                if (parameter != null && parameter.ToString().Equals("NickName"))
                {
                    retValue = "请先登录";
                }
                else
                {
                    retValue = "账号登录";
                }
            }
            return retValue;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            return value;
        }
    }
}
