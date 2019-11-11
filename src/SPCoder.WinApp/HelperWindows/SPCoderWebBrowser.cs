using SPCoder.Core.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SPCoder.HelperWindows
{
    public partial class SPCoderWebBrowser : Form
    {
        public SPCoderWebBrowser()
        {
            InitializeComponent();
        }

        public OneParamDelegate BrowserNavigated { get; set; }

        private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            BrowserNavigated(e.Url);
        }

        public void Navigate(Uri url)
        {
            webBrowser1.Navigate(url);
        }        
    }
}
