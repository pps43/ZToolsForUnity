using UnityEngine;
using System.Text;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
using System;
using System.Collections.Generic;
#endif
namespace ZTools.Debug
{
    /// <summary>
    /// Use LogFilter to do filename-based filtering.
    /// Use Zlog.logLevel to do importance-based filtering.
    /// Modity _line, _filePath according to your project before use.
    /// </summary>
    public class ZLog
    {
        public static bool isLogOn = true; // swith on/off
        public static LogLevel logLevel = LogLevel.info;
        private static int _line = 176;// Must be the line number of "UnityEngine.Debug.Log" in this file.
        private static string _filePath = "Assets/ZTools/Debug/Log/LogUtil.cs";//Modify to your actual path
        public enum LogLevel
        {
            verbose,
            info,
            warning,
            error,
        }

        private const string INFO_COLOR = "green";
        private const string WARNING_COLOR = "magenta";
        private const string ERROR_COLOR = "red";
        private static StringBuilder _sb = new StringBuilder();

        /// <summary>
        /// Helps to concate strings
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static string str(params string[] msg)
        {
            _sb.Length = 0;
            for (int i = 0; i < msg.Length; i++)
            {
                _sb.Append(msg[i]).Append(" ");
            }
            return _sb.ToString();
        }

        public static void log(string msg)
        {
            if (isLogOn)
            {
                logFormat(LogLevel.info, msg);
            }
        }
        public static void log(params string[] msg) //todo params 会创建数组有开销。多加几个重载吧
        {
            if (isLogOn)
            {
                _sb.Length = 0;
                for (int i = 0; i < msg.Length; i++)
                {
                    _sb.Append(msg[i]).Append(" ");
                }
                logFormat(LogLevel.info, _sb.ToString());
            }
        }

        public static void warn(string msg)
        {
            if (isLogOn)
            {
                logFormat(LogLevel.warning, msg);
            }
        }
        public static void warn(params string[] msg) //TODO "params" has overhead
        {
            if (isLogOn)
            {
                _sb.Length = 0;
                for (int i = 0; i < msg.Length; i++)
                {
                    _sb.Append(msg[i]).Append(" ");
                }
                logFormat(LogLevel.warning, _sb.ToString());
            }
        }

        public static void error(string msg)
        {
            if (isLogOn)
            {
                logFormat(LogLevel.error, msg);
            }
        }
        public static void error(params string[] msg) //TODO "params" has overhead
        {
            if (isLogOn)
            {
                _sb.Length = 0;
                for (int i = 0; i < msg.Length; i++)
                {
                    _sb.Append(msg[i]).Append(" ");
                }
                logFormat(LogLevel.error, _sb.ToString());
            }
        }

        public static void verbose(string msg)
        {
            if (isLogOn)
            {
                logFormat(LogLevel.verbose, msg);
            }
        }
        public static void verbose(params string[] msg) //TODO "params" has overhead
        {
            if (isLogOn)
            {
                _sb.Length = 0;
                for (int i = 0; i < msg.Length; i++)
                {
                    _sb.Append(msg[i]).Append(" ");
                }
                logFormat(LogLevel.verbose, _sb.ToString());
            }
        }

        public static void assert(bool condition, string msg = "")
        {
            if (string.IsNullOrEmpty(msg))
            {
                UnityEngine.Debug.Assert(condition);
            }
            else
            {
                UnityEngine.Debug.Assert(condition, msg);
            }
        }


        private static void logFormat(LogLevel level, string msg)
        {
            if ((int)level < (int)logLevel)
            {
                return;
            }

#if UNITY_EDITOR
            System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace(true);
            var stackFrame = stackTrace.GetFrame(2);
            string fullFileName = stackFrame.GetFileName();
            string fileName = Path.GetFileName(fullFileName);

            if (LogFilter.blackList.Contains(fileName))
            {
                return;
            }
            _logStackFrames.Add(stackFrame);
#endif
            _sb.Length = 0;
            string colorStr = INFO_COLOR;
            if (level == LogLevel.warning)
            {
                colorStr = WARNING_COLOR;
            }
            else if (level == LogLevel.error)
            {
                colorStr = ERROR_COLOR;
            }
            string timeStr = Time.realtimeSinceStartup.ToString("0.000");

            _sb.AppendFormat("Z|{1}| <color={0}>{2}</color>", colorStr, timeStr, msg/*, stackMsgStr*/);
            UnityEngine.Debug.Log(_sb.ToString());
        }

#if UNITY_EDITOR
        private static int _instanceID;
        private static List<System.Diagnostics.StackFrame> _logStackFrames = new List<System.Diagnostics.StackFrame>();
        //ConsoleWindow
        private static object _consoleWindowInstance;
        private static object _logListView;
        private static FieldInfo _logListViewTotalRowsFieldInfo;
        private static FieldInfo _logListViewCurrentRowFieldInfo;
        //LogEntry
        private static MethodInfo _logEntriesGetEntryMethod;
        private static object _logEntryInstance;

        private static FieldInfo _logEntryConditionFieldInfo;
        private static FieldInfo _consoleWindowFieldInfo;
        private static FieldInfo _listViewFieldInfo;
        static ZLog()
        {
            _instanceID = AssetDatabase.LoadAssetAtPath<MonoScript>(_filePath).GetInstanceID();
            _logStackFrames.Clear();

            getConsoleWindowListView();
        }

        private static void getConsoleWindowListView()
        {
            if (_logListView == null)
            {
                Assembly unityEditorAssembly = Assembly.GetAssembly(typeof(EditorWindow));
                Type consoleWindowType = unityEditorAssembly.GetType("UnityEditor.ConsoleWindow");

                _consoleWindowFieldInfo = consoleWindowType.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);
                _consoleWindowInstance = _consoleWindowFieldInfo.GetValue(null);
                if (_consoleWindowInstance == null)
                {
                    return;
                }
                _listViewFieldInfo = consoleWindowType.GetField("m_ListView", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Default);
                _logListView = _listViewFieldInfo.GetValue(_consoleWindowInstance);
                _logListViewTotalRowsFieldInfo = _listViewFieldInfo.FieldType.GetField("totalRows", BindingFlags.Instance | BindingFlags.Public);
                _logListViewCurrentRowFieldInfo = _listViewFieldInfo.FieldType.GetField("row", BindingFlags.Instance | BindingFlags.Public);

                Type logEntriesType = unityEditorAssembly.GetType("UnityEditor.LogEntries");
                _logEntriesGetEntryMethod = logEntriesType.GetMethod("GetEntryInternal", BindingFlags.Static | BindingFlags.Public);
                Type logEntryType = unityEditorAssembly.GetType("UnityEditor.LogEntry");
                _logEntryInstance = Activator.CreateInstance(logEntryType);
                _logEntryConditionFieldInfo = logEntryType.GetField("condition", BindingFlags.Instance | BindingFlags.Public);
            }
        }
        private static System.Diagnostics.StackFrame getListViewRowCount()
        {
            getConsoleWindowListView();
            if (_logListView == null)
                return null;
            else
            {
                int totalRows = (int)_logListViewTotalRowsFieldInfo.GetValue(_logListView);
                int row = (int)_logListViewCurrentRowFieldInfo.GetValue(_logListView);
                int logByThisClassCount = 0;
                for (int i = totalRows - 1; i >= row; i--)
                {
                    _logEntriesGetEntryMethod.Invoke(null, new object[] { i, _logEntryInstance });
                    string condition = _logEntryConditionFieldInfo.GetValue(_logEntryInstance) as string;
                    if (condition.Contains("Z|"))// special mark to identify if it's called by this script
                        logByThisClassCount++;
                }

                while (_logStackFrames.Count > totalRows)
                    _logStackFrames.RemoveAt(0);
                if (_logStackFrames.Count >= logByThisClassCount)
                    return _logStackFrames[_logStackFrames.Count - logByThisClassCount];
                return null;
            }
        }

        [UnityEditor.Callbacks.OnOpenAssetAttribute(0)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            if (instanceID == _instanceID && _line == line)
            {
                var stackFrame = getListViewRowCount();
                if (stackFrame != null)
                {
                    string fileName = stackFrame.GetFileName();
                    string fileAssetPath = fileName.Substring(fileName.IndexOf("Assets"));
                    AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<MonoScript>(fileAssetPath), stackFrame.GetFileLineNumber());
                    return true;
                }
            }

            return false;
        }
#endif
    }

    public class LogFilter
    {
        // log called by filenames listed here will be blocked.
        public static HashSet<string> blackList = new HashSet<string>()
        {
            //"xxx.cs",
            //"yyy.cs",
            
        };
    }
}