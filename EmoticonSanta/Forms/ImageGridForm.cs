#region
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using EmoticonSanta.Properties;
using NHotkey;
using NHotkey.WindowsForms;
#endregion

namespace EmoticonSanta.Controls
{
    public partial class ImageGridForm : Form
    {
        public ImageGridForm()
        {
            InitializeComponent();

            HotkeyManager.Current.AddOrReplace("HotKey_All", Keys.Alt | Keys.A, HotKey_All);
            HotkeyManager.Current.AddOrReplace("HotKey_None", Keys.Alt | Keys.N, HotKey_None);
            HotkeyManager.Current.AddOrReplace("HotKey_save", Keys.Alt | Keys.S, HotKey_save);

            for (int i = 0; i < 5; i++)
            for (int j = 0; j < 5; j++)
                _boxes.Add(CreateEmoticonBox(i, j));
        }

        private void HotKey_save(object sender, HotkeyEventArgs e)
        {
            tsbSave.PerformClick();
        }

        private void HotKey_None(object sender, HotkeyEventArgs e)
        {
            tsbNone.PerformClick();
        }

        private void HotKey_All(object sender, HotkeyEventArgs e)
        {
            tsbAll.PerformClick();
        }

        public ImageGridForm(EmoticonSet emoticonSet) : this()
        {
            _emoticonSet = emoticonSet;
        }

        private readonly EmoticonSet _emoticonSet;

        private readonly List<EmoticonBoxControl> _boxes = new List<EmoticonBoxControl>(25);

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (DesignMode)
                return;

            Text = _emoticonSet.Title;

            for (int i = 0; i < 24; i++)
            {
                var bytes = Utility.Instance.DownloadImage(_emoticonSet[i]);
                _boxes[i].LoadImage(bytes);
            }

            _boxes.ForEach(x => x.SelectImage());
        }

        private EmoticonBoxControl CreateEmoticonBox(int row, int column)
        {
            EmoticonBoxControl box = new EmoticonBoxControl(row * 5 + column);
            box.ImageSelectionChanged += EmoticonBox_ImageSelectionChanged;
            tlpLayout.Controls.Add(box, column, row);

            return box;
        }

        private void EmoticonBox_ImageSelectionChanged(object sender, EmoticonBoxControl.ImageSelectionChangedEventArgs e)
        {
            string text = $"{_boxes.FindAll(x => x.Selected).Count} item(s) selected";
            tsbLabel.Text = text;
        }

        private void tsbAll_Click(object sender, EventArgs e)
        {
            _boxes.ForEach(x => x.SelectImage());
        }

        private void tsbNone_Click(object sender, EventArgs e)
        {
            _boxes.ForEach(x => x.UnselectImage());
        }

        private void tsbSave_Click(object sender, EventArgs e)
        {
            var boxes = _boxes.FindAll(x => x.Selected);
            boxes.ForEach(x =>
                          {
                              x.DownloadImage(_emoticonSet.FileName);
                              x.Finish();
                          });

            string message = $"{boxes.Count} 개의 파일을 아래 경로에 저장하였습니다.\r\n{Settings.Default.DirectoryToSave}";
            MessageBox.Show(message);
            Close();
        }
    }
}