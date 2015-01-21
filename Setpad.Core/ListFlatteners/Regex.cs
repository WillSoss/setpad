using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Setpad.ListFlatteners
{
	public class Regex : ListFlattener
	{
		public Regex()
			: base("Regex", "Creates a regex optimized to match a list of strings")
		{
		}

		public override string GetString(IEnumerable<string> list)
		{
			return ConstructRegex(list, 0);
		}

		private string ConstructRegex(IEnumerable<string> words, int commonWordLength)
		{
			StringBuilder segment = new StringBuilder();

			// Add the letter that is at the end of the common parts of the words, except for pos = -1, which is the initial position
			if (commonWordLength != 0)
				segment.Append(words.First()[commonWordLength - 1]);

			// If there are words longer than the common part, we will need to add an inner regex
			if (words.Any(w => w.Length > commonWordLength))
			{
				// Get a l
				StringBuilder subSegment = new StringBuilder();
				var wordsInSubSegment = words.Where(w => w.Length > commonWordLength);

				foreach (char l in wordsInSubSegment.Select(w => w[commonWordLength]).Distinct())
				{
					subSegment.Append(ConstructRegex(wordsInSubSegment.Where(w => w[commonWordLength] == l), commonWordLength + 1));
					subSegment.Append("|");
				}

				// Remove last |
				subSegment.Remove(subSegment.Length - 1, 1);

				var sub = subSegment.ToString();

				if (sub.Contains("|") || sub.Contains("?"))
					segment.AppendFormat("({0})", sub);
				else
					segment.Append(sub);

				// If there is a single word that *is* the common part, then the inner regex is optional (e.g., THE | THESE, where THE is common -> "THE(SE)?")
				if (words.Any(w => w.Length == commonWordLength))
					segment.Append("?");
			}

			return segment.ToString();
		}
	}
}
