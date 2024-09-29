using System.Diagnostics;

namespace local_dbms.core
{
	internal abstract class ColumnValue
	{
		protected readonly bool _isNullable;

		public ColumnValue(object value, bool isNullable)
		{
			_isNullable = isNullable;
			SetFromObject(value);
		}

		public bool IsNullable => _isNullable;

		public abstract string StringValue { get; }
		public abstract bool IsNull { get; }
		public abstract bool SetFromObject(object value);
		public abstract bool SetFromString(string value);
	}

	internal class IntValue : ColumnValue
	{
		private int? _value = null;

		public IntValue(object value, bool isNullable) : base(value, isNullable) { }

		public override bool IsNull => _value == null;

		public override string StringValue => _value?.ToString() ?? "";

		public override bool SetFromObject(object value)
		{
			if (value == null)
			{
				if (_isNullable) _value = null;
				return _isNullable;
			}
			if (value is string strValue) return SetFromString(strValue);
			if (value is int intValue)
			{
				_value = intValue;
				return true;
			}
			return false;
		}

		public override bool SetFromString(string value)
		{
			if (value.Equals("null", StringComparison.CurrentCultureIgnoreCase))
			{
				if (_isNullable) _value = null;
				return _isNullable;
			}

			bool isValid = int.TryParse(value, out int result);
			if (isValid) _value = result;
			return isValid;
		}
	}

	internal class TextValue : ColumnValue
	{
		private string? _value = null;

		public TextValue(object value, bool isNullable) : base(value, isNullable) { }

		public override bool IsNull => _value == null;

		public override string StringValue => _value == null ? "null value" : $"'{_value}'";

		public override bool SetFromObject(object value)
		{
			if (value == null)
			{
				if (_isNullable) _value = null;
				return _isNullable;
			}
			if (value is string strValue) return SetFromString(strValue);
			return false;
		}

		public override bool SetFromString(string value)
		{
			Debug.WriteLine($"Set from string: {value}");
			if (value.Equals("null", StringComparison.CurrentCultureIgnoreCase))
			{
				if (_isNullable) _value = null;
				return _isNullable;
			}
			if (value.StartsWith('\'') && value.EndsWith('\''))
			{
				_value = value[1..^1];
				return true;
			}
			_value = value;
			return true;
		}
	}
}
