using local_dbms.core;
using System.Data;

namespace local_dbms.windows.DatabaseWindow
{
	class TablePanelController
	{
		public Table? SelectedTable { get; private set; } = null;

		public DataTable Data { get; private set; } = new DataTable();

		public void SetTableData(Table selectedTable)
		{
			SelectedTable = selectedTable;
			selectedTable.GetAllRows();
			Data = new DataTable();

			foreach (var column in selectedTable.Columns)
			{
				string columnName = column.Name.Replace("_", "__");
				if (column.IsPk) columnName += " (pk)";
				else if (column.IsNotNull) columnName += " (nn)";
				Data.Columns.Add(columnName, typeof(string));
			}

			foreach (var row in selectedTable.Rows)
			{
				AddDataRow(row);
			}
		}

		public bool OnChange(int row, int column, string value)
		{
			if (SelectedTable == null) return false;
			bool isValid;

			try
			{
				isValid = SelectedTable.ChangeCell(row, column, value);
			}
			finally
			{
				Data.Rows[row][column] = SelectedTable.Rows[row][column].StringValue;
			}

			return isValid;
		}

		public bool AddRow(Row row)
		{
			if (SelectedTable == null) return false;

			bool isValid = SelectedTable.AddRow(row);
			if (isValid) AddDataRow(row);
			return isValid;
		}

		public void DeleteRow(int row)
		{
			if (SelectedTable == null) return;
			SelectedTable.DeleteRow(row);
			Data.Rows.RemoveAt(row);
		}

		public void DropTable()
		{
			if (SelectedTable == null) return;
			SelectedTable = null;
			Data.Columns.Clear();
			Data.Rows.Clear();
		}

		private void AddDataRow(Row row)
		{
			var dataRow = Data.NewRow();
			for (int i = 0; i < SelectedTable!.Columns.Count; i++)
			{
				dataRow[i] = row[i].StringValue;
			}
			Data.Rows.Add(dataRow);
		}
	}
}
