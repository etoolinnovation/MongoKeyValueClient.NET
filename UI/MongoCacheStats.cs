using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows.Forms;
using MongoDB.Driver;

namespace EtoolTech.Mongo.KeyValueClient.UI
{
    public partial class MongoCacheStats : Form
    {
        private readonly List<object> _col = new List<object>();
        private Client _client;
        private bool _multiDatabase = false;
        private readonly Dictionary<string,string> _prefixDatabase = new Dictionary<string, string>();  

        public MongoCacheStats()
        {
            InitializeComponent();
        }

        private void MongoCacheStatsLoad(object sender, EventArgs e)
        {

            if (String.IsNullOrEmpty(ConfigurationManager.AppSettings["MongoKeyValueClient_Database"]))
            {
                _multiDatabase = true;
                comboCollections.Items.Add("");
                var mongoServer = MongoServer.Create(ConfigurationManager.AppSettings["MongoKeyValueClient_ConnStr"]);                
                foreach (string databaseName in mongoServer.GetDatabaseNames())
                {                    
                    foreach (string collectionName in mongoServer.GetDatabase(databaseName).GetCollectionNames())
                    {
                        if (collectionName.EndsWith(ConfigurationManager.AppSettings["MongoKeyValueClient_Collection"]))
                        {
                            string preFix = collectionName.Replace(ConfigurationManager.AppSettings["MongoKeyValueClient_Collection"], "");
                            if (!_prefixDatabase.ContainsKey(preFix))
                            {
                                comboCollections.Items.Add(preFix);
                                _prefixDatabase.Add(preFix, databaseName);
                            }
                        }
                    }
            
                }
                comboCollections.SelectedIndex = 0;
            }
            else
            {
                if (String.IsNullOrEmpty(ConfigurationManager.AppSettings["PrefixCollection"]))
                {
                    comboCollections.Items.Add("");
                    foreach (
                        string collectionName in
                            MongoServer.Create(ConfigurationManager.AppSettings["MongoKeyValueClient_ConnStr"]).
                                GetDatabase(ConfigurationManager.AppSettings["MongoKeyValueClient_Database"]).
                                GetCollectionNames())
                    {
                        if (collectionName.EndsWith(ConfigurationManager.AppSettings["MongoKeyValueClient_Collection"]))
                            comboCollections.Items.Add(
                                collectionName.Replace(
                                    ConfigurationManager.AppSettings["MongoKeyValueClient_Collection"], ""));
                    }

                    comboCollections.SelectedIndex = 0;
                }
                else
                {
                    comboCollections.Items.Add("");
                    foreach (
                        string companyKey in
                            ConfigurationManager.AppSettings["PrefixCollection"].Split('|'))
                    {
                        comboCollections.Items.Add(companyKey);
                    }
                    comboCollections.SelectedIndex = 0;
                }
            }            
        }

        private void Reset()
        {
            if (comboCollections.SelectedItem == null || String.IsNullOrEmpty(comboCollections.SelectedItem.ToString()))
                return;

            try
            {
                Cursor = Cursors.WaitCursor;
                if (_multiDatabase)
                    ConfigurationManager.AppSettings["MongoKeyValueClient_Database"] =
                        _prefixDatabase[comboCollections.SelectedItem.ToString()];
                _client = new Client(comboCollections.SelectedItem.ToString());
                listBoxKeys.Items.Clear();
                _col.Clear();
                foreach (var key in _client.GetAllKeysWithSize())
                {
                    if (ConfigurationManager.AppSettings["MongoKeyValueClient_ShowSizes"] == "1")
                    {                        
                        string nkey = string.Format("{0} # ({1} kb ) #", key.Key, key.Value);
                        listBoxKeys.Items.Add(nkey);
                        _col.Add(nkey);                        
                    }
                    else
                    {
                        listBoxKeys.Items.Add(key);
                        _col.Add(key);
                    }
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

            text += string.Format("Compresion: {0}",
                                  (ConfigurationManager.AppSettings["MongoKeyValueClient_CompressionEnabled"] == "1"));

            text += Environment.NewLine;
            text += string.Format("Keys: {0}", listBoxKeys.Items.Count);
            text += string.Format(" | Database: {0}", ConfigurationManager.AppSettings["MongoKeyValueClient_Database"]);
            text += string.Format(" | Collection: {0}",
                                  ConfigurationManager.AppSettings["MongoKeyValueClient_Collection"]);
            textBoxStats.Text = text;
        }

        private void ButtonFindKeysClick(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBoxFindKey.Text))
            {
                listBoxKeys.Items.Clear();
                listBoxKeys.Items.AddRange(_col.ToArray());
            }
            else
            {
                var col = new object[listBoxKeys.Items.Count];
                listBoxKeys.Items.CopyTo(col, 0);
                listBoxKeys.Items.Clear();

                foreach (object item in col)
                {
                    if (item.ToString().Contains(textBoxFindKey.Text))
                        listBoxKeys.Items.Add(item);
                }
            }

            BuildStats();
        }

        private void ButtonRefreshFromServerClick(object sender, EventArgs e)
        {
            Reset();
        }

        private static IEnumerable<string> GetNodes()
        {
            string constr = ConfigurationManager.AppSettings["MongoKeyValueClient_ConnStr"];
            constr = constr.Replace("mongodb://", "");
            constr = constr.Substring(0, constr.IndexOf("/", StringComparison.Ordinal));

            string[] servers = constr.Split(',');
            return servers;
        }
      

        private void ListBoxKeysDoubleClick(object sender, EventArgs e)
        {

            string key = listBoxKeys.SelectedItem.ToString();            
            if (key.EndsWith(") #"))
            {
                key = key.Substring(0, key.IndexOf("#", System.StringComparison.Ordinal)).Trim();
            }

            var f = new ObjectViewer {Key = key};
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
                        _client.Remove(key);
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
                        _client.Remove(key);
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