using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Pad.Core
{
	public class RawSet : Set
	{
		private readonly HashSet<string> data;

		public RawSet(string name, string definedAs, HashSet<string> data)
			: base(name, definedAs, false)
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

        protected override bool RemoveElements(string[] elements)
        {
            foreach (var element in elements)
                data.Remove(element);

            return true;
        }
    }
}
