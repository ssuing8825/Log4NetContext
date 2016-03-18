using System;
using System.IO;

using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace Log4NetContext
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = LogManager.GetLogger(typeof(Program));
            LogicalThreadContext.Stacks["activity"].Push("First Activity");
            LogicalThreadContext.Stacks["activity"].Push("Second Activity");
            logger.Info("File Appender with JSON formatter");

            //add a memory appender so we don't seem crazy.
            var hierarchy = (Hierarchy)LogManager.GetRepository();
            hierarchy.Root.RemoveAllAppenders();
            var layout = new JsonLayout();
            layout.ActivateOptions();
            var memoryAppender = new MemoryAppender { Layout = layout };
            memoryAppender.ActivateOptions();
            hierarchy.Root.AddAppender(memoryAppender);
            hierarchy.Configured = true;
            logger.Info("Memory Appender with JSON Formatter");

            //have to get it out of the memoryappendor to trigger the code. 
            memoryAppender.Layout.Format(Console.Out, memoryAppender.GetEvents()[0]);
        }
    }

    public class JsonLayout : ILayout, IOptionHandler
    {
        public void Format(TextWriter writer, LoggingEvent loggingEvent)
        {
            if (loggingEvent.Properties["activity"] == null)
                Console.WriteLine(loggingEvent.RenderedMessage + " The activity was NULL. ");
            else
                writer.WriteLine(loggingEvent.RenderedMessage + " The activity was set. " + loggingEvent.Properties["activity"]);
        }

        public string ContentType
        {
            get { return "application/json"; }
        }

        public string Footer { get; set; }
        public string Header { get; set; }
        public bool IgnoresException { get { return true; } }
        public void ActivateOptions() { }
    }
}
