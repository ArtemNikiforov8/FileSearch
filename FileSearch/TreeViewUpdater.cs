using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileSearch
{
    static class TreeViewUpdater
    {
        private delegate void DelegateAddNode(TreeNode Node, String Key, String Name);
        private static DelegateAddNode myDelegateAddNode;

        static TreeViewUpdater()
        {
            myDelegateAddNode = new DelegateAddNode(AddNodeFunction);
        }

        public static void AddFileToTreeView(FileInfo file, Form1 Form)
        {
            String fullPath = file.FullName.Substring(Form.TempPath.Length + 2);
            List<String> paths = fullPath.Split(new String[] { "\\" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            AddNode(Form.treeView1.Nodes[0], 0, paths, Form);
        }

        private static void AddNode(TreeNode Node, Int32 index, List<String> paths, Form1 Form)
        {
            if (paths.Count > index)
            {
                if (!Node.Nodes.ContainsKey(paths[index]))
                {
                    Form.Invoke(myDelegateAddNode, Node, paths[index], paths[index]);
                }
                AddNode(Node.Nodes[paths[index]], ++index, paths, Form);
            }
        }

        private static void AddNodeFunction(TreeNode Node, String Key, String Name)
        {
            Node.Nodes.Add(Key, Name);
        }

    }
}
