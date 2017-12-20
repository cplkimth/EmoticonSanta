#region
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using EmoticonSanta.Properties;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
#endregion

namespace EmoticonSanta.Controls
{
    public class Utility
    {
        #region singleton
        private static Utility _instance;

        public static Utility Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Utility();
                return _instance;
            }
        }

        private Utility()
        {
            _web.Encoding = Encoding.UTF8;
        }
        #endregion

        private readonly WebClient _web = new WebClient();

        public EmoticonSet LoadEmoticonSet(string url)
        {
            try
            {
                var urls = ExtractImageUrls(url, out string title);

                var set = new EmoticonSet(title);
                set.Urls.AddRange(urls);
                return set;
            }
            catch
            {
                return null;
            }
        }

        private List<string> ExtractImageUrls(string url, out string title)
        {
            var html = _web.DownloadString(url);
            var itemCode = ExtractItemCode(html);
            title = ExtractTitle(html);

            var jsonUrl = "https://e.kakao.com/detail/thumb_url?item_code=" + itemCode;
            var json = _web.DownloadString(jsonUrl);

            JObject jo = (JObject) JsonConvert.DeserializeObject(json);
            var values = jo["body"].Values<string>();

            return new List<string>(values);
        }

        private string ExtractCore(string html, string expression)
        {
            Match match = Regex.Match(html, expression, RegexOptions.IgnoreCase | RegexOptions.Singleline);

            if (match.Success)
                return match.Groups[1].ToString();
            else
                throw new Exception();
        }

        private string ExtractItemCode(string html)
        {
            return ExtractCore(html, @"item_code : .*?(\d+)");
        }

        private string ExtractTitle(string html)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);
            var node = document.DocumentNode.SelectSingleNode("//h3[contains(@class, 'tit_product')]");
            var builder = new StringBuilder(node.InnerText.Trim());

            string[] removables = {"&amp;"};
            foreach (var removable in removables)
                builder.Replace(removable, string.Empty);

            return builder.ToString();
        }

        public byte[] DownloadImage(string url)
        {
            return _web.DownloadData(url);
        }

        public void DownloadImage(string url, int imageIndex, string folderToSave)
        {
            var bytes = _web.DownloadData(url);
            var image = (Bitmap) new ImageConverter().ConvertFrom(bytes);
            image = Transparent2Color(image, Color.FromArgb(0x9A, 0xBB, 0xD3));

            if (Directory.Exists(folderToSave) == false)
                Directory.CreateDirectory(folderToSave);

            image.Save(Path.Combine(folderToSave, $"{imageIndex + 1:D2}.png"));
        }

        public Bitmap Transparent2Color(Image source, Color color)
        {
            var target = new Bitmap(source.Width, source.Height);
            Rectangle rect = new Rectangle(Point.Empty, source.Size);
            using (Graphics G = Graphics.FromImage(target))
            {
                G.CompositingMode = CompositingMode.SourceOver;
                G.Clear(color);
                G.DrawImageUnscaledAndClipped(source, rect);
            }
            return target;
        }

        public string ToValidFileName(string fileName)
        {
            StringBuilder builder = new StringBuilder(fileName);
            foreach (char c in Path.GetInvalidFileNameChars())
                builder.Replace(c, '_');

            return builder.ToString();
        }

        public void EnsureSaveDirectory()
        {
            if (string.IsNullOrEmpty(Settings.Default.DirectoryToSave))
            {
                string[] specialFolders =
                {
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    @"D:\Desktop",
                    Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                };

                string path = null;
                foreach (var specialFolder in specialFolders)
                {
                    if (string.IsNullOrEmpty(specialFolder) == false)
                    {
                        path = specialFolder;
                        break;
                    }
                }

                Settings.Default.DirectoryToSave = path;
            }

            if (Directory.Exists(Settings.Default.DirectoryToSave) == false)
                Directory.CreateDirectory(Settings.Default.DirectoryToSave);
        }
    }
}