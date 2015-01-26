using System;
using System.Collections.Generic;

namespace Pad.Core.ListFlatteners
{
	public class SqlIn : ListFlattener
	{
		public SqlIn()
			: base("SQL In", "Creates an expression for use with a SQL IN logical operator")
		{
		}

		public override string GetString(IEnumerable<string> list)
		{
			return this.SimpleFlatten(list, "'", ", ", "IN (", ")", "," + Environment.NewLine, 6);
		}
	}
}
