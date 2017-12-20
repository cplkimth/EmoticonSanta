using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EmoticonSanta.Properties;
using NHotkey;
using NHotkey.WindowsForms;

namespace EmoticonSanta.Controls
{
    public partial class BrowserForm : Form
    {
        private const string HomeUrl = "https://e.kakao.com/";

        public BrowserForm()
        {
            InitializeComponent();

            HotkeyManager.Current.AddOrReplace("HotKey_Home", Keys.Alt | Keys.Home, HotKey_Home);
            HotkeyManager.Current.AddOrReplace("HotKey_Back", Keys.Alt | Keys.Back, HotKey_Back);
            HotkeyManager.Current.AddOrReplace("HotKey_Forward", Keys.Alt | Keys.Next, HotKey_Forward);
            HotkeyManager.Current.AddOrReplace("HotKey_Download", Keys.Alt | Keys.Down, HotKey_Download);
        }

        private void HotKey_Download(object sender, HotkeyEventArgs e)
        {
            tsbDowload.PerformClick();
        }

        private void HotKey_Forward(object sender, HotkeyEventArgs e)
        {
            tsbForward.PerformClick();
        }

        private void HotKey_Back(object sender, HotkeyEventArgs e)
        {
            tsbBack.PerformClick();
        }

        private void HotKey_Home(object sender, HotkeyEventArgs e)
        {
            tsbHome.PerformClick();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (DesignMode)
                return;

            wbrBrowser.Navigate(HomeUrl);
        }

        private void tsbHome_Click(object sender, EventArgs e)
        {
            wbrBrowser.Navigate(HomeUrl);
        }

        private void tsbBack_Click(object sender, EventArgs e)
        {
            if (wbrBrowser.CanGoBack)
                wbrBrowser.GoBack();
        }

        private void tsbForward_Click(object sender, EventArgs e)
        {
            if (wbrBrowser.CanGoForward)
                wbrBrowser.GoForward();
        }

        private void tsbDowload_Click(object sender, EventArgs e)
        {
            var set = Utility.Instance.LoadEmoticonSet(wbrBrowser.Url.ToString());

            if (set == null)
            {
                MessageBox.Show("유효하지 않은 URL 입니다.");
            }
            else
            {
                var form = new ImageGridForm(set);
                form.ShowDialog();
            }
        }

        private void wbrBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            tsbBack.Enabled = wbrBrowser.CanGoBack;
            tsbForward.Enabled = wbrBrowser.CanGoForward;
        }
    }
}