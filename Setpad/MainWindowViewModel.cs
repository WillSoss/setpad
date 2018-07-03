using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using Pad.Core;
using Pad.UI;

namespace Setpad.UI
{
	public class MainWindowViewModel : ViewModel
	{
		private readonly ObservableCollection<NamedSet> sets = new ObservableCollection<NamedSet>();
		private readonly ObservableCollection<NamedSet> selectedSets = new ObservableCollection<NamedSet>();
		private readonly ObservableCollection<string> selectedSetElements = new ObservableCollection<string>();
        private readonly ObservableCollection<string> selectedElements = new ObservableCollection<string>();

		private int setNumber = 1;
 
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
                    if (selectedSet != null)
                        selectedSet.CollectionChanged -= SelectedSet_CollectionChanged;

					selectedSet = value;
                 
					selectedSetElements.Clear();

					if (value != null)
                    {
                        selectedSet.CollectionChanged += SelectedSet_CollectionChanged;

                        foreach (var e in value.GetQueryable())
							selectedSetElements.Add(e);

						SetDetail = string.Format("{0}: {1} elements", SelectedSet.Name, SelectedSet.Count);
					}
					else
					{
						SetDetail = string.Empty;
					}

					OnPropertyChanged("SelectedSet");
				}
			}
		}

        private void SelectedSet_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
                foreach (string element in e.OldItems)
                    selectedSetElements.Remove(element);

            SetDetail = string.Format("{0}: {1} elements", SelectedSet.Name, SelectedSet.Count);
        }

        public ObservableCollection<NamedSet> SelectedSets
		{
			get { return selectedSets; }
		}

		public ObservableCollection<string> SelectedSetElements
		{
			get { return selectedSetElements; }
		}

		private string selectedElement;

		public string SelectedElement
		{
			get { return selectedElement; }
			set
			{
				if (selectedElement != value)
				{
					selectedElement = value;

					OnPropertyChanged("SelectedElement");
				}
			}
		}

        public ObservableCollection<string> SelectedElements
        {
            get { return selectedElements; }
        }

		private string setDetail;

		public string SetDetail
		{
			get { return setDetail; }
			set
			{
				if (setDetail != value)
				{
					setDetail = value;

					OnPropertyChanged("SetDetail");
				}
			}
		}

		private string selectedSetOrder;

		public string SelectedSetOrder
		{
			get { return selectedSetOrder; }
			set
			{
				if (selectedSetOrder != value)
				{
					selectedSetOrder = value;

					OnPropertyChanged("SelectedSetOrder");
				}
			}
		}

		public bool CanPaste
		{
			get { return StaClipboard.ContainsText(); }
		}

		public void Paste()
		{
			var text = StaClipboard.GetText();

			AddRawSet(text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries));
		}

		public MainWindowViewModel()
		{
			selectedSets.CollectionChanged += selectedSets_CollectionChanged;
		}

		void selectedSets_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			SelectedSet = selectedSets.FirstOrDefault();

			if (selectedSets.Count > 0)
			{
				StringBuilder s = new StringBuilder();

				foreach (var set in selectedSets)
						s.AppendFormat("{0} □ ", set.OrderedName);

				SelectedSetOrder = s.Remove(s.Length - 3, 3).ToString();
			}
			else
			{
				SelectedSetOrder = string.Empty;
			}
		}

		public void AddRawSet(IEnumerable<string> data)
		{
			Execute(() =>
				{
					var name = "S" + setNumber++;
					var set = new RawSet(name, new HashSet<string>(data));
					sets.Add(set);
					selectedSets.Add(set);
				});
		}

		public bool CanDoBinaryOp { get { return SelectedSets.Count > 1; } }

		public bool CanDoUnaryOp { get { return SelectedSets.Count > 0; } }

        public bool CanDoElementOp { get { return SelectedElements.Count > 0; } }
		
		public void Copy(ListFlattener flattener)
		{
			StaClipboard.SetText(flattener.GetString(selectedSet.GetQueryable()));
		}

		public void UnionSelectedSets()
		{
			Execute(() =>
				{
					var set = new CalculatedSet(SelectedSets, SetOperation.Union);
					sets.Add(set);
				});
		}

		public void IntersectSelectedSets()
		{
			Execute(() =>
			{
				var set = new CalculatedSet(SelectedSets, SetOperation.Intersection);
				sets.Add(set);
			});
		}

		public void DifferenceSelectedSets()
		{
			Execute(() =>
			{
				var set = new CalculatedSet(SelectedSets, SetOperation.Difference);
				sets.Add(set);
			});
		}

		public void SymmetricDifferenceSelectedSets()
		{
			Execute(() =>
			{
				var set = new CalculatedSet(SelectedSets, SetOperation.SymmetricDifference);
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

        public void RemoveSelectedElements()
        {
            Execute(() =>
            {
                selectedSet.Remove(SelectedElements.ToArray());
            });
        }
    }
}
