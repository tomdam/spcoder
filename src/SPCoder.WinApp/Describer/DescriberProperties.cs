using System;
using System.Windows.Forms;

namespace SPCoder.Describer
{
    /// <summary>
    /// 
    /// </summary>
    /// <author>Damjan Tomic</author>
    public partial class DescriberProperties : Form
    {
        private DescriberPropertiesData describerPropertiesData;

        public DescriberProperties()
        {
            InitializeComponent();
        }
        
        public DescriberProperties(DescriberPropertiesData describerPropertiesData) : this()
        {
            this.describerPropertiesData = describerPropertiesData;
            this.txtMaxDisplaySize.Text = describerPropertiesData.MaxDisplayedSize.ToString();
            this.chkIsEditable.Checked = describerPropertiesData.IsEditable;
        }

        private void txtMaxDisplaySize_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int enteredValue = Convert.ToInt32(this.txtMaxDisplaySize.Text);
                describerPropertiesData.MaxDisplayedSize = enteredValue;
            }
            catch (Exception)
            {
                MessageBox.Show("Entered value is not valid integer.");
            }
        }

        private void chkIsEditable_CheckedChanged(object sender, EventArgs e)
        {
            bool isChecked = this.chkIsEditable.Checked;
            describerPropertiesData.IsEditable = isChecked;
        }

    }
}