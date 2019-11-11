using SPCoder.Context;
using System;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace SPCoder.Windows
{
    public partial class Properties : DockContent
    {
        public static int PropertiesCount = 0;
        private int myPropertiesCount;
        private bool addedToContext = false;
        public Properties()
        {
            myPropertiesCount = ++PropertiesCount;
            InitializeComponent();
            PgEditor = propertyGrid1;

        }

        public PropertyGrid PgEditor { get; private set; }

        private void btnDescribe_Click(object sender, EventArgs e)
        {
            //SPCoderForm.MainForm.ShowProperties
            if (!addedToContext)
            {
                SPCoderForm.MainForm.MyContext.AddOrUpdateItem(new ContextItem { Data = this, Name = GetMyNameForScriptContext(), Type = this.GetType().ToString() });
                addedToContext = true;
            }
            string sourceObject = this.txtPropertiesObject.Text.Trim();
            CallDoLocalShowProperties(sourceObject);
        }

        private string GetMyNameForScriptContext()
        {
            return "propertiesForm_" + myPropertiesCount;
        }

        private bool numChangingFromCode = false;
        private void CallDoLocalShowProperties(string sourceCodeForObject)
        {
            if (!string.IsNullOrEmpty(sourceCodeForObject))
            {
                DoLocalShowProperties(sourceCodeForObject);
                if (!sourceCodeForObject.EndsWith("]"))
                {
                    numChangingFromCode = true;
                    numCounter.Value = 0;
                    numChangingFromCode = false;
                }
            }
        }

        private void DoLocalShowProperties(string sourceCodeForObject)
        {
            try
            {
                //"main.ShowProperties(" + sourceCodeForObject + ")";
                string code = GetMyNameForScriptContext() + ".ShowProperties(" + sourceCodeForObject + ")";
                SPCoderForm.MainForm.ExecuteScriptCSharp(code);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        public void ShowProperties(object item)
        {
            if (!addedToContext)
            {
                SPCoderForm.MainForm.MyContext.AddOrUpdateItem(new ContextItem { Data = this, Name = GetMyNameForScriptContext(), Type = this.GetType().ToString() });
                addedToContext = true;
            }
            
            {
                PgEditor.SelectedObject = item;
            }


            //ShowProperties();
        }

        private void numCounter_ValueChanged(object sender, EventArgs e)
        {
            if (numChangingFromCode)
            {
                return;
            }
            string sourceObject = this.txtPropertiesObject.Text.Trim();
            if (sourceObject.EndsWith("]") && sourceObject.Contains("["))
            {
                sourceObject = sourceObject.Substring(0, sourceObject.LastIndexOf('['));
            }
            sourceObject += "[" + numCounter.Value + "]";
            this.txtPropertiesObject.Text = sourceObject;

            CallDoLocalShowProperties(sourceObject);
        }
    }
}
