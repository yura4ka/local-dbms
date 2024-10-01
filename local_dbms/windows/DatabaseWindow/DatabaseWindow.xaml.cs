using local_dbms.core;
using local_dbms.utils;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace local_dbms.windows.DatabaseWindow
{
	/// <summary>
	/// Interaction logic for DatabaseWindow.xaml
	/// </summary>
	public partial class DatabaseWindow : Window
	{
		private readonly IDatabase _database;
		private readonly TablePanelController _tablePanelController;

		public DatabaseWindow(string connectionString)
		{
			InitializeComponent();

			_tablePanelController = new TablePanelController();
			_database = new SqliteDatabase(connectionString);

			TableDataGrid.ItemsSource = _tablePanelController.Data.DefaultView;
			PopulateTreeView();
		}

		private void PopulateTreeView()
		{
			foreach (var table in _database.Tables)
			{
				var treeViewItem = new TreeViewItem
				{
					Header = table.Name,
					Tag = table
				};

				foreach (var column in table.Columns)
				{
					var columnItem = new TreeViewItem
					{
						Header = column.Name,
						Tag = column
					};
					treeViewItem.Items.Add(columnItem);
				}

				DatabaseTreeView.Items.Add(treeViewItem);
			}
		}

		private void DisplayTableData(Table selectedTable)
		{
			TablePanel.Visibility = Visibility.Visible;
			TableHeader.Text = selectedTable.Name;

			_tablePanelController.SetTableData(selectedTable);
		}

		private void TableDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
		{
			var rowIndex = e.Row.GetIndex();
			var columnIndex = e.Column.DisplayIndex;
			var editingElement = e.EditingElement as TextBox;
			string? newValue = editingElement?.Text;
			if (newValue == null) return;

			try
			{
				bool isValid = _tablePanelController.OnChange(rowIndex, columnIndex, newValue);
				if (!isValid) WindowUtils.ShowErrorMessage("Wrong value");
			}
			catch (Exception ex)
			{
				WindowUtils.ShowErrorMessage(ex.Message);
			}
		}

		private void DisplayColumnData(Column selectedColumn)
		{
			ColumnPanel.Visibility = Visibility.Visible;
			ColumnHeader.Text = selectedColumn.Name;
			ColumnDetails.Text = $"Data Type: {selectedColumn.Type.Name}";
		}

		private void DeleteRow_Click(object sender, RoutedEventArgs e)
		{
			if (!(sender is Button button && button.Tag is DataRowView rowView)) return;

			int rowIndex = TableDataGrid.Items.IndexOf(rowView);
			var result = MessageBox.Show("Are you sure you want to delete the row? This action cannot be undone.", "Delete row", MessageBoxButton.YesNo, MessageBoxImage.Warning);
			if (result != MessageBoxResult.Yes) return;

			try
			{
				_tablePanelController.DeleteRow(rowIndex);
			}
			catch (Exception ex)
			{
				WindowUtils.ShowErrorMessage(ex.Message);
			}
		}

		private void DatabaseTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if (DatabaseTreeView.SelectedItem is TreeViewItem selectedItem)
			{
				TablePanel.Visibility = Visibility.Collapsed;
				ColumnPanel.Visibility = Visibility.Collapsed;

				if (selectedItem.Tag is Table selectedTable)
					DisplayTableData(selectedTable);
				else if (selectedItem.Tag is Column selectedColumn)
					DisplayColumnData(selectedColumn);
			}
		}

		private void AddButton_Click(object sender, RoutedEventArgs e)
		{
			if (_tablePanelController.SelectedTable == null) return;
			var dialog = new AddRowDialog(_tablePanelController.SelectedTable.Columns, _tablePanelController.AddRow);
			dialog.ShowDialog();
		}
	}
}
