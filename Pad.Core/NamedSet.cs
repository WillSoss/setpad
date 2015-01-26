using System;
using System.Linq;

namespace Pad.Core
{
	public abstract class NamedSet
	{
		protected NamedSet(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("Name is required");

			this.Name = name;
		}

		public string Name { get; private set; }

		public abstract int Count { get; }
		public abstract IQueryable<string> GetQueryable();
	}
}
