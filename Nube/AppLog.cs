using System;
using System.IO;

namespace Nube
{
    public static class AppLog
    {

        public static string WriteLogState = "On";
        public static string WriteLogFileName = "NUBE_log";

        public static void WriteLog(String str)
        {
            if (WriteLogState.ToLower() != "off")
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter(Path.GetTempPath() + WriteLogFileName + ".txt", true))
                    {
                        writer.WriteLine(str);
                    }
                }
                catch (Exception) { }
            }

        }

        public static void WriteLog(String str, params object[] args)
        {
            if (WriteLogState.ToLower() != "off")
            {
                try
                {
                    WriteLog(string.Format(str, args));
                }
                catch (Exception ex) { WriteLog(ex); }
            }
        }


        public static void WriteLogDT(String str)
        {
            if (WriteLogState.ToLower() != "off")
            {
                try
                {
                    WriteLog(string.Format("{0:dd/MM/yyyy hh:mm:ss} => {1}", DateTime.Now, str));
                }
                catch (Exception) { }
            }

        }


        public static void WriteLogDT(String str, params object[] args)
        {
            if (WriteLogState.ToLower() != "off")
            {
                try
                {
                    WriteLogDT(string.Format(str, args));
                }
                catch (Exception ex) { WriteLog(ex); }
            }
        }


        public static void WriteLog(Exception ex)
        {
            WriteLogDT(string.Format("Error=> ExMessage:{0},StackTrace:{1}", ex.Message, ex.StackTrace));
        }

        public static void DisplayMsg(int Left, int Top, string Msg)
        {
            Console.CursorLeft = Left;
            Console.CursorTop = Top;
            Console.WriteLine(Msg);
        }
        public static string ReadMsg(int Left, int Top)
        {
            Console.CursorLeft = Left;
            Console.CursorTop = Top;
            return Console.ReadLine();
        }
        public static void DisplayClear()
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
        }
    }
}
