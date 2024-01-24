namespace Employee_Shared_Service.Contracts
{
    public interface ILogHeaders
    {
        void WriteLog(string msg);
        void WriteLogComplete();
        void setFileLog(string filepath);

    }
}
