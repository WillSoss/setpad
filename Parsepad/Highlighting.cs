using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;
using Pad.Core;

namespace Parsepad
{
	class Highlighting
	{
		Regex Regex;
		string Format;

		public IEnumerable<string> Parse(FlowDocument doc)
		{
			foreach (Run run in LogicalTreeUtility.GetChildren<Run>(doc, true))
				run.TextEffects = new TextEffectCollection();

			var text = new TextRange(doc.ContentStart, doc.ContentEnd).Text;

			var matches = Regex.Matches(text);

			List<string> results = new List<string>();

			TextEffect effect = new TextEffect()
			{
				Foreground = Brushes.Blue
			};

			var highlight = matches.Count <= 5000;

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
					results.Add(m.Value);
				}
				else
				{
					object[] groups = new object[m.Groups.Count];
					m.Groups.CopyTo(groups, 0);

					results.Add(string.Format(format, groups));
				}

				if (highlight)
				{
					var range = doc.GetTextRange(m.Index, m.Length);

					foreach (var target in TextEffectResolver.Resolve(range.Start, range.End, effect))
						target.Enable();
				}
			}

			return results;
		}
	}
}
