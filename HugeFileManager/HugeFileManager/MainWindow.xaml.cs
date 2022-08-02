using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.Win32;

namespace HugeFileManager
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

        //Upload file
        private async void btnUploadFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string file = this.txtFilePath.Text;
                Task<FileData> task = Task.Run(() => this.StartAsync(file));
                await Task.WhenAll(task);

                if (task.IsCompleted)
                {
                    var result = from s in FileData.fileDataList
                                 select s;

                    this.dataGrid.ItemsSource = result.ToList();
                    this.btnUploadFile.IsEnabled = false;

                    int records = this.dataGrid.Items.Count;
                    this.txtRecordNumber.Text = $"{records}";
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while processing request: {ex}");
                return;
            }            
        }

        FileData StartAsync(string filePath)
        {
            var result = new FileData();

            using (StreamReader reader = new StreamReader(filePath))
            {
                int counter = 1;

                while (true)
                {
                    string readLine = reader.ReadLine();
                    if (readLine != null)
                    {
                        FileData.GetData(readLine);
                        counter++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return result;
        }

        //Search term
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            this.txtRecordNumber.Text = null;
            if (this.txtUserEntry.Text != null)
            {
                string searchTerm = this.txtUserEntry.Text;
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var filtered = from data in FileData.fileDataList
                                   where data.DataItem.Contains(searchTerm)
                                   select data;

                    this.dataGrid.ItemsSource = filtered.ToList();
                    int records = filtered.Count();
                    this.txtRecordNumber.Text = $"{records}";
                }
                else
                {
                    this.dataGrid.ItemsSource = "No match";
                }
            }
        }

        //Write result into a new file
        private void btnDownloadFile_Click(object sender, RoutedEventArgs e)
        {
            FileStream fs = new FileStream(this.txtFilePathDownload.Text, FileMode.Create, FileAccess.Write);
            StreamWriter objWrite = new StreamWriter(fs);
            objWrite.Write(CreateFile(dataGrid), UnicodeEncoding.UTF8);
            objWrite.Close();
            MessageBox.Show("File saved!");
        }

        private string CreateFile(System.Windows.Controls.DataGrid dataGrid)
        {
            dataGrid.SelectAllCells();
            dataGrid.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
            ApplicationCommands.Copy.Execute(null, dataGrid);
            dataGrid.UnselectAllCells();
            string result = (string)System.Windows.Clipboard.GetData(System.Windows.DataFormats.CommaSeparatedValue);
            return result;
        }

        //Clear set
        private void btnClearSearch_Click(object sender, RoutedEventArgs e)
        {
            this.txtUserEntry.Text = null;
            this.dataGrid.ItemsSource = FileData.fileDataList.ToList();
        }

        private void btnClearUpload_Click(object sender, RoutedEventArgs e)
        {
            this.txtFilePath.Text = null;
            this.btnUploadFile.IsEnabled = true;
        }

        private void btnClearDownload_Click(object sender, RoutedEventArgs e)
        {
            this.txtFilePathDownload.Text = null;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            this.txtFilePath.Text = null;
            this.txtUserEntry.Text = null;
            this.dataGrid.ItemsSource = null;
            this.txtFilePathDownload.Text = null;
            this.btnUploadFile.IsEnabled = true;
            this.btnDownloadFile.IsEnabled = true;
        }

        //Field input
        private void txtUserEntry_TextChanged(object sender, TextChangedEventArgs e) { }
        private void txtFilePath_TextChanged(object sender, TextChangedEventArgs e) { }
        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) { }
        private void txtRecordNumber_TextChanged(object sender, TextChangedEventArgs e) { }
        private void txtFilePathDownload_TextChanged(object sender, TextChangedEventArgs e) { }
    }
}
