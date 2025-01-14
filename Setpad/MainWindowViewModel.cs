﻿using System;
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
		private readonly ObservableCollection<Set> sets = new ObservableCollection<Set>();
		private readonly ObservableCollection<Set> selectedSets = new ObservableCollection<Set>();
		private readonly ObservableCollection<string> selectedSetElements = new ObservableCollection<string>();
        private readonly ObservableCollection<string> selectedElements = new ObservableCollection<string>();

		private int setNumber = 1;
 
		public ObservableCollection<Set> Sets
		{
			get { return sets; }
		}

		private Set selectedSet;

		public Set SelectedSet
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

						SetDetail = string.Format("{0}: {1} elements", SelectedSet.DefinedAs, SelectedSet.Count);
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

            SetDetail = string.Format("{0}: {1} elements", SelectedSet.DefinedAs, SelectedSet.Count);
        }

        public ObservableCollection<Set> SelectedSets
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

		private string GetSetName()
		{
			return "S" + setNumber++;
		}

		void selectedSets_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			SelectedSet = selectedSets.FirstOrDefault();

			if (selectedSets.Count > 0)
			{
				StringBuilder s = new StringBuilder();

				foreach (var set in selectedSets)
						s.AppendFormat("{0} □ ", set.DefinedAsParens);

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
					var set = new RawSet(GetSetName(), null, new HashSet<string>(data));
					sets.Add(set);
					selectedSets.Add(set);
				});
		}

		public bool CanDoBinaryOp { get { return SelectedSets.Count > 1; } }

		public bool CanDoUnaryOp { get { return SelectedSets.Count > 0; } }

        public bool CanDoElementOp { get { return SelectedElements.Count > 0; } }
		
		public void Copy(ListFlattener flattener, bool copyElements)
		{
			StaClipboard.SetText(flattener.GetString(copyElements ? selectedElements.ToList() : selectedSet.GetQueryable().ToList()));
		}

		public void UnionSelectedSets()
		{
			Execute(() =>
				{
					var set = new CalculatedSet(GetSetName(), SelectedSets, Op.Union);
					sets.Add(set);
				});
		}

		public void IntersectSelectedSets()
		{
			Execute(() =>
			{
				var set = new CalculatedSet(GetSetName(), SelectedSets, Op.Intersection);
				sets.Add(set);
			});
		}

		public void DifferenceSelectedSets()
		{
			Execute(() =>
			{
				var set = new CalculatedSet(GetSetName(), SelectedSets, Op.Difference);
				sets.Add(set);
			});
		}

		public void SymmetricDifferenceSelectedSets()
		{
			Execute(() =>
			{
				var set = new CalculatedSet(GetSetName(), SelectedSets, Op.SymmetricDifference);
				sets.Add(set);
			});
		}

		public void Subset()
		{
			Execute(() =>
			{
				sets.Add(new Subset(GetSetName(), selectedSet, new HashSet<string>(selectedElements)));
			});
		}

		public void InvertedSubset()
		{
			Execute(() =>
			{
				sets.Add(new Subset(GetSetName(), selectedSet, new HashSet<string>(selectedElements), true));
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

		public void Trim()
		{
			Execute(() =>
			{
				HashSet<string> set = new HashSet<string>();

				foreach (var value in selectedSet.GetQueryable())
				{
					var trimAt = value.IndexOfAny(new char[] { '\t', ',', ';', '|' });

					if (trimAt > 0)
						set.Add(value.Substring(0, trimAt));
					else
						set.Add(value);
				}

				sets.Add(new RawSet(GetSetName(), $"Trim({selectedSet.Name})", set));
			});
		}
    }
}
