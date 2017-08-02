namespace Loadlibrayy
{
    partial class DankInjectionForm
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
            this.components = new System.ComponentModel.Container();
            this.listImageListView = new System.Windows.Forms.ListView();
            this.columnImagePaths = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonAddImage = new System.Windows.Forms.Button();
            this.buttonInitiateInjection = new System.Windows.Forms.Button();
            this.contextDeleteImage = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonClearImages = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboInjectionMethod = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.comboExecutionMethod = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chkEraseHeaders = new System.Windows.Forms.CheckBox();
            this.chkElevateHandle = new System.Windows.Forms.CheckBox();
            this.buttonSelectProcess = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.contextDeleteImage.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // listImageListView
            // 
            this.listImageListView.BackColor = System.Drawing.Color.White;
            this.listImageListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnImagePaths});
            this.listImageListView.ForeColor = System.Drawing.SystemColors.InfoText;
            this.listImageListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listImageListView.Location = new System.Drawing.Point(16, 18);
            this.listImageListView.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.listImageListView.Name = "listImageListView";
            this.listImageListView.Scrollable = false;
            this.listImageListView.Size = new System.Drawing.Size(364, 139);
            this.listImageListView.TabIndex = 0;
            this.listImageListView.UseCompatibleStateImageBehavior = false;
            this.listImageListView.View = System.Windows.Forms.View.Details;
            this.listImageListView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ListImageListView_MouseDown);
            // 
            // columnImagePaths
            // 
            this.columnImagePaths.Text = "Image Path";
            this.columnImagePaths.Width = 641;
            // 
            // buttonAddImage
            // 
            this.buttonAddImage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.buttonAddImage.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonAddImage.ForeColor = System.Drawing.Color.White;
            this.buttonAddImage.Location = new System.Drawing.Point(396, 17);
            this.buttonAddImage.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.buttonAddImage.Name = "buttonAddImage";
            this.buttonAddImage.Size = new System.Drawing.Size(65, 33);
            this.buttonAddImage.TabIndex = 1;
            this.buttonAddImage.Text = "Add";
            this.buttonAddImage.UseVisualStyleBackColor = false;
            this.buttonAddImage.Click += new System.EventHandler(this.ButtonAddImage_Click);
            // 
            // buttonInitiateInjection
            // 
            this.buttonInitiateInjection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.buttonInitiateInjection.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonInitiateInjection.ForeColor = System.Drawing.Color.White;
            this.buttonInitiateInjection.Location = new System.Drawing.Point(240, 243);
            this.buttonInitiateInjection.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.buttonInitiateInjection.Name = "buttonInitiateInjection";
            this.buttonInitiateInjection.Size = new System.Drawing.Size(88, 38);
            this.buttonInitiateInjection.TabIndex = 2;
            this.buttonInitiateInjection.Text = "x64 Inject";
            this.buttonInitiateInjection.UseVisualStyleBackColor = false;
            this.buttonInitiateInjection.Click += new System.EventHandler(this.ButtonInitiateInjection_Click);
            // 
            // contextImageOptions
            // 
            this.contextDeleteImage.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem1});
            this.contextDeleteImage.Name = "contextImageOptions";
            this.contextDeleteImage.Size = new System.Drawing.Size(108, 26);
            // 
            // deleteToolStripMenuItem1
            // 
            this.deleteToolStripMenuItem1.Name = "deleteToolStripMenuItem1";
            this.deleteToolStripMenuItem1.Size = new System.Drawing.Size(107, 22);
            this.deleteToolStripMenuItem1.Text = "Delete";
            this.deleteToolStripMenuItem1.Click += new System.EventHandler(this.DeleteToolStripMenuItem1_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            // 
            // buttonClearImages
            // 
            this.buttonClearImages.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.buttonClearImages.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonClearImages.ForeColor = System.Drawing.Color.White;
            this.buttonClearImages.Location = new System.Drawing.Point(385, 60);
            this.buttonClearImages.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.buttonClearImages.Name = "buttonClearImages";
            this.buttonClearImages.Size = new System.Drawing.Size(65, 33);
            this.buttonClearImages.TabIndex = 3;
            this.buttonClearImages.Text = "Clear";
            this.buttonClearImages.UseVisualStyleBackColor = false;
            this.buttonClearImages.Click += new System.EventHandler(this.ButtonClearImages_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.groupBox1.Controls.Add(this.comboInjectionMethod);
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(17, 169);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.groupBox1.Size = new System.Drawing.Size(215, 75);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Injection Method";
            // 
            // comboInjectionMethod
            // 
            this.comboInjectionMethod.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.comboInjectionMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboInjectionMethod.FormattingEnabled = true;
            this.comboInjectionMethod.Items.AddRange(new object[] {
            "Load Library",
            "Manual Map"});
            this.comboInjectionMethod.Location = new System.Drawing.Point(9, 33);
            this.comboInjectionMethod.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.comboInjectionMethod.Name = "comboInjectionMethod";
            this.comboInjectionMethod.Size = new System.Drawing.Size(183, 27);
            this.comboInjectionMethod.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.groupBox2.Controls.Add(this.comboExecutionMethod);
            this.groupBox2.ForeColor = System.Drawing.Color.White;
            this.groupBox2.Location = new System.Drawing.Point(240, 169);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.groupBox2.Size = new System.Drawing.Size(215, 64);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Execution Method";
            // 
            // comboExecutionMethod
            // 
            this.comboExecutionMethod.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.comboExecutionMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboExecutionMethod.FormattingEnabled = true;
            this.comboExecutionMethod.Items.AddRange(new object[] {
            "Create Thread",
            "Hijack Thread"});
            this.comboExecutionMethod.Location = new System.Drawing.Point(23, 20);
            this.comboExecutionMethod.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.comboExecutionMethod.Name = "comboExecutionMethod";
            this.comboExecutionMethod.Size = new System.Drawing.Size(183, 27);
            this.comboExecutionMethod.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.groupBox3.Controls.Add(this.chkEraseHeaders);
            this.groupBox3.Controls.Add(this.chkElevateHandle);
            this.groupBox3.ForeColor = System.Drawing.Color.White;
            this.groupBox3.Location = new System.Drawing.Point(17, 252);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.groupBox3.Size = new System.Drawing.Size(203, 93);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Injection Options";
            // 
            // chkEraseHeaders
            // 
            this.chkEraseHeaders.AutoSize = true;
            this.chkEraseHeaders.Location = new System.Drawing.Point(11, 49);
            this.chkEraseHeaders.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.chkEraseHeaders.Name = "chkEraseHeaders";
            this.chkEraseHeaders.Size = new System.Drawing.Size(117, 23);
            this.chkEraseHeaders.TabIndex = 1;
            this.chkEraseHeaders.Text = "Erase headers";
            this.chkEraseHeaders.UseVisualStyleBackColor = true;
            this.chkEraseHeaders.CheckedChanged += new System.EventHandler(this.ChkEraseHeaders_CheckedChanged);
            // 
            // chkElevateHandle
            // 
            this.chkElevateHandle.AutoSize = true;
            this.chkElevateHandle.Location = new System.Drawing.Point(-5, 28);
            this.chkElevateHandle.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.chkElevateHandle.Name = "chkElevateHandle";
            this.chkElevateHandle.Size = new System.Drawing.Size(120, 23);
            this.chkElevateHandle.TabIndex = 0;
            this.chkElevateHandle.Text = "Elevate handle";
            this.chkElevateHandle.UseVisualStyleBackColor = true;
            this.chkElevateHandle.CheckedChanged += new System.EventHandler(this.ChkElevateHandle_CheckedChanged);
            // 
            // buttonSelectProcess
            // 
            this.buttonSelectProcess.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.buttonSelectProcess.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonSelectProcess.ForeColor = System.Drawing.Color.White;
            this.buttonSelectProcess.Location = new System.Drawing.Point(367, 247);
            this.buttonSelectProcess.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.buttonSelectProcess.Name = "buttonSelectProcess";
            this.buttonSelectProcess.Size = new System.Drawing.Size(88, 38);
            this.buttonSelectProcess.TabIndex = 7;
            this.buttonSelectProcess.Text = "Select";
            this.buttonSelectProcess.UseVisualStyleBackColor = false;
            this.buttonSelectProcess.Click += new System.EventHandler(this.ButtonSelectProcess_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.button1.Enabled = false;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(385, 324);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(88, 38);
            this.button1.TabIndex = 8;
            this.button1.Text = "x86 Inject";
            this.button1.UseVisualStyleBackColor = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Wingdings", 9.75F, ((System.Drawing.FontStyle)((((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic) 
                | System.Drawing.FontStyle.Underline) 
                | System.Drawing.FontStyle.Strikeout))), System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.label1.Location = new System.Drawing.Point(155, 1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(180, 15);
            this.label1.TabIndex = 9;
            this.label1.Text = "wav did 9/11";
            // 
            // InjectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::Loadlibrayy.Properties.Resources.ayylmao;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(464, 355);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.buttonSelectProcess);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonClearImages);
            this.Controls.Add(this.buttonInitiateInjection);
            this.Controls.Add(this.buttonAddImage);
            this.Controls.Add(this.listImageListView);
            this.Font = new System.Drawing.Font("Comic Sans MS", 9.75F, ((System.Drawing.FontStyle)((((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic) 
                | System.Drawing.FontStyle.Underline) 
                | System.Drawing.FontStyle.Strikeout))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.MaximizeBox = false;
            this.Name = "InjectionForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Loadlibrayy";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InjectionForm_FormClosing);
            this.Load += new System.EventHandler(this.InjectionForm_Load);
            this.contextDeleteImage.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listImageListView;
        private System.Windows.Forms.ColumnHeader columnImagePaths;
        private System.Windows.Forms.Button buttonAddImage;
        private System.Windows.Forms.Button buttonInitiateInjection;
        private System.Windows.Forms.ContextMenuStrip contextDeleteImage;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem1;
        private System.Windows.Forms.Button buttonClearImages;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboInjectionMethod;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox comboExecutionMethod;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox chkEraseHeaders;
        private System.Windows.Forms.CheckBox chkElevateHandle;
        private System.Windows.Forms.Button buttonSelectProcess;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
    }
}

