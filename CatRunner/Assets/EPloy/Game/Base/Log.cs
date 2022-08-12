using System.Diagnostics;

/// <summary>
/// 日志工具集。
/// </summary>
public static class Log
{
    /// <summary>
    /// 打印调试级别日志，用于记录调试类日志信息。
    /// </summary>
    /// <param name="message">日志内容。</param>
    /// <remarks>仅在带有 DEBUG 预编译选项且带有 ENABLE_LOG、ENABLE_DEBUG_LOG 或 ENABLE_DEBUG_AND_ABOVE_LOG 预编译选项时生效。</remarks>
    [Conditional("ENABLE_LOG")]
    public static void Debug(object message)
    {
        LogMessage(LogLevel.Debug, message);
    }


    /// <summary>
    /// 打印信息级别日志，用于记录程序正常运行日志信息。
    /// </summary>
    /// <param name="message">日志内容</param>
    /// <remarks>仅在带有 ENABLE_LOG、ENABLE_INFO_LOG、ENABLE_DEBUG_AND_ABOVE_LOG 或 ENABLE_INFO_AND_ABOVE_LOG 预编译选项时生效。</remarks>
    [Conditional("ENABLE_LOG")]
    public static void Info(object message)
    {
        LogMessage(LogLevel.Info, message);
    }


    /// <summary>
    /// 打印警告级别日志，建议在发生局部功能逻辑错误，但尚不会导致游戏崩溃或异常时使用。
    /// </summary>
    /// <param name="message">日志内容。</param>
    /// <remarks>仅在带有 ENABLE_LOG、ENABLE_INFO_LOG、ENABLE_DEBUG_AND_ABOVE_LOG、ENABLE_INFO_AND_ABOVE_LOG 或 ENABLE_WARNING_AND_ABOVE_LOG 预编译选项时生效。</remarks>
    [Conditional("ENABLE_LOG")]
    [Conditional("ENABLE_WARNING_LOG")]
    public static void Warning(object message)
    {
        LogMessage(LogLevel.Warning, message);
    }


    /// <summary>
    /// 打印错误级别日志，建议在发生功能逻辑错误，但尚不会导致游戏崩溃或异常时使用。
    /// </summary>
    /// <param name="message">日志内容。</param>
    /// <remarks>仅在带有 ENABLE_LOG、ENABLE_INFO_LOG、ENABLE_DEBUG_AND_ABOVE_LOG、ENABLE_INFO_AND_ABOVE_LOG、ENABLE_WARNING_AND_ABOVE_LOG 或 ENABLE_ERROR_AND_ABOVE_LOG 预编译选项时生效。</remarks>
    [Conditional("ENABLE_LOG")]
    [Conditional("ENABLE_ERROR_LOG")]
    public static void Error(object message)
    {
        LogMessage(LogLevel.Error, message);
    }

    /// <summary>
    /// 打印严重错误级别日志，建议在发生严重错误，可能导致游戏崩溃或异常时使用，此时应尝试重启进程或重建游戏框架。
    /// </summary>
    /// <param name="message">日志内容。</param>
    /// <remarks>仅在带有 ENABLE_LOG、ENABLE_INFO_LOG、ENABLE_DEBUG_AND_ABOVE_LOG、ENABLE_INFO_AND_ABOVE_LOG、ENABLE_WARNING_AND_ABOVE_LOG、ENABLE_ERROR_AND_ABOVE_LOG 或 ENABLE_FATAL_AND_ABOVE_LOG 预编译选项时生效。</remarks>
    [Conditional("ENABLE_LOG")]
    [Conditional("ENABLE_FATAL_LOG")]
    public static void Fatal(object message)
    {
        LogMessage(LogLevel.Fatal, message);
    }

    /// <summary>
    /// 记录日志。
    /// </summary>
    /// <param name="level">日志等级。</param>
    /// <param name="message">日志内容。</param>
    private static void LogMessage(LogLevel level, object message)
    {
        switch (level)
        {
            case LogLevel.Debug:
                UnityEngine.Debug.Log(UtilText.Format("<color=#888888>{0}</color>", message.ToString()));
                break;

            case LogLevel.Info:
                UnityEngine.Debug.Log(message.ToString());
                break;

            case LogLevel.Warning:
                UnityEngine.Debug.LogWarning(message.ToString());
                break;

            case LogLevel.Error:
                UnityEngine.Debug.LogError(message.ToString());
                break;

            default:
                UnityEngine.Debug.LogError(message.ToString());
                break;
        }
    }

    //
    // 摘要:
    //     游戏框架日志等级。
    public enum LogLevel : byte
    {
        //
        // 摘要:
        //     调试。
        Debug = 0,
        //
        // 摘要:
        //     信息。
        Info = 1,
        //
        // 摘要:
        //     警告。
        Warning = 2,
        //
        // 摘要:
        //     错误。
        Error = 3,
        //
        // 摘要:
        //     严重错误。
        Fatal = 4
    }
}

