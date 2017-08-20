using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DriverExploits;
using Loadlibrayy.Injection;
using Loadlibrayy.Logger;

namespace Loadlibrayy.Forms
{
    public partial class NewInjectionForm : Form
    {
        private Process g_SelectedProcess;

        public NewInjectionForm()
        {
            InitializeComponent();
        }

        private void InjectButton_Click(object sender, EventArgs e)
        {
            // SANITY CHECKS
            if (g_SelectedProcess == null)
            {
                Log.ShowError("Select a process!", "Error");
                return;
            }

            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                Filter = "Dynamic Link Library|*.dll",
                Multiselect = false
            };

            if (fileDialog.ShowDialog() != DialogResult.OK)
                return;
            
            // LOAD EXPLOITABLE DRIVER 
            bool driverLoaded = false;
            if (ElevateHandleCheckbox.Checked)
            {
                if (!(driverLoaded = ElevateHandle.Driver.Load()))
                {
                    Log.ShowError("CPUZ141.sys failed to load", "lol fuck");
                    return;
                }

                ElevateHandle.UpdateDynamicData(); // UPDATE KERNEL OFFSETS
                ElevateHandle.Attach();            // ATTACH TO CURRENT PROCESS
                ElevateHandle.Elevate((ulong)g_SelectedProcess.Handle, 0x1fffff);
            }

            InjectionOptions options = new InjectionOptions()
            {
                ElevateHandle = ElevateHandleCheckbox.Checked,
                EraseHeaders = EraseHeadersCheckbox.Checked,
                CreateLoaderReference = LinkModuleCheckbox.Checked,
                LoaderImagePath = fileDialog.FileName
            };

            ExecutionType executionType = 0;
            switch (TypeCombo.SelectedIndex)
            {
                case 0:
                    executionType = ExecutionType.CreateThread;
                    break;

                case 1:
                    executionType = ExecutionType.HijackThread;
                    break;
            }

            IInjectionMethod injectionMethod = null;
            switch (ModeCombo.SelectedIndex)
            {
                case 0: // MANUAL MAP
                    injectionMethod = new ManualMapInjection(g_SelectedProcess, executionType, options);
                    break;

                case 1: // LOAD LIBRARY
                    injectionMethod = new LoadLibraryInjection(g_SelectedProcess, executionType, options);
                    break;
            }

            injectionMethod.InjectImage(fileDialog.FileName);

            if (driverLoaded)
                ElevateHandle.Driver.Unload();
        }
        private void SelectButton_Click(object sender, EventArgs e)
        {
            TaskListForm taskListForm = new TaskListForm();
            taskListForm.ShowDialog();

            if (taskListForm.SelectedProcess != null)
                g_SelectedProcess = taskListForm.SelectedProcess;
        }

        private void NewInjectionForm_Load(object sender, EventArgs e)
        {
            this.ModeCombo.SelectedIndex = 0;
            this.TypeCombo.SelectedIndex = 0;

            Random random = new Random();
            
            // FADE FORM
            Timer fadeTimer = new Timer();
            fadeTimer.Tick += delegate
            {
                this.Opacity += 0.1;
                this.CreditLabel.ForeColor = Color.FromArgb((this.CreditLabel.ForeColor.R + 10) % 255, 0, 0);
            };
            fadeTimer.Start();
        }

        
    }
}
