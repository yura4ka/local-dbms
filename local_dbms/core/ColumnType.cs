namespace local_dbms.core
{
	internal interface IColumnType
	{
		public abstract string Name { get; }
		public abstract ColumnValue Instance(object value, bool isNullable);
	}

	internal sealed class IntType : IColumnType
	{
		private static IntType? _instance = null;

		private IntType() { }

		public static IntType GetInstance()
		{
			_instance ??= new IntType();
			return _instance;
		}

		public string Name => "Int";

		public ColumnValue Instance(object value, bool isNullable)
		{
			return new IntValue(value, isNullable);
		}
	}

	internal sealed class TextType : IColumnType
	{
		private static TextType? _instance = null;

		private TextType() { }

		public static TextType GetInstance()
		{
			_instance ??= new TextType();
			return _instance;
		}

		public string Name => "Text";

		public ColumnValue Instance(object value, bool isNullable)
		{
			return new TextValue(value, isNullable);
		}
	}

	internal static class TypeManager
	{
		internal static Dictionary<string, Func<IColumnType>> TypeMappings = new(){
			{ "INT", IntType.GetInstance },
			{ "TEXT", TextType.GetInstance },
			{ "Complex", IntType.GetInstance },
		};
	}
}
