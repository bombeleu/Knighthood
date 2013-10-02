// Steve Yeager
// 8.18.2013
#define LOG
#define WARNING
#define ERROR

using System;
using UnityEngine;
using Object = UnityEngine.Object;
using System.IO;

/// <summary>
/// Wrapper for Unity's Debug class.
/// </summary>
public class Debugger : Singleton<Debugger>
{
    #region Public Fields

    public bool overwrite;
    public bool[] logFlags =
    {
        true,    // None
        true,    // Navigation
        true     // Combat
    };
    public enum LogTypes
    {
        Default = 0,
        Navigation = 1,
        Combat = 2
    }

    #endregion

    #region Const Fields

    private const string LOGPATH = "/Files/Debug/Log_";

    #endregion


    #region MonoBehaviour Overrides

    private void Awake()
    {
        string[] logTypes = Enum.GetNames(typeof (LogTypes));
        for (int i = 0; i < logFlags.Length; i++)
        {
            string path = Application.dataPath + LOGPATH + logTypes[i] + ".txt";
            if (overwrite)
            {
                File.WriteAllText(path, String.Empty);
            }
            else
            {
                //using (StreamWriter writer = new StreamWriter(Application.dataPath + LOGPATH + logTypes[i] + ".txt", true))
                //{
                //    writer.WriteLine(String.Empty);
                //}
                File.AppendAllText(path, "\r\n");
            }

            File.AppendAllText(path, "*** " + DateTime.Now + " ***\r\n");
        }
    }

    #endregion

    #region Public Methods

    [System.Diagnostics.Conditional("LOG")]
    public new static void Log(object message, bool save = true, LogTypes logType = LogTypes.Default)
    {
        if (Instance.logFlags[(int) logType])
        {
            Debug.Log(message);
        }

        if (save)
        {
            using (StreamWriter writer = new StreamWriter(Application.dataPath + LOGPATH + logType + ".txt", true))
            {
                writer.WriteLine(Time.timeSinceLevelLoad + ": " + message);
            }
        }
    }


    [System.Diagnostics.Conditional("LOG")]
    public static void Log(object message, Object context, bool save = true, LogTypes logType = LogTypes.Default)
    {
        if (Instance.logFlags[(int)logType]) Debug.Log(message, context);

        if (save)
        {
            using (StreamWriter writer = new StreamWriter(Application.dataPath + LOGPATH + logType + ".txt", true))
            {
                writer.WriteLine(Time.timeSinceLevelLoad + ": " + message);
            }
        }
    }


    [System.Diagnostics.Conditional("WARNING")]
    public static void LogWarning(object message)
    {
        Debug.LogWarning(message);
    }


    [System.Diagnostics.Conditional("WARNING")]
    public static void LogWarning(object message, Object context)
    {
        Debug.LogWarning(message, context);
    }


    [System.Diagnostics.Conditional("WARNING")]
    public static void LogError(object message)
    {
        Debug.LogError(message);
    }


    [System.Diagnostics.Conditional("ERROR")]
    public static void LogError(object message, Object context)
    {
        Debug.LogError(message, context);
    }


    [System.Diagnostics.Conditional("ERROR")]
    public static void LogException(System.Exception exception)
    {
        Debug.LogException(exception);
    }


    [System.Diagnostics.Conditional("ERROR")]
    public static void LogException(System.Exception exception, Object context)
    {
        Debug.LogException(exception, context);
    }

    #endregion
}