namespace Loadlibrayy.Forms
{
    partial class InjectionForm
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
            this.buttonSelectProcess = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chkEraseHeaders = new System.Windows.Forms.CheckBox();
            this.chkElevateHandle = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.comboExecutionMethod = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboInjectionMethod = new System.Windows.Forms.ComboBox();
            this.buttonInitiateInjection = new System.Windows.Forms.Button();
            this.contextSelectedmage = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.contextNoSelectedImage = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.contextSelectedmage.SuspendLayout();
            this.contextNoSelectedImage.SuspendLayout();
            this.SuspendLayout();
            // 
            // listImageListView
            // 
            this.listImageListView.BackColor = System.Drawing.Color.Gray;
            this.listImageListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnImagePaths});
            this.listImageListView.ForeColor = System.Drawing.Color.White;
            this.listImageListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listImageListView.Location = new System.Drawing.Point(16, 19);
            this.listImageListView.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.listImageListView.Name = "listImageListView";
            this.listImageListView.Scrollable = false;
            this.listImageListView.Size = new System.Drawing.Size(484, 190);
            this.listImageListView.TabIndex = 1;
            this.listImageListView.UseCompatibleStateImageBehavior = false;
            this.listImageListView.View = System.Windows.Forms.View.Details;
            this.listImageListView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ListImageListView_MouseDown);
            // 
            // columnImagePaths
            // 
            this.columnImagePaths.Text = "Image Path";
            this.columnImagePaths.Width = 641;
            // 
            // buttonSelectProcess
            // 
            this.buttonSelectProcess.BackColor = System.Drawing.Color.Gray;
            this.buttonSelectProcess.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonSelectProcess.ForeColor = System.Drawing.Color.White;
            this.buttonSelectProcess.Location = new System.Drawing.Point(391, 337);
            this.buttonSelectProcess.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.buttonSelectProcess.Name = "buttonSelectProcess";
            this.buttonSelectProcess.Size = new System.Drawing.Size(87, 40);
            this.buttonSelectProcess.TabIndex = 12;
            this.buttonSelectProcess.Text = "Select";
            this.buttonSelectProcess.UseVisualStyleBackColor = false;
            this.buttonSelectProcess.Click += new System.EventHandler(this.ButtonSelectProcess_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.Gray;
            this.groupBox3.Controls.Add(this.chkEraseHeaders);
            this.groupBox3.Controls.Add(this.chkElevateHandle);
            this.groupBox3.ForeColor = System.Drawing.Color.White;
            this.groupBox3.Location = new System.Drawing.Point(16, 321);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.groupBox3.Size = new System.Drawing.Size(240, 91);
            this.groupBox3.TabIndex = 11;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Injection Options";
            // 
            // chkEraseHeaders
            // 
            this.chkEraseHeaders.AutoSize = true;
            this.chkEraseHeaders.Checked = true;
            this.chkEraseHeaders.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEraseHeaders.Location = new System.Drawing.Point(11, 57);
            this.chkEraseHeaders.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.chkEraseHeaders.Name = "chkEraseHeaders";
            this.chkEraseHeaders.Size = new System.Drawing.Size(120, 22);
            this.chkEraseHeaders.TabIndex = 1;
            this.chkEraseHeaders.Text = "Erase headers";
            this.chkEraseHeaders.UseVisualStyleBackColor = true;
            // 
            // chkElevateHandle
            // 
            this.chkElevateHandle.AutoSize = true;
            this.chkElevateHandle.Location = new System.Drawing.Point(11, 32);
            this.chkElevateHandle.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.chkElevateHandle.Name = "chkElevateHandle";
            this.chkElevateHandle.Size = new System.Drawing.Size(121, 22);
            this.chkElevateHandle.TabIndex = 0;
            this.chkElevateHandle.Text = "Elevate handle";
            this.chkElevateHandle.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Gray;
            this.groupBox2.Controls.Add(this.comboExecutionMethod);
            this.groupBox2.ForeColor = System.Drawing.Color.White;
            this.groupBox2.Location = new System.Drawing.Point(262, 226);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.groupBox2.Size = new System.Drawing.Size(240, 82);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Execution Method";
            // 
            // comboExecutionMethod
            // 
            this.comboExecutionMethod.BackColor = System.Drawing.Color.White;
            this.comboExecutionMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboExecutionMethod.FormattingEnabled = true;
            this.comboExecutionMethod.Items.AddRange(new object[] {
            "Create Thread",
            "Hijack Thread"});
            this.comboExecutionMethod.Location = new System.Drawing.Point(11, 32);
            this.comboExecutionMethod.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.comboExecutionMethod.Name = "comboExecutionMethod";
            this.comboExecutionMethod.Size = new System.Drawing.Size(219, 26);
            this.comboExecutionMethod.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Gray;
            this.groupBox1.Controls.Add(this.comboInjectionMethod);
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(16, 226);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.groupBox1.Size = new System.Drawing.Size(240, 82);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Injection Method";
            // 
            // comboInjectionMethod
            // 
            this.comboInjectionMethod.BackColor = System.Drawing.Color.White;
            this.comboInjectionMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboInjectionMethod.FormattingEnabled = true;
            this.comboInjectionMethod.Items.AddRange(new object[] {
            "Load Library",
            "Manual Map"});
            this.comboInjectionMethod.Location = new System.Drawing.Point(11, 32);
            this.comboInjectionMethod.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.comboInjectionMethod.Name = "comboInjectionMethod";
            this.comboInjectionMethod.Size = new System.Drawing.Size(219, 26);
            this.comboInjectionMethod.TabIndex = 0;
            // 
            // buttonInitiateInjection
            // 
            this.buttonInitiateInjection.BackColor = System.Drawing.Color.Gray;
            this.buttonInitiateInjection.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonInitiateInjection.ForeColor = System.Drawing.Color.White;
            this.buttonInitiateInjection.Location = new System.Drawing.Point(287, 337);
            this.buttonInitiateInjection.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.buttonInitiateInjection.Name = "buttonInitiateInjection";
            this.buttonInitiateInjection.Size = new System.Drawing.Size(87, 40);
            this.buttonInitiateInjection.TabIndex = 8;
            this.buttonInitiateInjection.Text = "x64 Inject";
            this.buttonInitiateInjection.UseVisualStyleBackColor = false;
            this.buttonInitiateInjection.Click += new System.EventHandler(this.ButtonInitiateInjection_Click);
            // 
            // contextSelectedmage
            // 
            this.contextSelectedmage.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem1});
            this.contextSelectedmage.Name = "contextImageOptions";
            this.contextSelectedmage.Size = new System.Drawing.Size(108, 26);
            // 
            // deleteToolStripMenuItem1
            // 
            this.deleteToolStripMenuItem1.Name = "deleteToolStripMenuItem1";
            this.deleteToolStripMenuItem1.Size = new System.Drawing.Size(107, 22);
            this.deleteToolStripMenuItem1.Text = "Delete";
            this.deleteToolStripMenuItem1.Click += new System.EventHandler(this.DeleteToolStripMenuItem1_Click);
            // 
            // contextNoSelectedImage
            // 
            this.contextNoSelectedImage.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.clearToolStripMenuItem});
            this.contextNoSelectedImage.Name = "contextNoSelectedImage";
            this.contextNoSelectedImage.Size = new System.Drawing.Size(102, 48);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(101, 22);
            this.addToolStripMenuItem.Text = "Add";
            this.addToolStripMenuItem.Click += new System.EventHandler(this.AddToolStripMenuItem_Click);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(101, 22);
            this.clearToolStripMenuItem.Text = "Clear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.ClearToolStripMenuItem_Click);
            // 
            // InjectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(520, 426);
            this.Controls.Add(this.buttonSelectProcess);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonInitiateInjection);
            this.Controls.Add(this.listImageListView);
            this.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "InjectionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Loadlibrayy - by Striekcarl\\^genesis^ - Unknowncheats";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.InjectionForm_FormClosed);
            this.Load += new System.EventHandler(this.InjectionForm_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.contextSelectedmage.ResumeLayout(false);
            this.contextNoSelectedImage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listImageListView;
        private System.Windows.Forms.ColumnHeader columnImagePaths;
        private System.Windows.Forms.Button buttonSelectProcess;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox chkEraseHeaders;
        private System.Windows.Forms.CheckBox chkElevateHandle;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox comboExecutionMethod;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboInjectionMethod;
        private System.Windows.Forms.Button buttonInitiateInjection;
        private System.Windows.Forms.ContextMenuStrip contextSelectedmage;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem1;
        private System.Windows.Forms.ContextMenuStrip contextNoSelectedImage;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
    }
}