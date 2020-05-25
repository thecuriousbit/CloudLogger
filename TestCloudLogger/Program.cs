

namespace TestCloudLogger
{
    using CloudLogger;
    using Microsoft.Azure.Cosmos.Table;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    class Program
    {
        static void Main(string[] args)
        {
            string connStr = "Enter Storage Connection String here";
            string logTableName = "mylogtable";

            CloudStorageAccount sa = CloudStorageAccount.Parse(connStr);
            CloudTableClient tClient = sa.CreateCloudTableClient();
            CloudTable cTable = tClient.GetTableReference(logTableName);

            // Initializing Logger before using it
            // As this is to test, setting ptional param to true
            Logger.Initialize(cTable, true);

            List<Task> allTasks = new List<Task>();
            for(int i=0; i<5; i++)
            {
                allTasks.Add(Task.Run(() =>
                {
                    Test();
                }));
            }

            Task.WaitAll(allTasks.ToArray());

            Console.WriteLine("Done");
        }


        static void Test()
        {
            var aLogTask = Logger.Instance.LogAsync(LogLevel.Information, "Test", "Using LogAsync method");
            Logger.Instance.Log(LogLevel.Warning, "Test", "Using Log method");
            aLogTask.Wait();
        }
    }
}
