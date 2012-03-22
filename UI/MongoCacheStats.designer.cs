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
            this.SuspendLayout();
            // 
            // listBoxKeys
            // 
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
            this.buttonRefreshFromServer.Location = new System.Drawing.Point(786, 12);
            this.buttonRefreshFromServer.Name = "buttonRefreshFromServer";
            this.buttonRefreshFromServer.Size = new System.Drawing.Size(124, 24);
            this.buttonRefreshFromServer.TabIndex = 1;
            this.buttonRefreshFromServer.Text = "Refresh from Server";
            this.buttonRefreshFromServer.UseVisualStyleBackColor = true;
            this.buttonRefreshFromServer.Click += new System.EventHandler(this.ButtonRefreshFromServerClick);
            // 
            // buttonFindKey
            // 
            this.buttonFindKey.Location = new System.Drawing.Point(840, 38);
            this.buttonFindKey.Name = "buttonFindKey";
            this.buttonFindKey.Size = new System.Drawing.Size(70, 24);
            this.buttonFindKey.TabIndex = 2;
            this.buttonFindKey.Text = "Find Key";
            this.buttonFindKey.UseVisualStyleBackColor = true;
            this.buttonFindKey.Click += new System.EventHandler(this.ButtonFindKeysClick);
            // 
            // textBoxFindKey
            // 
            this.textBoxFindKey.Location = new System.Drawing.Point(526, 42);
            this.textBoxFindKey.Name = "textBoxFindKey";
            this.textBoxFindKey.Size = new System.Drawing.Size(308, 20);
            this.textBoxFindKey.TabIndex = 3;
            // 
            // buttonDeleteAll
            // 
            this.buttonDeleteAll.Location = new System.Drawing.Point(656, 12);
            this.buttonDeleteAll.Name = "buttonDeleteAll";
            this.buttonDeleteAll.Size = new System.Drawing.Size(124, 24);
            this.buttonDeleteAll.TabIndex = 4;
            this.buttonDeleteAll.Text = "Delete All Keys";
            this.buttonDeleteAll.UseVisualStyleBackColor = true;
            this.buttonDeleteAll.Click += new System.EventHandler(this.ButtonDeleteAllKeysClick);
            // 
            // buttonDeleteSelected
            // 
            this.buttonDeleteSelected.Location = new System.Drawing.Point(526, 12);
            this.buttonDeleteSelected.Name = "buttonDeleteSelected";
            this.buttonDeleteSelected.Size = new System.Drawing.Size(124, 24);
            this.buttonDeleteSelected.TabIndex = 5;
            this.buttonDeleteSelected.Text = "Delete Selected";
            this.buttonDeleteSelected.UseVisualStyleBackColor = true;
            this.buttonDeleteSelected.Click += new System.EventHandler(this.ButtonDeleteSelectedKeysClick);
            // 
            // comboCollections
            // 
            this.comboCollections.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboCollections.FormattingEnabled = true;
            this.comboCollections.Location = new System.Drawing.Point(12, 5);
            this.comboCollections.Name = "comboCollections";
            this.comboCollections.Size = new System.Drawing.Size(493, 21);
            this.comboCollections.TabIndex = 6;
            this.comboCollections.SelectedIndexChanged += new System.EventHandler(this.ComboCollectionsSelectedIndexChanged);
            // 
            // textBoxStats
            // 
            this.textBoxStats.Location = new System.Drawing.Point(13, 32);
            this.textBoxStats.Multiline = true;
            this.textBoxStats.Name = "textBoxStats";
            this.textBoxStats.ReadOnly = true;
            this.textBoxStats.Size = new System.Drawing.Size(492, 37);
            this.textBoxStats.TabIndex = 7;
            // 
            // MongoCacheStats
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(922, 557);
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
            this.MaximizeBox = false;
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
    }
}