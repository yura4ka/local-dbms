namespace local_dbms.core
{
	internal interface IColumnValue
	{
		public bool IsValid { get; }
		public string? StringValue { get; }
		public bool SetString(string value);
		public bool SetObject(object value);
	}

	internal class IntValue : IColumnValue
	{
		private bool _isValid;
		private int? _value = null;

		public IntValue(object value)
		{
			SetObject(value);
		}

		public bool IsValid => _isValid;

		public string? StringValue => _value?.ToString();

		public bool SetObject(object value)
		{
			_isValid = false;

			if (value is int intValue)
			{
				_value = intValue;
				_isValid = true;
			}
			else if (value is string stringValue)
			{
				SetString(stringValue);
			}

			return _isValid;
		}

		public bool SetString(string value)
		{
			_isValid = int.TryParse(value, out int result);
			_value = result;
			return _isValid;
		}
	}

	internal class TextValue : IColumnValue
	{
		private bool _isValid;
		private string? _value = null;

		public TextValue(object value)
		{
			SetObject(value);
		}

		public bool IsValid => _isValid;

		public string? StringValue => _value;

		public bool SetObject(object value)
		{
			_isValid = false;

			if (value is string stringValue)
			{
				SetString(stringValue);
			}

			return _isValid;
		}

		public bool SetString(string value)
		{
			_value = value;
			_isValid = true;
			return _isValid;
		}
	}
}
