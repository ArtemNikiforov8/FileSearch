using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FileSearch
{
    static class DirectoryView
    {
        private delegate void DelegateUpdateLabelTempDirectory(String Text, Form1 Form);
        private delegate void DelegateUpdateLabelCountFiles(Int32 MatchedFiles, Int32 TotalFiles, Form1 Form);
        private static DelegateUpdateLabelTempDirectory myDelegateUpdateDirectory;
        private static DelegateUpdateLabelCountFiles myDelegateUpdateCountFiles;
        private static Int32 totalFiles;
        private static Int32 matchedFiles;

        static DirectoryView()
        {
            myDelegateUpdateDirectory = new DelegateUpdateLabelTempDirectory(FunctionUpdateLabelDirectory);
            myDelegateUpdateCountFiles = new DelegateUpdateLabelCountFiles(FunctionUpdateLabelCountFiles);
        }

        public static async Task CheckDirectory(DirectoryInfo Directory, Form1 Form, CancellationToken Token)
        {
            await Task.Run(() => checkDirectory(Directory, Form, Token));
            if (!Token.IsCancellationRequested)
            {
                Stop(Form);
            }
        }

        private static async Task checkDirectory(DirectoryInfo Directory, Form1 Form, CancellationToken Token)
        {
            try
            {
                if (!Token.IsCancellationRequested)
                {
                    while (Form.Pause)
                    {
                        Thread.Sleep(100);
                    }
                    Form.Invoke(myDelegateUpdateDirectory, Directory.FullName, Form);
                    List<FileInfo> files = new List<FileInfo>();
                    files = Directory.GetFiles().ToList();
                    totalFiles += files.Count();
                    Form.Invoke(myDelegateUpdateCountFiles, matchedFiles, totalFiles, Form);
                    foreach (FileInfo file in files)
                    {
                        if (Regex.IsMatch(file.Name, Form.FileNameRegex))
                        {
                            TreeViewUpdater.AddFileToTreeView(file, Form);
                            ++matchedFiles;
                            Form.Invoke(myDelegateUpdateCountFiles, matchedFiles, totalFiles, Form);
                        }
                    }
                    List<DirectoryInfo> directories = Directory.GetDirectories().ToList();
                    foreach (DirectoryInfo directory in directories)
                    {
                        await Task.Run(() => checkDirectory(directory, Form, Token));
                    }
                }
            }
            catch (Exception exc)
            {

            }
        }

        public static void Abort()
        {
            throw new NotImplementedException();
        }

        private static void FunctionUpdateLabelDirectory(String Text, Form1 Form)
        {
            Form.LabelTempDirectiory.Text = Text;
            Form.LabelTempDirectiory.Invalidate();
            Form.LabelTempDirectiory.Update();
        }

        private static void FunctionUpdateLabelCountFiles(Int32 MatchedFiles, Int32 TotalFiles, Form1 Form)
        {
            Form.LabelCountFiles.Text = String.Format("{0}/{1}", MatchedFiles, TotalFiles);
            Form.LabelCountFiles.Invalidate();
            Form.LabelCountFiles.Update();
        }

        public static void ResetCountFiles()
        {
            totalFiles = 0;
            matchedFiles = 0;
        }

        private static void Stop(Form1 Form)
        {
            Form.timer1.Stop();
            Form.ButtonStop.Enabled = false;
        }
    }
}
