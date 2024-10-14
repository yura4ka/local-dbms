using local_dbms.core;

namespace Tests
{
	[TestClass]
	public class SqliteDatabaseTests
	{
		[TestMethod]
		public void AddsTable_WithCorrectValue()
		{
			var database = new SqliteDatabase(":memory:");

			Column[] columns = [
				new Column("id", TypeManager.TypeMappings["INT"](), true, null, true),
				new Column("text", TypeManager.TypeMappings["TEXT"](), false, null, false),
				new Column("value", TypeManager.TypeMappings["REAL"](), true, 234.33, false),
			];

			var toAddTable = new Table(database.TableController, "Table1");
            foreach (var item in columns)
            {
				toAddTable.AddColumn(item);
            }

            bool isValid = database.CreateTable(toAddTable);
			Assert.IsTrue(isValid);
			Assert.AreEqual(1, database.Tables.Count);

			var table = database.Tables[0];
			Assert.AreEqual(columns.Length, table.Columns.Count);
		}
	}
}
