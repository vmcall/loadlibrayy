using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DriverExploits;
using Loadlibrayy.Injection;
using Loadlibrayy.Logger;

namespace Loadlibrayy.Forms
{
    public partial class InjectionForm : Form
    {
        private Process g_SelectedProcess;

        public InjectionForm()
        {
            InitializeComponent();
        }

        private void InjectionForm_Load(object sender, EventArgs e)
        {
            // DEFAULT VALUES
            comboInjectionMethod.SelectedIndex = 1;
            comboExecutionMethod.SelectedIndex = 1;
        }
        

        private void AddToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                Filter = "Dynamic Link Libraries|*.dll",
                Multiselect = false
            };

            if (fileDialog.ShowDialog() == DialogResult.OK)
                listImageListView.Items.Add(new ListViewItem(fileDialog.FileName));
        }

        private void ClearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listImageListView.Items.Clear();
        }

        private void ButtonSelectProcess_Click(object sender, EventArgs e)
        {
            TaskListForm taskListForm = new TaskListForm();
            taskListForm.ShowDialog();
            g_SelectedProcess = taskListForm.SelectedProcess;
        }

        private void ButtonInitiateInjection_Click(object sender, EventArgs e)
        {
            // SANITY CHECKS
            if (g_SelectedProcess == null)
            {
                Log.ShowError("Please select a process!", "Error");
                return;
            }
            if (listImageListView.Items.Count == 0)
            {
                Log.ShowError("Please select atleast one image to inject!", "Error");
                return;
            }

            // LOAD EXPLOITABLE DRIVER 
            bool driverLoaded = false;
            if (chkElevateHandle.Checked)
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

            ExecutionType executionType = 0;
            switch (comboExecutionMethod.SelectedIndex)
            {
                case 0:
                    executionType = ExecutionType.CreateThread;
                    break;

                case 1:
                    executionType = ExecutionType.HijackThread;
                    break;
            }

            IInjectionMethod injectionMethod = null;
            switch (comboInjectionMethod.SelectedIndex)
            {
                case 0: // LOAD LIBRARY
                    injectionMethod = new LoadLibraryInjection(g_SelectedProcess, executionType, chkElevateHandle.Checked, chkEraseHeaders.Checked);
                    break;

                case 1: // MANUAL MAP
                    injectionMethod = new ManualMapInjection(g_SelectedProcess, executionType, chkElevateHandle.Checked, chkEraseHeaders.Checked);
                    break;
            }

            foreach (ListViewItem item in listImageListView.Items)
            {
                injectionMethod.InjectImage(item.Text);
            }

            if (driverLoaded)
                ElevateHandle.Driver.Unload();

            //Environment.Exit(0);
        }

        private ListViewItem g_SelectedListViewItem;
        private void ListImageListView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                g_SelectedListViewItem = listImageListView.GetItemAt(e.X, e.Y);

                if (g_SelectedListViewItem == null)
                {
                    contextNoSelectedImage.Show(listImageListView, e.Location);
                }
                else
                {
                    g_SelectedListViewItem.Selected = true;
                    contextSelectedmage.Show(listImageListView, e.Location);
                }
            }
        }

        private void DeleteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            listImageListView.Items.Remove(g_SelectedListViewItem);
        }

        private void InjectionForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
