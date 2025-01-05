namespace BL.Interfaces
{
    public interface ILogService
    {
        void Log(string message);
        Task InsertLog(Exception exception, string func);
        Task InsertLog(string action, string data);
    }
}
