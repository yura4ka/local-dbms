﻿using Microsoft.Data.Sqlite;

namespace local_dbms.core
{
	internal interface ITableController
	{
		public List<Row> GetAllRows(Table table);
		public bool SaveCell(Table table, int row, int column);
		public bool ChangePrimaryKey(Table table, int row, int column, object? newPk);
		public bool AddRow(Table table, Row row);
	}

	internal class SqliteTableController : ITableController
	{
		private readonly SqliteConnection _connection;

		public SqliteTableController(SqliteConnection connection)
		{
			_connection = connection;
		}

		public List<Row> GetAllRows(Table table)
		{
			var result = new List<Row>();

			var command = _connection.CreateCommand();
			command.CommandText = $"SELECT * FROM {table.Name}";
			using var reader = command.ExecuteReader();
			while (reader.Read())
			{
				var rowValues = new ColumnValue[table.Columns.Count];
				for (int i = 0; i < rowValues.Length; i++)
				{
					var value = reader.IsDBNull(i) ? null : reader.GetString(i);
					rowValues[i] = table.Columns[i].Type.Instance(value, !table.Columns[i].IsNotNull);
				}
				result.Add(new Row(rowValues));
			}

			return result;
		}

		public bool SaveCell(Table table, int row, int column)
		{
			int pkIndex = table.Columns.FindIndex(c => c.IsPk);
			if (pkIndex == -1)
				throw new PkNotFoundException(table.Name);

			string pkColumnName = table.Columns[pkIndex].Name;
			object? pkValue = table.Rows[row][pkIndex].ObjectValue;

			string columnName = table.Columns[column].Name;
			object? value = table.Rows[row][column].ObjectValue;

			var command = CreateUpdateCommand(table.Name, columnName, pkColumnName, value, pkValue);
			return command.ExecuteNonQuery() == 1;
		}

		public bool ChangePrimaryKey(Table table, int row, int column, object? newPk)
		{
			string columnName = table.Columns[column].Name;
			object? oldPk = table.Rows[row][column].ObjectValue;

			var command = CreateUpdateCommand(table.Name, columnName, columnName, newPk, oldPk);
			return command.ExecuteNonQuery() == 1;
		}

		public bool AddRow(Table table, Row row)
		{
			var columns = table.Columns.Select(c => c.Name);
			var values = row.Select(c => c.ObjectValue).ToList();
			var paramNames = columns.Select(c => $"${c}").ToList();

			var command = _connection.CreateCommand();
			command.CommandText = $"INSERT INTO {table.Name} ({string.Join(',', columns)}) VALUES ({string.Join(',', paramNames)})";
			for (int i = 0; i < values.Count; i++)
				command.Parameters.AddWithValue(paramNames[i], values[i] ?? DBNull.Value);

			return command.ExecuteNonQuery() == 1;
		}

		private SqliteCommand CreateUpdateCommand(string tableName, string columnName, string pkColumnName, object? newValue, object? pkValue)
		{
			var command = _connection.CreateCommand();
			command.CommandText = $"UPDATE {tableName} SET {columnName} = $value WHERE {pkColumnName} = $id";
			command.Parameters.AddWithValue("$value", newValue ?? DBNull.Value);
			command.Parameters.AddWithValue("$id", pkValue);
			return command;
		}
	}
}
