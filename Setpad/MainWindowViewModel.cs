using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Pad.Core;
using Pad.UI;

namespace Setpad.UI
{
	public class MainWindowViewModel : ViewModel
	{
		private readonly ObservableCollection<NamedSet> sets = new ObservableCollection<NamedSet>();
		private readonly ObservableCollection<NamedSet> selectedSets = new ObservableCollection<NamedSet>();
 
		public ObservableCollection<NamedSet> Sets
		{
			get { return sets; }
		}

		private NamedSet selectedSet;

		public NamedSet SelectedSet
		{
			get { return selectedSet; }
			set
			{
				if (selectedSet != value)
				{
					selectedSets.Add(selectedSet);
					selectedSets.Remove(value);

					selectedSet = value;

					OnPropertyChanged("SelectedSet");
				}
			}
		}

		public ObservableCollection<NamedSet> SelectedSets
		{
			get { return selectedSets; }
		}

		public void AddRawSet(IEnumerable<string> data)
		{
			Execute(() =>
				{
					var name = "S" + (sets.Where(s => s is RawSet).Count() + 1);
					var set = new RawSet(name, new HashSet<string>(data));
					sets.Add(set);
					selectedSets.Add(set);
				});
		}

		public void UnionSelectedSets()
		{
			Execute(() =>
				{
					var set = new CalculatedSet(SelectedSets.First(), SelectedSets.Skip(1).First(), SetOperation.Union);
					sets.Add(set);
				});
		}

		public void IntersectSelectedSets()
		{
			Execute(() =>
			{
				var set = new CalculatedSet(SelectedSets.First(), SelectedSets.Skip(1).First(), SetOperation.Intersection);
				sets.Add(set);
			});
		}

		public void DifferenceSelectedSets()
		{
			Execute(() =>
			{
				var set = new CalculatedSet(SelectedSets.First(), SelectedSets.Skip(1).First(), SetOperation.Difference);
				sets.Add(set);
			});
		}

		public void SymmetricDifferenceSelectedSets()
		{
			Execute(() =>
			{
				var set = new CalculatedSet(SelectedSets.First(), SelectedSets.Skip(1).First(), SetOperation.SymmetricDifference);
				sets.Add(set);
			});
		}

		public void RemoveSelectedSets()
		{
			Execute(() =>
			{
				foreach (var set in SelectedSets.ToArray())
					Sets.Remove(set);

				SelectedSets.Clear();
			});
		}
	}
}
