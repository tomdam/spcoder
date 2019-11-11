public void print(object o)
{
    if (o != null)
        Console.Write(o.ToString());
}

public void println(object o)
{
    if (o != null)
        Console.WriteLine(o.ToString());
}


public string __SP_CODER_EXECUTE_NEXT__ = null;
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
    public void Log(object text)
    {
        if (text != null)
            main.SpLog.LogInfo(text.ToString());
    }

    public void LogInfo(object text)
    {
        if (text != null)
            main.SpLog.LogInfo(text.ToString());
    }

    public void LogError(object text)
    {
        if (text != null)
            main.SpLog.LogError(text.ToString());
    }

    public void LogWarning(object text)
    {
        if (text != null)
            main.SpLog.LogWarning(text.ToString());
    }
}
var logger = new SPCoderLog(main);

