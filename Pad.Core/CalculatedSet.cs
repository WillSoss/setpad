using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pad.Core
{
	public class CalculatedSet : Set
	{
		public CalculatedSet(string name, IEnumerable<Set> sets, Op op)
			: base(name, GetSetName(sets, op), true)
		{
			if (sets == null)
				throw new ArgumentNullException("Sets is required");

			if (sets.Count() < 2)
				throw new ArgumentOutOfRangeException("A calculated set must be created from two or more sets");

			this.Sets = sets.ToArray();
			this.Operation = op;
		}

		private static string GetSetName(IEnumerable<Set> sets, Op op)
		{
			StringBuilder name = new StringBuilder();

			foreach(var set in sets)
			{
				name.AppendFormat("{0} {1} ", set.DefinedAsParens, Operator(op));
			}

			return name.Remove(name.Length - 3, 3).ToString();
		}

		public IEnumerable<Set> Sets { get; private set; }

		public Op Operation { get; private set; }

		public override int Count
		{
			get { return GetQueryable().Count(); }
		}

		public override IQueryable<string> GetQueryable()
		{
			var query = GetQueryable(Operation);

			if (Operation == Op.SymmetricDifference)
			{
				var intersection = GetQueryable(Op.Intersection);

				query = query.Except(intersection);
			}

			return query;
		}

		private IQueryable<string> GetQueryable(Op op)
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

		private static IQueryable<string> DoOp(IQueryable<string> a, IQueryable<string> b, Op op)
		{
			switch (op)
			{
				case Op.Union:
				case Op.SymmetricDifference:
					return a.Union(b);

				case Op.Intersection:
					return a.Intersect(b);

				case Op.Difference:
					return a.Except(b);

				default:
					throw new ArgumentOutOfRangeException("Invalid operation");
			}
		}
    }
}
