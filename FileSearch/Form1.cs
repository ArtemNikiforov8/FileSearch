using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileSearch
{
    public partial class Form1 : Form
    {
        private Int32 time;
        CancellationTokenSource cancelTokenSource;
        Task task;
        Boolean programWorking;
        public Form1()
        {
            cancelTokenSource = new CancellationTokenSource();
            Token = cancelTokenSource.Token;
            programWorking = false;
            InitializeComponent();
        }

        private void ButtonNewSearch_Click(object sender, EventArgs e)
        {
            if (programWorking)
            {
                cancelTokenSource.Cancel();
                Thread.Sleep(1000);
            }
            cancelTokenSource = new CancellationTokenSource();
            String directoryName = Path.GetFileName(TextBoxDirectory.Text);
            TempPath = TextBoxDirectory.Text;
            FileNameRegex = TextBoxFileName.Text;
            if (Directory.Exists(TempPath))
            {
                treeView1.Nodes.Clear();
                treeView1.Nodes.Add(TempPath, directoryName);
                DirectoryInfo directory = new DirectoryInfo(TempPath);
                DirectoryView.ResetCountFiles();
                Pause = false;
                time = 0;
                timer1.Start();
                task = Task.Run(() => DirectoryView.CheckDirectory(directory, this, cancelTokenSource.Token));
                programWorking = true;
                ButtonStop.Enabled = true;
            }
            else
            {
                MessageBox.Show("Неверно задан путь");
            }
            
        }

        private void ButtonStop_Click(object sender, EventArgs e)
        {
            if (Pause)
            {
                Pause = false;
                timer1.Start();
            }
            else
            {
                Pause = true;
                timer1.Stop();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ++time;
            LabelTime.Text = String.Format("{0}:{1}", time / 60, time % 60);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TextBoxDirectory.Text = Properties.Settings.Default.TextBox_Directory;
            TextBoxFileName.Text = Properties.Settings.Default.TextBox_FileName;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.TextBox_Directory = TextBoxDirectory.Text;
            Properties.Settings.Default.TextBox_FileName = TextBoxFileName.Text;
            Properties.Settings.Default.Save();
        }

        public String TempPath { get; set; }
        public String FileNameRegex { get; set; }
        public Boolean Pause { get; set; }
        public CancellationToken Token { get; set; }
    }
}
