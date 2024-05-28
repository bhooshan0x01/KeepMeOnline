using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeepMeOnline
{
    internal class Logger
    {
        public static void LogMessage(string message)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string logFilePath = $"{path}\\KeepMeAliveLog.txt";
            File.AppendAllText(logFilePath, $"{DateTime.Now}: {message}{Environment.NewLine}");
        }
    }
}
