

namespace CloudLogger
{
    using Microsoft.Azure.Cosmos.Table;
    using System;
    using System.Threading.Tasks;

    public class Logger
    {
        private bool _throwOnFailure;
        private CloudTable _table;
        private static object _lock = new object();

        /// <summary>
        /// Initialize need to be called before calling any other function if this class
        /// </summary>
        /// <param name="table">Azure CloudTable object</param>
        /// <param name="throwOnFailure">If set to true, it won't eat up any exception</param>
        public static void Initialize(CloudTable table, bool throwOnFailure = false)
        {
            if(Instance == null) // this is to avoid unnecessary locking of _lock
            {
                lock(_lock)
                {
                    if (Instance == null)
                    {
                        Instance = new Logger(table, throwOnFailure);
                    }
                }
            }
        }

        private Logger(CloudTable table,  bool throwOnFailure)
        {
            _throwOnFailure = throwOnFailure;
            _table = table;

            try
            {
                _table.CreateIfNotExists();
            }
            catch(Exception)
            {
                if(_throwOnFailure)
                {
                    throw;
                }
            }            
        }

        /// <summary>
        /// Returns the Instance of the Logger. Should be called only after calling Initialize() 
        /// </summary>
        public static Logger Instance { get; private set; }

        /// <summary>
        /// To log asynchronously
        /// </summary>
        /// <param name="level">Log level</param>
        /// <param name="moduleName">Module name, this will facilitate log filtering when needed</param>
        /// <param name="content">Log content</param>
        /// <returns>Task object, can be used to await/wait</returns>
        public async Task LogAsync(LogLevel level, string moduleName, string content)
        {
            await _LogAsync(level, moduleName, content);
        }


        /// <summary>
        /// To log synchronously
        /// </summary>
        /// <param name="level">Log level</param>
        /// <param name="moduleName">Module name, this will facilitate log filtering when needed</param>
        /// <param name="content">Log content</param>
        public void Log(LogLevel level, string moduleName, string content)
        {
            _LogAsync(level, moduleName, content).Wait();
        }

        private async Task _LogAsync(LogLevel level, string moduleName, string content)
        {
            try
            {
                DateTime timeStamp = DateTime.UtcNow;

                LogRowEntity logEntity = new LogRowEntity(moduleName, timeStamp, level, content);

                TableOperation insertOperation = TableOperation.Insert(logEntity);

                await _table.ExecuteAsync(insertOperation);
            }
            catch(Exception)
            {
                if(_throwOnFailure)
                {
                    throw;
                }
            }
        }
    }

}
