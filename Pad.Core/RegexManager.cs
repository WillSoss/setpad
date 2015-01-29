using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace Pad.Core
{
	public static class RegexManager
	{
		private const string namedpath = "Software\\Sossamon\\Regexpad\\Named Regexes";
		private const string recentpath = "Software\\Sossamon\\Regexpad\\Recent Regexes";
		private static Dictionary<string, System.Text.RegularExpressions.Regex> regexes;

		static RegexManager()
		{
			regexes = new Dictionary<string, System.Text.RegularExpressions.Regex>();
		}

		public static System.Text.RegularExpressions.Regex GetRegex(string regexText, RegexOptions options)
		{
			options = options | RegexOptions.Compiled;

			if (string.IsNullOrEmpty(regexText))
				throw new ArgumentNullException("Regex text is required");

			if (regexes.ContainsKey(regexText) && regexes[regexText].Options == options)
			{
				System.Diagnostics.Debug.WriteLine(string.Format("Cache hit for '{0}'", regexText));

				return regexes[regexText];
			}
			else
			{
				System.Diagnostics.Debug.WriteLine(string.Format("Cache miss for '{0}'", regexText));

				System.Text.RegularExpressions.Regex regex = null;

				try
				{
					regex = new System.Text.RegularExpressions.Regex(regexText, options);
				}
				catch (ArgumentException)
				{
					return null;
				}

				if (regexes.ContainsKey(regexText))
					regexes[regexText] = regex;
				else
					regexes.Add(regexText, regex);

				return regex;
			}
		}

		public static void AddNamed(string name, string regex, string format)
		{
			var key = GetNamedKey();

			key.SetValue(name, regex);

			if (!string.IsNullOrWhiteSpace(format))
				key.SetValue(name + "__Format", format);
		}

		public static void AddRecent(string regex, string format)
		{
			AddRecentType("Regex", regex, !GetRecent().Contains(format));

			if (!string.IsNullOrWhiteSpace(format))
				AddRecentType("Format", format, !GetRecentFormats().Contains(format));
		}

		private static void AddRecentType(string type, string value, bool moving)
		{
			var key = GetRecentKey();

			for (int i = 10; i > 0; i--)
			{
				if (!moving)
				{
					var v2 = key.GetValue(string.Format("{0}{1}", type, i));

					if (v2 != null && ((string)v2) == value)
						moving = true;
				}
				
				if (moving)
				{
					var val = key.GetValue(string.Format("{0}{1}", type, i - 1));

					if (val != null)
						key.SetValue(string.Format("{0}{1}", type, i), val);
				}
			}
				
			key.SetValue(string.Format("{0}1", type), value);
		}

		public static IEnumerable<string> GetRecent()
		{
			var key = GetRecentKey();

			for (int i = 1; i < 11; i++)
			{
				var val = key.GetValue(string.Format("Regex{0}", i));

				if (val != null)
					yield return (string)val;
			}
		}

		public static IEnumerable<string> GetRecentFormats()
		{
			var key = GetRecentKey();

			for (int i = 1; i < 11; i++)
			{
				var val = key.GetValue(string.Format("Format{0}", i));

				if (val != null)
					yield return (string)val;
			}
		}

		public static IEnumerable<NamedRegex> GetNamed()
		{
			var key = GetNamedKey();
			var patterns = key.GetValueNames().Where(n => !n.EndsWith("__Format"));
			var formats = key.GetValueNames().Where(n => n.EndsWith("__Format"));

			foreach (var name in patterns)
				yield return new NamedRegex(name, (string)key.GetValue(name), patterns.Contains(name + "__Format") ? (string)key.GetValue(name + "__Format") : null);
		}

		private static RegistryKey GetNamedKey()
		{
			return OpenOrCreateSubKey(Registry.CurrentUser, namedpath);
		}

		private static RegistryKey GetRecentKey()
		{
			return OpenOrCreateSubKey(Registry.CurrentUser, recentpath);
		}

		private static RegistryKey OpenOrCreateSubKey(RegistryKey parent, string name)
		{
			var key = parent.OpenSubKey(name, true);

			if (key == null)
				key = parent.CreateSubKey(name);

			return key;
		}
	}
}
