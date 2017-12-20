#region
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
#endregion

namespace EmoticonSanta.Controls
{
    public class EmoticonSet : IEnumerable<string>
    {
        public EmoticonSet(string title)
        {
            Title = title;
            Urls = new List<string>(24);

            FileName = Utility.Instance.ToValidFileName(Title);
        }

        public string Title { get; }
        public List<string> Urls { get; }

        public string FileName { get; }

        public string this[int index] => Urls[index];

        #region IEnumerable<string>
        public IEnumerator<string> GetEnumerator()
        {
            foreach (var emoticon in Urls)
            {
                yield return emoticon;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}