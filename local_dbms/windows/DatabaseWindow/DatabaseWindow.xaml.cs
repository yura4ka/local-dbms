using local_dbms.core;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace local_dbms.windows.DatabaseWindow
{
	/// <summary>
	/// Interaction logic for DatabaseWindow.xaml
	/// </summary>
	public partial class DatabaseWindow : Window
	{
		private IDatabase _database;
		public DatabaseWindow(string connectionString)
		{
			InitializeComponent();
			_database = new SqliteDatabase(connectionString);
			PopulateTreeView();
		}

		private void PopulateTreeView()
		{
			foreach (var table in _database.Tables)
			{
				Debug.WriteLine(table.Name);
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
			TableDataGrid.Columns.Clear();
			selectedTable.GetAllRows();

			for (int i = 0; i < selectedTable.Columns.Count; i++)
			{
				var column = selectedTable.Columns[i];
				string headerText = column.Name;
				if (column.IsPk) headerText += " (pk)";
				else if (column.IsNotNull) headerText += " (nn)";

				var dataGridColumn = new DataGridTextColumn
				{
					Header = headerText,
					Binding = new System.Windows.Data.Binding($"[{i}].StringValue")
				};

				TableDataGrid.Columns.Add(dataGridColumn);
			}

			var actionsColumn = new DataGridTemplateColumn
			{
				Header = "Actions",
				CellTemplate = CreateSaveButtonTemplate()
			};
			TableDataGrid.Columns.Add(actionsColumn);

			TableDataGrid.ItemsSource = selectedTable.Rows;
			TableDataGrid.RowStyle = CreateRowStyle();
		}

		private DataTemplate CreateSaveButtonTemplate()
		{
			var buttonFactory = new FrameworkElementFactory(typeof(Button));
			buttonFactory.SetValue(Button.ContentProperty, "Save");
			buttonFactory.SetValue(Button.IsEnabledProperty, new System.Windows.Data.Binding("CanBeSaved"));
			buttonFactory.AddHandler(Button.ClickEvent, new RoutedEventHandler(SaveButton_Click));

			return new DataTemplate { VisualTree = buttonFactory };
		}

		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button button && button.DataContext is Row row)
			{
				Debug.WriteLine(row[0].StringValue);
			}
		}

		private Style CreateRowStyle()
		{
			var style = new Style(typeof(DataGridRow));

			var trigger = new DataTrigger
			{
				Binding = new System.Windows.Data.Binding("IsValid"),
				Value = false
			};
			trigger.Setters.Add(new Setter(BackgroundProperty, Brushes.Red));
			style.Triggers.Add(trigger);

			return style;
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
