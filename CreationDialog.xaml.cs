using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;

namespace WPF_FileManager
{
    /// <summary>
    /// Interaction logic for CreationDialog.xaml
    /// </summary>
    public partial class CreationDialog : Window
    {
        private string path;
        private bool fileCreated;
        private string newPath;

        public CreationDialog(string path)
        {
            InitializeComponent();
            this.path = path;
            fileCreated = false;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            //regex check
            string input = fileName.Text;
            string baseName = input.Split('.')[0];

            string pattern = "[a-zA-Z0-9_~-]{1,8}[.](txt|php|html)$";
            newPath = path + "\\" + input;

            if ((bool)fileType.IsChecked)
            {
                if (Regex.IsMatch(input, pattern) && baseName.Length <= 8)
                {
                    FileStream newFile = File.Create(newPath);
                    newFile.Close();
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Invalid file name", "File creation error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                DirectoryInfo directory = Directory.CreateDirectory(newPath);
            }

            FileAttributes attributes = File.GetAttributes(newPath);
            if ((bool)ReadOnlyCheck.IsChecked)
                attributes = attributes | FileAttributes.ReadOnly;
            if ((bool)ArchiveCheck.IsChecked)
                attributes = attributes | FileAttributes.Archive;
            if ((bool)HiddenCheck.IsChecked)
                attributes = attributes | FileAttributes.Hidden;
            if ((bool)SystemCheck.IsChecked)
                attributes = attributes | FileAttributes.System;
            File.SetAttributes(newPath, attributes);
            fileCreated = true;

            if (fileCreated)
                Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public bool GetStatus()
        {
            return fileCreated;
        }

        public string GetPath()
        {
            return newPath;
        }
    }
}
