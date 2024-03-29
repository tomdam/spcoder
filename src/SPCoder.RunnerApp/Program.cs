﻿using System;
using System.Collections.Generic;
using SPCoder.Core.Utils;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System.IO;
using log4net.Config;
using System.Configuration;

namespace SPCoder.RunnerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                log4net.GlobalContext.Properties["RunnerId"] = args[0];
            }
            XmlConfigurator.Configure();
            SPCoderLogging.Logger.Info("Start");
            RoslynExecutor re = new RoslynExecutor();
            re.Arguments = args;
            re.Execute();
            SPCoderLogging.Logger.Info("Finish");
        }
    }

    public class RoslynExecutor
    {
        public string[] Arguments = null;
        public List<string> FilesRegisteredForExecution = new List<string>();
        public static ScriptState<object> ScriptStateCSharp = null;
        public void Execute()
        {
            string scriptCode = "";
            string pathToExecute = ConfigurationManager.AppSettings["CodePath"];
            if (!string.IsNullOrEmpty(pathToExecute))
            {
                scriptCode = File.ReadAllText(pathToExecute);
            }

            ExecuteScriptCSharp(scriptCode, 0);

            foreach (string script in FilesRegisteredForExecution)
            {
                try
                {
                    string code = File.ReadAllText(script);
                    ExecuteScriptCSharp(code);
                }
                catch (Exception ex)
                {
                    SPCoderLogging.Logger.Error("Error during execution of FilesRegisteredForExecution: " + script + "; " + ex.Message + "; " + ex.StackTrace);
                    Console.Error.WriteLine("Error during execution of FilesRegisteredForExecution: " + script + "; " + ex.Message + "; " + ex.StackTrace);
                    //Environment.ExitCode = 500;
                    //Environment.Exit(500);
                }
            }
        }

        public object ExecuteScriptCSharp(string script, int timesCalled = 0)
        {
            try
            {
                //check to see if it is the one liner script and if it ends with ;
                if (!string.IsNullOrEmpty(script) && script.IndexOf("\n") == -1 && !script.EndsWith(";"))
                    script += ";";
                string workingDirectoryPath = ConfigurationManager.AppSettings["WorkingDirectoryPath"];

                if (!string.IsNullOrEmpty(script) && script.Contains(workingDirectoryPath))
                {
                    script = script.Replace(workingDirectoryPath, Environment.CurrentDirectory);
                }

                ScriptStateCSharp = ScriptStateCSharp == null ?
                        CSharpScript.RunAsync(script, ScriptOptions.Default.AddImports("System"), this).Result :
                        ScriptStateCSharp.ContinueWithAsync(script)
                        .Result;
                //check if this is the executeFile call
                var execNext = ScriptStateCSharp.GetVariable("__SP_CODER_EXECUTE_NEXT__");
                if (execNext != null && execNext.Value != null)
                {
                    string codeToExecute = execNext.Value.ToString();
                    execNext.Value = null;
                    //prevent the further recursion
                    if (timesCalled == 0)
                    {
                        ExecuteScriptCSharp(codeToExecute, 1);
                    }

                }
                return ScriptStateCSharp;
            }
            catch (Exception exc)
            {
                if (exc != null && exc.InnerException != null)
                    throw exc.InnerException;
                else
                    throw exc;
            }
        }

        public static void LogError(string err)
        {
            SPCoderLogging.Logger.Error(err);
        }

        public static void LogError(object err, Exception exc)
        {
            SPCoderLogging.Logger.Error(err, exc);
        }

        public static void LogInfo(string err)
        {
            SPCoderLogging.Logger.Info(err);
        }

        public static void LogWarning(string text)
        {
            SPCoderLogging.Logger.Warn(text);
        }
    }
}
