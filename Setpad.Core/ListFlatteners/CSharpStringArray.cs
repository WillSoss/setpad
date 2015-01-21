using System;
using System.Collections.Generic;

namespace Setpad.ListFlatteners
{
	public class CSharpStringArray : ListFlattener
	{
		public CSharpStringArray()
			: base("C# String Array", "Creates a C# string array initialized with string literals")
		{
		}

		public override string GetString(IEnumerable<string> list)
		{
			return this.SimpleFlatten(list, "\"", ", ", "new string[] {", "};", "," + Environment.NewLine, 6);
		}
	}
}
