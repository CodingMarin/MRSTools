using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace MRSTools
{
    public partial class main : Form
    {
        public main()
        {
            InitializeComponent();
        }

        ArrayList Paths = new ArrayList();

        private void RefreshMe()
        {
            listView1.Clear();
            string[] files = System.IO.Directory.GetFiles(Application.StartupPath, "*.mrs");
            foreach (string file in files)
            {
                Paths.Add(file);
                listView1.Items.Add(new ListViewItem(Path.GetFileName(file), 1));
            }

            List<string> dirs = new List<string>(Directory.EnumerateDirectories(Application.StartupPath));
            foreach (string file in dirs)
            {
                Paths.Add(file);
                listView1.Items.Add(new ListViewItem(Path.GetFileName(file), 0));
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!File.Exists(Application.StartupPath + "/zlib.dll"))
                File.WriteAllBytes(Application.StartupPath + "/zlib.dll", Properties.Resources.zlib);
            if (!File.Exists(Application.StartupPath + "/encryptcustom.dll"))
                File.WriteAllBytes(Application.StartupPath + "/encryptcustom.dll", Properties.Resources.encryptcustom);
            if (!File.Exists(Application.StartupPath + "/mrsencryption.exe"))
                File.WriteAllBytes(Application.StartupPath + "/mrsencryption.exe", Properties.Resources.mrsencryption);
            imageList1.Images.Add(Properties.Resources.unpacked);
            imageList1.Images.Add(Properties.Resources.packed);
            RefreshMe();
        }

        private void decompileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                if (Path.GetExtension(listView1.SelectedItems[0].Text) != ".mrs")
                {
                    MessageBox.Show("Thats Not A Mrs File.");
                }
                Thread myNewThread = new Thread(() => ExecuteCMD(Path.GetFileNameWithoutExtension(listView1.SelectedItems[0].Text), "mrsencryption.exe d " + listView1.SelectedItems[0].Text));
                myNewThread.Start();
            }
        }
        private void UpdateText(string text)
        {
            if (text.Split(':')[1] == " ")
            {
                richTextBox1.AppendText(text + "Completed" + Environment.NewLine);
                RefreshMe();
                Process[] lol = Process.GetProcessesByName("mrsencryption");
                if (lol.Length > 0)
                    lol[0].Kill();
            }
            else
                richTextBox1.AppendText(text + Environment.NewLine);
        }
        public delegate void UpdateTextCallback(string text);

        private void ExecuteCMD(string Name, string Command)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("cmd", "/c " + Command)
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            Process process = Process.Start(startInfo);
            process.OutputDataReceived += (sender, e) => richTextBox1.Invoke(new UpdateTextCallback(this.UpdateText), new object[] { "[" + Name + "]: " + e.Data });
            process.BeginOutputReadLine();
            process.Start();
            process.WaitForExit();
        }

        private void compileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                if (Path.GetExtension(listView1.SelectedItems[0].Text) != ".mrs")
                {
                    Thread myNewThread = new Thread(() => ExecuteCMD(Path.GetFileNameWithoutExtension(listView1.SelectedItems[0].Text), "mrsencryption.exe c " + listView1.SelectedItems[0].Text));
                    myNewThread.Start();
                }
                else MessageBox.Show("Thats A Mrs File.");
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (File.Exists(Application.StartupPath + "/zlib.dll"))
                File.Delete(Application.StartupPath + "/zlib.dll");
            if (File.Exists(Application.StartupPath + "/encryptcustom.dll"))
                File.Delete(Application.StartupPath + "/encryptcustom.dll");
            if (File.Exists(Application.StartupPath + "/mrsencryption.exe"))
                File.Delete(Application.StartupPath + "/mrsencryption.exe");
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshMe();
        }

        private void decompileAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Decompile All?", "Decsion", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string[] files = System.IO.Directory.GetFiles(Application.StartupPath+"", "*.mrs", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    ExecuteCMD(Path.GetFileNameWithoutExtension(file), "mrsencryption.exe d " + file.Replace(Application.StartupPath + @"\", ""));
                }
            }
        }
    }
}