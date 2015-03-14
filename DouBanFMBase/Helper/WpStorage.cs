using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.Threading;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;

namespace DouBanFMBase
{
    public class WpStorage
    {
        public static object GetIsoSetting(string name)
        {
            try
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(name))
                {
                    return IsolatedStorageSettings.ApplicationSettings[name] != null ? IsolatedStorageSettings.ApplicationSettings[name] : null;
                }
                return null;
            }
            catch
            {
                return null;
            }
           
        }
        public static void SetIsoSetting(string name,object value)
        {
            try
            {
                if (value == null)
                {
                    IsolatedStorageSettings.ApplicationSettings.Remove(name);
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings[name] = value;
                }
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
            catch { }
        }

        public static IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication();

        /// <summary>
        /// 保存文件到手机独立存储
        /// </summary>
        /// <param name="fileName"></param>
        public static void SaveToIsoStore(string fileName, string assemblyName)
        {
            try 
            {
                using (Stream stream = Application.GetResourceStream(new Uri(assemblyName + fileName, UriKind.Relative)).Stream)
                {
                    string strFileContent = string.Empty;
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        strFileContent = reader.ReadToEnd();
                        SaveStringToIsoStore(fileName, strFileContent);
                    }
                } 
               // Stream stream = Microsoft.Xna.Framework.TitleContainer.OpenStream(fileName);
               // byte[] data = new byte[stream.Length];
               // stream.Read(data, 0, (int)stream.Length);
               // SaveFilesToIsoStore(fileName, data);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(" SaveToIsoStore " + e.Message + e.Source);
            }
         
        }

        public static void SaveStringToIsoStore(string fileName, string data)
        {
            SaveFilesToIsoStore(fileName, System.Text.UTF8Encoding.UTF8.GetBytes(data));
        }
        /// <summary>
        ///保存文件夹内容到手机存储
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="data"></param>
        public static void SaveFilesToIsoStore(string fileName, byte[] data)
        {
            try
            {
                if (!isoFile.FileExists(fileName))
                {
                    string strBaseDir = string.Empty;
                    string delimStr = "/";
                    char[] delimiter = delimStr.ToCharArray();
                    string[] dirsPath = fileName.Split(delimiter);
                    for (int i = 0; i < dirsPath.Length - 1; i++)
                    {
                        strBaseDir = System.IO.Path.Combine(strBaseDir, dirsPath[i]);
                        isoFile.CreateDirectory(strBaseDir);
                    }
                    using (BinaryWriter bw = new BinaryWriter(isoFile.CreateFile(fileName)))
                    {
                        bw.Write(data);
                        bw.Close();
                    }
                }
            }
            catch
            {
                MessageBox.Show("存储错误");
            }
        }

        public static string readIsolatedStorageFile(string fileName)
        {
            string rn = "";
            if (isoFile.FileExists(fileName))
            {
                IsolatedStorageFileStream isofs = isoFile.OpenFile(fileName, FileMode.Open, FileAccess.Read);
                byte[] data = new byte[isofs.Length];
                isofs.Read(data, 0, (int)isofs.Length);
                isofs.Close();
                rn = System.Text.Encoding.UTF8.GetString(data, 0, data.Length);
            }
            return rn;
        }
    }
}
