﻿namespace FPLedit.BildfahrplanExport
{
    partial class TrainColorForm
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
            this.trainListView = new System.Windows.Forms.ListView();
            this.cancelButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.editButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // trainListView
            // 
            this.trainListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trainListView.Location = new System.Drawing.Point(12, 12);
            this.trainListView.MultiSelect = false;
            this.trainListView.Name = "trainListView";
            this.trainListView.Size = new System.Drawing.Size(360, 440);
            this.trainListView.TabIndex = 0;
            this.trainListView.UseCompatibleStateImageBehavior = false;
            this.trainListView.View = System.Windows.Forms.View.Details;
            this.trainListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.trainListView_MouseDoubleClick);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(351, 458);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 8;
            this.cancelButton.Text = "Abbrechen";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.Location = new System.Drawing.Point(432, 458);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 7;
            this.closeButton.Text = "Schließen";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // editButton
            // 
            this.editButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.editButton.Location = new System.Drawing.Point(378, 12);
            this.editButton.Name = "editButton";
            this.editButton.Size = new System.Drawing.Size(129, 23);
            this.editButton.TabIndex = 17;
            this.editButton.Text = "Darstellung bearbeiten";
            this.editButton.UseVisualStyleBackColor = true;
            this.editButton.Click += new System.EventHandler(this.editButton_Click);
            // 
            // TrainColorForm
            // 
            this.AcceptButton = this.closeButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(519, 493);
            this.Controls.Add(this.editButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.trainListView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "TrainColorForm";
            this.ShowInTaskbar = false;
            this.Text = "Zugdarstellung ändern";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView trainListView;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button editButton;
    }
}