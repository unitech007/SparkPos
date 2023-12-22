using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SparkPOS.Logger
{
    public interface ILogger
    {
        void LogError(Exception ex);

        void LogMessage(string Message);
    }
}
