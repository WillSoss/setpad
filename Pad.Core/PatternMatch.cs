using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pad.Core
{
	public class PatternMatch
	{
		public string Text { get; private set; }
		public string FormattedText { get; private set; }
		public int Position { get; private set; }
		public int Length { get; private set; }

		public PatternMatch(string text, string formatted, int position, int length)
		{
			this.Text = text;
			this.FormattedText = formatted;
			this.Position = position;
			this.Length = length;
		}

		public override string ToString()
		{
			return FormattedText;
		}
	}
}
