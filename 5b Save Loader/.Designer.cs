namespace _5b_Save_Loader
{
    partial class MainWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.SelectButton = new System.Windows.Forms.Button();
            this.OpenFile = new System.Windows.Forms.OpenFileDialog();
            this.step1 = new System.Windows.Forms.TextBox();
            this.step2 = new System.Windows.Forms.TextBox();
            this.LoadButton = new System.Windows.Forms.Button();
            this.SaveButton = new System.Windows.Forms.Button();
            this.SavesList = new System.Windows.Forms.ListBox();
            this.RenameButton = new System.Windows.Forms.Button();
            this.DeleteButton = new System.Windows.Forms.Button();
            this.ImportButton = new System.Windows.Forms.Button();
            this.ExportButton = new System.Windows.Forms.Button();
            this.DescriptionText = new System.Windows.Forms.RichTextBox();
            this.SaveFile = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // SelectButton
            // 
            this.SelectButton.Location = new System.Drawing.Point(63, 12);
            this.SelectButton.Name = "SelectButton";
            this.SelectButton.Size = new System.Drawing.Size(91, 23);
            this.SelectButton.TabIndex = 4;
            this.SelectButton.Text = "Select SWF";
            this.SelectButton.UseVisualStyleBackColor = true;
            this.SelectButton.Click += new System.EventHandler(this.SelectButton_Click);
            // 
            // step1
            // 
            this.step1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.step1.Location = new System.Drawing.Point(12, 17);
            this.step1.Name = "step1";
            this.step1.ReadOnly = true;
            this.step1.Size = new System.Drawing.Size(45, 13);
            this.step1.TabIndex = 5;
            this.step1.Text = "Step 1:";
            // 
            // step2
            // 
            this.step2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.step2.Location = new System.Drawing.Point(12, 51);
            this.step2.Name = "step2";
            this.step2.ReadOnly = true;
            this.step2.Size = new System.Drawing.Size(45, 13);
            this.step2.TabIndex = 6;
            this.step2.Text = "Step 2:";
            // 
            // LoadButton
            // 
            this.LoadButton.Location = new System.Drawing.Point(63, 46);
            this.LoadButton.Name = "LoadButton";
            this.LoadButton.Size = new System.Drawing.Size(91, 23);
            this.LoadButton.TabIndex = 7;
            this.LoadButton.Text = "Load Backup";
            this.LoadButton.UseVisualStyleBackColor = true;
            this.LoadButton.Click += new System.EventHandler(this.LoadButton_Click);
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(160, 46);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(91, 23);
            this.SaveButton.TabIndex = 8;
            this.SaveButton.Text = "Save Backup";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // SavesList
            // 
            this.SavesList.FormattingEnabled = true;
            this.SavesList.Location = new System.Drawing.Point(12, 88);
            this.SavesList.Name = "SavesList";
            this.SavesList.Size = new System.Drawing.Size(323, 147);
            this.SavesList.TabIndex = 9;
            this.SavesList.SelectedIndexChanged += new System.EventHandler(this.SavesList_SelectedIndexChanged);
            // 
            // RenameButton
            // 
            this.RenameButton.Location = new System.Drawing.Point(341, 88);
            this.RenameButton.Name = "RenameButton";
            this.RenameButton.Size = new System.Drawing.Size(91, 23);
            this.RenameButton.TabIndex = 10;
            this.RenameButton.Text = "Rename";
            this.RenameButton.UseVisualStyleBackColor = true;
            this.RenameButton.Click += new System.EventHandler(this.RenameButton_Click);
            // 
            // DeleteButton
            // 
            this.DeleteButton.Location = new System.Drawing.Point(341, 117);
            this.DeleteButton.Name = "DeleteButton";
            this.DeleteButton.Size = new System.Drawing.Size(91, 23);
            this.DeleteButton.TabIndex = 11;
            this.DeleteButton.Text = "Delete";
            this.DeleteButton.UseVisualStyleBackColor = true;
            this.DeleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
            // 
            // ImportButton
            // 
            this.ImportButton.Location = new System.Drawing.Point(341, 183);
            this.ImportButton.Name = "ImportButton";
            this.ImportButton.Size = new System.Drawing.Size(91, 23);
            this.ImportButton.TabIndex = 13;
            this.ImportButton.Text = "Import";
            this.ImportButton.UseVisualStyleBackColor = true;
            this.ImportButton.Click += new System.EventHandler(this.ImportButton_Click);
            // 
            // ExportButton
            // 
            this.ExportButton.Location = new System.Drawing.Point(341, 212);
            this.ExportButton.Name = "ExportButton";
            this.ExportButton.Size = new System.Drawing.Size(91, 23);
            this.ExportButton.TabIndex = 14;
            this.ExportButton.Text = "Export";
            this.ExportButton.UseVisualStyleBackColor = true;
            this.ExportButton.Click += new System.EventHandler(this.ExportButton_Click);
            // 
            // DescriptionText
            // 
            this.DescriptionText.Location = new System.Drawing.Point(259, 12);
            this.DescriptionText.Name = "DescriptionText";
            this.DescriptionText.Size = new System.Drawing.Size(173, 70);
            this.DescriptionText.TabIndex = 15;
            this.DescriptionText.Text = "";
            this.DescriptionText.TextChanged += new System.EventHandler(this.DescriptionText_TextChanged);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(442, 251);
            this.Controls.Add(this.DescriptionText);
            this.Controls.Add(this.ExportButton);
            this.Controls.Add(this.ImportButton);
            this.Controls.Add(this.DeleteButton);
            this.Controls.Add(this.RenameButton);
            this.Controls.Add(this.SavesList);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.LoadButton);
            this.Controls.Add(this.step2);
            this.Controls.Add(this.SelectButton);
            this.Controls.Add(this.step1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainWindow";
            this.Text = "5b Save Loader";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button SelectButton;
        private System.Windows.Forms.OpenFileDialog OpenFile;
        private System.Windows.Forms.TextBox step1;
        private System.Windows.Forms.TextBox step2;
        private System.Windows.Forms.Button LoadButton;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.ListBox SavesList;
        private System.Windows.Forms.Button RenameButton;
        private System.Windows.Forms.Button DeleteButton;
        private System.Windows.Forms.Button ImportButton;
        private System.Windows.Forms.Button ExportButton;
        private System.Windows.Forms.RichTextBox DescriptionText;
        private System.Windows.Forms.SaveFileDialog SaveFile;
    }
}

