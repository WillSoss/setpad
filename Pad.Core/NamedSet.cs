using System;
using System.Linq;

namespace Pad.Core
{
	public abstract class NamedSet
	{
		protected NamedSet(string name, bool isCompositeSet)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("Name is required");

			this.Name = name;
			this.IsCompositeSet = isCompositeSet;
		}

		public string Name { get; private set; }
		public string OrderedName { get { return IsCompositeSet ? "(" + Name + ")" : Name; } }
		public bool IsCompositeSet { get; private set; }

		public abstract int Count { get; }
		public abstract IQueryable<string> GetQueryable();
	}
}
