using System.Windows;

namespace local_dbms.utils
{
	public static class WindowUtils
	{
		public static void ShowErrorMessage(string error)
		{
			MessageBox.Show(error, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}
}
