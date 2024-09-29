namespace local_dbms.core
{
	internal class Row
	{
		private readonly ColumnValue[] _data;

		public Row(ColumnValue[] data)
		{
			_data = data;
		}

		public string this[int position] => _data[position].StringValue;

		public bool UpdateValue(int position, string value)
		{
			return _data[position].SetFromString(value);
		}
	}
}
