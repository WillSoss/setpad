using System;
using System.Collections.Generic;
using System.Linq;

namespace Pad.Core
{
	public class Subset : NamedSet
	{
		private HashSet<string> elements;

		public Subset(NamedSet superset, HashSet<string> elements)
			: base($"{GetOp(SetOperation.Subset)} {superset.Name}", false)
		{
			if (superset == null)
				throw new ArgumentNullException("Superset is required");

			if (elements.Count() < 1)
				throw new ArgumentOutOfRangeException("A subset must be created from one or more element of the superset");

			this.Superset = superset;
			this.elements = elements;
		}

		public NamedSet Superset { get; private set; }

		public override int Count
		{
			get { return elements.Count(); }
		}

		public override IQueryable<string> GetQueryable()
		{
			return elements.AsQueryable();
		}
	}
}
