using local_dbms.core;
using local_dbms.utils;
using System.Windows;
using System.Windows.Controls;

namespace local_dbms.windows.DatabaseWindow
{
	/// <summary>
	/// Interaction logic for AddTableDialog.xaml
	/// </summary>
	public partial class AddTableDialog : Window
	{
		private readonly ITableController _tableController;
		private readonly Func<Table, bool> _addTable;
		public AddTableDialog(ITableController tableController, Func<Table, bool> addTable)
		{
			_tableController = tableController;
			_addTable = addTable;
			InitializeComponent();
		}

		private static class ElementNames
		{
			public const string ColumnName = "ColumnName";
			public const string DefaultValue = "DefaultValue";
			public const string DataType = "DataType";
			public const string IsPk = "IsPk";
			public const string IsNotNull = "IsNotNull";
			public const string RemoveColumn = "RemoveColumn";
		}

		private void AddColumn_Click(object sender, RoutedEventArgs e)
		{
			var border = new Border
			{
				BorderBrush = SystemColors.ActiveBorderBrush,
				BorderThickness = new Thickness(1),
				Padding = new Thickness(5),
				Margin = new Thickness(0, 5, 0, 5),
			};
			var columnPanel = new StackPanel { Orientation = Orientation.Vertical };

			columnPanel.Children.Add(new TextBlock { Text = "Column Name:", Margin = new Thickness(0, 0, 0, 5) });
			var columnName = new TextBox { Margin = new Thickness(0, 0, 0, 10), Name = ElementNames.ColumnName };
			columnName.LostFocus += ColumnName_LostFocus;
			columnPanel.Children.Add(columnName);

			columnPanel.Children.Add(new TextBlock { Text = "Type:", Margin = new Thickness(0, 0, 0, 5) });
			var typeComboBox = new ComboBox { Margin = new Thickness(0, 0, 0, 10), Name = ElementNames.DataType };
			foreach (var t in TypeManager.AvailableTypes)
			{
				typeComboBox.Items.Add(t);
			}
			typeComboBox.SelectedIndex = 0;
			typeComboBox.SelectionChanged += Type_SelectionChanged;
			columnPanel.Children.Add(typeComboBox);

			columnPanel.Children.Add(new TextBlock { Text = "Default Value:", Margin = new Thickness(0, 0, 0, 5) });
			var defaultValue = new TextBox { Margin = new Thickness(0, 0, 0, 10), Name = ElementNames.DefaultValue };
			defaultValue.LostFocus += DefaultValue_LostFocus;
			columnPanel.Children.Add(defaultValue);

			columnPanel.Children.Add(new RadioButton { Content = "Primary Key", GroupName = "PkGroup", Margin = new Thickness(0, 0, 0, 10), Name = ElementNames.IsPk });

			columnPanel.Children.Add(new CheckBox { Content = "Not Null", Margin = new Thickness(0, 0, 0, 10), Name = ElementNames.IsNotNull });

			var removeBtn = new Button { Content = "Remove", Width = 100, HorizontalAlignment = HorizontalAlignment.Left, Name = ElementNames.RemoveColumn };
			removeBtn.Click += RemoveColumn_Click;
			columnPanel.Children.Add(removeBtn);

			border.Child = columnPanel;
			ColumnsPanel.Children.Add(border);
		}

		private void ColumnName_LostFocus(object sender, RoutedEventArgs e)
		{
			if (sender is not TextBox textBox) return;
			if (textBox.Parent is not StackPanel panel) return;
			if (panel.Parent is not Border border) return;

			int index = ColumnsPanel.Children.IndexOf(border);
			if (index == -1) return;

			textBox.Text = textBox.Text.Trim();
			bool isValid = CheckUniqueName(index, textBox.Text);
			if (!isValid)
			{
				WindowUtils.ShowErrorMessage("The value must be unique!");
				textBox.Text = "";
			}
		}

		private void Type_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (sender is not ComboBox comboBox) return;
			if (comboBox.Parent is not StackPanel panel) return;
			ValidateDefaultValue(panel);
		}

		private void DefaultValue_LostFocus(object sender, RoutedEventArgs e)
		{
			if (sender is not TextBox textBox) return;
			if (textBox.Parent is not StackPanel panel) return;
			if (string.IsNullOrEmpty(textBox.Text.Trim())) return;

			bool isvalid = ValidateDefaultValue(panel);
			if (!isvalid) WindowUtils.ShowErrorMessage("Invalid value for this data type!");
		}

		private static FrameworkElement? FindByName(string name, StackPanel panel)
		{
			foreach (var item in panel.Children)
			{
				if (item is FrameworkElement element)
				{
					if (element.Name == name) return element;
				}
			}
			return null;
		}

		private bool CheckUniqueName(int index, string text)
		{
			if (string.IsNullOrEmpty(text)) return true;
			for (int i = 0; i < ColumnsPanel.Children.Count; i++)
			{
				if (i == index) continue;
				if (ColumnsPanel.Children[i] is not Border border || border.Child is not StackPanel panel) return false;
				if (FindByName(ElementNames.ColumnName, panel) is not TextBox box) return false;
				if (box.Text == text) return false;
			}

			return true;
		}

		private bool ValidateDefaultValue(StackPanel panel)
		{
			if (FindByName(ElementNames.DefaultValue, panel) is not TextBox defaultValueBox) return false;
			if (FindByName(ElementNames.DataType, panel) is not ComboBox typeBox) return false;

			string? dataType = typeBox.SelectedValue?.ToString();
			if (string.IsNullOrEmpty(dataType)) return false;

			var defaultValueObject = TypeManager.TypeMappings[dataType.ToUpper()]().Instance(null, false);
			bool isValid = defaultValueObject.ParseString(defaultValueBox.Text);
			if (!isValid)
			{
				defaultValueBox.Text = "";
				return false;
			}

			defaultValueBox.Text = defaultValueObject.StringValue;
			return true;
		}

		private void RemoveColumn_Click(object sender, EventArgs e)
		{
			if (sender is not Button btn) return;
			if (btn.Parent is not StackPanel panel) return;
			if (panel.Parent is not Border border) return;
			ColumnsPanel.Children.Remove(border);
		}

		private void CreateTable_Click(object sender, RoutedEventArgs e)
		{
			string tableName = TableNameInput.Text.Trim();
			var table = new Table(_tableController, tableName);

			if (!ValidateTableName(tableName))
			{
				WindowUtils.ShowErrorMessage("Invalid table name!");
				return;
			}

			foreach (var item in ColumnsPanel.Children)
			{
				if (item is not Border border || border.Child is not StackPanel panel)
				{
					WindowUtils.ShowErrorMessage("Unknown error!");
					return;
				}

				if (FindByName(ElementNames.ColumnName, panel) is not TextBox columnName) return;
				if (FindByName(ElementNames.DataType, panel) is not ComboBox dataType) return;
				if (FindByName(ElementNames.DefaultValue, panel) is not TextBox defaultValue) return;
				if (FindByName(ElementNames.IsPk, panel) is not RadioButton isPk) return;
				if (FindByName(ElementNames.IsNotNull, panel) is not CheckBox isNotNull) return;
				if (dataType.SelectedValue?.ToString() is not string stringDataType) return;

				var typeObject = TypeManager.TypeMappings[stringDataType.ToUpper()]();
				var defaultValueObject = typeObject.Instance(null, false);
				defaultValueObject.ParseString(defaultValue.Text.Trim());

				var column = new Column(columnName.Text.Trim(),
							typeObject,
							isNotNull.IsChecked ?? false,
							defaultValueObject.ObjectValue,
							isPk.IsChecked ?? false);
				table.AddColumn(column);
			}

			try
			{
				if (_addTable(table))
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

		private bool ValidateTableName(string tableName)
		{
			return !string.IsNullOrEmpty(tableName);
		}
	}
}
