using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Core.Logging {
    public enum LogLevel {
        INFO,
        WARNING,
        ERROR
    }
    
    public static class NCLogger {

        private static Dictionary<LogLevel, string> _colors = new () {
            {LogLevel.INFO, "white"},
            {LogLevel.WARNING, "orange"},
            {LogLevel.ERROR, "red"}
        };

        public static void Log(string message, LogLevel level = LogLevel.INFO) {
            string content = $"<color={_colors[level]}>[{GetClassName()}]</color> {message}";
            UnityEngine.Debug.Log(content);
        }

        private static string GetClassName() {
            string name;
            Type type;
            int frames = 2;
            do {
                MethodBase method = new StackFrame(frames, false).GetMethod();
                type = method.DeclaringType;
                if (type == null) return method.Name;
                frames++;
                name = type.FullName;
            } while (type.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase));
            return name;
        }
    }
}