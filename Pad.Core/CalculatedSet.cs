using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pad.Core
{
	public enum SetOperation
	{
		Union,
		Intersection,
		Difference,
		SymmetricDifference
	}

	public class CalculatedSet : NamedSet
	{
		public CalculatedSet(NamedSet a, NamedSet b, SetOperation op)
			: base(GetSetName(a,b,op), true)
		{
			if (a == null)
				throw new ArgumentNullException("Set A is required");
			
			if (b == null)
				throw new ArgumentNullException("Set B is required");

			this.A = a;
			this.B = b;
			this.Operation = op;
		}

		private static string GetSetName(NamedSet a, NamedSet b, SetOperation op)
		{
			switch (op)
			{
				case SetOperation.Union: return string.Format("{0} ∪ {1}", a.OrderedName, b.OrderedName);

				case SetOperation.Intersection: return string.Format("{0} ∩ {1}", a.OrderedName, b.OrderedName);

				case SetOperation.Difference: return string.Format("{0} \\ {1}", a.OrderedName, b.OrderedName);

				case SetOperation.SymmetricDifference: return string.Format("{0} ∆ {1}", a.OrderedName, b.OrderedName);

				default: throw new ArgumentOutOfRangeException("Invalid operation");
			}
		}


		public NamedSet A { get; private set; }

		public NamedSet B { get; private set; }

		public SetOperation Operation { get; private set; }

		public override int Count
		{
			get { return GetQueryable().Count(); }
		}

		public override IQueryable<string> GetQueryable()
		{
			switch (Operation)
			{
				case SetOperation.Union:
					return A.GetQueryable().Union(B.GetQueryable());

				case SetOperation.Intersection:
					return A.GetQueryable().Intersect(B.GetQueryable());

				case SetOperation.Difference:
					return A.GetQueryable().Except(B.GetQueryable());

				case SetOperation.SymmetricDifference:
					return A.GetQueryable().Union(B.GetQueryable()).Except(A.GetQueryable().Intersect(B.GetQueryable()));

				default:
					throw new ArgumentOutOfRangeException("Invalid operation");
			}
		}

	}
}
