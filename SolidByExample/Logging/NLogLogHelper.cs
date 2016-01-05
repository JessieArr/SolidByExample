using System;
using NLog;

namespace SolidByExample.Logging
{
	// Open Closed Principle dictates that this code should be extensible.
	// Being sealed and keeping the Logger private makes this impossible
	// without knowing the internal behavior of our class.
	public sealed class NLogLogHelper : ILogHelper
	{
		private static Logger _logger;
		public NLogLogHelper(string logName)
		{
			_logger = LogManager.GetLogger(logName);
		}

		public void LogInfo(string textToLog)
		{
			if (String.IsNullOrEmpty(textToLog))
			{
				_logger.Error("Info logging was called with no message!");
			}
			_logger.Info(textToLog);
		}

		public void LogError(string textToLog)
		{
			if (String.IsNullOrEmpty(textToLog))
			{
				_logger.Error("Info logging was called with no message!");
			}
			_logger.Error(textToLog);
		}
	}
}
