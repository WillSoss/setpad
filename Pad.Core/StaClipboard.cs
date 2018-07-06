using System.Threading;
using System.Windows;

namespace Pad.Core
{
	public static class StaClipboard
	{
		public static bool ContainsText()
		{
			bool value = false;

			var t = new Thread(x =>
			{
				value = Clipboard.ContainsText();
			});

			t.SetApartmentState(ApartmentState.STA);

			t.Start();
			t.Join(2000);

			return value;
		}

		public static void SetText(string text)
		{
			var t = new Thread(x =>
			{
				Clipboard.SetText((string)text);
			});

			t.SetApartmentState(ApartmentState.STA);

			t.Start();
			t.Join(2000);
		}

		public static string GetText()
		{
			string value = null;

			var t = new Thread(x =>
			{
				value = Clipboard.GetText();
			});

			t.SetApartmentState(ApartmentState.STA);

			t.Start();
			t.Join(2000);

			return value;
		}
	}
}
