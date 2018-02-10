﻿using System.IO;
using System.Net;

namespace RequestifyTF2.Utils
{
    public class AutoHotKeyChecker
    {
        public static void Check(string path)

        {
            if (Directory.Exists(path + "/ahk/"))
            {
                if (!File.Exists(path + "/ahk/ahk.exe"))
                    using (var web = new WebClient())
                    {
                        web.DownloadFile("https://github.com/weespin/RequestifyTF2/releases/download/0.0.1/ahk.exe",
                            path + "/ahk/ahk.exe");
                    }
            }
            else
            {
                Directory.CreateDirectory(path + "/ahk/");
                using (var web = new WebClient())
                {
                    web.DownloadFile("https://github.com/weespin/RequestifyTF2/releases/download/0.0.1/ahk.exe",
                        path + "/ahk/ahk.exe");
                }
            }
        }
    }
}