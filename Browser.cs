using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace FB_API_Plugin
{
    internal partial class Browser : Form
    {
        public Browser()
        {
            InitializeComponent();
        }
        //ool nav = false;
        public string Url { get; set; }
        public string RedirectUrl { get; set; }

        public new void ShowDialog()
        {
            throw new NotImplementedException();
        }

        public string GetToken { get; set; }
        private void Browser_Load(object sender, EventArgs e)
        {

            webBrowser1.Navigate(Url);
            
            

            //WaitForPageLoad();
            //nav = true;
        }

        private bool Pageready { get; set; }

        //#region "Page Loading Functions"
        //public void WaitForPageLoad()
        //{
        //    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(PageWaiter);
        //    while (!pageready)
        //    {
        //        Application.DoEvents();
        //    }
        //    pageready = false;
        //}

        //private void PageWaiter(object sender, WebBrowserDocumentCompletedEventArgs e)
        //{
        //    if (webBrowser1.ReadyState == WebBrowserReadyState.Complete)
        //    {
        //        pageready = true;
        //        string url = webBrowser1.Url.ToString();
        //        string host = new Uri(RedirectUrl).Host;
        //        if (url.Contains(host))
        //        {
        //            Match mt = Regex.Match(url, "(?<=#access_token=).*(?=&expires_in)");
        //            if (mt.Success)
        //            {
        //                this.GetToken = mt.Groups[0].Value;
        //                this.DialogResult = System.Windows.Forms.DialogResult.OK;
        //            }
        //        }
        //       // webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(PageWaiter);
        //    }
        //}

        //#endregion

        private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            //if (nav == true)
            //{
            //    WaitForPageLoad();
            //}
        }

        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            //WaitForPageLoad();
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

            
            var url = webBrowser1.Url.ToString();
            var host = new Uri(RedirectUrl).Host;
 
            if (url.Contains(host))
            {
       
                var mt = Regex.Match(url, "(?<=#access_token=).*(?=&expires_in)");
                if (!mt.Success) return;
                GetToken = mt.Groups[0].Value;
                DialogResult = DialogResult.OK;
            }
            else if (url.Contains("developers.facebook.com/tools/explorer/callback"))
            {
                GetToken = "";
                DialogResult = DialogResult.OK;
            }

        }
    }
}
