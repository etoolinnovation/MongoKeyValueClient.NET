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
        private Client _client;
        private bool _multiDatabase = false;
        private readonly Dictionary<string,string> _prefixDatabase = new Dictionary<string, string>();  
        private List<string> _localKeys = new List<string>(); 

        public MongoCacheStats()
        {
            InitializeComponent();
        }

        private void MongoCacheStatsLoad(object sender, EventArgs e)
        {

            if (String.IsNullOrEmpty(Config.Instance.Database))
            {
                _multiDatabase = true;
                comboCollections.Items.Add("");
                var mongoServer = MongoServer.Create(Config.Instance.ConnStr);                
                foreach (string databaseName in mongoServer.GetDatabaseNames())
                {                    
                    foreach (string collectionName in mongoServer.GetDatabase(databaseName).GetCollectionNames())
                    {
                        if (collectionName.EndsWith(Config.Instance.Collection))
                        {
                            string preFix = collectionName.Replace(Config.Instance.Collection, "");
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
                if (String.IsNullOrEmpty(Config.Instance.PrefixCollection))
                {
                    comboCollections.Items.Add("");
                    foreach (
                        string collectionName in
                            MongoServer.Create(Config.Instance.ConnStr).
                                GetDatabase(Config.Instance.Database).
                                GetCollectionNames())
                    {
                        if (collectionName == Config.Instance.Collection)
                        {
                            comboCollections.Items.Add(collectionName);
                        }
                        else if (
                            collectionName.EndsWith(Config.Instance.Collection))
                        {
                            comboCollections.Items.Add(
                                collectionName.Replace(Config.Instance.Collection, ""));
                        }

                    }

                    comboCollections.SelectedIndex = 0;
                }
                else
                {
                    comboCollections.Items.Add("");
                    foreach (
                        string companyKey in
                            Config.Instance.PrefixCollection.Split('|'))
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
                    Config.Instance.Database = _prefixDatabase[comboCollections.SelectedItem.ToString()];

                if (comboCollections.SelectedItem.ToString() == Config.Instance.Collection)
                    _client = new Client();
                else
                    _client = new Client(comboCollections.SelectedItem.ToString());
                listBoxKeys.Items.Clear();                

                if (Config.Instance.ShowSizes)
                {
                 
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
                                  (Config.Instance.CompresionEnabled));

            text += Environment.NewLine;
            text += string.Format("Server Keys: {0}", _client.GetAllKeysAsCursor().Count());
            text += string.Format(" | Local Keys: {0}", listBoxKeys.Items.Count);
            text += string.Format(" | Database: {0}", Config.Instance.Database);
            text += string.Format(" | Collection: {0}",Config.Instance.Collection);
            textBoxStats.Text = text;
        }

        private void ButtonFindKeysClick(object sender, EventArgs e)
        {            
            try
            {
                Cursor = Cursors.WaitCursor;

                if (String.IsNullOrEmpty(textBoxFindKey.Text))
                {
                    listBoxKeys.Items.Clear();                    
                }
                else
                {
                    if (this.localFind.Checked)
                    {
                        listBoxKeys.Items.Clear(); 
                        foreach (var item in _localKeys.Where(k=>k.Contains(textBoxFindKey.Text)))
                        {
                            listBoxKeys.Items.Add(item);
                        }
                        _localKeys.RemoveAll(k=>!k.Contains(textBoxFindKey.Text));
                    }
                    else
                    {
                        var cursor = _client.GetKeysRegex(textBoxFindKey.Text);
                        _localKeys.Clear();
                        foreach (var item in cursor)
                        {
                            listBoxKeys.Items.Add(item._id);                            
                            _localKeys.Add(item._id);
                        }
                    }
                }


                BuildStats();
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void ButtonRefreshFromServerClick(object sender, EventArgs e)
        {
            _localKeys.Clear();
            Reset();
        }

        private static IEnumerable<string> GetNodes()
        {
            string constr = Config.Instance.ConnStr;
            constr = constr.Replace("mongodb://", "");
            if (!constr.Contains(',')) return new List<string>() {constr};
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
            if (MessageBox.Show("Delete all Keys in the Grid?", "Mongo Cache Client", MessageBoxButtons.YesNo) ==
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


        private void buttonDeleteServerAll_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Delete all Keys in the Server?", "Mongo Cache Client", MessageBoxButtons.YesNo) ==
              DialogResult.Yes)
            {
                try
                {
                    Cursor = Cursors.WaitCursor;
                    _client.RemoveAll();

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