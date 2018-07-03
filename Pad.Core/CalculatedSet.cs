using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pad.Core
{
	public class CalculatedSet : Set
	{
		public CalculatedSet(IEnumerable<Set> sets, SetOperation op)
			: base(GetSetName(sets, op), true)
		{
			if (sets == null)
				throw new ArgumentNullException("Sets is required");

			if (sets.Count() < 2)
				throw new ArgumentOutOfRangeException("A calculated set must be created from two or more sets");

			this.Sets = sets.ToArray();
			this.Operation = op;
		}

		private static string GetSetName(IEnumerable<Set> sets, SetOperation op)
		{
			StringBuilder name = new StringBuilder();

			foreach(var set in sets)
			{
				name.AppendFormat("{0} {1} ", set.OrderedName, Operator(op));
			}

			return name.Remove(name.Length - 3, 3).ToString();
		}

		public IEnumerable<Set> Sets { get; private set; }

		public SetOperation Operation { get; private set; }

		public override int Count
		{
			get { return GetQueryable().Count(); }
		}

		public override IQueryable<string> GetQueryable()
		{
			var query = GetQueryable(Operation);

			if (Operation == SetOperation.SymmetricDifference)
			{
				var intersection = GetQueryable(SetOperation.Intersection);

				query = query.Except(intersection);
			}

			return query;
		}

		private IQueryable<string> GetQueryable(SetOperation op)
		{
			IQueryable<string> query = null;

			foreach (var set in Sets)
			{
				if (query == null)
					query = set.GetQueryable();
				else
					query = DoOp(query, set.GetQueryable(), op);
			}

			return query;
		}

		private static IQueryable<string> DoOp(IQueryable<string> a, IQueryable<string> b, SetOperation op)
		{
			switch (op)
			{
				case SetOperation.Union:
				case SetOperation.SymmetricDifference:
					return a.Union(b);

				case SetOperation.Intersection:
					return a.Intersect(b);

				case SetOperation.Difference:
					return a.Except(b);

				default:
					throw new ArgumentOutOfRangeException("Invalid operation");
			}
		}
    }
}
