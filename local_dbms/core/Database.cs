namespace local_dbms.core
{
	internal interface IDatabase : IDisposable
	{
		public List<Table> Tables { get; }
	}
}
