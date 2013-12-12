using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using JsonPrettyPrinterPlus;
using JsonPrettyPrinterPlus.JsonPrettyPrinterInternals;
using MongoDB.Bson;
using Newtonsoft.Json;
using Formatting = System.Xml.Formatting;


namespace EtoolTech.Mongo.KeyValueClient.UI
{
    public partial class ObjectViewer : Form
    {
        private int _indexOfSearchText;
        private int _start;

        public ObjectViewer()
        {
            InitializeComponent();
        }

        internal string Key { get; set; }
        internal string Prefix { get; set; }

        private void ObjectViewerLoad(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                var cacheClient = new Client(Prefix);
                tbKey.Text = Key;
                string data = cacheClient.GetAsString(Key);
                Text = string.Format("Object Viewer | Key : {0} | Lenght: ", Key);
                richTb.Text = BeautifyJson(data);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

    

        private string BeautifyJson(string json)
        {
            var jpp = new JsonPrettyPrinter(new JsonPPStrategyContext());
            return jpp.PrettyPrint(json);
        }

     

        private void ButtonFindTextClick(object sender, EventArgs e)
        {
            int startindex = 0;

            if (tbFindText.Text.Length > 0)
                startindex = FindText(tbFindText.Text.Trim(), _start, richTb.Text.Length);

            if (startindex >= 0)
            {
                richTb.SelectionColor = Color.White;
                richTb.SelectionBackColor = Color.Red;

                int endindex = tbFindText.Text.Length;

                richTb.Select(startindex, endindex);

                _start = startindex + endindex;
            }
            else
            {
                MessageBox.Show(startindex == 0
                                    ? string.Format("{0} not Finded", tbFindText.Text)
                                    : string.Format("No more ocurrences of {0} Finded", tbFindText.Text),
                                "Mongo Cache Client");
            }
        }

        private int FindText(string txt, int start, int end)
        {
            if (start > 0 && end > 0 && _indexOfSearchText >= 0)
            {
                richTb.Undo();
                richTb.Undo();
            }
            int retVal = -1;

            if (start >= 0 && _indexOfSearchText >= 0)
            {
                if (end > start || end == -1)
                {
                    _indexOfSearchText = richTb.Find(txt, start, end, RichTextBoxFinds.None);
                    if (_indexOfSearchText != -1)
                    {
                        retVal = _indexOfSearchText;
                    }
                }
            }
            return retVal;
        }
    }
}