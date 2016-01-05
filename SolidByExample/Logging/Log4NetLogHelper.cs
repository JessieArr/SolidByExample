using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace SolidByExample.Logging
{
	// Open Closed Principle dictates that this code should be extensible.
	// Being sealed and keeping the Logger private makes this impossible
	// without knowing the internal behavior of our class.
	public sealed class Log4NetLogHelper : ILogHelper
	{
		private ILog _logger;

		public void SetUpLogger(string logName)
		{
			Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
			if (hierarchy.Configured)
			{
				// Logging has been configured already, we leave it alone.
				return;
			}
			TraceAppender tracer = new TraceAppender();
			PatternLayout patternLayout = new PatternLayout();

			patternLayout.ConversionPattern = "%d [%t] %-5p %m%n";
			patternLayout.ActivateOptions();

			tracer.Layout = patternLayout;
			tracer.ActivateOptions();
			hierarchy.Root.AddAppender(tracer);

			RollingFileAppender roller = new RollingFileAppender();
			roller.Layout = patternLayout;
			roller.AppendToFile = true;
			roller.RollingStyle = RollingFileAppender.RollingMode.Size;
			roller.MaxSizeRollBackups = 4;
			roller.MaximumFileSize = "100KB";
			roller.StaticLogFileName = true;
			roller.File = "log4netOutput.log";
			roller.ActivateOptions();
			hierarchy.Root.AddAppender(roller);

			hierarchy.Root.Level = Level.All;
			hierarchy.Configured = true;

			_logger = LogManager.GetLogger(logName);
		}

		public void LogInfo(string textToLog)
		{
			if (_logger == null)
			{
				// Liskov violation: This implementation of ILogHelper throws exceptions
				// that other implementations do not. This is not a valid drop-in replacement
				// for the NLogLogHelper class.
				throw new Exception("Logger must be set up first! Be sure to call SetUpLogger!");
			}
			if (String.IsNullOrEmpty(textToLog))
			{
				throw new Exception("textToLog cannot be null or empty!");
			}
			_logger.Info(textToLog);
		}

		public void LogError(string textToLog)
		{
			if (_logger == null)
			{
				// Liskov violation: This implementation of ILogHelper throws exceptions
				// that other implementations do not. This is not a valid drop-in replacement
				// for the NLogLogHelper class.
				throw new Exception("Logger must be set up first! Be sure to call SetUpLogger!");
			}
			if (String.IsNullOrEmpty(textToLog))
			{
				throw new Exception("textToLog cannot be null or empty!");
			}
			_logger.Info(textToLog);
		}
	}
}
