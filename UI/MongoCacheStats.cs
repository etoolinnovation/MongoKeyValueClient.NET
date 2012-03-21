using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows.Forms;

namespace EtoolTech.Mongo.KeyValueClient.UI
{
    public partial class MongoCacheStats : Form
    {
        private CacheClient _cacheClient;

        public MongoCacheStats()
        {
            InitializeComponent();
        }

        private void MongoCacheStatsLoad(object sender, EventArgs e)
        {
            comboCollections.Items.Add(ConfigurationManager.AppSettings["MongoCacheClient_Collection"]);
            comboCollections.SelectedIndex = 0;
            Reset();
        }

        private void Reset()
        {
            if (comboCollections.SelectedItem == null || String.IsNullOrEmpty(comboCollections.SelectedItem.ToString())) return;

            try
            {
                Cursor = Cursors.WaitCursor;
                _cacheClient = new CacheClient();
                listBoxKeys.Items.Clear();
                foreach (string key in _cacheClient.GetAllKeys())
                {
                    listBoxKeys.Items.Add(key);
                }

                textBoxFindKey.Text = "";

                BuildStats();
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void BuildStats()
        {
            string text = "";
            text += "Nodes: ";
            text = GetNodes().Aggregate(text, (current, node) => current + (node + " | "));

            text += string.Format("Compresion: {0}", (ConfigurationManager.AppSettings["MongoCacheClient_CompressionEnabled"] == "1"));

            text += Environment.NewLine;
            text += string.Format("Keys: {0}", listBoxKeys.Items.Count);
            text += string.Format(" | Database: {0}", ConfigurationManager.AppSettings["MongoCacheClient_Database"]);
            text += string.Format(" | Collection: {0}", ConfigurationManager.AppSettings["MongoCacheClient_Collection"]);
            textBoxStats.Text = text;
        }

        private void ButtonFindKeysClick(object sender, EventArgs e)
        {
            var col = new object[listBoxKeys.Items.Count];
            listBoxKeys.Items.CopyTo(col, 0);
            listBoxKeys.Items.Clear();

            foreach (object item in col)
            {
                if (item.ToString().Contains(textBoxFindKey.Text))
                    listBoxKeys.Items.Add(item);
            }

            BuildStats();
        }

        private void ButtonRefreshFromServerClick(object sender, EventArgs e)
        {
            Reset();
        }

        private static IEnumerable<string> GetNodes()
        {            
            string constr = ConfigurationManager.AppSettings["MongoCacheClient_ConnStr"];
            constr = constr.Replace("mongodb://", "");
            constr = constr.Substring(0, constr.IndexOf("/", StringComparison.Ordinal));

            string[] servers = constr.Split(',');
            return servers;
        }

        private void ListBoxKeysDoubleClick(object sender, EventArgs e)
        {
            var f = new ObjectViewer {Key = listBoxKeys.SelectedItem.ToString()};
            f.ShowDialog(this);
        }

        private void ButtonDeleteAllKeysClick(object sender, EventArgs e)
        {
            if (MessageBox.Show("Delete all Keys in the Server?", "Mongo Cache Client", MessageBoxButtons.YesNo) ==
                DialogResult.Yes)
            {
                try
                {
                    Cursor = Cursors.WaitCursor;
                    foreach (object item in listBoxKeys.Items)
                    {
                        string key = item.ToString();
                        _cacheClient.Remove(key);
                    }

                    Reset();
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        private void ButtonDeleteSelectedKeysClick(object sender, EventArgs e)
        {
            if (
                MessageBox.Show(
                    string.Format("Delete the {0} selected Keys in the Server?", listBoxKeys.SelectedItems.Count),
                    "Mongo Cache Client", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    Cursor = Cursors.WaitCursor;
                    foreach (object item in listBoxKeys.SelectedItems)
                    {
                        string key = item.ToString();
                        _cacheClient.Remove(key);
                    }

                    Reset();
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        private void ComboCollectionsSelectedIndexChanged(object sender, EventArgs e)
        {
            Reset();
        }
    }
}