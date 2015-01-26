using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Pad.UI
{

	public class RegHelper
	{
		public static RegistryKey GetCurrentUser(string app)
		{
			return OpenOrCreateSubKey(Registry.CurrentUser, "Software\\Sossamon\\" + app);
		}

		public static RegistryKey OpenOrCreateSubKey(RegistryKey parent, string name)
		{
			var key = parent.OpenSubKey(name, true);

			if (key == null)
				key = parent.CreateSubKey(name);

			return key;
		}
	}
}
