using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pad.Core
{
	public class PatternMatchEqualityComparer : IEqualityComparer<PatternMatch>
	{
		#region IEqualityComparer<PatternMatch> Members

		public bool Equals(PatternMatch x, PatternMatch y)
		{
			return x.FormattedText.Equals(y.FormattedText);
		}

		public int GetHashCode(PatternMatch obj)
		{
			return obj.FormattedText.GetHashCode();
		}

		#endregion
	}
}
