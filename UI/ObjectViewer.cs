using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using JsonPrettyPrinterPlus;
using JsonPrettyPrinterPlus.JsonPrettyPrinterInternals;
using ServiceStack.Text;

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
                this.Cursor = Cursors.WaitCursor;
                var cacheClient = new CacheClient(Prefix);
                Text = string.Format("Object Viewer | Key : {0}", Key);
                tbKey.Text = Key;
                object data = cacheClient.Get(Key);

                if (System.Configuration.ConfigurationManager.AppSettings["ObjectViewerMode"] == "JSON")
                {
                    this.richTbXml.Text = BeautifyJson(data.ToJson());
                }
                else
                {
                    string strdata = data.ToXml();
                    AddColouredText(IndentXml(strdata));
                }
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }


        private string BeautifyJson(string json)
        {
            var jpp = new JsonPrettyPrinter(new JsonPPStrategyContext());            
            return jpp.PrettyPrint(json);
        }


        private void AddColouredText(string strTextToAdd)
        {
            richTbXml.Clear();
            richTbXml.AppendText(strTextToAdd);
            string strRtf = richTbXml.Rtf;
            richTbXml.Clear();
            int iCTableStart = strRtf.IndexOf("colortbl;", System.StringComparison.Ordinal);

            if (iCTableStart != -1)
            {
                int iCTableEnd = strRtf.IndexOf('}', iCTableStart);
                strRtf = strRtf.Remove(iCTableStart, iCTableEnd - iCTableStart);

                strRtf = strRtf.Insert(iCTableStart,
                                       "colortbl ;\\red255\\green0\\blue0;\\red0\\green128\\blue0;\\red0\\green0\\blue255;}");
            }

            else
            {
                int iRtfLoc = strRtf.IndexOf("\\rtf", System.StringComparison.Ordinal);
                int iInsertLoc = strRtf.IndexOf('{', iRtfLoc);

                if (iInsertLoc == -1) iInsertLoc = strRtf.IndexOf('}', iRtfLoc) - 1;

                strRtf = strRtf.Insert(iInsertLoc,
                                       "{\\colortbl ;\\red128\\green0\\blue0;\\red0\\green128\\blue0;\\red0\\green0\\blue255;}");
            }


            for (int i = 0; i < strRtf.Length; i++)
            {
                if (strRtf[i] == '<')
                {
                    strRtf = strRtf[i + 1] == '!' ? strRtf.Insert(i + 4, "\\cf2 ") : strRtf.Insert(i + 1, "\\cf1 ");
                    strRtf = strRtf.Insert(i, "\\cf3 ");
                    i += 6;
                }
                else if (strRtf[i] == '>')
                {
                    strRtf = strRtf.Insert(i + 1, "\\cf0 ");
                    if (strRtf[i - 1] == '-')
                    {
                        strRtf = strRtf.Insert(i - 2, "\\cf3 ");
                        i += 8;
                    }
                    else
                    {
                        strRtf = strRtf.Insert(i, "\\cf3 ");
                        i += 6;
                    }
                }
            }
            richTbXml.Rtf = strRtf;
        }

        private static string IndentXml(string xml)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.Unicode))
                {
                    try
                    {
                        var xmlDocument = new XmlDocument();
                        xmlDocument.LoadXml(xml);
                        xmlTextWriter.Formatting = Formatting.Indented;
                        xmlDocument.WriteContentTo(xmlTextWriter);
                        xmlTextWriter.Flush();
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        using (var streamReader = new StreamReader(memoryStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "Mongo Cache Client");
                        return string.Empty;
                    }
                }
            }
        }

        private void ButtonFindTextClick(object sender, EventArgs e)
        {
            int startindex = 0;

            if (tbFindText.Text.Length > 0)
                startindex = FindText(tbFindText.Text.Trim(), _start, richTbXml.Text.Length);

            if (startindex >= 0)
            {
                richTbXml.SelectionColor = Color.White;
                richTbXml.SelectionBackColor = Color.Red;

                int endindex = tbFindText.Text.Length;

                richTbXml.Select(startindex, endindex);

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
                richTbXml.Undo();
                richTbXml.Undo();
            }
            int retVal = -1;

            if (start >= 0 && _indexOfSearchText >= 0)
            {
                if (end > start || end == -1)
                {
                    _indexOfSearchText = richTbXml.Find(txt, start, end, RichTextBoxFinds.None);
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