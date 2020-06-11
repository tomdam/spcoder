//Here you can add references to other assemblies if necessary
//#r "System.Data"

Console.WriteLine("init");

public string __SP_CODER_EXECUTE_NEXT__ = null;
public void execFile(string path)
{
    if (!string.IsNullOrEmpty(path))
    {
        string code = System.IO.File.ReadAllText(path);
        if (!string.IsNullOrEmpty(code))
        {
            __SP_CODER_EXECUTE_NEXT__ = code;
        }
    }
}


//here you can prepare all the csx files that should be executed
string folder = @"Code\";
FilesRegisteredForExecution.Add(folder + "code.csx");

//FilesRegisteredForExecution.Add(folder + "code1.csx");
//FilesRegisteredForExecution.Add(folder + "code2.csx");
//...