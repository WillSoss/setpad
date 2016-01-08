using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Pad.Core
{
	public abstract class NamedSet : INotifyCollectionChanged, INotifyPropertyChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

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
