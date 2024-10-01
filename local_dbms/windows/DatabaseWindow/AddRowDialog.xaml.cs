using local_dbms.core;
using local_dbms.utils;
using System.Windows;
using System.Windows.Controls;


namespace local_dbms.windows.DatabaseWindow
{
	/// <summary>
	/// Interaction logic for AddRowDialog.xaml
	/// </summary>
	public partial class AddRowDialog : Window
	{
		private readonly List<Column> _columns;
		private readonly Func<Row, bool> _addRow;
		private readonly Row _row;

		public AddRowDialog(List<Column> columns, Func<Row, bool> addRow)
		{
			InitializeComponent();
			_columns = columns;
			_addRow = addRow;
			var columnValues = new ColumnValue[columns.Count];

			for (int i = 0; i < columns.Count; i++)
			{
				var c = columns[i];
				columnValues[i] = c.Type.Instance(null, !c.IsNotNull);

				var panel = new StackPanel { Orientation = Orientation.Horizontal };
				panel.Children.Add(new TextBlock { Text = c.Name, Width = 250, Margin = new Thickness(5) });

				var inputBox = new TextBox { Width = 350, Margin = new Thickness(5), Text = c.DefaultValue?.ToString() ?? "" };
				panel.Children.Add(inputBox);

				InputFields.Items.Add(panel);
			}

			_row = new Row(columnValues);
		}

		private void Add_Click(object sender, RoutedEventArgs e)
		{
			for (int i = 0; i < InputFields.Items.Count; i++)
			{
				StackPanel panel = (StackPanel)InputFields.Items[i];
				TextBox? textBox = panel.Children[1] as TextBox;
				bool isValid = _row[i].ParseString(textBox?.Text ?? "");
				if (!isValid)
				{
					WindowUtils.ShowErrorMessage($"Wrong value for '{_columns[i].Name}' column");
					return;
				}
			}

			try
			{
				if (_addRow(_row))
				{
					DialogResult = true;
					Close();
				}

			}
			catch (Exception ex)
			{
				WindowUtils.ShowErrorMessage(ex.Message);
			}
		}
	}

}
