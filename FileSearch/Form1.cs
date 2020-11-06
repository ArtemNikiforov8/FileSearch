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
        public Form1()
        {
            InitializeComponent();
        }

        private void ButtonNewSearch_Click(object sender, EventArgs e)
        {
            String directoryName = Path.GetFileName(TextBoxDirectory.Text);
            TempPath = TextBoxDirectory.Text;
            FileNameRegex = TextBoxFileName.Text;
            treeView1.Nodes.Clear();
            treeView1.Nodes.Add(TempPath, directoryName);
            DirectoryInfo directory = new DirectoryInfo(TempPath);
            DirectoryView.ResetCountFiles();
            Pause = false;
            time = 0;
            timer1.Start();
            Task.Run(() => DirectoryView.CheckDirectory(directory, this));
            ButtonStop.Enabled = true;
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

        public String TempPath { get; set; }
        public String FileNameRegex { get; set; }
        public Boolean Pause { get; set; }

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
    }
}
