using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using EmoticonSanta.Properties;

namespace EmoticonSanta.Controls
{
    public partial class EmoticonBoxControl : UserControl
    {
        private EmoticonBoxControl()
        {
            InitializeComponent();
        }

        public EmoticonBoxControl(int index) : this()
        {
            Index = index;
        }

        public int Index { get; }

        public bool Selected { get; private set; }

        private void pcbImage_Click(object sender, EventArgs e)
        {
            if (Selected)
                UnselectImage();
            else
                SelectImage();
        }

        public void LoadImage(byte[] bytes)
        {
            var image = (Bitmap) new ImageConverter().ConvertFrom(bytes);
            image = Utility.Instance.Transparent2Color(image, Color.FromArgb(0x9A, 0xBB, 0xD3));
            pcbImage.Image = image;
        }

        public void SelectImage()
        {
            if (pcbImage.Image == null)
                return;

            Selected = true;
            BackColor = Color.Beige;

            OnImageSelectionChanged(true);
        }

        public void UnselectImage()
        {
            Selected = false;
            BackColor = Color.Transparent;

            OnImageSelectionChanged(false);
        }

        public void Finish()
        {
            UnselectImage();
            pcbImage.Visible = false;
        }

        #region ImageSelectionChanged event things for C# 3.0
        public event EventHandler<ImageSelectionChangedEventArgs> ImageSelectionChanged;

        protected virtual void OnImageSelectionChanged(ImageSelectionChangedEventArgs e)
        {
            if (ImageSelectionChanged != null)
                ImageSelectionChanged(this, e);
        }

        private ImageSelectionChangedEventArgs OnImageSelectionChanged(bool selected)
        {
            ImageSelectionChangedEventArgs args = new ImageSelectionChangedEventArgs(selected);
            OnImageSelectionChanged(args);

            return args;
        }

        private ImageSelectionChangedEventArgs OnImageSelectionChangedForOut()
        {
            ImageSelectionChangedEventArgs args = new ImageSelectionChangedEventArgs();
            OnImageSelectionChanged(args);

            return args;
        }

        public class ImageSelectionChangedEventArgs : EventArgs
        {
            public bool Selected { get; set; }

            public ImageSelectionChangedEventArgs()
            {
            }

            public ImageSelectionChangedEventArgs(bool selected)
            {
                Selected = selected;
            }
        }
        #endregion

        public void DownloadImage(string fileName)
        {
            var path = Path.Combine(Settings.Default.DirectoryToSave, fileName);

            if (Directory.Exists(path) == false)
                Directory.CreateDirectory(path);

            path = Path.Combine(path, $"{Index:D2}.png");
            pcbImage.Image.Save(path);
        }
    }
}