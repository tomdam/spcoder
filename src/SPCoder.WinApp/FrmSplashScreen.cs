using log4net.Config;
using System;
using System.Windows.Forms;

namespace SPCoder
{
    public partial class FrmSplashScreen : Form
    {
        SPCoderForm spCoderForm;

        public FrmSplashScreen()
        {
            InitializeComponent();
            //find version and append it to the label
            this.lblVersion.Text += GetType().Assembly.GetName().Version.ToString();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;

            //configure log.net
            XmlConfigurator.Configure();

            spCoderForm = new SPCoderForm(this);
            Application.DoEvents();
            spCoderForm.ShowDialog();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (progressBar1.Value < progressBar1.Maximum)
            {
                progressBar1.PerformStep();
                Application.DoEvents();
            }
            else
            {
                timer2.Enabled = false;
            }
        }

        public void turnOffTimer()
        {
            timer2.Enabled = false;
            progressBar1.Value = progressBar1.Maximum;
        }
    }
}