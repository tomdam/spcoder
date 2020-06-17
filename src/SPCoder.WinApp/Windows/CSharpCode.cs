using FastColoredTextBoxNS;
using System.Diagnostics;
using SPCoder.Config;
using SPCoder.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.Threading;

namespace SPCoder.Windows
{
    public partial class CSharpCode : SPCoderInnerWindow
    {
        //public static readonly string pythonStyle = @"Resources\python.xml";

        public AutocompleteMenu popupMenu;

        //string lang = "CSharp (custom highlighter)";
        //string lang = "CSharp";

        //styles

        TextStyle RedErrorStyle = new TextStyle(null, Brushes.LightPink, FontStyle.Regular);
        
        MarkerStyle SameWordsStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(40, Color.Gray)));

        TextStyle LinkStyle = new TextStyle(Brushes.Blue, null, FontStyle.Underline);

        public FastColoredTextBox Fctb
        {
            get { return fctb; }
        }
        public CSharpCode()
        {
            InitializeComponent();

            //create autocomplete popup menu
            popupMenu = new AutocompleteMenu(fctb);
            //popupMenu.SearchPattern = @"[\w\.:=!<>()(.*)]";
            popupMenu.SearchPattern = @"[\w\.:=!<>()]";
            //popupMenu.SearchPattern = @"[\w\.]";
            popupMenu.AllowTabKey = true;
            popupMenu.AppearInterval = 200;
            //assign DynamicCollection as items source
            popupMenu.Items.SetAutocompleteItems(new DynamicCollection(popupMenu, fctb));

            //popupMenu.Items.MaximumSize = new System.Drawing.Size(300, 400);
            popupMenu.Items.MaximumSize = new System.Drawing.Size(300, 300);
            popupMenu.Items.Width = 250;
            
            popupMenu.Items.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            
            //fctb.OnTextChangedDelayed(fctb.Range);

            //fctb.DescriptionFile = pythonStyle;
            fctb.Language = Language.CSharp;
            //fctb.OnTextChangedDelayed(fctb.Range);
            fctb.CurrentLineColor = Color.FromArgb(200, 200, 255);

            fctb.HighlightingRangeType = HighlightingRangeType.VisibleRange;

            fctb.CustomAction += fctb_CustomAction;
            
            fctb.HotkeysMapping.Add(Keys.F5, FCTBAction.CustomAction1);
            fctb.HotkeysMapping.Add(Keys.Control | Keys.S, FCTBAction.CustomAction2);
            fctb.HotkeysMapping.Add(Keys.Control | Keys.W, FCTBAction.CustomAction3);
            
            fctb.ClearStylesBuffer();
            fctb.AddStyle(RedErrorStyle);
            fctb.AddStyle(LinkStyle);
            fctb.AddStyle(SameWordsStyle);
            
            //CSharpSyntaxHighlight(e);


        }


        private void fctb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.Space | Keys.Control))
            {
                popupMenu.Show(true);
                e.Handled = true;
            }
        }

        private void InitStylesPriority()
        {
            //add this style explicitly for drawing under other styles
            fctb.AddStyle(SameWordsStyle);
        }

        private void fctb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!this.Text.EndsWith("*") && !textChangeFromCode)
            {
                this.Text += "*";
            }

            if (this.fctb.Language == Language.CSharp)
            {
                //fixing the issue with arrow syntax where default regex splits the = and the >
                this.fctb.AutoIndentCharsPatterns
                = @"
^\s*[\w\.]+(\s\w+)?\s*(?<range>=)\s*(?<range>[^=>;]+);
^\s*(case|default)\s*[^:]*(?<range>:)\s*(?<range>[^;]+);
";
            }
        }

        private void fctb_TextChanged_delayed2(object sender, TextChangedEventArgs e)
        {
            if (errorRange != null)
            {
                e.ChangedRange.ClearStyle(RedErrorStyle);
            }

            e.ChangedRange.ClearStyle(LinkStyle);
            e.ChangedRange.SetStyle(LinkStyle, @"(http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?");
            
        }

        bool CharIsHyperlink(Place place)
        {
            var mask = fctb.GetStyleIndexMask(new Style[] { LinkStyle });
            if (place.iChar < fctb.GetLineLength(place.iLine))
                if ((fctb[place].style & mask) != 0)
                    return true;

            return false;
        }

        private void fctb_MouseMove(object sender, MouseEventArgs e)
        {
            var p = fctb.PointToPlace(e.Location);
            if (CharIsHyperlink(p))
                fctb.Cursor = Cursors.Hand;
            else
                fctb.Cursor = Cursors.IBeam;

            /*Here we can also check if the cursor is over the variable and get the info about it*/

        }

        private void fctb_MouseDown(object sender, MouseEventArgs e)
        {
            /*var p = fctb.PointToPlace(e.Location);
            if (CharIsHyperlink(p))
            {
                var url = fctb.GetRange(p, p).GetFragment(@"[\S]").Text;
                Process.Start(url);
            }*/
        }

        DateTime lastNavigatedDateTime = DateTime.Now;
        private void fctb_SelectionChangedDelayed(object sender, EventArgs e)
        {
            //update icons - enabled/disabled
            SPCoderForm.MainForm.UpdateMenuButtons();
            //var tb = sender as FastColoredTextBox;
            //remember last visit time
            if (fctb.Selection.IsEmpty && fctb.Selection.Start.iLine < fctb.LinesCount)
            {
                if (lastNavigatedDateTime != fctb[fctb.Selection.Start.iLine].LastVisit)
                {
                    fctb[fctb.Selection.Start.iLine].LastVisit = DateTime.Now;
                    lastNavigatedDateTime = fctb[fctb.Selection.Start.iLine].LastVisit;
                }
            }

            fctb.VisibleRange.ClearStyle(SameWordsStyle);
            if (!fctb.Selection.IsEmpty)
                return;//user selected diapason

            //get fragment around caret
            var fragment = fctb.Selection.GetFragment(@"\w");
            string text = fragment.Text;
            if (text.Length == 0)
                return;
            //highlight same words
            var ranges = fctb.VisibleRange.GetRanges("\\b" + text + "\\b").ToArray();
            if (ranges.Length > 1)
                foreach (var r in ranges)
                    r.SetStyle(SameWordsStyle);
        }

        

        private void fctb_AutoIndentNeeded(object sender, AutoIndentEventArgs args)
        {
            //block {}
            if (Regex.IsMatch(args.LineText, @"^[^""']*\{.*\}[^""']*$"))
                return;
            //start of block {}
            if (Regex.IsMatch(args.LineText, @"^[^""']*\{"))
            {
                args.ShiftNextLines = args.TabLength;
                return;
            }
            //end of block {}
            if (Regex.IsMatch(args.LineText, @"}[^""']*$"))
            {
                args.Shift = -args.TabLength;
                args.ShiftNextLines = -args.TabLength;
                return;
            }
            //label
            if (Regex.IsMatch(args.LineText, @"^\s*\w+\s*:\s*($|//)") &&
                !Regex.IsMatch(args.LineText, @"^\s*default\s*:"))
            {
                args.Shift = -args.TabLength;
                return;
            }
            //some statements: case, default
            if (Regex.IsMatch(args.LineText, @"^\s*(case|default)\b.*:\s*($|//)"))
            {
                args.Shift = -args.TabLength / 2;
                return;
            }
            //is unclosed operator in previous line ?
            //if (Regex.IsMatch(args.PrevLineText, @"^\s*(if|for|foreach|while|[\}\s]*else)\b[^{]*$"))
            //    if (!Regex.IsMatch(args.PrevLineText, @"(;\s*$)|(;\s*//)"))//operator is unclosed
            //    {
            //        args.Shift = args.TabLength;
            //        return;
            //    }
        }
        

        void fctb_CustomAction(object sender, CustomActionEventArgs e)
        {
            if (e.Action == FCTBAction.CustomAction1)
            {
                //this.ExecuteSelectionCSharp(true);
                this.ExecuteSelectionCSharp(SPCoderForm.AsynchronousExecution);
            } 
            else if (e.Action == FCTBAction.CustomAction2)
                {
                    this.SaveCurrentCode();    
                }
            else if (e.Action == FCTBAction.CustomAction3)
            {
                this.CloseCodeWindow();
            }
        }

        protected bool textChangeFromCode;
        public string Source
        {
            get
            {
                return fctb.Text;
            }
            set
            {
                textChangeFromCode = true;
                fctb.Text = value;
                textChangeFromCode = false;
            }
        }

        public string Title
        {
            get
            {
                return this.Text;
            }
            set
            {
                this.Text = value;
            }

        }
        public string FullFileName
        {
            get
            {
                return fctb.Tag as string;
            }
            set
            {
                fctb.Tag = value;
            }
        }
        
        public string FileName { get; set; }

        Style invisibleCharsStyle = new InvisibleCharsRenderer(Pens.Gray);

        public void HighlightInvisibleChars()
        {
            Range range = fctb.Range;
            HighlightInvisibleChars(range);
        }

        public void ChangeWordWrap(bool val)
        {
            this.fctb.WordWrap = val;
        }

        public void HighlightInvisibleChars(Range range)
        {             
            range.ClearStyle(invisibleCharsStyle);
            if (SPCoderForm.MainForm.ShouldHighlightInvisibleChars())
                range.SetStyle(invisibleCharsStyle, @".$|.\r\n|\s");
        }

        public void ExecuteSelectionCSharp(bool isasync = false)
        {
            string text = null;
            try
            {               
                string selection = fctb.SelectedText;
                if (string.IsNullOrEmpty(selection))
                {
                    text = fctb.Text;
                }
                else
                {
                    text = selection;
                }
                if (errorRange != null)
                {
                    errorRange.ClearStyle(RedErrorStyle);
                    errorRange = null;
                }
                if (!isasync)
                {
                    SPCoderForm.MainForm.ExecuteScriptCSharp(text);
                }
                else
                {
                    SPCoderForm.MainForm.ExecuteScriptAsync(text);
                }
                
            }
            catch (Exception ex)
            {
                SPCoderForm.MainForm.LogException(ex);
                HighlightErrorLine(ex.Message, text);
            }
        }

        public void HighlightErrorLineInvoke(string message, string code)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate ()
                {
                    HighlightErrorLine(message, code);
                });
            }
            else
            {
                HighlightErrorLine(message, code);
            }
        }

        public Range errorRange;
        public void HighlightErrorLine(string message, string code)
        {
            //it can be line in selected text or in the whole file
            try
            {
                if (!string.IsNullOrEmpty(message) && message.StartsWith("("))
                {
                    Match match = Regex.Match(message, @"\((.+?)\)", RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        string[] coordinatesStr = match.Groups[1].Value.Trim().Split(',');
                        if (coordinatesStr != null && coordinatesStr.Length == 2)
                        {
                           
                            int row = int.Parse(coordinatesStr[0]) - 1;
                            int col = int.Parse(coordinatesStr[1]) + 1;
                            //now highlight the row in the text
                            if (fctb.Selection.Length > 0)
                            {
                                row += fctb.Selection.FromLine;
                            }
                            col = 0;
                            Place start = new Place(col, row);
                            Place stop = new Place(col + 100, row);
                            //fctb.ClearStyle(StyleIndex.)
                            errorRange = fctb.GetRange(start, stop);
                            errorRange.ClearStyle(RedErrorStyle);
                            errorRange.SetStyle(RedErrorStyle);
                            //fctb.DoRangeVisible(errorRange, true);
                            this.Show();
                            //fctb.AutoScrollPosition = fctb.PlaceToPoint(start);
                            fctb.DoCaretVisible();
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                SPCoderForm.MainForm.LogException(exc);
            }
        }

        public void SaveCurrentCode()
        {
            SaveCurrentCode(true);
        }

        public bool SaveCurrentCode(bool forceOverwrite)
        {
            //Save a file            
            bool result = false;
            
            string text = fctb.Text;
            bool written = false;
            if (forceOverwrite)
            {
                if (fctb.Tag != null && !string.IsNullOrEmpty(fctb.Tag.ToString()))
                {
                    string tempFileName = fctb.Tag.ToString();
                    if (System.IO.File.Exists(tempFileName))
                    {
                        File.WriteAllText(tempFileName, text, Encoding.UTF8);
                        
                        SPCoderForm.MainForm.AppendToLog(string.Format("File '{0}' has been saved.", tempFileName));
                        written = true;                        
                        this.Text = Path.GetFileName(tempFileName);
                        result = true;
                    }
                }
                else
                {
                    forceOverwrite = false;
                }
            }

            if (!forceOverwrite)
            {
                if (!written)
                {
                    //else, open the dialog and save it
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Title = "Save script";
                    saveFileDialog.DefaultExt = "csx";
                    saveFileDialog.InitialDirectory = ConfigUtils.FullDirectoryPath;
                    saveFileDialog.Filter = "CSharp script files|*.csx|CSharp files|*.cs|All files|*.*";

                    saveFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(saveFileDialog_FileOk);
                    saveFileDialog.ShowDialog(this);
                    if (string.IsNullOrEmpty(saveFileDialog.FileName))
                    {
                        return false;
                    }

                    File.WriteAllText(saveFileDialog.FileName, text, Encoding.UTF8);
                    written = true;                    
                    this.Text = Path.GetFileName(saveFileDialog.FileName);
                    fctb.Tag = saveFileDialog.FileName;
                    SPCoderForm.MainForm.AppendToLog(string.Format("File '{0}' has been saved.", saveFileDialog.FileName));

                    result = true;
                }
            }
            return result;
        }

        public override string GetFilePath()
        {
            if (fctb.Tag != null && !string.IsNullOrEmpty(fctb.Tag.ToString()))
            {
                return fctb.Tag.ToString();
            }
            return null;
        }

        private void saveFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            //SaveFileDialog dialog = (SaveFileDialog) sender;


        }

        private void Code_FormClosed(object sender, FormClosedEventArgs e)
        {
            SPCoderForm.MainForm.CodeFormClosed(this);
        }

        private void Code_FormClosing(object sender, FormClosingEventArgs e)
        {           
            if (this.Text.EndsWith("*"))
            {
                var result = MessageBox.Show(this, "Save " + this.Text.TrimEnd('*') + "?", "Save",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    SPCoderForm.MainForm.AllowClose = false;
                    return;
                }
                else if (result == DialogResult.Yes)
                {
                    SaveCurrentCode(true);                    
                }
            }
            //add to history
            if (fctb != null && fctb.Tag != null)
            {
                SPCoderForm.MainForm.AddRecentMenuItem(fctb.Tag.ToString());
            }
            SPCoderForm.MainForm.RemoveCodeWindow(this);
        }

        protected void CloseCodeWindow()
        {            
            this.Close();
        }

        private void fctb_TextChangedDelayed(object sender, TextChangedEventArgs e)
        {
            fctb_TextChanged_delayed2(sender, e);
            //if (!this.Text.EndsWith("*"))
            //{
            //    this.Text += "*";
            //}
            //show invisible chars
            HighlightInvisibleChars(e.ChangedRange);
            SPCoderForm.MainForm.UpdateMenuButtons();
        }

       
        private void CSharpCode_Activated_1(object sender, EventArgs e)
        {
            SPCoderForm.MainForm.UpdateMenuButtons();
            fctb.WordWrap = SPCoderForm.MainForm.ShouldWordwrapBeActivated();
        }

        private void CSharpCode_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            SPCoderForm.MainForm.GenerateNewSourceTabsFromPaths(files);
        }

        private void CSharpCode_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void fctb_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            SPCoderForm.MainForm.GenerateNewSourceTabsFromPaths(files);
        }

        private void fctb_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }
    }

    internal class DynamicCollection : IEnumerable<AutocompleteItem>
    {
        private AutocompleteMenu menu;
        private FastColoredTextBox tb;

        string[] keywords = { "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override", "params", "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual", "void", "volatile", "while", "add", "alias", "ascending", "descending", "dynamic", "from", "get", "global", "group", "into", "join", "let", "orderby", "partial", "remove", "select", "set", "value", "var", "where", "yield" };
        string[] methods = { "Equals()", "GetHashCode()", "GetType()", "ToString()" };
        string[] snippets = { "if(^)\n{\n;\n}", "if(^)\n{\n;\n}\nelse\n{\n;\n}", "for(^;;)\n{\n;\n}", "while(^)\n{\n;\n}", "do${\n^;\n}while();", "switch(^)\n{\ncase : break;\n}" };
        string[] declarationSnippets = {
               "public class ^\n{\n}", "private class ^\n{\n}", "internal class ^\n{\n}",
               "public struct ^\n{\n;\n}", "private struct ^\n{\n;\n}", "internal struct ^\n{\n;\n}",
               "public void ^()\n{\n;\n}", "private void ^()\n{\n;\n}", "internal void ^()\n{\n;\n}", "protected void ^()\n{\n;\n}",
               "public ^{ get; set; }", "private ^{ get; set; }", "internal ^{ get; set; }", "protected ^{ get; set; }"
               };

        public DynamicCollection(AutocompleteMenu menu, FastColoredTextBox tb)
        {
            this.menu = menu;
            this.tb = tb;
            
        }

        public IEnumerator<AutocompleteItem> GetEnumerator()
        {
            foreach (var item in snippets)
                yield return new SnippetAutocompleteItem(item);
            foreach (var item in declarationSnippets)
                yield return new DeclarationSnippet(item);

            foreach (var item in keywords)
                yield return new AutocompleteItem(item);

            //get the variables from context also
            var variables = SPCoderForm.ScriptStateCSharp.Variables.Select(m => m.Name).ToList();
            foreach (var item in variables)
                yield return new AutocompleteItem(item);

            //IList<AutocompleteItem> items = new List<AutocompleteItem>();
            //get current fragment of the text
            var text = menu.Fragment.Text;
            if (text.Contains(' '))
            {
                text = text.Substring(text.LastIndexOf(' ')).Trim();
            }

            //get the parameter from the function call
            if (text.Contains('('))
            {
                text = text.Substring(text.LastIndexOf('(') + 1).Trim();
            }

            //extract class name (part before dot)
           var parts = text.Split('.');
            if (parts.Length < 2)
                yield break;
            var myVar = parts[parts.Length - 2];

            //Samo prvi se gleda da li je u context-u!!!
            if (parts.Length <= 2)
            {
                var contextVariable = SPCoderForm.ScriptStateCSharp.GetVariable(myVar);
                
                if (contextVariable != null)  
                {
                    object variable = ((Microsoft.CodeAnalysis.Scripting.ScriptVariable)contextVariable).Value;
                    IList<string> all = SPCoderUtils.GetPropertiesAndMethods(variable, myVar, SPCoderForm.MainForm.PutExtensionMethodsToAutocomplete);

                    foreach (string propMeth in all)
                    {
                        yield return new SPCoderMethodAutocompleteItem(propMeth)
                        {
                            ToolTipTitle = propMeth
                        };
                    }
                }
                else
                {
                    //check if it was class
                    var type = FindTypeByName(myVar);
                    if (type == null)
                        yield break;

                    if (type.IsEnum)
                    {
                        var enumValues = type.GetEnumValues();
                        foreach (var pi in enumValues)
                            yield return new SPCoderMethodAutocompleteItem(pi.ToString())
                            {
                                ToolTipTitle = pi.ToString()
                            };
                    }

                    //return static methods of the class
                    foreach (var meth in type.GetMethods().AsEnumerable().Select(mi => new { Name = mi.Name, Params = mi.GetParameters(), ReturnType = mi.ReturnType.Name }).Distinct())
                        yield return new SPCoderMethodAutocompleteItem(meth.Name + SPCoderUtils.GetSignature(meth.Params))
                        {
                            ToolTipTitle = meth.ReturnType + " " + meth.Name + SPCoderUtils.GetSignature(meth.Params)
                        };

                    //return static properties of the class
                    foreach (var pi in type.GetProperties())
                        yield return new SPCoderMethodAutocompleteItem(pi.Name)
                        {
                            ToolTipTitle = pi.PropertyType.Name + " " + pi.Name
                        };
                }
            }
            else
            {
                //Here try to find properties/methods from expression
                IList<string> all = SPCoderUtils.GetPropertiesAndMethods(parts, SPCoderForm.MainForm.PutExtensionMethodsToAutocomplete);
                if (all.Count > 0)
                {
                    foreach (string propMeth in all)
                    {
                        yield return new SPCoderMethodAutocompleteItem(propMeth)
                        {
                            ToolTipTitle = propMeth
                        };
                    }
                }
                else
                    foreach (var variable in SPCoderForm.MainForm.MyContext.GetContext.Keys)
                    {
                        yield return new SPCoderMethodAutocompleteItem(variable)
                        {
                            ToolTipTitle = variable
                        };
                    }
            }
        }

        Type FindTypeByName(string name)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            //Type type = null;
            foreach (var a in assemblies)
            {

                try
                {
                    foreach (var t in a.GetTypes())
                        if (t.Name == name)
                        {
                            return t;
                        }
                }
                catch (Exception)
                {
                    //throw;
                }
            }

            return null;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    
    /// <summary>
    /// This item appears when any part of snippet text is typed
    /// </summary>
    class DeclarationSnippet : SnippetAutocompleteItem
    {
        public DeclarationSnippet(string snippet)
            : base(snippet)
        {
        }

        public override CompareResult Compare(string fragmentText)
        {
            var pattern = Regex.Escape(fragmentText);
            if (Regex.IsMatch(Text, "\\b" + pattern, RegexOptions.IgnoreCase))
                return CompareResult.Visible;
            return CompareResult.Hidden;
        }
    }

    class SPCoderMethodAutocompleteItem : MethodAutocompleteItem
    {
        public SPCoderMethodAutocompleteItem(string snippet)
            : base(snippet)
        {
        }

        public override string GetTextForReplace()
        {
            return base.GetTextForReplace();
            /*string rez = base.GetTextForReplace();
            Match match = Regex.Match(rez, @"\((.+?)\)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                
                string[] pars = match.Groups[1].Value.Trim().Split(',');
                foreach (string p in pars)
                {
                    //rez = rez.Replace(
                    //println(p);
                    if (p.Trim().StartsWith("Int32") || p.Trim().StartsWith("Double") || p.Trim().StartsWith("Float"))
                    {
                        rez = rez.Replace(p, " 0");
                    }
                    else
                        if (p.Trim().StartsWith("String"))
                    {
                        rez = rez.Replace(p, " \"\"");
                    }
                    else
                        if (p.Trim().StartsWith("Char"))
                    {
                        rez = rez.Replace(p, " \'\'");
                    }
                    else
                    if (p.Trim().StartsWith("Boolean"))
                    {
                        rez = rez.Replace(p, " false");
                    }
                }
            }
            return rez;
            */
            //return base.GetTextForReplace();
        }
    }

    

    /// <summary>
    /// Divides numbers and words: "123AND456" -> "123 AND 456"
    /// Or "i=2" -> "i = 2"
    /// </summary>
    class InsertSpaceSnippet : AutocompleteItem
    {
        string pattern;

        public InsertSpaceSnippet(string pattern)
            : base("")
        {
            this.pattern = pattern;
        }

        public InsertSpaceSnippet()
            : this(@"^(\d+)([a-zA-Z_]+)(\d*)$")
        {
        }
        
        public override CompareResult Compare(string fragmentText)
        {
            if (Regex.IsMatch(fragmentText, pattern))
            {
                Text = InsertSpaces(fragmentText);
                if (Text != fragmentText)
                    return CompareResult.Visible;
            }
            return CompareResult.Hidden;
        }

        public string InsertSpaces(string fragment)
        {
            var m = Regex.Match(fragment, pattern);
            if (m == null)
                return fragment;
            if (m.Groups[1].Value == "" && m.Groups[3].Value == "")
                return fragment;
            return (m.Groups[1].Value + " " + m.Groups[2].Value + " " + m.Groups[3].Value).Trim();
        }

        public override string ToolTipTitle
        {
            get
            {
                return Text;
            }
        }
    }

    /// <summary>
    /// Inerts line break after '}'
    /// </summary>
    class InsertEnterSnippet : AutocompleteItem
    {
        Place enterPlace = Place.Empty;

        public InsertEnterSnippet()
            : base("[Line break]")
        {
        }

        public override CompareResult Compare(string fragmentText)
        {
            var r = Parent.Fragment.Clone();
            while (r.Start.iChar > 0)
            {
                if (r.CharBeforeStart == '}')
                {
                    enterPlace = r.Start;
                    return CompareResult.Visible;
                }

                r.GoLeftThroughFolded();
            }

            return CompareResult.Hidden;
        }

        public override string GetTextForReplace()
        {
            //extend range
            Range r = Parent.Fragment;
            Place end = r.End;
            r.Start = enterPlace;
            r.End = r.End;
            //insert line break
            return Environment.NewLine + r.Text;
        }

        public override void OnSelected(AutocompleteMenu popupMenu, SelectedEventArgs e)
        {
            base.OnSelected(popupMenu, e);
            if (Parent.Fragment.tb.AutoIndent)
                Parent.Fragment.tb.DoAutoIndent();
        }

        public override string ToolTipTitle
        {
            get
            {
                return "Insert line break after '}'";
            }
        }
    }

    public class InvisibleCharsRenderer : Style
    {
        Pen pen;

        public InvisibleCharsRenderer(Pen pen)
        {
            this.pen = pen;
        }

        public override void Draw(Graphics gr, Point position, Range range)
        {
            var tb = range.tb;
            using (Brush brush = new SolidBrush(pen.Color))
                foreach (var place in range)
                {
                    switch (tb[place].c)
                    {
                        case ' ':
                            var point = tb.PlaceToPoint(place);
                            point.Offset(tb.CharWidth / 2, tb.CharHeight / 2);
                            gr.DrawLine(pen, point.X, point.Y, point.X + 1, point.Y);
                            break;
                    }

                    if (tb[place.iLine].Count - 1 == place.iChar)
                    {
                        var point = tb.PlaceToPoint(place);
                        point.Offset(tb.CharWidth, 0);
                        gr.DrawString("¶", tb.Font, brush, point);
                    }
                }
        }
    }
}