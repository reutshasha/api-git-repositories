namespace DAL.Interfaces
{
    internal interface ILogManager
    {
        Task InsertLog(Exception exception, string func);
        Task InsertLog(string action, string data);
    }
}