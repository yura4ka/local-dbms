using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Xml.Linq;
using local_dbms.dialogs;
using local_dbms.windows.DatabaseWindow;
using Microsoft.Data.Sqlite;

namespace local_dbms
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			LoadDbFiles();
		}

		private void LoadDbFiles()
		{
			var dbFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.db")
								   .Select(f => new DbFile { Name = Path.GetFileName(f) })
								   .ToList();
			DbFilesDataGrid.ItemsSource = dbFiles;
		}

		private void NavigateToDatabase(string connectionString)
		{
			var dbWindow = new DatabaseWindow(connectionString);
			Close();
			dbWindow.Show();
		}

		private void CreateDatabaseButton_Click(object sender, RoutedEventArgs e)
		{
			var createDbDialog = new CreateDatabaseDialog();
			if (createDbDialog.ShowDialog() == true)
			{
				var dbName = createDbDialog.DatabaseName;
				NavigateToDatabase(dbName);
			}
		}

		private void ConnectButton_Click(object sender, RoutedEventArgs e)
		{
			var button = sender as Button;
			var fileName = button?.CommandParameter.ToString();
			if (!string.IsNullOrEmpty(fileName))
			{
				NavigateToDatabase(fileName);
			}
		}

		private void RemoveButton_Click(object sender, RoutedEventArgs e)
		{
			var button = sender as Button;
			var fileName = button?.CommandParameter.ToString();
			if (!string.IsNullOrEmpty(fileName))
			{
				File.Delete(Path.Combine(Directory.GetCurrentDirectory(), fileName));
				LoadDbFiles();
			}
		}
	}

	public class DbFile
	{
		public required string Name { get; set; }
	}
}