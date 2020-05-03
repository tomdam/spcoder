using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using SPCoder;
using SPCoder.Context;
using WeifenLuo.WinFormsUI.Docking;
using System.Text.RegularExpressions;
using FastColoredTextBoxNS;
using SPCoder.Utils;
using System.Collections.Generic;
using SPCoder.Core.Utils;

namespace SPCoder.Describer
{
    /// <summary>
    /// 
    /// </summary>
    /// <author>Damjan Tomic</author>
    public partial class DescriberForm : DockContent
    {
        public static int DescriberCount = 0;

        private int myDescriberCount;
        protected DescriberPropertiesData describerPropertiesData;
        protected ObjectDescription objectDescription = new ObjectDescription();

        private bool addedToContext = false;

        TextStyle blueStyle = new TextStyle(Brushes.Blue, null, FontStyle.Underline);

        #region Fctb 
        //range.SetStyle(GreenStyle, @"//.*$", RegexOptions.Multiline);
        #endregion

        public DescriberForm()
        {
            InitializeComponent();
            myDescriberCount = ++DescriberCount;
            
            describerPropertiesData = DescriberUtils.GetValuesFromConfig();
            objectDescription.DescribedValueMaxLength = describerPropertiesData.MaxDisplayedSize;

            //SPCoderForm.ironPythonEngine.SetVariable(GetMyNameForScriptContext(), this);
            //SPCoderForm.MainForm.MyContext.AddItem(new ContextItem { Data = this, Name = GetMyNameForScriptContext(), Type = this.GetType().ToString() });

            cb_wordwrap.Checked = describerPropertiesData.WordWrap;

            if (describerPropertiesData.WordWrap)
            {
                fctb.WordWrap = true;
                fctb.WordWrapMode = WordWrapMode.WordWrapControlWidth;
            }

            LoadSettings();
            cmbSortByWhat.SelectedIndex = 0;
        }

        string msdnLinkFormat = "";

        private void LoadSettings()
        {
            try
            {
                if (SPCoderSettings.Settings[SPCoderConstants.SP_SETTINGS_DESCRIBER] != null)
                {
                    var gridSettings = (Dictionary<string, object>)SPCoderSettings.Settings[SPCoderConstants.SP_SETTINGS_DESCRIBER];
                    if (gridSettings[SPCoderConstants.SP_SETTINGS_EXPRESSION] != null)
                    {
                        string expression = gridSettings[SPCoderConstants.SP_SETTINGS_EXPRESSION].ToString();
                        this.txtDescribeObject.Text = expression;
                    }

                    if (gridSettings[SPCoderConstants.SP_SETTINGS_MAX_CHARACTERS] != null)
                    {
                        string value = gridSettings[SPCoderConstants.SP_SETTINGS_MAX_CHARACTERS].ToString();
                        //objectDescription.DescribedValueMaxLength = Int32.Parse(value);
                        txtMaxDisplaySize.Text = value;
                    }

                    if (gridSettings[SPCoderConstants.SP_SETTINGS_EDITABLE] != null)
                    {
                        string value = gridSettings[SPCoderConstants.SP_SETTINGS_EDITABLE].ToString();
                        bool editable = bool.Parse(value);
                        this.chkIsEditable.Checked = editable;
                        fctb.ReadOnly = !editable;
                    }

                    if (gridSettings[SPCoderConstants.SP_SETTINGS_WORD_WRAP] != null)
                    {
                        string value = gridSettings[SPCoderConstants.SP_SETTINGS_WORD_WRAP].ToString();
                        bool wordwrap = bool.Parse(value);
                        //this.cb_wordwrap_CheckedChanged
                        this.cb_wordwrap.Checked = wordwrap;
                        if (wordwrap)
                        {
                            fctb.WordWrap = true;
                            fctb.WordWrapMode = WordWrapMode.WordWrapControlWidth;
                        }
                        else
                        {
                            fctb.WordWrap = false;
                        }
                    }

                    if (gridSettings[SPCoderConstants.SP_SETTINGS_LINK_FORMAT] != null)
                    {
                        string value = gridSettings[SPCoderConstants.SP_SETTINGS_LINK_FORMAT].ToString();
                        msdnLinkFormat = value;
                    }
                }
            }
            catch (Exception exc)
            {
                SPCoderLogging.Logger.Error("Error while loading describer properties from settings.json", exc);
            }
        }

        private void SetExpression(string expression)
        {
            if (SPCoderSettings.Settings[SPCoderConstants.SP_SETTINGS_DESCRIBER] != null)
            {
                var gridSettings = (Dictionary<string, object>)SPCoderSettings.Settings[SPCoderConstants.SP_SETTINGS_DESCRIBER];
                this.txtDescribeObject.Text = expression;
                gridSettings[SPCoderConstants.SP_SETTINGS_EXPRESSION] = expression;
            }
        }



        bool CharIsHyperlink(Place place)
        {
            var mask = fctb.GetStyleIndexMask(new Style[] { blueStyle });
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
            var p = fctb.PointToPlace(e.Location);
            if (CharIsHyperlink(p))
            {
                var url = fctb.GetRange(p, p).GetFragment(@"[\S]").Text;
                Process.Start(url);
            }
        }


        public DescriberForm(object item) : this()
        {
            objectDescription.DescribedObject = item;
            Describe();
        }

        public DescriberForm(ContextItem item): this(item.Data)
        {
            this.txtDescribeObject.Text = item.Name;
        }

        public void Describe(object item)
        {
            if (!addedToContext)
            {
                SPCoderForm.MainForm.MyContext.AddOrUpdateItem(new ContextItem { Data = this, Name = GetMyNameForScriptContext(), Type = this.GetType().ToString() });
                addedToContext = true;
            }
            
            if (item is ContextItem)
            {
                ContextItem ci = (ContextItem)item;
                this.txtDescribeObject.Text = ci.Name;
                objectDescription.DescribedObject = ci.Data;
            }
            else
            {
                objectDescription.DescribedObject = item;
            }

            
            Describe();
        }

        public void Describe()
        {            
            string description = objectDescription.GetObjectDescription();
            this.fctb.Clear();
            this.fctb.Text = description;

            if (objectDescription.DescribedObject != null)
            {
                string link = GenerateLinkToMsdn(objectDescription.DescribedObject.GetType().FullName);
                linkMsdn.Links[0].LinkData = link;
                AddBreadCrumbLinks();
            }
        }

        private void rtDescriber_DoubleClick(object sender, EventArgs e)
        {
            //int charPosition = fctb.GetCharIndexFromPosition(((MouseEventArgs)e).Location);
            //int lineNumber = fctb.GetLineFromCharIndex(charPosition);
            ////check to see if the text that is clicked is after the third line (we cannot describe "Value" or "Type")
            //if (lineNumber > 2)
            //{
            //    this.txtDescribeObject.Text += "." + this.fctb.SelectedText.Trim();
            //}
        }

        private void btnDescribe_Click(object sender, EventArgs e)
        {
            if (!addedToContext)
            {
                SPCoderForm.MainForm.MyContext.AddOrUpdateItem(new ContextItem { Data = this, Name = GetMyNameForScriptContext(), Type = this.GetType().ToString() });
                addedToContext = true;
            }
            string sourceObject = this.txtDescribeObject.Text.Trim();
            SetExpression(sourceObject);
            CallDoLocalDescribe(sourceObject);
        }

        private void CallDoLocalDescribe(string sourceCodeForObject)
        {
            if (!string.IsNullOrEmpty(sourceCodeForObject))
            {
                DoLocalDescribe(sourceCodeForObject);
                if (!sourceCodeForObject.EndsWith("]"))
                {
                    numChangingFromCode = true;
                    numCounter.Value = 0;
                    numChangingFromCode = false;
                }
                //linkMsdn.Links[0]                
            }            
        }

        private void DoLocalDescribe(string sourceCodeForObject)
        {
            try
            {
                //SPCoderForm.ironPythonEngine.SetVariable("obj", objectDescription.DescribedObject);

                //SPCoderForm.MainForm.MyContext.AddOrUpdateItem(new ContextItem { Data = objectDescription.DescribedObject, Name = "obj", Type = objectDescription.DescribedObject.GetType().ToString() });

                string code = GetMyNameForScriptContext() + ".Describe(" + sourceCodeForObject + ")";
                //SPCoderForm.ironPythonEngine.Execute(code);
                SPCoderForm.MainForm.ExecuteScriptCSharp(code);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            } 
        }


        private void AddBreadCrumbLinks()
        {
            flowLayoutPanel1.Controls.Clear();
            string text = this.txtDescribeObject.Text;
            string[] parts = text.Split('.');
            int ind = 0;
            foreach (string part in parts)
            {
                LinkLabel label = new LinkLabel();
                label.Text = part;                
                label.AutoSize = true;
                label.Tag = string.Join(".", parts, 0, ind + 1);
                label.Click += new EventHandler(label_Click);                
                flowLayoutPanel1.Controls.Add(label);
                ind++;
            }
        }

        void label_Click(object sender, EventArgs e)
        {
            LinkLabel label = (LinkLabel) sender;
            string codeToDescribe = label.Tag.ToString();
            this.txtDescribeObject.Text = codeToDescribe;
            //DoLocalDescribe(codeToDescribe);
            SetExpression(codeToDescribe);
            CallDoLocalDescribe(codeToDescribe);
        }

        
        private string GetMyNameForScriptContext()
        {
            return "describerForm_" + myDescriberCount;
        }

        private void btnDoSortAsc_Click(object sender, EventArgs e)
        {
            objectDescription.Sort("Asc",cmbSortByWhat.SelectedItem.ToString());
            Describe();
        }

        private void btnDoSortDesc_Click(object sender, EventArgs e)
        {
            objectDescription.Sort("Desc", cmbSortByWhat.SelectedItem.ToString());
            Describe();
        }     

        private void chkIsEditable_CheckedChanged(object sender, EventArgs e)
        {
            bool isChecked = this.chkIsEditable.Checked;
            //describerPropertiesData.IsEditable = isChecked;
            var gridSettings = (Dictionary<string, object>)SPCoderSettings.Settings[SPCoderConstants.SP_SETTINGS_DESCRIBER];
            gridSettings[SPCoderConstants.SP_SETTINGS_EDITABLE] = isChecked;

            fctb.ReadOnly = !isChecked;
        }

        private void txtMaxDisplaySize_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int enteredValue = Convert.ToInt32(this.txtMaxDisplaySize.Text);
                //describerPropertiesData.MaxDisplayedSize = enteredValue;
                var gridSettings = (Dictionary<string, object>)SPCoderSettings.Settings[SPCoderConstants.SP_SETTINGS_DESCRIBER];
                gridSettings[SPCoderConstants.SP_SETTINGS_MAX_CHARACTERS] = enteredValue;
                objectDescription.DescribedValueMaxLength = enteredValue;
            }
            catch (Exception)
            {
                MessageBox.Show("Entered value was not valid integer.");
            }
        }

        private bool numChangingFromCode = false;

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (numChangingFromCode)
            {
                return;
            }
            string sourceObject = this.txtDescribeObject.Text.Trim();
            if (sourceObject.EndsWith("]") && sourceObject.Contains("["))
            {
                sourceObject = sourceObject.Substring(0, sourceObject.LastIndexOf('['));
            }
            sourceObject += "[" + numCounter.Value + "]";
            this.txtDescribeObject.Text = sourceObject;
            
            CallDoLocalDescribe(sourceObject);
        }        

        private string GenerateLinkToMsdn(string type)
        {
            //string url = string.Format(describerPropertiesData.MsdnLinkFormat, type.ToLower()); //"http://msdn.microsoft.com/en-us/library/" + type.ToLower() + ".aspx";
            string url = string.Format(msdnLinkFormat, type.ToLower());
            return url;
        }

        private void linkMsdn_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(linkMsdn.Links[0].LinkData.ToString());
        }

        TextStyle infoStyle = new TextStyle(Brushes.Blue, Brushes.WhiteSmoke, FontStyle.Bold);

        private void fctb_TextChangedDelayed(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
        {
            Range range = (sender as FastColoredTextBox).Range;
            range.SetStyle(infoStyle, @"^\b.*?[:]", RegexOptions.Multiline);

            e.ChangedRange.ClearStyle(blueStyle);
            e.ChangedRange.SetStyle(blueStyle, @"(http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?");
        }

        private void fctb_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Place p = fctb.PointToPlace(e.Location);
            int fromX = p.iChar;
            int toX = p.iChar;

            for (int i = p.iChar; i < fctb.TextSource[p.iLine].Count; i++)
            {
                char c = fctb.TextSource[p.iLine][i].c;
                if (char.IsLetterOrDigit(c) || c == '_')
                    toX = i + 1;
                else
                    break;
            }

            for (int i = p.iChar - 1; i >= 0; i--)
            {
                char c = fctb.TextSource[p.iLine][i].c;
                if (char.IsLetterOrDigit(c) || c == '_')
                    fromX = i;
                else
                    break;
            }

            fctb.Selection.Start = new Place(toX, p.iLine);
            fctb.Selection.End = new Place(fromX, p.iLine);

            string sel = this.fctb.SelectedText.Trim();
            if (!string.IsNullOrEmpty(sel))
            {
                this.txtDescribeObject.Text += "." + sel;
            }

        }

        private void cb_wordwrap_CheckedChanged(object sender, EventArgs e)
        {
            bool isChecked = this.cb_wordwrap.Checked;
            //describerPropertiesData.WordWrap = isChecked;
            var gridSettings = (Dictionary<string, object>)SPCoderSettings.Settings[SPCoderConstants.SP_SETTINGS_DESCRIBER];
            gridSettings[SPCoderConstants.SP_SETTINGS_WORD_WRAP] = isChecked;

            if (describerPropertiesData.WordWrap)
            {
                fctb.WordWrap = true;
                fctb.WordWrapMode = WordWrapMode.WordWrapControlWidth;
            }
            else
            {
                fctb.WordWrap = false;
            }
        }

        private void txtDescribeObject_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtDescribeObject_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                btnDescribe_Click(sender, null);
            }
            //Autocomplete?
        }

        private void txtDescribeObject_VisibleChanged(object sender, EventArgs e)
        {
            txtDescribeObject.Focus();
        }
    }
}