using BL.Interfaces;

namespace BL.Services
{
    public class LogService : ILogService
    {
        public LogService() { }
        public void Log(string message){}
        public async Task InsertLog(Exception exception, string? func)
        {
        }
        public async Task InsertLog(string action, string data)
        {

        }


        private void WriteToLogFile(string msg, string func) 
        {
            string currentDateDirectory = "Logs";
            string path = currentDateDirectory;
            string fileName = DateTime.Now.ToString("ddMMyy") + "errorLog.txt";

            string pathWithFileName = path + "\\" + fileName;

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            using (StreamWriter sw = File.AppendText(pathWithFileName))
            {
                sw.WriteLine(msg + ";" + " in func: " + func + Environment.NewLine);
            }
        }
    }

}
