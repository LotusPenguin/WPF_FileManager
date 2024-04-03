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

namespace WPF_FileManager
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

            root.MouseRightButtonDown += new MouseButtonEventHandler(RMB_Click);
            root.Selected += new RoutedEventHandler(LMB_Click);

            root.ContextMenu = new ContextMenu();
            MenuItem createOption = new MenuItem { Header = "Create" };
            MenuItem deleteOption = new MenuItem { Header = "Delete" };

            root.ContextMenu.Items.Add(createOption);
            root.ContextMenu.Items.Add(deleteOption);

            createOption.Click += new RoutedEventHandler(CreateOption_Click);
            deleteOption.Click += new RoutedEventHandler(DeleteOption_Click);

            return root;
        }

        private TreeViewItem BuildFileTree(FileInfo file)
        {
            var item = new TreeViewItem
            {
                Header = file.Name,
                Tag = file.FullName
            };

            item.MouseRightButtonDown += new MouseButtonEventHandler(RMB_Click);
            item.Selected += new RoutedEventHandler(LMB_Click);

            item.ContextMenu = new ContextMenu();
            MenuItem openOption = new MenuItem { Header = "Open" };
            MenuItem deleteOption = new MenuItem { Header = "Delete" };

            item.ContextMenu.Items.Add(openOption);
            item.ContextMenu.Items.Add(deleteOption);

            openOption.Click += new RoutedEventHandler(OpenOption_Click);
            deleteOption.Click += new RoutedEventHandler(DeleteOption_Click);

            return item;
        }   

        private void OpenOption_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem file = (TreeViewItem)treeView.SelectedItem;
            textBlock.Text = File.ReadAllText(file.Tag.ToString());
        }

        private void CreateOption_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem directory = (TreeViewItem)treeView.SelectedItem;

            CreationDialog creationDialog = new CreationDialog(directory.Tag.ToString());
            creationDialog.ShowDialog();

            if(creationDialog.GetStatus())
            {
                if (File.Exists(creationDialog.GetPath()))
                {
                    FileInfo file = new FileInfo(creationDialog.GetPath());
                    directory.Items.Add(BuildFileTree(file));
                }    
                else if (Directory.Exists(creationDialog.GetPath()))
                {
                    DirectoryInfo newDirectory = new DirectoryInfo(creationDialog.GetPath());
                    directory.Items.Add(BuildDirectoryTree(newDirectory));
                }
            }
        }

        private void DeleteOption_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem record = (TreeViewItem)treeView.SelectedItem;
            string path = record.Tag.ToString();
            FileAttributes attributes = File.GetAttributes(path);
            TreeViewItem parent = (TreeViewItem)record.Parent;
            parent.Items.Remove(record);

            RemoveReadOnly(path);
            if ((attributes & FileAttributes.Directory) == FileAttributes.Directory)
            {
                try
                {
                    Directory.Delete(path, true);
                }
                catch
                {
                    RecursiveReadOnlyHandle(path);
                    Directory.Delete(path, true);
                }
            }
            else
                File.Delete(path);
        }

        private void RMB_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
                treeViewItem.Focus();

            PrintArguments();
        }

        private void LMB_Click(object sender, RoutedEventArgs e)
        {
            PrintArguments();
        }

        static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }

        private void RecursiveReadOnlyHandle(string path)
        {
            foreach (string entry in Directory.GetDirectories(path))
                RecursiveReadOnlyHandle(entry);

            foreach (string entry in Directory.GetFiles(path))
                RemoveReadOnly(entry);
        }

        private void RemoveReadOnly(string path)
        {
            FileAttributes attributes = File.GetAttributes(path);
            attributes = attributes & (~FileAttributes.ReadOnly);
            File.SetAttributes(path, attributes);
        }

        private void PrintArguments()
        {
            TreeViewItem selectedItem = (TreeViewItem)treeView.SelectedItem;
            if (File.Exists(selectedItem.Tag.ToString()))
            {
                FileInfo selectedElement = new FileInfo(selectedItem.Tag.ToString());
                statusText.Text = GetRASH(selectedElement);
            }
            else if (Directory.Exists(selectedItem.Tag.ToString()))
            {
                DirectoryInfo selectedElement = new DirectoryInfo(selectedItem.Tag.ToString());
                statusText.Text = GetRASH(selectedElement);
            }
        }

        public static string GetRASH(FileSystemInfo element)
        {
            string output = "";

            if ((element.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                output += "r";
            else
                output += "-";

            if ((element.Attributes & FileAttributes.Archive) == FileAttributes.Archive)
                output += "a";
            else
                output += "-";

            if ((element.Attributes & FileAttributes.System) == FileAttributes.System)
                output += "s";
            else
                output += "-";

            if ((element.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                output += "h";
            else
                output += "-";

            return output;
        }

    }
}