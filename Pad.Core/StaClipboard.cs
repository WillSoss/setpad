using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
			t.Join();

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
			t.Join();
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
			t.Join();

			return value;
		}
	}
}
