

namespace CloudLogger
{
    using Microsoft.Azure.Cosmos.Table;
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Threading;

    class LogRowEntity : TableEntity
    {
        public DateTime ActualTimeStamp { get; set; }

        public int LogLevel { get; set; }

        public string LogContent { get; set; }

        public string ModuleName { get; set; }

        public string HostName { get; set; }

        public int ProcessId { get; set; }

        public int ThreadId { get; set; }

        public LogRowEntity()
        {

        }

        public LogRowEntity(string moduleName, DateTime timeStamp, LogLevel logLevel, string logContent)
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            var processId = Process.GetCurrentProcess().Id;
            var hostname = Dns.GetHostName();
            var preciseTimeStamp = DateTime.UtcNow;
            var date = preciseTimeStamp.ToString("yyyy-MM-dd");
            var time = preciseTimeStamp.ToString("hh:mm:ss-ffff");

            PartitionKey = date;
            RowKey = $"{hostname}_{processId}_{threadId}_{time}";

            ActualTimeStamp = timeStamp;
            LogLevel = (int)logLevel;
            LogContent = logContent;
            ModuleName = moduleName;
            HostName = hostname;
            ProcessId = processId;
            ThreadId = threadId;
        }
    }
}
