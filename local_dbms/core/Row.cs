namespace local_dbms.core
{
	internal class Row
	{
		private readonly IColumnValue[] _data;
		private bool _isSaved;
		private bool _isValid;

		public Row(IColumnValue[] data, bool isFromDatabase = false)
		{
			_data = data;
			_isSaved = isFromDatabase;
			_isValid = data.All(v => v.IsValid);
		}

		public IColumnValue this[int position] => _data[position];
		public bool IsSaved => _isSaved;
		public bool CanBeSaved => _isValid && !_isSaved;
		public bool IsValid => _isValid;
	}
}
