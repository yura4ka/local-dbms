using local_dbms.core;
using System.Data;
using System.Data.Common;

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
			Data.Columns.Clear();
			Data.Rows.Clear();

			foreach (var column in selectedTable.Columns)
			{
				string columnName = column.Name;
				if (column.IsPk) columnName += " (pk)";
				else if (column.IsNotNull) columnName += " (nn)";
				Data.Columns.Add(columnName, typeof(string));
			}

			foreach (var row in selectedTable.Rows)
			{
				var dataRow = Data.NewRow();
				for (int i = 0; i < selectedTable.Columns.Count; i++)
				{
					dataRow[i] = row[i];
				}
				Data.Rows.Add(dataRow);
			}
		}

		public void OnChange(int row, int column, string value)
		{
			if (SelectedTable == null) return;

			bool isValid = SelectedTable.Rows[row].UpdateValue(column, value);
			Data.Rows[row][column] = SelectedTable.Rows[row][column];
		}
	}
}
