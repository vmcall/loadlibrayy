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

namespace Loadlibrayy.Forms
{
    public partial class TaskListForm : Form
    {
        public Process SelectedProcess { get; set; }
        public TaskListForm()
        {
            InitializeComponent();
        }

        private void TaskListForm_Load(object sender, EventArgs e)      => UpdateTaskList();
        private void buttonRefresh_Click(object sender, EventArgs e)    => UpdateTaskList();

        private void buttonSelectProcess_Click(object sender, EventArgs e)
        {
            SelectedProcess = Process.GetProcessById(Convert.ToInt32(listAllTasks.SelectedItems[0].Text));
            this.Close();
        }

        private void UpdateTaskList()
        {
            listAllTasks.Items.Clear();

            foreach (var process in Process.GetProcesses().OrderBy(x => x.ProcessName))
            {
                listAllTasks.Items.Add(
                    new ListViewItem(new string[] {
                        process.Id.ToString(),
                        process.ProcessName
                    })
                 );
            }
        }
    }
}
