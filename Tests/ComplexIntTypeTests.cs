using local_dbms.core;

namespace Tests
{
	[TestClass]
	public class ComplexIntTypeTests
	{
		[TestMethod]
		public void Creates_WithValidValue()
		{
			var value = new ComplexIntValue("12 + 3i", false);
			var complex = value.ObjectValue;
			Assert.IsNotNull(complex);
			Assert.IsInstanceOfType(complex, typeof(string));
			Assert.AreEqual("12+3i", value.StringValue);
		}

		[TestMethod]
		public void Parses_WithFloatValue()
		{
			var value = new ComplexIntValue(null, false);
			bool result = value.ParseString("67.123 + 4.8i");
			var complex = value.ObjectValue;
			Assert.IsNull(complex);
			Assert.IsFalse(result);
		}
	}
}