using System.Diagnostics;
using Serilog.Core;
using Serilog.Events;

namespace LokiLogger
{
    public class ClassMethodEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var stackTrace = new StackTrace(skipFrames: 5); // Adjust based on your call stack
            foreach (StackFrame frame in stackTrace.GetFrames())
            {
                var method = frame.GetMethod();
                if (method?.DeclaringType == null) continue;
                if (method.DeclaringType.Assembly == typeof(ClassMethodEnricher).Assembly) 
                    continue;

                logEvent.AddPropertyIfAbsent(
                    propertyFactory.CreateProperty("ClassName", method.DeclaringType.Name));
                logEvent.AddPropertyIfAbsent(
                    propertyFactory.CreateProperty("MethodName", method.Name));
                break;
            }
        }
    }
}
