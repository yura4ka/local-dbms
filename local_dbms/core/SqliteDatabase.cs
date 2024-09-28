using Microsoft.Data.Sqlite;

namespace local_dbms.core
{
	internal class SqliteDatabase : IDatabase
	{
		private readonly SqliteConnection _connection;
		private readonly ITableController _tableController;
		private readonly List<Table> _tables;

		public List<Table> Tables => _tables;

		public SqliteDatabase(string name)
		{
			_tables = [];
			_connection = new SqliteConnection($"Data Source={name}");
			_tableController = new SqliteTableController(_connection);
			_connection.Open();
			InitDatabase();
		}

		private void InitDatabase()
		{
			var getTablesCommand = _connection.CreateCommand();
			getTablesCommand.CommandText = "SELECT name FROM sqlite_master WHERE type='table';";

			using var tableReader = getTablesCommand.ExecuteReader();
			while (tableReader.Read())
			{
				string tableName = tableReader.GetString(0);
				var table = new Table(_tableController, tableName);

				var getColumnInfoCommand = _connection.CreateCommand();
				getColumnInfoCommand.CommandText = $"PRAGMA table_info({tableName});";

				using var columnReader = getColumnInfoCommand.ExecuteReader();
				while (columnReader.Read())
				{
					string columnName = columnReader.GetString(1);
					string columnType = columnReader.GetString(2);
					bool notNull = columnReader.GetBoolean(3);
					string? defaultValue = columnReader.IsDBNull(4) ? null : columnReader.GetString(4);
					bool isPrimaryKey = columnReader.GetBoolean(5);

					var column = new Column(columnName, TypeManager.TypeMappings[columnType](), notNull, defaultValue, isPrimaryKey);
					table.AddColumn(column);
				}
				AddTable(table);
			}
		}

		private void AddTable(Table table)
		{
			_tables.Add(table);
		}

		public void Dispose()
		{
			_connection.Dispose();
		}
	}
}
