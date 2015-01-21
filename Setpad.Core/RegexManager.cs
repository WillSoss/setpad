using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace Setpad
{
	public static class RegexManager
	{
		private const string regpath = "Software\\Archon\\Regex Data Tool";
		private static List<NamedRegex> persisted;
		private static Dictionary<string, System.Text.RegularExpressions.Regex> regexes;

		public static IEnumerable<NamedRegex> PersistedRegexes
		{
			get { return persisted; }
		}

		static RegexManager()
		{
			regexes = new Dictionary<string, System.Text.RegularExpressions.Regex>();
			persisted = new List<NamedRegex>();
			LoadRegexes();
		}

		public static System.Text.RegularExpressions.Regex GetRegex(string regexText, bool ignoreCase)
		{
			if (string.IsNullOrEmpty(regexText))
				throw new ArgumentNullException("Regex text is required");

			if (regexes.ContainsKey(regexText) && (!ignoreCase || regexes[regexText].Options.HasFlag(RegexOptions.IgnoreCase)))
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
					regex = new System.Text.RegularExpressions.Regex(regexText, RegexOptions.Compiled | (ignoreCase ? RegexOptions.IgnoreCase : 0));
				}
				catch (ArgumentException)
				{
					return null;
				}

				regexes.Add(regexText, regex);

				return regex;
			}
		}

		public static void Persist(string name, string regex)
		{
			var namedRegex = persisted.SingleOrDefault(nr => nr.Name == name);

			if (namedRegex != null)
				namedRegex.Regex = regex;
			else
				namedRegex = new NamedRegex(name, regex);

			var key = GetKey();

			key.SetValue(name, regex);
		}

		private static void LoadRegexes()
		{
			var key = GetKey();

			foreach (var name in key.GetValueNames())
				persisted.Add(new NamedRegex(name, (string)key.GetValue(name)));
		}

		private static RegistryKey GetKey()
		{
			return OpenOrCreateSubKey(Registry.CurrentUser, regpath);
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
