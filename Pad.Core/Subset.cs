﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Pad.Core
{
	public class Subset : Set
	{
		private HashSet<string> elements;

		public Subset(string name, Set superset, HashSet<string> elements)
			: base(name, $"{Operator(SetOperation.Subset)} {superset.DefinedAsParens}", false)
		{
			if (superset == null)
				throw new ArgumentNullException("Superset is required");

			if (elements.Count() < 1)
				throw new ArgumentOutOfRangeException("A subset must be created from one or more element of the superset");

			this.Superset = superset;
			this.elements = elements;
		}

		public Set Superset { get; private set; }

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
