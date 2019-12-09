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

        string lang = "CSharp (custom highlighter)";

        //styles

        TextStyle RedErrorStyle = new TextStyle(null, Brushes.LightPink, FontStyle.Regular);

        TextStyle BlueStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);
        TextStyle BoldStyle = new TextStyle(null, null, FontStyle.Bold | FontStyle.Underline);
        TextStyle GrayStyle = new TextStyle(Brushes.Gray, null, FontStyle.Regular);
        TextStyle MagentaStyle = new TextStyle(Brushes.Magenta, null, FontStyle.Regular);
        TextStyle GreenStyle = new TextStyle(Brushes.Green, null, FontStyle.Italic);
        TextStyle BrownStyle = new TextStyle(Brushes.Brown, null, FontStyle.Italic);
        TextStyle MaroonStyle = new TextStyle(Brushes.Maroon, null, FontStyle.Regular);
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
            popupMenu.SearchPattern = @"[\w\.:=!<>]";
            popupMenu.AllowTabKey = true;
            //assign DynamicCollection as items source
            popupMenu.Items.SetAutocompleteItems(new DynamicCollection(popupMenu, fctb));
            
            //popupMenu.Items.MaximumSize = new System.Drawing.Size(300, 400);
            popupMenu.Items.Width = 300;
            popupMenu.Items.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowOnly;
            
            
            //fctb.OnTextChangedDelayed(fctb.Range);

            //fctb.DescriptionFile = pythonStyle;
            fctb.Language = Language.CSharp;
            //fctb.OnTextChangedDelayed(fctb.Range);
            fctb.CurrentLineColor = Color.FromArgb(200, 200, 255);

            fctb.CustomAction += fctb_CustomAction;
            
            fctb.HotkeysMapping.Add(Keys.F5, FCTBAction.CustomAction1);
            fctb.HotkeysMapping.Add(Keys.Control | Keys.S, FCTBAction.CustomAction2);
            fctb.HotkeysMapping.Add(Keys.Control | Keys.W, FCTBAction.CustomAction3);


            //fctb.AutoIndentNeeded += new EventHandler<AutoIndentEventArgs>(fctb_AutoIndentNeeded);
            fctb.ClearStylesBuffer();
            fctb.AddStyle(RedErrorStyle);
            fctb.AddStyle(LinkStyle);
            fctb.AddStyle(SameWordsStyle);
            fctb.AddStyle(MaroonStyle);
            fctb.AddStyle(BrownStyle);
            fctb.AddStyle(GreenStyle);
            fctb.AddStyle(MagentaStyle);
            fctb.AddStyle(GrayStyle);
            fctb.AddStyle(BoldStyle);
            fctb.AddStyle(BlueStyle);

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
        { }

        private void fctb_TextChanged_delayed2(object sender, TextChangedEventArgs e)
        {
            //switch (lang)
            //{
            //    case "CSharp (custom highlighter)":
            //        CSharpSyntaxHighlight(e);//custom highlighting
            //        break;
            //    default:
            //        break;//for highlighting of other languages, we use built-in FastColoredTextBox highlighter
            //}

            
            string trimmedText = fctb.Text.Trim();
            if (trimmedText.StartsWith("<?xml"))
            {
                fctb.Language = Language.XML;

                fctb.ClearStylesBuffer();
                fctb.Range.ClearStyle(StyleIndex.All);
                InitStylesPriority();
                fctb.AutoIndentNeeded -= fctb_AutoIndentNeeded;

                fctb.OnSyntaxHighlight(new TextChangedEventArgs(fctb.Range));
            } else
            //
            if (trimmedText.StartsWith("<!DOCTYPE html") || trimmedText.StartsWith("<html") || trimmedText.StartsWith("<body"))
            {
                fctb.Language = Language.HTML;

                fctb.ClearStylesBuffer();
                fctb.Range.ClearStyle(StyleIndex.All);
                InitStylesPriority();
                fctb.AutoIndentNeeded -= fctb_AutoIndentNeeded;

                fctb.OnSyntaxHighlight(new TextChangedEventArgs(fctb.Range));
            }

            if (!this.Text.EndsWith("*"))
            {
                this.Text += "*";
            }

            /*if (errorRange != null)
            {
                e.ChangedRange.ClearStyle(RedErrorStyle);
                errorRange.ClearStyle(RedErrorStyle);
                errorRange = null;
            }*/



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

        private void CSharpSyntaxHighlight(TextChangedEventArgs e)
        {
            fctb.LeftBracket = '(';
            fctb.RightBracket = ')';
            fctb.LeftBracket2 = '\x0';
            fctb.RightBracket2 = '\x0';
            //clear style of changed range
            e.ChangedRange.ClearStyle(BlueStyle, BoldStyle, GrayStyle, MagentaStyle, GreenStyle, BrownStyle, RedErrorStyle);

            //string highlighting
            e.ChangedRange.SetStyle(BrownStyle, @"""""|@""""|''|@"".*?""|(?<!@)(?<range>"".*?[^\\]"")|'.*?[^\\]'");
            //comment highlighting
            e.ChangedRange.SetStyle(GreenStyle, @"//.*$", RegexOptions.Multiline);
            e.ChangedRange.SetStyle(GreenStyle, @"(/\*.*?\*/)|(/\*.*)", RegexOptions.Singleline);
            e.ChangedRange.SetStyle(GreenStyle, @"(/\*.*?\*/)|(.*\*/)", RegexOptions.Singleline | RegexOptions.RightToLeft);
            //number highlighting
            e.ChangedRange.SetStyle(MagentaStyle, @"\b\d+[\.]?\d*([eE]\-?\d+)?[lLdDfF]?\b|\b0x[a-fA-F\d]+\b");
            //attribute highlighting
            e.ChangedRange.SetStyle(GrayStyle, @"^\s*(?<range>\[.+?\])\s*$", RegexOptions.Multiline);
            //class name highlighting
            e.ChangedRange.SetStyle(BoldStyle, @"\b(class|struct|enum|interface)\s+(?<range>\w+?)\b");
            //keyword highlighting
            e.ChangedRange.SetStyle(BlueStyle, @"\b(abstract|as|base|bool|break|byte|case|catch|char|checked|class|const|continue|decimal|default|delegate|do|double|else|enum|event|explicit|extern|false|finally|fixed|float|for|foreach|goto|if|implicit|in|int|interface|internal|is|lock|long|namespace|new|null|object|operator|out|override|params|private|protected|public|readonly|ref|return|sbyte|sealed|short|sizeof|stackalloc|static|string|struct|switch|this|throw|true|try|typeof|uint|ulong|unchecked|unsafe|ushort|using|virtual|void|volatile|while|add|alias|ascending|descending|dynamic|from|get|global|group|into|join|let|orderby|partial|remove|select|set|value|var|where|yield)\b|#region\b|#endregion\b");

            //clear folding markers
            e.ChangedRange.ClearFoldingMarkers();

            //set folding markers
            e.ChangedRange.SetFoldingMarkers("{", "}");//allow to collapse brackets block
            e.ChangedRange.SetFoldingMarkers(@"#region\b", @"#endregion\b");//allow to collapse #region blocks
            e.ChangedRange.SetFoldingMarkers(@"/\*", @"\*/");//allow to collapse comment block
        }
        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fctb.ShowFindDialog();
        }
        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fctb.ShowReplaceDialog();
        }
        
        private void collapseSelectedBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fctb.CollapseBlock(fctb.Selection.Start.iLine, fctb.Selection.End.iLine);
        }

        private void collapseAllregionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //this example shows how to collapse all #region blocks (C#)
            if (!lang.StartsWith("CSharp")) return;
            for (int iLine = 0; iLine < fctb.LinesCount; iLine++)
            {
                if (fctb[iLine].FoldingStartMarker == @"#region\b")//marker @"#region\b" was used in SetFoldingMarkers()
                    fctb.CollapseFoldingBlock(iLine);
            }
        }

        private void exapndAllregionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //this example shows how to expand all #region blocks (C#)
            if (!lang.StartsWith("CSharp")) return;
            for (int iLine = 0; iLine < fctb.LinesCount; iLine++)
            {
                if (fctb[iLine].FoldingStartMarker == @"#region\b")//marker @"#region\b" was used in SetFoldingMarkers()
                    fctb.ExpandFoldedBlock(iLine);
            }
        }

        private void increaseIndentSiftTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fctb.IncreaseIndent();
        }

        private void decreaseIndentTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fctb.DecreaseIndent();
        }

        private void hTMLToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "HTML with <PRE> tag|*.html|HTML without <PRE> tag|*.html";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string html = "";

                if (sfd.FilterIndex == 1)
                {
                    html = fctb.Html;
                }
                if (sfd.FilterIndex == 2)
                {

                    ExportToHTML exporter = new ExportToHTML();
                    exporter.UseBr = true;
                    exporter.UseNbsp = false;
                    exporter.UseForwardNbsp = true;
                    exporter.UseStyleTag = true;
                    html = exporter.GetHtml(fctb);
                }
                File.WriteAllText(sfd.FileName, html);
            }
        }

        DateTime lastNavigatedDateTime = DateTime.Now;
        private void fctb_SelectionChangedDelayed(object sender, EventArgs e)
        {
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

        private void goForwardCtrlShiftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fctb.NavigateForward();
        }

        private void goBackwardCtrlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fctb.NavigateBackward();
        }

        private void autoIndentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fctb.DoAutoIndent();
        }

        const int maxBracketSearchIterations = 2000;

        void GoLeftBracket(FastColoredTextBox tb, char leftBracket, char rightBracket)
        {
            Range range = tb.Selection.Clone();//need to clone because we will move caret
            int counter = 0;
            int maxIterations = maxBracketSearchIterations;
            while (range.GoLeftThroughFolded())//move caret left
            {
                if (range.CharAfterStart == leftBracket) counter++;
                if (range.CharAfterStart == rightBracket) counter--;
                if (counter == 1)
                {
                    //found
                    tb.Selection.Start = range.Start;
                    tb.DoSelectionVisible();
                    break;
                }
                //
                maxIterations--;
                if (maxIterations <= 0) break;
            }
            tb.Invalidate();
        }

        void GoRightBracket(FastColoredTextBox tb, char leftBracket, char rightBracket)
        {
            var range = tb.Selection.Clone();//need clone because we will move caret
            int counter = 0;
            int maxIterations = maxBracketSearchIterations;
            do
            {
                if (range.CharAfterStart == leftBracket) counter++;
                if (range.CharAfterStart == rightBracket) counter--;
                if (counter == -1)
                {
                    //found
                    tb.Selection.Start = range.Start;
                    tb.Selection.GoRightThroughFolded();
                    tb.DoSelectionVisible();
                    break;
                }
                //
                maxIterations--;
                if (maxIterations <= 0) break;
            } while (range.GoRightThroughFolded());//move caret right

            tb.Invalidate();
        }

        private void goLeftBracketToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GoLeftBracket(fctb, '{', '}');
        }

        private void goRightBracketToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GoRightBracket(fctb, '{', '}');
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
            if (Regex.IsMatch(args.PrevLineText, @"^\s*(if|for|foreach|while|[\}\s]*else)\b[^{]*$"))
                if (!Regex.IsMatch(args.PrevLineText, @"(;\s*$)|(;\s*//)"))//operator is unclosed
                {
                    args.Shift = args.TabLength;
                    return;
                }
        }

        private void miPrint_Click(object sender, EventArgs e)
        {
            fctb.Print(new PrintDialogSettings() { ShowPrintPreviewDialog = true });
        }

       
        private void setSelectedAsReadonlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fctb.Selection.ReadOnly = true;
        }

        private void setSelectedAsWritableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fctb.Selection.ReadOnly = false;
        }

        private void startStopMacroRecordingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fctb.MacrosManager.IsRecording = !fctb.MacrosManager.IsRecording;
        }

        private void executeMacroToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fctb.MacrosManager.ExecuteMacros();
        }

        private void changeHotkeysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new HotkeysEditorForm(fctb.HotkeysMapping);
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                fctb.HotkeysMapping = form.GetHotkeys();
        }

        private void rTFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "RTF|*.rtf";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string rtf = fctb.Rtf;
                File.WriteAllText(sfd.FileName, rtf);
            }
        }

        
        private void commentSelectedLinesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fctb.InsertLinePrefix(fctb.CommentPrefix);
        }

        private void uncommentSelectedLinesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fctb.RemoveLinePrefix(fctb.CommentPrefix);
        }

        void fctb_CustomAction(object sender, CustomActionEventArgs e)
        {
            if (e.Action == FCTBAction.CustomAction1)
            {
                this.ExecuteSelectionCSharp();
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

        public string Source
        {
            get
            {
                return fctb.Text;
            }
            set
            {
                fctb.Text = value;
                //remove '*' from the end of the title
                this.Text = this.Text.TrimEnd('*');
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

        public void HighlightInvisibleChars(Range range)
        {             
            range.ClearStyle(invisibleCharsStyle);
            if (SPCoderForm.MainForm.ShouldHighlightInvisibleChars())
                range.SetStyle(invisibleCharsStyle, @".$|.\r\n|\s");
        }

        public void ExecuteSelectionCSharp(bool async = false)
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
                //Check if the output window should be cleared
                

                if (!async)
                {
                    SPCoderForm.MainForm.ExecuteScriptCSharp(text);
                }
                else
                {
                    //SPCoderForm.MainForm.ExecuteScriptAsync(text);
                    SPCoderForm.MainForm.ExecuteScriptCSharp(text);
                }
            }
            catch (Exception ex)
            {
                SPCoderForm.MainForm.LogException(ex);
                HighlightErrorLine(ex.Message, text);
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
                    saveFileDialog.Filter = "CSharp files|*.cs|CSharp script files|*.csx|All files|*.*";

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

            //IList<AutocompleteItem> items = new List<AutocompleteItem>();
            //get current fragment of the text
            var text = menu.Fragment.Text;
            if (text.Contains(' '))
            {
                text = text.Substring(text.LastIndexOf(' ')).Trim();
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
                    IList<string> all = SPCoderUtils.GetPropertiesAndMethods(variable, myVar);

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
                IList<string> all = SPCoderUtils.GetPropertiesAndMethods(parts);
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
                catch (Exception exc)
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
            string rez = base.GetTextForReplace();
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

                    //if (tb[place.iLine].Count - 1 == place.iChar)
                    //{
                    //    var point = tb.PlaceToPoint(place);
                    //    point.Offset(tb.CharWidth, 0);
                    //    gr.DrawString("¶", tb.Font, brush, point);
                    //}
                }
        }
    }
}