namespace local_dbms.core
{
	public class Row
	{
		private readonly ColumnValue[] _data;

		public Row(ColumnValue[] data)
		{
			_data = data;
		}

		public ColumnValue this[int position] => _data[position];
	}
}
