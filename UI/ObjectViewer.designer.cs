namespace EtoolTech.Mongo.KeyValueClient.UI
{
    partial class ObjectViewer
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
            this.richTbXml = new System.Windows.Forms.RichTextBox();
            this.tbFindText = new System.Windows.Forms.TextBox();
            this.buttonFindText = new System.Windows.Forms.Button();
            this.tbKey = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // richTbXml
            // 
            this.richTbXml.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTbXml.Location = new System.Drawing.Point(12, 39);
            this.richTbXml.Name = "richTbXml";
            this.richTbXml.ReadOnly = true;
            this.richTbXml.Size = new System.Drawing.Size(737, 537);
            this.richTbXml.TabIndex = 0;
            this.richTbXml.Text = "";
            // 
            // tbFindText
            // 
            this.tbFindText.Location = new System.Drawing.Point(442, 13);
            this.tbFindText.Name = "tbFindText";
            this.tbFindText.Size = new System.Drawing.Size(231, 20);
            this.tbFindText.TabIndex = 5;
            // 
            // buttonFindText
            // 
            this.buttonFindText.Location = new System.Drawing.Point(679, 9);
            this.buttonFindText.Name = "buttonFindText";
            this.buttonFindText.Size = new System.Drawing.Size(70, 24);
            this.buttonFindText.TabIndex = 4;
            this.buttonFindText.Text = "Find Text";
            this.buttonFindText.UseVisualStyleBackColor = true;
            this.buttonFindText.Click += new System.EventHandler(this.ButtonFindTextClick);
            // 
            // tbKey
            // 
            this.tbKey.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbKey.Location = new System.Drawing.Point(12, 13);
            this.tbKey.Name = "tbKey";
            this.tbKey.ReadOnly = true;
            this.tbKey.Size = new System.Drawing.Size(424, 20);
            this.tbKey.TabIndex = 6;
            // 
            // ObjectViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(759, 588);
            this.Controls.Add(this.tbKey);
            this.Controls.Add(this.tbFindText);
            this.Controls.Add(this.buttonFindText);
            this.Controls.Add(this.richTbXml);
            this.MaximizeBox = false;
            this.Name = "ObjectViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Object Viewer";
            this.Load += new System.EventHandler(this.ObjectViewerLoad);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTbXml;
        private System.Windows.Forms.TextBox tbFindText;
        private System.Windows.Forms.Button buttonFindText;
        private System.Windows.Forms.TextBox tbKey;

    }
}