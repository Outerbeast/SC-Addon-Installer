/* Sven Co-op Addon Installer - Sven Co-op addon installation made easy
Copyright (C) 2025 Outerbeast
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program. If not, see <https://www.gnu.org/licenses/>.
*/
namespace SCAddonInstaller;

partial class GUI
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
        if (disposing)
        {
            // Explicitly dispose controls that may cause issues
            if (listAddons != null)
            {
                listAddons.Dispose();
                listAddons = null;
            }
            if (components != null)
            {
                components.Dispose();
            }
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
        this.components = new System.ComponentModel.Container();
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GUI));
        this.listAddons = new System.Windows.Forms.ListBox();
        this.btnInstall = new System.Windows.Forms.Button();
        this.btnClear = new System.Windows.Forms.Button();
        this.btnAdd = new System.Windows.Forms.Button();
        this.lblInstruction = new System.Windows.Forms.Label();
        this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
        this.btnDownload = new System.Windows.Forms.Button();
        this.btnRemove = new System.Windows.Forms.Button();
        this.lblInstallPath = new System.Windows.Forms.LinkLabel();
        ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
        this.SuspendLayout();
        // 
        // listAddons
        // 
        this.listAddons.AllowDrop = true;
        this.listAddons.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right)));
        this.listAddons.FormattingEnabled = true;
        this.listAddons.Location = new System.Drawing.Point(12, 35);
        this.listAddons.Name = "listAddons";
        this.listAddons.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
        this.listAddons.Size = new System.Drawing.Size(473, 212);
        this.listAddons.TabIndex = 0;
        this.listAddons.SelectedIndexChanged += new System.EventHandler(this.listAddons_SelectedIndexChanged);
        // 
        // btnInstall
        // 
        this.btnInstall.FlatStyle = System.Windows.Forms.FlatStyle.System;
        this.btnInstall.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.btnInstall.Location = new System.Drawing.Point(491, 222);
        this.btnInstall.Name = "btnInstall";
        this.btnInstall.Size = new System.Drawing.Size(85, 25);
        this.btnInstall.TabIndex = 1;
        this.btnInstall.Text = "Install";
        this.btnInstall.UseVisualStyleBackColor = true;
        this.btnInstall.Click += new System.EventHandler(this.btnInstall_Click);
        // 
        // btnClear
        // 
        this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.System;
        this.btnClear.Location = new System.Drawing.Point(491, 160);
        this.btnClear.Name = "btnClear";
        this.btnClear.Size = new System.Drawing.Size(85, 25);
        this.btnClear.TabIndex = 2;
        this.btnClear.Text = "Clear";
        this.btnClear.UseVisualStyleBackColor = true;
        this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
        // 
        // btnAdd
        // 
        this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.System;
        this.btnAdd.Location = new System.Drawing.Point(491, 35);
        this.btnAdd.Name = "btnAdd";
        this.btnAdd.Size = new System.Drawing.Size(85, 25);
        this.btnAdd.TabIndex = 3;
        this.btnAdd.Text = "Add files";
        this.btnAdd.UseVisualStyleBackColor = true;
        this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
        // 
        // lblInstruction
        // 
        this.lblInstruction.Location = new System.Drawing.Point(12, 9);
        this.lblInstruction.Name = "lblInstruction";
        this.lblInstruction.Size = new System.Drawing.Size(182, 23);
        this.lblInstruction.TabIndex = 8;
        this.lblInstruction.Text = "Drag and drop addons here:";
        // 
        // btnDownload
        // 
        this.btnDownload.Location = new System.Drawing.Point(491, 64);
        this.btnDownload.Name = "btnDownload";
        this.btnDownload.Size = new System.Drawing.Size(85, 25);
        this.btnDownload.TabIndex = 9;
        this.btnDownload.Text = "Add download";
        this.btnDownload.UseVisualStyleBackColor = true;
        this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
        // 
        // btnRemove
        // 
        this.btnRemove.Location = new System.Drawing.Point(491, 129);
        this.btnRemove.Name = "btnRemove";
        this.btnRemove.Size = new System.Drawing.Size(85, 25);
        this.btnRemove.TabIndex = 10;
        this.btnRemove.Text = "Remove";
        this.btnRemove.UseVisualStyleBackColor = true;
        this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
        // 
        // lblInstallPath
        // 
        this.lblInstallPath.AutoEllipsis = true;
        this.lblInstallPath.AutoSize = true;
        this.lblInstallPath.Location = new System.Drawing.Point(12, 260);
        this.lblInstallPath.Name = "lblInstallPath";
        this.lblInstallPath.Size = new System.Drawing.Size(215, 13);
        this.lblInstallPath.TabIndex = 11;
        this.lblInstallPath.TabStop = true;
        this.lblInstallPath.Text = "No addon install path set. Click to set path...";
        this.lblInstallPath.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
        // 
        // GUI
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(584, 307);
        this.Controls.Add(this.lblInstallPath);
        this.Controls.Add(this.btnRemove);
        this.Controls.Add(this.btnDownload);
        this.Controls.Add(this.lblInstruction);
        this.Controls.Add(this.btnAdd);
        this.Controls.Add(this.btnClear);
        this.Controls.Add(this.btnInstall);
        this.Controls.Add(this.listAddons);
        this.HelpButton = true;
        this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
        this.Name = "GUI";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "Sven Co-op Addon Installer";
        this.Load += new System.EventHandler(this.GUI_Load);
        ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ListBox listAddons;
    private System.Windows.Forms.Button btnInstall;
    private System.Windows.Forms.Button btnClear;
    private System.Windows.Forms.Button btnAdd;
    private System.Windows.Forms.Label lblInstruction;
    private System.Windows.Forms.BindingSource bindingSource1;
    private System.Windows.Forms.Button btnDownload;
    private System.Windows.Forms.Button btnRemove;
    private System.Windows.Forms.LinkLabel lblInstallPath;
}