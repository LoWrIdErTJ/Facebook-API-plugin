using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace FB_API_Plugin
{
    [Obfuscation(Feature = "Apply to member * when method or constructor: virtualization", Exclude = false)]
    public class PluginInfo
    {

        static readonly WebClient Client = new WebClient();

        static PluginInfo()
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var phantomjsfile = folder + "\\PhantomJS.exe";
            CheckFiles(phantomjsfile, "https://exbrowser.s3.amazonaws.com/PhantomJS.exe", "e22f66be7dd6a245f10ac4d3386216b0");
        }
        public static string HashCode => "39157c980c8024c7214335f5e9191f62607bd497";

        private static void CheckFiles(string file, string url, string md5)
        {
            if (File.Exists(file))
            {
                CheckMd5(file, md5);
            }

            if (File.Exists(file)) return;
            try
            {
                Client.DownloadFile(url, file);
                
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private static void CheckMd5(string filetoanalyze, string md5String)
        {
            try
            {
                using (var md5 = MD5.Create())
                {
                    var hash = md5.ComputeHash(File.ReadAllBytes(filetoanalyze));

                    var sBuilder = new StringBuilder();

                    foreach (var i in hash)
                    {
                        sBuilder.Append(i.ToString("x2"));
                    }


                    if (sBuilder.ToString() != md5String)
                    {
                        File.Delete(filetoanalyze);
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

    }
}
