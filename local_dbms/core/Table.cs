namespace local_dbms.core
{
	internal class Table
	{
		private readonly ITableController _tableController;
		private string _name;
		private List<Column> _columns;
		private List<Row> _rows;

		public Table(ITableController tableController, string name)
		{
			_tableController = tableController;
			_name = name;
			_columns = [];
			_rows = [];
		}

		public string Name => _name;
		public List<Column> Columns => _columns;
		public List<Row> Rows => _rows;

		public void AddColumn(Column column)
		{
			_columns.Add(column);
		}

		public void GetAllRows()
		{
			_rows = _tableController.GetAllRows(this);
		}
	}
}
