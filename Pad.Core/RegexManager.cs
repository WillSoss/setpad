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
		private const string regpath = "Software\\Sossamon\\Parsepad\\Regexes";
		private const string recentpath = "Software\\Sossamon\\Parsepad\\Recent Regexes";
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

		public static void AddRecent(string regex)
		{
			var key = GetRecentKey();

			bool moving = !GetRecent().Contains(regex);

			for (int i = 10; i > 0; i--)
			{
				if (!moving)
				{
					var v2 = key.GetValue(string.Format("Regex{0}", i));

					if (v2 != null && ((string)v2) == regex)
						moving = true;
				}
				
				if (moving)
				{
					var val = key.GetValue(string.Format("Regex{0}", i - 1));

					if (val != null)
						key.SetValue(string.Format("Regex{0}", i), val);
				}
			}
				
			key.SetValue("Regex1", regex);
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
