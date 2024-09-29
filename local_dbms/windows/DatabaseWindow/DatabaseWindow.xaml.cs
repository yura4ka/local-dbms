using local_dbms.core;
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

			_tablePanelController.OnChange(rowIndex, columnIndex, newValue);
		}

		private void DisplayColumnData(Column selectedColumn)
		{
			ColumnPanel.Visibility = Visibility.Visible;
			ColumnHeader.Text = selectedColumn.Name;
			ColumnDetails.Text = $"Data Type: {selectedColumn.Type}\nDescription: {selectedColumn.Type}";
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
	}
}
