//This file contains some of the common methods, classes and variables that can be used from other scripts

/// <summary>
/// Writes the text to output window.
/// </summary>
/// <param name="o">Object that will be written to the output window</param>
public void print(object o)
{
    if (o != null)
        Console.Write(o.ToString());
}

/// <summary>
/// Writes the text to output window and adds the new line character after the text.
/// </summary>
/// <param name="o">Object that will be written to the output window</param>
public void println(object o)
{
    if (o != null)
        Console.WriteLine(o.ToString());
}

/// <summary>
/// Field that contains the code that will be execute with the next execution.
/// </summary>
public string __SP_CODER_EXECUTE_NEXT__ = null;
/// <summary>
/// Utility method that can be used to execute code from the C# file
/// </summary>
/// <param name="path">Full path to the file with C# source code</param>
public void execFile(string path)
{
    if (!string.IsNullOrEmpty(path))
    {
        string code = System.IO.File.ReadAllText(path);
        if (!string.IsNullOrEmpty(code))
        {
            __SP_CODER_EXECUTE_NEXT__ =  code;
        }
    }
}
/// <summary>
/// The class responsible for writing to the Log window
/// </summary>
public class SPCoderLog
{ 
    private SPCoderForm main { get; set; }

    public SPCoderLog(SPCoderForm main)
    {
        this.main = main;
    }
    public void Log(object text, bool logTimestamp = true)
    {
        if (text != null)
            main.SpLog.LogInfo(text.ToString(), logTimestamp);
    }

    public void LogInfo(object text, bool logTimestamp = true)
    {
        if (text != null)
            main.SpLog.LogInfo(text.ToString(), logTimestamp);
    }

    public void LogError(object text, bool logTimestamp = true)
    {
        if (text != null)
            main.SpLog.LogError(text.ToString(), logTimestamp);
    }

    public void LogWarning(object text, bool logTimestamp = true)
    {
        if (text != null)
            main.SpLog.LogWarning(text.ToString(), logTimestamp);
    }

    public void ClearLog()
    {
        main.SpLog.ClearLog();
    }
}
/// <summary>
/// Object that can be used for writting to log window.
/// </summary>
var logger = new SPCoderLog(main);

