using Microsoft.Data.Sqlite;


namespace local_dbms.core
{
	internal interface ITableController
	{
		public List<Row> GetAllRows(Table table);
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
				var rowValues = new IColumnValue[table.Columns.Count];
				for (int i = 0; i < rowValues.Length; i++)
				{
					var value = reader.GetString(i);
					rowValues[i] = table.Columns[i].Type.Instance(value);
				}
				result.Add(new Row(rowValues));
			}

			return result;
		}
	}
}
