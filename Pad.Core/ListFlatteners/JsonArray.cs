using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pad.Core.ListFlatteners
{
	public class JsonStringArray : ListFlattener
	{
		public JsonStringArray()
			: base("JSON String Array", "Creates a JSON string array")
		{
		}

		public override string GetString(IEnumerable<string> list)
		{
			return this.SimpleFlatten(list, "\"", ", ", "[", "]", "," + Environment.NewLine, 6);
		}
	}
}
