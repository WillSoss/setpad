using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Text.RegularExpressions;

namespace Pad.Core
{
	public class FormattedMatch : DependencyObject
	{
		public static DependencyProperty FormatProperty = DependencyProperty.Register("Format", typeof(string), typeof(FormattedMatch));
		public static DependencyProperty FormattedValueProperty = DependencyProperty.Register("FormattedValue", typeof(string), typeof(FormattedMatch));

		private readonly Match inner;

		public int Index { get { return inner.Index; } }
		public int Length { get { return inner.Length; } }
		public string Value { get { return inner.Value; } }

		public string Format
		{
			get { return (string)GetValue(FormatProperty); }
			private set { SetValue(FormatProperty, value); }
		}

		public string FormattedValue
		{
			get { return (string)GetValue(FormattedValueProperty); }
			private set { SetValue(FormattedValueProperty, value); }
		}

		internal FormattedMatch(Match inner, string format)
		{
			if (inner == null)
				throw new ArgumentNullException("Inner match is required");

			this.inner = inner;
		}

		public override string ToString()
		{
			return FormattedValue;
		}
	}
}
