using UnityEngine;

public class DebugCtrl
{
    public static bool logEnable = false;

    public static bool LogEnable()
    {
        return Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor || logEnable;
    }
    public static void Log(object msg)
    {
        if (LogEnable()) Debug.Log(msg.ToString());
    }
    public static void Log(object msg, Color color)
    {
        if (LogEnable())
        {
            var msgStr = AddColor(msg, color);
            Debug.Log(msgStr);
        }
    }
    public static void LogError(object msg)
    {
        if (LogEnable()) Debug.LogError(msg.ToString());
    }
    public static void LogErrorFormat(string msg1, params object[] msg2)
    {
        if (LogEnable()) Debug.LogErrorFormat(msg1, msg2);
    }

    static string AddColor(object msg, Color color)
    {
        string colHtmlString = ColorUtility.ToHtmlStringRGB(color);
        string msgStr = msg.ToString();
        const string colorTagStart = "<color=#{0}>";
        const string colorTagEnd = "</color>";
        msgStr = string.Format(colorTagStart, colHtmlString) + msgStr + colorTagEnd;
        return msgStr;
    }
}
