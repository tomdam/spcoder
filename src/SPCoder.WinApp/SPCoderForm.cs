using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

using SPCoder.Autorun;
using SPCoder.Context;
using SPCoder.Describer;
using SPCoder.HelperWindows;
using AutoRunScriptsForm = SPCoder.Autorun.AutoRunScriptsForm;
using SPCoder.Utils.Nodes;
using SPCoder.Windows;
using System.Reflection;
using WeifenLuo.WinFormsUI.Docking;
using FastColoredTextBoxNS;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using SPCoder.Core.Utils;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using SPCoder.Utils;
using System.Runtime.Remoting.Messaging;

namespace SPCoder
{
    /// <summary>
    /// SPCoder main form.
    /// </summary>
    /// <author>Damjan Tomic</author>
    public partial class SPCoderForm : Form, ISPCoderForm
    {
        #region Fields

        public SPCoder.Context.Context MyContext = null;// ContextFactory.GetCurrentContext();

        public BaseNode DragedBaseNode;
        ObjectDescriber describer = new ObjectDescriber();

        private DeserializeDockContent m_deserializeDockContent;

        private Windows.Output m_output;
        private Windows.Properties m_properties;
        private Windows.Log m_log;

        public Windows.Context m_context;

        private Windows.ExplorerView m_explorerView;
        private Windows.GridViewer m_gridviewer;
        private AutoRunScriptsForm m_autoRun;
        private DescriberForm m_describer;

        public static SPCoderForm MainForm;
        FrmSplashScreen splashScreen;
        public string DefaultTitle = "SPCoder";

        public bool PutExtensionMethodsToAutocomplete = false;
        #endregion

        #region Properties
        public Output OutputWindow { get { return m_output; } }

        #endregion

        public CompositionContainer container;
        public SPCoderForm(FrmSplashScreen splashScreen)
        {
            MainForm = this;
            InitializeComponent();
            this.splashScreen = splashScreen;
            toolStripStatusLabel.Text = "Starting";
            //Load modules (MEF)
            var path = AppDomain.CurrentDomain.BaseDirectory;
            var d = new DirectoryCatalog(path, "SPCoder.*.dll");
            container = new CompositionContainer(d);

            try
            {
                //this will import the Modules/Connectors from all SPCoder dlls
                container.ComposeParts(this);
            }
            catch (Exception exc)
            {
                //Log
            }

            SPCoderSettings.ReadSettings();

            //Create all windows
            m_output = new Windows.Output();
            m_properties = new Windows.Properties();
            m_log = new Windows.Log();
            m_context = new Windows.Context();
            m_explorerView = new Windows.ExplorerView();
            m_gridviewer = new Windows.GridViewer();
            m_autoRun = new AutoRunScriptsForm();
            m_describer = new DescriberForm();

            this.SpLog = m_log;
            this.SpOutput = m_output;
            this.SpGrid = m_gridviewer;

            Application.DoEvents();

            m_deserializeDockContent = new DeserializeDockContent(GetContentFromPersistString);
            //dockPanel.Skin = new DockPanelSkin();
            var theme = new VS2015BlueTheme();
            //var theme = new VS2015LightTheme();
            //var theme = new VS2015DarkTheme();

            

            this.ReloadDockingSettings(theme);

            AddHistoryToRecentMenu();

            MyContext = ContextFactory.GetCurrentContext();
            toolStripStatusLabel.Text = "Ready";
            Application.DoEvents();
        }


        public void SetAppStatus(string statusText)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate ()
                {
                    toolStripStatusLabel.Text = statusText;
                });
            }
            else
            {
                toolStripStatusLabel.Text = statusText;
            }
            
        }


        public void ReloadDockingSettings(ThemeBase theme)
        {
            dockPanel.SuspendLayout(true);

            // In order to load layout from XML, we need to close all the DockContents
            CloseAllContents();

            this.dockPanel.Theme = theme;

            Assembly assembly = Assembly.GetAssembly(typeof(SPCoderForm));

            String xmlPath = System.IO.Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Config2016\CSharp\DockPanel.xml");
            dockPanel.LoadFromXml(xmlPath, m_deserializeDockContent);
            
            //load files that were open during the previous run
            LoadFilesThatWerePreviouslyOpen();

            dockPanel.ResumeLayout(true, true);

            //add new code window
            toolStripButton6_Click(null, null);
        }

        public void LoadFilesThatWerePreviouslyOpen()
        {
            var codeSettings = (Dictionary<string, object>)SPCoderSettings.Settings[SPCoderConstants.SP_SETTINGS_CODE];
            var windows = (System.Collections.ArrayList)codeSettings[SPCoderConstants.SP_SETTINGS_WINDOWS];
            foreach (Dictionary<string, object> w in windows)
            {
                string path = w[SPCoderConstants.SP_SETTINGS_PATH].ToString();
                if (!System.IO.Path.IsPathRooted(path))
                {
                    path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), path);
                }
                if (!System.IO.File.Exists(path))
                {
                    SPCoderForm.MainForm.LogError("File " + path + " does not exist");
                    continue;
                }
                
                string source = System.IO.File.ReadAllText(path);
                string title = System.IO.Path.GetFileName(path);
                string fullFileName = path;

                GenerateNewSourceTab(title, source, fullFileName);
            }
        }

        protected void AddHistoryToRecentMenu()
        {
            try
            {
                var codeSettings = (Dictionary<string, object>)SPCoderSettings.Settings[SPCoderConstants.SP_SETTINGS_CODE];
                var history = (System.Collections.ArrayList)codeSettings[SPCoderConstants.SP_SETTINGS_HISTORY];
                foreach (Dictionary<string, object> h in history)
                {
                    string path = h[SPCoderConstants.SP_SETTINGS_PATH].ToString();
                    if (!System.IO.Path.IsPathRooted(path))
                    {
                        path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), path);
                    }
                    AddRecentMenuItem(path);
                }
            }
            catch (Exception exc)
            {
                SPCoderLogging.Logger.Error(exc);
            }
        }

        public void AddRecentMenuItem(string path)
        {
            //check if it is already in history
            foreach (ToolStripMenuItem it in recentToolStripMenuItem.DropDownItems)
            {
                if (it.Tag.ToString() == path)
                    return;
            }

            //remove last one if there are already 15 entries
            if (recentToolStripMenuItem.DropDownItems.Count == 15)
            {
                var it = recentToolStripMenuItem.DropDownItems[0];
                //remove it from the history also
                SPCoderSettings.RemovePathFromHistory(it.Tag.ToString());
                recentToolStripMenuItem.DropDownItems.RemoveAt(0);
            }

            var item = recentToolStripMenuItem.DropDownItems.Add(path);
            SPCoderSettings.AddPathToHistory(path);

            item.Tag = path;
            item.Click += RecentMenuItemClick;
        }

        private void RecentMenuItemClick(object sender, EventArgs e)
        {
            var item = (ToolStripItem)sender;
            string path = item.Tag.ToString();
            if (!System.IO.File.Exists(path))
            {
                LogError("File " + path + " does not exist");
                return;
            }

            string source = System.IO.File.ReadAllText(path);
            string title = System.IO.Path.GetFileName(path);
            string fullFileName = path;

            GenerateNewSourceTab(title, source, fullFileName);
        }

        private void LoadOtherSettings()
        {
            //
            if (SPCoderSettings.Settings[SPCoderConstants.SP_SETTINGS_CODE] != null)
            {
                var codeSettings = (Dictionary<string, object>)SPCoderSettings.Settings[SPCoderConstants.SP_SETTINGS_CODE];
                if (codeSettings[SPCoderConstants.SP_SETTINGS_EXPRESSION] != null)
                {
                    string value = codeSettings[SPCoderConstants.SP_SETTINGS_AUTOCOMPLETE_SHOW_EXTENSION_METHODS].ToString();
                    bool isChecked = bool.Parse(value);
                    toolStripMenuItemAutocompleteExtensionMethods.Checked = isChecked;
                    PutExtensionMethodsToAutocomplete = isChecked;
                }
            }
        }

        public void SetTitle(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                this.Text = this.DefaultTitle;
            }
            else
            {
                this.Text = this.DefaultTitle + " - " + path;
            }
        }

        public WeifenLuo.WinFormsUI.Docking.DockPanel DockPanel 
        {
            get { return this.dockPanel; }
        }

        /// <summary>
        /// Used by MEF to dynamicaly load all ModuleDescription classes from dll files stored in the same directory where
        /// the exe application is located.
        /// </summary>
        [ImportMany]
        public List<ModuleDescription> Modules { get; set; }

        private IDockContent GetContentFromPersistString(string persistString)
        {            
            if (persistString == typeof(Windows.Properties).ToString())
                return m_properties;
            else if (persistString == typeof(Windows.Context).ToString())
                return m_context;
            else if (persistString == typeof(Output).ToString())
                return m_output;
            else if (persistString == typeof(Log).ToString())
                return m_log;
            else if (persistString == typeof(AutoRunScriptsForm).ToString())
                return m_autoRun;      
            else if (persistString == typeof(DescriberForm).ToString())
                return m_describer;
            else if (persistString == typeof(GridViewer).ToString())
                return m_gridviewer;
            else if (persistString == typeof(ExplorerView).ToString())
                return m_explorerView;
            else
            {
                
                string[] parsedStrings = persistString.Split(new char[] { ',' });
                if (parsedStrings.Length != 3)
                    return null;

                if (parsedStrings[0] != typeof(CSharpCode).ToString())
                    return null;

                CSharpCode myDoc = new CSharpCode();
                codeWindows.Add(myDoc);

                if (parsedStrings[1] != string.Empty)
                    myDoc.FileName = parsedStrings[1];
                if (parsedStrings[2] != string.Empty)
                    myDoc.Text = parsedStrings[2];

                return myDoc;
            }
        }

        private void CloseAllContents()
        {
            // Close all other document windows
            CloseAllDocuments();
        }

        private void CloseAllDocuments()
        {
            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
            {
                foreach (Form form in MdiChildren)
                    CloseCodeWindow(form);
            }
            else
            {
                for (int index = dockPanel.Contents.Count - 1; index >= 0; index--)
                {
                    if (dockPanel.Contents[index] is IDockContent)
                    {
                        IDockContent content = (IDockContent)dockPanel.Contents[index];
                        content.DockHandler.Close();
                    }
                }
            }
        }

        public void CloseAllCodeWindows(Form keepOpened = null)
        {
            foreach (Form form in MdiChildren)
            {
                if (form is CSharpCode && form != keepOpened)
                {
                    CloseCodeWindow(form);
                }
            }
        }

        protected void CloseCodeWindow(Form form)
        {
            //AddPathToHistory
            if (form is CSharpCode)
            {
                CSharpCode f = (CSharpCode)form;
                
                if (f.FullFileName != null)
                {
                    SPCoderSettings.AddPathToHistory(f.FullFileName.ToString());
                }
            }
            form.Close();
        }

        private IDockContent FindDocument(string text)
        {
            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
            {
                foreach (Form form in MdiChildren)
                    if (form.Text == text)
                        return form as IDockContent;

                return null;
            }
            else
            {
                foreach (IDockContent content in dockPanel.Documents)
                    if (content.DockHandler.TabText == text)
                        return content;

                return null;
            }
        }

  
        //private void toolStripButton1_Click(object sender, EventArgs e)
        //{
        //    CSharpCode c = ActiveDocument;
        //    if (c != null)
        //    {
        //        c.ExecuteSelectionCSharp();
        //    }            
        //}

     
        public void LogException(Exception ex)
        {
            SPCoderLogging.Logger.Error(ex);
            m_log.LogError(ex.Message);
            if (ex.StackTrace != null && !ex.StackTrace.Contains("SPCoder.SPCoderForm.ExecuteScriptCSharp"))
            {
                m_log.LogError(ex.StackTrace);
            }

            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate ()
                {
                    m_log.Show(dockPanel);
                });
            }
            else
            {
                m_log.Show(dockPanel);
            }
            
            //MessageBox.Show("An error has occurred during execution of the script:" + ex.Message);
        }

        public void LogError(string err)
        {
            SPCoderLogging.Logger.Error(err);
            m_log.LogError(err);            
            m_log.Show(dockPanel);
            //MessageBox.Show("An error has occurred during execution of the script:" + ex.Message);
        }

        public void AppendToLog(string text)
        {            
            m_log.AppendToLog(text);
            SPCoderLogging.Logger.Info(text);
        }

        public static ScriptState<object> ScriptStateCSharp = null;

        public void ExecuteScriptCSharp(string script, int timesCalled = 0)
        {
            try
            {
                //check to see if it is the one liner script and if it ends with ;
                if (!string.IsNullOrEmpty(script) && script.IndexOf("\n") == -1 && !script.EndsWith(";"))
                    script += ";";

                if (timesCalled == 0)
                {
                    m_output.ClearOutputIfChecked();
                }

                //if (ScriptStateCSharp == null)
                //{
                //    ScriptStateCSharp = CSharpScript.RunAsync(script, ScriptOptions.Default.AddImports("System"), this).Result;
                //}
                //else
                //{
                    ScriptStateCSharp = ScriptStateCSharp.ContinueWithAsync(script).Result;
                    //Console.WriteLine(DateTime.Now);
                //}
            //ScriptStateCSharp = ScriptStateCSharp == null ? 
                    //CSharpScript.RunAsync(script, DefaultScriptOptions(), MyContext).Result :
              //       :
                    
                
                //check if this is the executeFile call
                var execNext = ScriptStateCSharp.GetVariable(SPCoderConstants.SP_CODER_EXECUTE_NEXT);
                if (execNext != null && execNext.Value != null)
                {
                    string codeToExecute = execNext.Value.ToString();
                    //ScriptStateCSharp.Variables.Remove(execNext);
                    execNext.Value = null;
                    //prevent the further recursion
                    if (timesCalled == 0)
                    {
                        ExecuteScriptCSharp(codeToExecute, 1);
                    }
                }
                //return ScriptStateCSharp;
            }
            catch (Exception exc)
            {
                if (exc != null && exc.InnerException != null)
                    throw exc.InnerException;
                else
                    throw exc;
            }
        }

        public delegate void ExecuteScriptDelegate(string script);

        public void ExecuteScript(string script)
        {
            ExecuteScriptCSharp(script, 0);
            //SetAppStatus("Ready");
        }
        public object ExecuteScriptAsync(string script)
        {
            try
            {
                ExecuteScriptDelegate execScript = new ExecuteScriptDelegate(ExecuteScript);
                SetAppStatus("Executing script...");
                var r = execScript.BeginInvoke(script, new AsyncCallback(ExecuteScriptEndInvoke), new ScriptExecutionErrorHelper { Script = script, CodeWindow = ActiveDocument });
                //execScript.BeginInvoke(script, null, null);
                //execScript.Invoke(script);
                //SetAppStatus("Ready1");

                return null;
            }
            catch (Exception exc)
            {
                SPCoderLogging.Logger.Error(exc);
                throw exc;
            }
        }




        public void ExecuteScriptEndInvoke(IAsyncResult asyncResult)
        {
            try
            {
                AsyncResult res = (AsyncResult)asyncResult;
                ExecuteScriptDelegate del = (ExecuteScriptDelegate)res.AsyncDelegate;
                
                del.EndInvoke(asyncResult);
                //Console.WriteLine(obj);
                //Console.WriteLine(DateTime.Now);
                SetAppStatus("Ready");
            }
            catch (Exception exc)
            {
                SetAppStatus("Error");
                //SetAppStatus("Ready3");
                SPCoderLogging.Logger.Error(exc);
                SPCoderForm.MainForm.LogException(exc);
                ScriptExecutionErrorHelper state = (ScriptExecutionErrorHelper)asyncResult.AsyncState;
                state.CodeWindow.HighlightErrorLineInvoke(exc.Message, state.Script);

                //this.Invoke((MethodInvoker)delegate ()
                //{
                    
                //    var ad = ActiveDocument;
                //    if (ad != null)
                //    {
                //        if (asyncResult.AsyncState != null)
                //        {
                //            ad.HighlightErrorLineInvoke(exc.Message, asyncResult.AsyncState.ToString());
                //        }
                //    }
                //});
                //var ad = ActiveDocument;
                //if (ad != null)
                {
                  //  if (asyncResult.AsyncState != null)
                    {
                    //    ad.HighlightErrorLineInvoke(exc.Message, asyncResult.AsyncState.ToString());

                        //if (this.InvokeRequired)
                        //{
                        //    this.BeginInvoke((MethodInvoker)delegate ()
                        //    {
                        //        ad.HighlightErrorLine(exc.Message, asyncResult.AsyncState.ToString());
                        //    });
                        //}
                        //else
                        //{
                        //    ad.HighlightErrorLine(exc.Message, asyncResult.AsyncState.ToString());
                        //}
                    }
                }
                
            }
        }

        private void AddItemToContext(object item)
        {            
            m_context.AddToContext(item);
        }

        //This can be called by the user from the script
        public void AddToContext(object item)
        {
            m_context.AddToContext(item);
        }


        public void RemoveDataItemFromContext(object objectToRemove)
        {
            m_context.RemoveDataItemFromContext(objectToRemove);
        }

        private void Main_Load(object sender, EventArgs e)
        {
            Application.DoEvents();
            Init();
            Application.DoEvents();
            splashScreen.Hide();
        }

        private void Init()
        {
            this.CenterToScreen();

            Application.DoEvents();

            this.BringToFront();

            ExecuteAutorunScripts();

            //This has been moved to run after the first autorun script, inside the ExecuteAutorunScripts() method
            //because the main.Connect must be available in the autorun scripts that run after the first one
            /*
            m_context.AddToContext(new ContextItem { Data = m_output.RtOutput, Name = "rtTxt", Type = m_output.RtOutput.GetType().ToString() });
            m_context.AddToContext(new ContextItem { Data = m_log, Name = "spLog", Type = typeof(ISPCoderLog).ToString() });
            //AddItemToContext(new ContextItem { Data = describer, Name = "describer", Type = describer.GetType().ToString() });
            m_context.AddToContext(new ContextItem { Data = this, Name = "main", Type = this.GetType().ToString() });
            */

            splashScreen.turnOffTimer();
        }
        
        public FastColoredTextBox SourceCodeBox
        {
            get
            {
                Form activeMdi = ActiveMdiChild;
                FastColoredTextBox tempsbc = null;
                if (activeMdi != null && activeMdi is CSharpCode)
                {
                    foreach (Control cont in activeMdi.Controls)
                    {
                        if (cont is FastColoredTextBox)
                        {
                            tempsbc = (FastColoredTextBox)cont;
                            break;
                        }
                    }
                }
                return tempsbc;
            }
        }

        public CSharpCode ActiveDocument
        {
            get
            {
                Form activeMdi = ActiveMdiChild;
                
                if (activeMdi != null && activeMdi is CSharpCode)
                {
                    return activeMdi as CSharpCode;
                }
                return null;
            }
        }

        public List<string> FilesRegisteredForExecution = new List<string>();
        private void ExecuteAutorunScripts()
        {
            try
            {
                List<string> scripts = AutorunScriptUtils.GetAutorunScriptSources();
                int numOfScripts = 0; // scripts.Count;
                foreach (string script in scripts)
                {
                    try
                    {
                       
                      
                        ExecuteScriptCSharp(script);
                     
                        if (numOfScripts == 0)
                        {
                            //m_context.AddToContext(new ContextItem { Data = m_output.RtOutput, Name = "rtTxt", Type = m_output.RtOutput.GetType().ToString() });
                            //m_context.AddToContext(new ContextItem { Data = m_log, Name = "spLog", Type = typeof(ISPCoderLog).ToString() });
                            //AddItemToContext(new ContextItem { Data = describer, Name = "describer", Type = describer.GetType().ToString() });
                            m_context.AddToContext(new ContextItem { Data = this, Name = "main", Type = this.GetType().ToString() });
                        }
                        numOfScripts++;
                    }
                    catch (Exception ex)
                    {
                        //LogException(ex);
                        //AppendToLog(ex.Message); 
                        LogError(ex.Message);
                    }
                }

                //if a file has been registered for execution during the autorunscripts run (plugin?), run it
                foreach (string script in FilesRegisteredForExecution)
                {
                    try
                    {
                        string code = File.ReadAllText(script);
                        ExecuteScriptCSharp(code);
                    }
                    catch (Exception ex)
                    {
                        LogError("Error during execution of FilesRegisteredForExecution: " + script + "; " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                LogException(ex);                
            }
        }

        public Log SpLog { get; set; }
        public Output SpOutput { get; set; }

        public GridViewer SpGrid { get; set; }
        public bool AllowClose { get; set; }
        

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            AllowClose = true;
            //Here try to close all code windows and if any of them is not closed (cancel is pressed) stop closing the app.
            //var forms = codeWindows
            SPCoderSettings.ClearOpenedWindows();
            foreach (var c in codeWindows.ToArray())
            {
                CSharpCode f = (CSharpCode)c;
                if (f.FullFileName != null)
                {
                    SPCoderSettings.AddPathToOpenedWindows(f.FullFileName.ToString());
                }

                CloseCodeWindow(c);
                //c.Close();
                if (!AllowClose)
                {
                    e.Cancel = true;
                    return;
                }
            }
            
            String xmlPath = System.IO.Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Config2016\CSharp\DockPanel.xml");
            dockPanel.SaveAsXml(xmlPath);
            //save settings
            SPCoderSettings.SaveSettings();
        }

        public void RemoveCodeWindow(CSharpCode code)
        {
            if (codeWindows.Contains(code))
                codeWindows.Remove(code);
        }

        private void lbContext_KeyPress(object sender, KeyPressEventArgs e)
        {
            //char c = (Char) Keys.Delete;
        }

        private void virToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            string name = "New Script " + codeWindows.Count;
            GenerateNewSourceTab(name, "", null);
        }


        List<CSharpCode> codeWindows = new List<CSharpCode>();
        /// <summary>
        /// Creates the new source tab.
        /// </summary>
        /// <param name="title">Title of the window</param>
        /// <param name="source">Source code</param>
        /// <param name="fullFileName">Full file path</param>
        public void GenerateNewSourceTab(string title, string source, string fullFileName, string extension = null)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate ()
                {
                    GenerateNewSourceTabPrivate(title, source, fullFileName, extension);
                });
            }
            else
            {
                GenerateNewSourceTabPrivate(title, source, fullFileName, extension);
            }            
        }

        private void GenerateNewSourceTabPrivate(string title, string source, string fullFileName, string extension = null)
        {
            CSharpCode newCode = new CSharpCode();
            newCode.Fctb.ContextMenuStrip = cmMain;
            if (string.IsNullOrEmpty(extension))
            {
                newCode.Fctb.Language = GetLanguageFromFileName(fullFileName);
            }
            else
            {
                newCode.Fctb.Language = SPCoderSettings.GetLanguageFromExtension(extension);
            }

            newCode.Source = source;
            newCode.Title = title;
            newCode.FullFileName = fullFileName;
            newCode.FileName = fullFileName;
            newCode.Show(dockPanel, DockState.Document);
            codeWindows.Add(newCode);
        }



        private Language GetLanguageFromFileName(string fileName)
        {
            Language lang = Language.CSharp;
            if (!string.IsNullOrEmpty(fileName))
            {
                //get the extension without dot
                bool found = false;
                string ext = Path.GetExtension(fileName).Remove(0,1);
                foreach (Language l in (Language[])Enum.GetValues(typeof(Language)))
                {
                    if (l.ToString().ToUpper() == ext.ToUpper())
                    {
                        lang = l;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    //try to get the extension from the settings
                    return SPCoderSettings.GetLanguageFromExtension(ext);
                }
            }

            return lang;
        }
       
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.FileOk += openFileDialog1_FileOk;
            fd.ShowDialog(this);            
        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OpenFileDialog fd = (OpenFileDialog)sender;
            string fileName = fd.FileName;
            //Check if the file is already opened in SPCoder and if it is switch to the tab
            bool alreadyOpened = false;
            foreach (CSharpCode window in codeWindows)
            {
                string path = window.GetFilePath();
                if (path != null && path == fileName)
                {
                    window.Show(dockPanel, DockState.Document);
                    alreadyOpened = true;
                    break;
                }
            }

            if (!alreadyOpened)
            {
                Stream s = fd.OpenFile();
                StreamReader tr = new StreamReader(s);
                string source = tr.ReadToEnd();
                tr.Close();
                string title = fd.SafeFileName;
                string fullFileName = fd.FileName;

                GenerateNewSourceTab(title, source, fullFileName);
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            var a = ActiveDocument;
            if (a != null)
            {
                a.SaveCurrentCode(true);
            }   
        }

        private void SaveCurrentCode()
        {
            var a = ActiveDocument;
            if (a != null)
            {
                a.SaveCurrentCode(true);
            }            
        }
        private bool SaveCurrentCode(bool forceOverwrite)
        { 
            var a = ActiveDocument;
            if (a != null)
            {
                return a.SaveCurrentCode(forceOverwrite);
            }
            return false;
        }
      
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GenerateNewSourceTab("New Script ", "", null);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.FileOk += openFileDialog1_FileOk;
            fd.ShowDialog(this);      
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            SaveCurrentCode(false);
        }

        //private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    TabPage tab = sourceTabs.SelectedTab;
        //    if (tab.Text.EndsWith("*"))
        //    {
        //        DialogResult dialog = MessageBox.Show("Do you want to save script before closing?","Save the script",MessageBoxButtons.YesNoCancel,MessageBoxIcon.Question);
        //        if (dialog == DialogResult.Yes)
        //        {
        //            SaveCurrentCode(false);
        //            sourceTabs.TabPages.Remove(tab);
        //        }
        //        else if (dialog == DialogResult.No)
        //        {
        //            sourceTabs.TabPages.Remove(tab);
        //        }
        //        //in case of Cancel just continue.
        //    }
        //    else
        //    {
        //        sourceTabs.TabPages.Remove(tab);
        //    }
        //}

        private void btnSaveAsAutoRunScript_Click(object sender, EventArgs e)
        {
            SaveAsAutorunScript();
        }

        private void SaveAsAutorunScript()
        {
            //First check if this script has been saved.
            bool result = SaveCurrentCode(true);

            if (!result)
            {
                return;
            }
            FastColoredTextBox tempsbc = SourceCodeBox;
            if (tempsbc == null)
            {
                throw new Exception("Cannot find control box control!");
            }
            string tempFileName = tempsbc.Tag.ToString();

            //After saving the script add it as autorun script at the last position, and then open the autorun scripts window
            var currentConfig = AutorunScriptUtils.GetAutorunConfig();
            int order = (currentConfig.AutoRunScripts == null) ? 1 : currentConfig.AutoRunScripts.Count + 1;
            var script = new AutorunScriptConfigItem();
            script.Order = order;
            script.Path = tempFileName;
            script.Title = Path.GetFileName(tempFileName);

            currentConfig.AutoRunScripts.Add(script);
            AutorunScriptUtils.SaveConfig(currentConfig);
            AppendToLog("Added new autorun script");
            
            //AutoRunScriptsForm arsForm = new AutoRunScriptsForm();
            //arsForm.ShowDialog(this);
            m_autoRun.Show(dockPanel);
            m_autoRun.RefreshView();
            
        }

        private void propertiesToolStripMenuItem2_Click(object sender, EventArgs e)
        {

        }

        private void propertiesToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void cms2TreeView_Opening(object sender, CancelEventArgs e)
        {

        }

        private void codeWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveCurrentCode(true);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveCurrentCode(false);
        }

        private void saveAsAutorunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAsAutorunScript();
        }

        private void autorunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_autoRun.Show(dockPanel);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox box = new AboutBox();
            box.ShowDialog(this);
        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            var a = ActiveDocument;
            if (a != null)
            {
                a.ExecuteSelectionCSharp(true);
            }
        }

        public void Connect(string url)
        {
            m_explorerView.Connect(url);
        }

        public void Connect(string url, string omType)
        {
            this.Connect(url, omType, null, null);
        }

        public void Connect(string url, string omType, string username, string password)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate ()
                {
                    m_explorerView.Connect(url, omType, username, password);
                });
            }
            else
            {
                m_explorerView.Connect(url, omType, username, password);
            }
            
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            m_properties.Show(dockPanel);
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            m_output.Show(dockPanel);
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            m_context.Show(dockPanel);
        }

        public void ShowProperties(object obj, string name)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate ()
                {
                    this.ShowPropertiesInternal(obj, name);
                });
            }
            else
            {
                this.ShowPropertiesInternal(obj, name);
            }            
        }

        private void ShowPropertiesInternal(object obj, string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                m_properties.SetTextProperty(name);
            }
            m_properties.PgEditor.SelectedObject = obj;
            m_properties.Show(dockPanel);
        }

        internal void ClearOutput()
        {
            m_output.RtOutput.PerformSafely2<RichTextBox> (m => m.Clear(), m_output.RtOutput);
        }

        internal void Describe(ContextItem item)
        {
            m_describer.Describe(item);
            m_describer.Show(dockPanel, DockState.Document);
        }

        private void toolStripButtonDescriber_Click(object sender, EventArgs e)
        {
            m_describer.Show(dockPanel, DockState.Document);
        }

        private void toolStripButtonAutorun_Click(object sender, EventArgs e)
        {
            m_autoRun.Show(dockPanel);
        }

        public bool ShouldHighlightInvisibleChars() 
        {
            return btInvisibleChars.Checked;
        }

        private void btInvisibleChars_Click(object sender, EventArgs e)
        {
            foreach (CSharpCode tab in codeWindows)
            {
                
                tab.HighlightInvisibleChars();
            }
            if (SourceCodeBox != null)
                SourceCodeBox.Invalidate();
        }

        internal void CodeFormClosed(CSharpCode code)
        {
            codeWindows.Remove(code);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void btnExplorer_Click(object sender, EventArgs e)
        {
            m_explorerView.Show(dockPanel);
        }

        private void SPCoderForm_FormClosed(object sender, FormClosedEventArgs e)
        {            
            Application.Exit();
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            AboutBox box = new AboutBox();
            box.ShowDialog(this);
        }

        private void btnViewer_Click(object sender, EventArgs e)
        {
            m_gridviewer.Show(dockPanel, DockState.Document);
        }

        public void ShowGridWindow()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate ()
                {
                    m_gridviewer.Show(dockPanel, DockState.Document);
                });
            }
            else
            {
                m_gridviewer.Show(dockPanel, DockState.Document);
            }
        }

        //private void tsExecuteAsync_Click(object sender, EventArgs e)
        //{
        //    var a = ActiveDocument;
        //    if (a != null)
        //    {
        //        a.ExecuteSelectionCSharp(true);
        //    }
        //}

        //private void toolStripButton8_Click(object sender, EventArgs e)
        //{
        //    var a = ActiveDocument;
        //    if (a != null)
        //    {
        //        a.ExecuteSelectionCSharp(true);
        //    }
        //}

        private void toolStripButton8_Click_1(object sender, EventArgs e)
        {
            m_log.Show(dockPanel);
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SourceCodeBox.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SourceCodeBox.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SourceCodeBox.Paste();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SourceCodeBox.Selection.SelectAll();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SourceCodeBox.UndoEnabled)
                SourceCodeBox.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SourceCodeBox.RedoEnabled)
                SourceCodeBox.Redo();
        }


        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SourceCodeBox.ShowFindDialog();
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SourceCodeBox.ShowReplaceDialog();
        }

        private void autoIndentSelectedTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SourceCodeBox.DoAutoIndent();
        }

        private void commentSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SourceCodeBox.InsertLinePrefix("//");
        }

        private void uncommentSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SourceCodeBox.RemoveLinePrefix("//");
        }

        private void cloneLinesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var scb = SourceCodeBox;
            //expand selection
            scb.Selection.Expand();
            //get text of selected lines
            string text = Environment.NewLine + scb.Selection.Text;
            //move caret to end of selected lines
            scb.Selection.Start = scb.Selection.End;
            //insert text
            scb.InsertText(text);
        }

        private void cloneLinesAndCommentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var scb = SourceCodeBox;
            //start autoUndo block
            scb.BeginAutoUndo();
            //expand selection
            scb.Selection.Expand();
            //get text of selected lines
            string text = Environment.NewLine + scb.Selection.Text;
            //comment lines
            scb.InsertLinePrefix("//");
            //move caret to end of selected lines
            scb.Selection.Start = scb.Selection.End;
            //insert text
            scb.InsertText(text);
            //end of autoUndo block
            scb.EndAutoUndo();
        }

        public void UpdateMenuButtons()
        {
            try
            {
                var scb = SourceCodeBox;
                if (scb != null)// && tsFiles.Items.Count > 0)
                {
                    var tb = scb;
                    undoStripButton.Enabled = undoToolStripMenuItem.Enabled = undoToolStripMenuItem1.Enabled = tb.UndoEnabled;
                    redoStripButton.Enabled = redoToolStripMenuItem.Enabled = redoToolStripMenuItem1.Enabled = tb.RedoEnabled;
                    saveToolStripButton.Enabled = saveToolStripMenuItem.Enabled = tb.IsChanged;
                    saveAsToolStripButton.Enabled = saveAsToolStripMenuItem.Enabled = true;
                    saveAsAutorunToolStripMenuItem.Enabled = btnSaveAsAutoRunScript.Enabled = true;
                    //pasteToolStripButton.Enabled = 
                    pasteToolStripMenuItem.Enabled = pasteToolStripMenuItem1.Enabled = true;
                    //cutToolStripButton.Enabled = 
                    cutToolStripMenuItem.Enabled = cutToolStripMenuItem1.Enabled =
                    //copyToolStripButton.Enabled = 
                    copyToolStripMenuItem.Enabled = copyToolStripMenuItem1.Enabled = !tb.Selection.IsEmpty;
                    printToolStripButton.Enabled = printToolStripMenuItem.Enabled = true;
                    wordWrapStripButton.Checked = tb.WordWrap;
                    findToolStripMenuItem1.Enabled = true;
                    replaceToolStripMenuItem1.Enabled = true;
                }
                else
                {
                    saveToolStripButton.Enabled = saveToolStripMenuItem.Enabled = false;
                    saveAsToolStripButton.Enabled = saveAsToolStripMenuItem.Enabled = false;
                    saveAsAutorunToolStripMenuItem.Enabled = btnSaveAsAutoRunScript.Enabled = false;
                    //cutToolStripButton.Enabled = 
                    cutToolStripMenuItem.Enabled = cutToolStripMenuItem1.Enabled =
                    //copyToolStripButton.Enabled = 
                    copyToolStripMenuItem.Enabled = copyToolStripMenuItem1.Enabled = false;
                    //pasteToolStripButton.Enabled = 
                    pasteToolStripMenuItem.Enabled = pasteToolStripMenuItem1.Enabled = false;
                    printToolStripButton.Enabled = printToolStripMenuItem.Enabled = false;
                    undoStripButton.Enabled = undoToolStripMenuItem.Enabled = undoToolStripMenuItem1.Enabled = false;
                    redoStripButton.Enabled = redoToolStripMenuItem.Enabled = redoToolStripMenuItem1.Enabled = false;
                    findToolStripMenuItem1.Enabled = false;
                    replaceToolStripMenuItem1.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void printToolStripButton_Click(object sender, EventArgs e)
        {
            if (SourceCodeBox != null)
            {
                var settings = new PrintDialogSettings();
                settings.Title = ActiveDocument.Title;
                settings.Header = "&b&w&b";
                settings.Footer = "&b&p";
                SourceCodeBox.Print(settings);
            }
        }

        private void wordWrapStripButton_Click(object sender, EventArgs e)
        {
            var scb = SourceCodeBox;
            if (scb != null)// && tsFiles.Items.Count > 0)
            {
                if (wordWrapStripButton.Checked)
                {
                    scb.WordWrap = true;
                    scb.WordWrapMode = WordWrapMode.WordWrapControlWidth;
                }
                else
                {
                    scb.WordWrap = false;
                }
            }
        }

        private void autorunToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            m_autoRun.Show(dockPanel);
        }

        private void contextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_context.Show(dockPanel);
        }

        private void describerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_describer.Show(dockPanel);
        }

        private void explorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_explorerView.Show(dockPanel);
        }

        private void logToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_log.Show(dockPanel);
        }

        private void outputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_output.Show(dockPanel);
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_properties.Show(dockPanel);
        }

        private void viewerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_gridviewer.Show(dockPanel);
        }

        private void newToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            toolStripButton6_Click(null, null);
        }

        private void openToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            toolStripButton2_Click(null, null);
        }

        private void saveToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            toolStripButton4_Click(null, null);
        }

        private void saveAsToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            btnSaveAs_Click(null, null);
        }

        private void saveAsAutorunToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            SaveAsAutorunScript();
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            printToolStripButton_Click(null, null);
        }

        private void changeLanguage_Click(object sender, EventArgs e)
        {
            if (SourceCodeBox != null)
            {
                //set language
                string lang = (sender as ToolStripMenuItem).Text;
                var fctb = SourceCodeBox;
                
                fctb.ClearStylesBuffer();
                fctb.Range.ClearStyle(StyleIndex.All);
                //InitStylesPriority();
                //fctb.AutoIndentNeeded -= fctb_AutoIndentNeeded;
                //
                switch (lang)
                {
                    case "CSharp": fctb.Language = Language.CSharp; break;
                    case "VB": fctb.Language = Language.VB; break;
                    case "HTML": fctb.Language = Language.HTML; break;
                    case "XML": fctb.Language = Language.XML; break;
                    case "SQL": fctb.Language = Language.SQL; break;
                    case "PHP": fctb.Language = Language.PHP; break;
                    case "JS": fctb.Language = Language.JS; break;
                    case "JSON": fctb.Language = Language.JSON; break;
                }
                fctb.OnSyntaxHighlight(new TextChangedEventArgs(fctb.Range));
            }
        }
        private void cSharpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeLanguage_Click(sender, e);
        }

        private void hTMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeLanguage_Click(sender, e);
        }

        private void jSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeLanguage_Click(sender, e);
        }

        private void jSONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeLanguage_Click(sender, e);
        }

        private void vBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeLanguage_Click(sender, e);
        }

        private void xMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeLanguage_Click(sender, e);
        }

        private void pHPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeLanguage_Click(sender, e);
        }

        private void sQLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeLanguage_Click(sender, e);
        }

        private void languageToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            if (SourceCodeBox != null)
            {
                string lang = SourceCodeBox.Language.ToString().ToUpper();

                foreach (ToolStripMenuItem mi in languageToolStripMenuItem.DropDownItems)
                {
                    mi.Checked = mi.Text.ToUpper() == lang;
                }
            }
            else
            {
                foreach (ToolStripMenuItem mi in languageToolStripMenuItem.DropDownItems)
                {
                    mi.Checked = false;
                }
            }
        }

        private void formatToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            if (SourceCodeBox != null)
            {
                paragraphToolStripMenuItem.Checked = btInvisibleChars.Checked;
                wordWrapToolStripMenuItem.Checked = SourceCodeBox.WordWrap;
            }
            else
            {
                foreach (ToolStripMenuItem mi in languageToolStripMenuItem.DropDownItems)
                {
                    mi.Checked = false;
                }
            }
        }

        private void paragraphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btInvisibleChars.Checked = !btInvisibleChars.Checked;
            btInvisibleChars_Click(sender, e);
        }

        private void wordWrapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            wordWrapStripButton.Checked = !wordWrapStripButton.Checked;
            wordWrapStripButton_Click(sender, e);
        }

        private void undoToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            undoToolStripMenuItem_Click(sender, e);
        }

        private void redoToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            redoToolStripMenuItem_Click(sender, e);
        }

        private void cutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            cutToolStripMenuItem_Click(sender, e);
        }

        private void copyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            copyToolStripMenuItem_Click(sender, e);
        }

        private void pasteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            pasteToolStripMenuItem_Click(sender, e);
        }

        private void findToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (SourceCodeBox != null)
            {
                SourceCodeBox.ShowFindDialog();
            }
        }

        private void replaceToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (SourceCodeBox != null)
            {
                SourceCodeBox.ShowReplaceDialog();
            }
        }

        private void closeAllCodeWindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseAllCodeWindows();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild is CSharpCode)
            {
                ActiveMdiChild.Close();
            }
        }

        private void editToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            //if (SourceCodeBox != null)
            //{
            //    foreach (ToolStripMenuItem mi in editToolStripMenuItem.DropDownItems)
            //    {
            //        mi.Enabled = true;
            //    }
            //}
            //else
            //{
            //    foreach (ToolStripMenuItem mi in editToolStripMenuItem.DropDownItems)
            //    {
            //        mi.Enabled = false;
            //    }
            //}
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void SPCoderForm_MdiChildActivate(object sender, EventArgs e)
        {
            Form window = ((Form)sender).ActiveMdiChild;
            if (window == null)
            {
                SPCoderForm.MainForm.SetTitle(null);
                return;
            }

            if (window is CSharpCode)
            {
                CSharpCode w = (CSharpCode)window;
                if (w.Fctb != null && w.Fctb.Tag != null)
                {
                    SPCoderForm.MainForm.SetTitle(w.Fctb.Tag.ToString());
                }
                else
                {
                    SPCoderForm.MainForm.SetTitle(window.Text);
                }
            }
            else
            {
                SPCoderForm.MainForm.SetTitle(window.Text);
            }
        }

        //private void toolStripButton1_Click_2(object sender, EventArgs e)
        //{
        //    var a = ActiveDocument;
        //    if (a != null)
        //    {
        //        a.ExecuteSelectionCSharp(true);
        //    }

        //}

        private void toolStripMenuItemAutocompleteExtensionMethods_Click(object sender, EventArgs e)
        {
            //toolStripMenuItemAutocompleteExtensionMethods
            bool isChecked = !toolStripMenuItemAutocompleteExtensionMethods.Checked;
            toolStripMenuItemAutocompleteExtensionMethods.Checked = isChecked;
            PutExtensionMethodsToAutocomplete = isChecked;
            if (SPCoderSettings.Settings[SPCoderConstants.SP_SETTINGS_CODE] != null)
            {
                var codeSettings = (Dictionary<string, object>)SPCoderSettings.Settings[SPCoderConstants.SP_SETTINGS_CODE];
                codeSettings[SPCoderConstants.SP_SETTINGS_AUTOCOMPLETE_SHOW_EXTENSION_METHODS] = isChecked;
            }
            //reset the cache
            SPCoderUtils.ResetAutocompleteItemsCache();
        }

        private void cryptoHelperToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CryptoHelperWindow window = new CryptoHelperWindow();
            window.Show(this);
        }

        public string Encrypt(string text)
        {
            try
            {
                return CryptoHelper.Encrypt(SPCoderConstants.SP_CODER_CONTAINER_KEY_NAME, text);
            }
            catch (Exception exc)
            {
                MainForm.LogException(exc);                
                return "";
            }
        }

        public string Decrypt(string text)
        {
            try
            {
                return CryptoHelper.Decrypt(SPCoderConstants.SP_CODER_CONTAINER_KEY_NAME, text);
            }
            catch (Exception exc)
            {
                MainForm.LogException(exc);
                return "";
            }
        }
    }
}