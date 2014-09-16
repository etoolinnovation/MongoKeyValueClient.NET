namespace EtoolTech.Mongo.KeyValueClient.UI
{
    partial class MongoCacheStats
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listBoxKeys = new System.Windows.Forms.ListBox();
            this.buttonRefreshFromServer = new System.Windows.Forms.Button();
            this.buttonFindKey = new System.Windows.Forms.Button();
            this.textBoxFindKey = new System.Windows.Forms.TextBox();
            this.buttonDeleteAll = new System.Windows.Forms.Button();
            this.buttonDeleteSelected = new System.Windows.Forms.Button();
            this.comboCollections = new System.Windows.Forms.ComboBox();
            this.textBoxStats = new System.Windows.Forms.TextBox();
            this.localFind = new System.Windows.Forms.CheckBox();
            this.buttonDeleteServerAll = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listBoxKeys
            // 
            this.listBoxKeys.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxKeys.FormattingEnabled = true;
            this.listBoxKeys.Location = new System.Drawing.Point(12, 77);
            this.listBoxKeys.Name = "listBoxKeys";
            this.listBoxKeys.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxKeys.Size = new System.Drawing.Size(898, 472);
            this.listBoxKeys.TabIndex = 0;
            this.listBoxKeys.DoubleClick += new System.EventHandler(this.ListBoxKeysDoubleClick);
            // 
            // buttonRefreshFromServer
            // 
            this.buttonRefreshFromServer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRefreshFromServer.Location = new System.Drawing.Point(820, 12);
            this.buttonRefreshFromServer.Name = "buttonRefreshFromServer";
            this.buttonRefreshFromServer.Size = new System.Drawing.Size(93, 24);
            this.buttonRefreshFromServer.TabIndex = 1;
            this.buttonRefreshFromServer.Text = "Clear Form";
            this.buttonRefreshFromServer.UseVisualStyleBackColor = true;
            this.buttonRefreshFromServer.Click += new System.EventHandler(this.ButtonRefreshFromServerClick);
            // 
            // buttonFindKey
            // 
            this.buttonFindKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonFindKey.Location = new System.Drawing.Point(839, 38);
            this.buttonFindKey.Name = "buttonFindKey";
            this.buttonFindKey.Size = new System.Drawing.Size(71, 24);
            this.buttonFindKey.TabIndex = 2;
            this.buttonFindKey.Text = "Find Keys";
            this.buttonFindKey.UseVisualStyleBackColor = true;
            this.buttonFindKey.Click += new System.EventHandler(this.ButtonFindKeysClick);
            // 
            // textBoxFindKey
            // 
            this.textBoxFindKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFindKey.Location = new System.Drawing.Point(468, 42);
            this.textBoxFindKey.Name = "textBoxFindKey";
            this.textBoxFindKey.Size = new System.Drawing.Size(312, 20);
            this.textBoxFindKey.TabIndex = 3;
            // 
            // buttonDeleteAll
            // 
            this.buttonDeleteAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDeleteAll.Location = new System.Drawing.Point(586, 12);
            this.buttonDeleteAll.Name = "buttonDeleteAll";
            this.buttonDeleteAll.Size = new System.Drawing.Size(116, 24);
            this.buttonDeleteAll.TabIndex = 4;
            this.buttonDeleteAll.Text = "Del All Local Keys";
            this.buttonDeleteAll.UseVisualStyleBackColor = true;
            this.buttonDeleteAll.Click += new System.EventHandler(this.ButtonDeleteAllKeysClick);
            // 
            // buttonDeleteSelected
            // 
            this.buttonDeleteSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDeleteSelected.Location = new System.Drawing.Point(469, 12);
            this.buttonDeleteSelected.Name = "buttonDeleteSelected";
            this.buttonDeleteSelected.Size = new System.Drawing.Size(116, 24);
            this.buttonDeleteSelected.TabIndex = 5;
            this.buttonDeleteSelected.Text = "Delete Selected";
            this.buttonDeleteSelected.UseVisualStyleBackColor = true;
            this.buttonDeleteSelected.Click += new System.EventHandler(this.ButtonDeleteSelectedKeysClick);
            // 
            // comboCollections
            // 
            this.comboCollections.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboCollections.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboCollections.FormattingEnabled = true;
            this.comboCollections.Location = new System.Drawing.Point(12, 5);
            this.comboCollections.Name = "comboCollections";
            this.comboCollections.Size = new System.Drawing.Size(430, 21);
            this.comboCollections.TabIndex = 6;
            this.comboCollections.SelectedIndexChanged += new System.EventHandler(this.ComboCollectionsSelectedIndexChanged);
            // 
            // textBoxStats
            // 
            this.textBoxStats.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxStats.Location = new System.Drawing.Point(13, 32);
            this.textBoxStats.Multiline = true;
            this.textBoxStats.Name = "textBoxStats";
            this.textBoxStats.ReadOnly = true;
            this.textBoxStats.Size = new System.Drawing.Size(429, 37);
            this.textBoxStats.TabIndex = 7;
            // 
            // localFind
            // 
            this.localFind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.localFind.AutoSize = true;
            this.localFind.Location = new System.Drawing.Point(786, 44);
            this.localFind.Name = "localFind";
            this.localFind.Size = new System.Drawing.Size(52, 17);
            this.localFind.TabIndex = 8;
            this.localFind.Text = "Local";
            this.localFind.UseVisualStyleBackColor = true;
            // 
            // buttonDeleteServerAll
            // 
            this.buttonDeleteServerAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDeleteServerAll.Location = new System.Drawing.Point(703, 12);
            this.buttonDeleteServerAll.Name = "buttonDeleteServerAll";
            this.buttonDeleteServerAll.Size = new System.Drawing.Size(116, 24);
            this.buttonDeleteServerAll.TabIndex = 9;
            this.buttonDeleteServerAll.Text = "Del All Server Keys";
            this.buttonDeleteServerAll.UseVisualStyleBackColor = true;
            this.buttonDeleteServerAll.Click += new System.EventHandler(this.buttonDeleteServerAll_Click);
            // 
            // MongoCacheStats
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(922, 557);
            this.Controls.Add(this.buttonDeleteServerAll);
            this.Controls.Add(this.localFind);
            this.Controls.Add(this.textBoxStats);
            this.Controls.Add(this.comboCollections);
            this.Controls.Add(this.buttonDeleteSelected);
            this.Controls.Add(this.buttonDeleteAll);
            this.Controls.Add(this.textBoxFindKey);
            this.Controls.Add(this.buttonFindKey);
            this.Controls.Add(this.buttonRefreshFromServer);
            this.Controls.Add(this.listBoxKeys);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MongoCacheStats";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MongoCache Stats";
            this.Load += new System.EventHandler(this.MongoCacheStatsLoad);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxKeys;
        private System.Windows.Forms.Button buttonRefreshFromServer;
        private System.Windows.Forms.Button buttonFindKey;
        private System.Windows.Forms.TextBox textBoxFindKey;
        private System.Windows.Forms.Button buttonDeleteAll;
        private System.Windows.Forms.Button buttonDeleteSelected;
        private System.Windows.Forms.ComboBox comboCollections;
        private System.Windows.Forms.TextBox textBoxStats;
        private System.Windows.Forms.CheckBox localFind;
        private System.Windows.Forms.Button buttonDeleteServerAll;
    }
}