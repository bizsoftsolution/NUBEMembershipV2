using System;
using System.IO;

namespace Nube
{
    class ExceptionLogging
    {
        private static String ErrorlineNo, Errormsg, extype, exurl, ErrorLocation;

        public static void SendErrorToText(Exception ex)
        {
            var line = Environment.NewLine + Environment.NewLine;

            ErrorlineNo = ex.StackTrace.Substring(ex.StackTrace.Length - 7, 7);
            Errormsg = ex.GetType().Name.ToString();
            extype = ex.GetType().ToString();
            exurl = ex.StackTrace.ToString();
            ErrorLocation = ex.Message.ToString();

            try
            {
                string filepath = Path.Combine(Environment.CurrentDirectory, @"Exception Files\" + DateTime.Now.Date.ToString("dd-MMM-yyyy") + "\\");
                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }
                filepath = filepath + DateTime.Now.ToString("dd-MM-yy-hh-tt-mm-ss") + ".txt";   //Text File Name
                if (!File.Exists(filepath))
                {
                    File.Create(filepath).Dispose();
                }
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    string error = "Log Written Date:" + " " + DateTime.Now.ToString() + line +
                                   "Error Line No :" + " " + ErrorlineNo + line +
                                   "Error Message:" + " " + Errormsg + line +
                                   "Exception Type:" + " " + extype + line +
                                   "Error Location :" + " " + ErrorLocation + line +
                                   "Error Form :" + " " + exurl + line;
                    sw.WriteLine("-----------Exception Details on " + " " + DateTime.Now.ToString() + "-----------------");
                    sw.WriteLine("-------------------------------------------------------------------------------------");
                    sw.WriteLine(line);
                    sw.WriteLine(error);
                    sw.WriteLine("--------------------------------*End*------------------------------------------");
                    sw.WriteLine(line);
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception e)
            {
                e.ToString();
            }
        }
    }
}
