using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace local_dbms.dialogs
{
	/// <summary>
	/// Interaction logic for CreateDatabaseDialog.xaml
	/// </summary>
	public partial class CreateDatabaseDialog : Window
	{
		public string DatabaseName { get => DatabaseNameTextBox.Text; set => DatabaseNameTextBox.Text = value; }

		public CreateDatabaseDialog()
		{
			InitializeComponent();
		}

		private bool ValidateFilename(string fileName)
		{
			if (Path.Exists(Path.GetFullPath(fileName))) return false;
			return !string.IsNullOrWhiteSpace(fileName);
		}

		private string FormatFilename(string fileName)
		{
			fileName = fileName.Trim();
			if (!fileName.EndsWith(".db")) fileName = fileName + ".db";
			return fileName;
		}

		private void CreateButton_Click(object sender, RoutedEventArgs e)
		{
			DatabaseName = FormatFilename(DatabaseName);
			if (ValidateFilename(DatabaseName))
			{
				DialogResult = true;
				Close();
			}
			else
			{
				MessageBox.Show("Please enter a valid database name.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}
	}
}
