using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Pad.Core
{
	public enum Op
	{
		Union,
		Intersection,
		Difference,
		SymmetricDifference,
		Subset
	}

	public abstract class Set : INotifyCollectionChanged, INotifyPropertyChanged
    {
		const string subscript = "₀₁₂₃₄₅₆₇₈₉";

		public static string Operator(Op op)
		{
			switch (op)
			{
				case Op.Union: return "∪";

				case Op.Intersection: return "∩";

				case Op.Difference: return "\\";

				case Op.SymmetricDifference: return "∆";

				case Op.Subset: return "⊆";

				default: throw new ArgumentOutOfRangeException("Invalid operation");
			}
		}

		public static string ReplaceNumbersWithSubscripts(string value)
		{
			return value
				.Replace('0', subscript[0])
				.Replace('1', subscript[1])
				.Replace('2', subscript[2])
				.Replace('3', subscript[3])
				.Replace('4', subscript[4])
				.Replace('5', subscript[5])
				.Replace('6', subscript[6])
				.Replace('7', subscript[7])
				.Replace('8', subscript[8])
				.Replace('9', subscript[9]);
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        protected Set(string name, string definedAs, bool isCompositeSet)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name), "Cannot be null");
				
			this.Name = ReplaceNumbersWithSubscripts(name); ;
			this.DefinedAs = string.IsNullOrWhiteSpace(definedAs) ? Name : definedAs;
			this.IsCompositeSet = isCompositeSet;
		}

		public string Name { get; private set; }
		public string DefinedAs { get; private set; }
		public string DefinedAsParens { get { return IsCompositeSet ? "(" + DefinedAs + ")" : DefinedAs; } }
		public bool IsCompositeSet { get; private set; }
		public abstract int Count { get; }

        public abstract IQueryable<string> GetQueryable();

        public void Remove(string[] elements)
        {
            if (RemoveElements(elements))
            {
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, elements));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
            }
        }

        protected virtual bool RemoveElements(string[] elements)
        {
            return false;
        }
	}
}
