using System;
using System.Collections.Generic;

namespace Setpad.ListFlatteners
{
	public class SqlUnion : ListFlattener
	{
		public SqlUnion()
			: base("SQL Union", "Creates a series of SQL selects that are unioned together to form a result set of the given list")
		{
		}

		public override string GetString(IEnumerable<string> list)
		{
			return this.SimpleFlatten(list, "'", " UNION" + Environment.NewLine + "SELECT ", "SELECT ", "");
		}
	}
}
