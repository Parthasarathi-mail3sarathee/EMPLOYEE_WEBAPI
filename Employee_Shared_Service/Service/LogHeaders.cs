using System;
using System.IO;
using Employee_Shared_Service.Contracts;

namespace Employee_Shared_Service.Service
{

    public  class LogHeaders: ILogHeaders
    {
        private  string fileName = AppDomain.CurrentDomain.GetData("ContentRootPath") +"\\logs\\logHeader_Each_Request_" + DateTime.Now.ToString("dd_MM_yy") + ".txt";
      
        public LogHeaders()
        {
            //Check if the file exists
            
        }
        public LogHeaders(string filepath)
        {
            fileName = filepath;
        }
        public void setFileLog(string filepath)
        {
            fileName = filepath;
        }
        public void WriteLog(string msg)
        {
            if (!File.Exists(fileName))
            {
                // Create the file and use streamWriter to write text to it.
                //If the file existence is not check, this will overwrite said file.
                //Use the using block so the file can close and vairable disposed correctly
                using (StreamWriter sw = File.CreateText(fileName))
                {//Write a line of text
                    sw.WriteLine(msg);
                }

            }
            else
            {
                using (StreamWriter sw = File.AppendText(fileName))
                {
                    sw.WriteLine(msg);
                }
            }
        }

        public void WriteLogComplete()
        {
            using (StreamWriter sw = File.AppendText(fileName))
            {
                sw.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
            }
        }
    }
}
