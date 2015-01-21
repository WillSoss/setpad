using System;
using System.Collections.Generic;

namespace Setpad.ListFlatteners
{
	public class TextWithLinebreaks : ListFlattener
	{
		public TextWithLinebreaks()
			: base("Plain Text (Line Breaks)", "Uses Environment.NewLine to create a plain text list")
		{
		}

		public override string GetString(IEnumerable<string> list)
		{
			return SimpleFlatten(list, null, Environment.NewLine, null, null);
		}
	}
}
