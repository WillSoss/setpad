using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.ComponentModel;

namespace Pad.Core
{
	public class DataParser
	{
		private readonly MatchCollection matches;

		public Regex Regex { get; private set; }
		public string Format { get; private set; }
		public string Text { get; private set; }
		public int Count { get { return matches.Count; } }

		public DataParser(Regex regex, string format, string textToParse)
		{
			if (regex == null)
				throw new ArgumentNullException("Regex is required");

			this.Regex = regex;
			this.Format = format ?? string.Empty;
			this.Text = textToParse ?? string.Empty;

			matches = Regex.Matches(Text);
		}

		public IEnumerable<string> GetMatches()
		{
			List<string> results = new List<string>();

			var captureGroups = Regex.GetGroupNames();

			string format = Format;

			for (int i = 0; i < captureGroups.Length; i++)
			{
				format = format.Replace("<" + captureGroups[i] + ">", "{" + i + "}");
			}

			System.Diagnostics.Debug.WriteLine("Using the format '{0}' from '{1}'", format, Format);

			foreach (Match m in matches)
			{
				if (string.IsNullOrEmpty(format))
				{
					yield return m.Value;
				}
				else
				{
					object[] groups = new object[m.Groups.Count];
					m.Groups.CopyTo(groups, 0);

					yield return string.Format(format, groups);
				}
			}
		}
	}
}
