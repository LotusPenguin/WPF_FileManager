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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Globalization;
using System.IO;

namespace WPF_PT_LAB2_ATTEMPT3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new FolderBrowserDialog() { Description = "Select directory to open" };
            dlg.ShowDialog();
            {
                treeView.Items.Clear();

                DirectoryInfo directory = new DirectoryInfo(dlg.SelectedPath);
                var root = BuildDirectoryTree(directory);
                treeView.Items.Add(root);
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private TreeViewItem BuildDirectoryTree(DirectoryInfo directory)
        {
            var root = new TreeViewItem
            {
                Header = directory.Name,
                Tag = directory.FullName
            };

            foreach (DirectoryInfo subdirectory in directory.GetDirectories())
            {
                root.Items.Add(BuildDirectoryTree(subdirectory));
            }
            foreach (FileInfo file in directory.GetFiles())
            {
                root.Items.Add(BuildFileTree(file));
            }

            return root;
        }

        private TreeViewItem BuildFileTree(FileInfo file)
        {
            var item = new TreeViewItem
            {
                Header = file.Name,
                Tag = file.FullName
            };

            return item;
        }
    }
}