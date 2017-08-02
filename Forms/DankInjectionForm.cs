using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Loadlibrayy.Injection;
using System.Diagnostics;
using System.Media;
using Loadlibrayy.Properties;
using Loadlibrayy.Forms;
using Loadlibrayy.Logger;
using DriverExploits;

namespace Loadlibrayy
{
    public partial class DankInjectionForm : Form
    {
        public DankInjectionForm()
        {
            InitializeComponent();
        }

        private SoundPlayer g_PhotonSoundPlayer;
        private void InjectionForm_Load(object sender, EventArgs e)
        {
            // [5:13 PM] aixxe: it should also appear slightly off screen when you open it
            this.SetDesktopLocation(-100, 0);

            g_PhotonSoundPlayer = new SoundPlayer(Resources.photonsound);

            // [12:33] wav: Your theme song needs work...
            // new SoundPlayer(Resources.Hey_Everybody__Im_looking_at_gay_porno_).PlayLooping();

            // DEFAULT VALUES
            comboInjectionMethod.SelectedIndex = 0;
            comboExecutionMethod.SelectedIndex = 0;

            Random rand = new Random();

            var wingdingsFont = new Font("Wingdings", 15);
            var comicsansFont = new Font("Comic Sans MS", 15);

            Timer timer = new Timer();
            timer.Tick += delegate
            {
                // FOR AYYFECT
                buttonAddImage.ForeColor = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
                 buttonClearImages.ForeColor = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
                buttonInitiateInjection.ForeColor = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
                buttonSelectProcess.ForeColor = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
                chkElevateHandle.ForeColor = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
                chkEraseHeaders.ForeColor = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
                groupBox1.ForeColor = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
                groupBox2.ForeColor = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
                groupBox3.ForeColor = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
                this.BackgroundImage = Environment.TickCount % 2 == 0 ? Resources.ayylmao : Resources.ayylmao2;
                this.label1.Font = Environment.TickCount % 2 == 0 ? wingdingsFont : comicsansFont;
                this.label1.ForeColor = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
                this.label1.BackColor = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
            };
            timer.Start();
        }

        private void InjectionForm_FormClosing(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }
        private Process g_SelectedProcess;
        private void ButtonInitiateInjection_Click(object sender, EventArgs e)
        {
            // SANITY CHECKS
            if (g_SelectedProcess == null)
            {
                Log.ShowError("Please select a process!", "Lol are you fucking retarded");
                return;
            }
            if (listImageListView.Items.Count == 0)
            {
                Log.ShowError("Please select atleast one image to inject!", "Lol are you fucking retarded");
                return;
            }

            // Theme Song ;)
            // SoundPlayer soundPlayer = new SoundPlayer(Resources.Le_Bretonniere);
            // soundPlayer.Play();

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
                if (injectionMethod.InjectImage(item.Text))
                    Log.ShowInformation($"Successfully injected {item.Text} -> {g_SelectedProcess.ProcessName}", "Success");
                else
                    Log.ShowError($"Failed injection {item.Text} -> {g_SelectedProcess.ProcessName}", "fuck");

            }
                
            if (driverLoaded)
                ElevateHandle.Driver.Unload();
        }

        private void ButtonAddImage_Click(object sender, EventArgs e)
        {
            g_PhotonSoundPlayer.Play();

            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                Filter = "Dynamic Link Libraries|*.dll",
                Multiselect = false
            };

            if (fileDialog.ShowDialog() == DialogResult.OK)
                listImageListView.Items.Add(new ListViewItem(fileDialog.FileName));
        }
        private void ButtonClearImages_Click(object sender, EventArgs e)
        {
            g_PhotonSoundPlayer.Play();
            listImageListView.Items.Clear();
        }

        private ListViewItem g_SelectedListViewItem;
        private void DeleteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            g_PhotonSoundPlayer.Play();
            listImageListView.Items.Remove(g_SelectedListViewItem);

            g_SelectedListViewItem = null;
        }

        private void ListImageListView_MouseDown(object sender, MouseEventArgs e)
        {
            g_PhotonSoundPlayer.Play();
            if (e.Button == MouseButtons.Right)
            {
                g_SelectedListViewItem = listImageListView.GetItemAt(e.X, e.Y);

                if (g_SelectedListViewItem == null)
                    return;

                g_SelectedListViewItem.Selected = true;
                contextDeleteImage.Show(listImageListView, e.Location);
            }
        }
        
        private void ButtonSelectProcess_Click(object sender, EventArgs e)
        {
            g_PhotonSoundPlayer.Play();
            TaskListForm taskListForm = new TaskListForm();
            taskListForm.ShowDialog();
            g_SelectedProcess = taskListForm.SelectedProcess;
        }

        private void ChkElevateHandle_CheckedChanged(object sender, EventArgs e)
        {
            g_PhotonSoundPlayer.Play();
            if (chkElevateHandle.Checked)
                MessageBox.Show(
                    "Kernel-land is dangerous for children, don't come crying if you blue screen (faggot)", 
                    "You are entering dangerous waters", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        private void ChkEraseHeaders_CheckedChanged(object sender, EventArgs e)
        {
            g_PhotonSoundPlayer.Play();
        }


        private void InjectionForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //new SoundPlayer(Resources.windows_xp_shutdown).PlaySync();
            Environment.Exit(0);
        }

        
    }
}
