namespace local_dbms.core
{
	internal interface IDatabase : IDisposable
	{
		public List<Table> Tables { get; }
		public ITableController TableController { get; }
		public bool CreateTable(Table table);
		public bool DropTable(Table table);
	}
}
