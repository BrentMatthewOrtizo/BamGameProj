using UnityEngine;

public class UnityLogger : ILog
{
    public void Info(string message)
    {
        Debug.Log(message);
    }

    public void Warn(string message)
    {
        Debug.LogWarning(message);
    }

    public void Error(string message)
    {
        Debug.LogError(message);
    }
}
