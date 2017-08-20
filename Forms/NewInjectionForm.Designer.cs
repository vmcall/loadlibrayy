namespace Loadlibrayy.Forms
{
    partial class NewInjectionForm
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
            this.InjectButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.LinkModuleCheckbox = new System.Windows.Forms.CheckBox();
            this.ElevateHandleCheckbox = new System.Windows.Forms.CheckBox();
            this.EraseHeadersCheckbox = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ModeCombo = new System.Windows.Forms.ComboBox();
            this.SelectButton = new System.Windows.Forms.Button();
            this.CreditLabel = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.TypeCombo = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // InjectButton
            // 
            this.InjectButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.InjectButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.InjectButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.InjectButton.Location = new System.Drawing.Point(127, 110);
            this.InjectButton.Name = "InjectButton";
            this.InjectButton.Size = new System.Drawing.Size(70, 23);
            this.InjectButton.TabIndex = 1;
            this.InjectButton.TabStop = false;
            this.InjectButton.Text = "Inject";
            this.InjectButton.UseVisualStyleBackColor = true;
            this.InjectButton.Click += new System.EventHandler(this.InjectButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.LinkModuleCheckbox);
            this.groupBox1.Controls.Add(this.ElevateHandleCheckbox);
            this.groupBox1.Controls.Add(this.EraseHeadersCheckbox);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.groupBox1.Location = new System.Drawing.Point(127, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(128, 92);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Options";
            // 
            // LinkModuleCheckbox
            // 
            this.LinkModuleCheckbox.AutoSize = true;
            this.LinkModuleCheckbox.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.LinkModuleCheckbox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.LinkModuleCheckbox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.LinkModuleCheckbox.Location = new System.Drawing.Point(6, 65);
            this.LinkModuleCheckbox.Name = "LinkModuleCheckbox";
            this.LinkModuleCheckbox.Size = new System.Drawing.Size(119, 17);
            this.LinkModuleCheckbox.TabIndex = 9;
            this.LinkModuleCheckbox.TabStop = false;
            this.LinkModuleCheckbox.Text = "Add loader entry";
            this.LinkModuleCheckbox.UseVisualStyleBackColor = true;
            // 
            // ElevateHandleCheckbox
            // 
            this.ElevateHandleCheckbox.AutoSize = true;
            this.ElevateHandleCheckbox.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.ElevateHandleCheckbox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ElevateHandleCheckbox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.ElevateHandleCheckbox.Location = new System.Drawing.Point(6, 42);
            this.ElevateHandleCheckbox.Name = "ElevateHandleCheckbox";
            this.ElevateHandleCheckbox.Size = new System.Drawing.Size(107, 17);
            this.ElevateHandleCheckbox.TabIndex = 8;
            this.ElevateHandleCheckbox.TabStop = false;
            this.ElevateHandleCheckbox.Text = "Elevate Handle";
            this.ElevateHandleCheckbox.UseVisualStyleBackColor = true;
            // 
            // EraseHeadersCheckbox
            // 
            this.EraseHeadersCheckbox.AutoSize = true;
            this.EraseHeadersCheckbox.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.EraseHeadersCheckbox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.EraseHeadersCheckbox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.EraseHeadersCheckbox.Location = new System.Drawing.Point(6, 19);
            this.EraseHeadersCheckbox.Name = "EraseHeadersCheckbox";
            this.EraseHeadersCheckbox.Size = new System.Drawing.Size(101, 17);
            this.EraseHeadersCheckbox.TabIndex = 7;
            this.EraseHeadersCheckbox.TabStop = false;
            this.EraseHeadersCheckbox.Text = "Erase Headers";
            this.EraseHeadersCheckbox.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.ModeCombo);
            this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBox2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.groupBox2.Location = new System.Drawing.Point(9, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(112, 43);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Mode";
            // 
            // ModeCombo
            // 
            this.ModeCombo.BackColor = System.Drawing.SystemColors.Control;
            this.ModeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ModeCombo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ModeCombo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.ModeCombo.FormattingEnabled = true;
            this.ModeCombo.Items.AddRange(new object[] {
            "Manualmap",
            "Loadlibrary"});
            this.ModeCombo.Location = new System.Drawing.Point(4, 15);
            this.ModeCombo.Name = "ModeCombo";
            this.ModeCombo.Size = new System.Drawing.Size(103, 21);
            this.ModeCombo.TabIndex = 0;
            this.ModeCombo.TabStop = false;
            // 
            // SelectButton
            // 
            this.SelectButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.SelectButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SelectButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.SelectButton.Location = new System.Drawing.Point(51, 110);
            this.SelectButton.Name = "SelectButton";
            this.SelectButton.Size = new System.Drawing.Size(70, 23);
            this.SelectButton.TabIndex = 5;
            this.SelectButton.TabStop = false;
            this.SelectButton.Text = "Select";
            this.SelectButton.UseVisualStyleBackColor = true;
            this.SelectButton.Click += new System.EventHandler(this.SelectButton_Click);
            // 
            // CreditLabel
            // 
            this.CreditLabel.AutoSize = true;
            this.CreditLabel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CreditLabel.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CreditLabel.Location = new System.Drawing.Point(78, 136);
            this.CreditLabel.Name = "CreditLabel";
            this.CreditLabel.Size = new System.Drawing.Size(97, 13);
            this.CreditLabel.TabIndex = 6;
            this.CreditLabel.Text = "By cheat.expert";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.TypeCombo);
            this.groupBox3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBox3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.groupBox3.Location = new System.Drawing.Point(9, 61);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(112, 43);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Thread Type";
            // 
            // TypeCombo
            // 
            this.TypeCombo.BackColor = System.Drawing.SystemColors.Control;
            this.TypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TypeCombo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.TypeCombo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.TypeCombo.FormattingEnabled = true;
            this.TypeCombo.Items.AddRange(new object[] {
            "Create",
            "Hijack"});
            this.TypeCombo.Location = new System.Drawing.Point(4, 15);
            this.TypeCombo.Name = "TypeCombo";
            this.TypeCombo.Size = new System.Drawing.Size(103, 21);
            this.TypeCombo.TabIndex = 0;
            this.TypeCombo.TabStop = false;
            // 
            // NewInjectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(265, 156);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.CreditLabel);
            this.Controls.Add(this.SelectButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.InjectButton);
            this.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "NewInjectionForm";
            this.Opacity = 0D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Loadlibrayy ";
            this.Load += new System.EventHandler(this.NewInjectionForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button InjectButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button SelectButton;
        private System.Windows.Forms.Label CreditLabel;
        private System.Windows.Forms.CheckBox EraseHeadersCheckbox;
        private System.Windows.Forms.CheckBox ElevateHandleCheckbox;
        private System.Windows.Forms.ComboBox ModeCombo;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox TypeCombo;
        private System.Windows.Forms.CheckBox LinkModuleCheckbox;
    }
}