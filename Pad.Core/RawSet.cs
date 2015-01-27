using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pad.Core
{
	public class RawSet : NamedSet
	{
		private readonly HashSet<string> data;

		public RawSet(string name, HashSet<string> data)
			: base(name, false)
		{
			if (data == null)
				throw new ArgumentNullException("A data set is required");

			this.data = data;
		}

		public override int Count
		{
			get { return data.Count; }
		}

		public override IQueryable<string> GetQueryable()
		{
			return data.AsQueryable();
		}
	}
}
