using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Pad.Core;
using Microsoft.Win32;
using System.IO;

namespace Setpad.UI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public static DependencyProperty MatchInfoProperty = DependencyProperty.Register("MatchInfo", typeof(string), typeof(MainWindow), new PropertyMetadata(string.Empty));
		public static DependencyProperty RemoveDuplicatesProperty = DependencyProperty.Register("RemoveDuplicates", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));
		public static DependencyProperty SortProperty = DependencyProperty.Register("Sort", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

		private Regex regex;
		private DataParser parser;
		private ListFlattener flattener;
		private IEnumerable<string> matches;
		private bool isItemFocus = false;

		public MainWindowViewModel ViewModel { get; private set; }

		public string MatchInfo
		{
			get { return (string)GetValue(MatchInfoProperty); }
			set { SetValue(MatchInfoProperty, value); }
		}

		public bool RemoveDuplicates
		{
			get { return (bool)GetValue(RemoveDuplicatesProperty); }
			set { SetValue(RemoveDuplicatesProperty, value); }
		}

		public bool Sort
		{
			get { return (bool)GetValue(SortProperty); }
			set { SetValue(SortProperty, value); }
		}

		private IEnumerable<string> StringMatches
		{
			get { return matches == null ? null : matches; }
		}

		public MainWindow()
		{
			InitializeComponent();

			this.ViewModel = new MainWindowViewModel();
			this.DataContext = this.ViewModel;
		}

		private void Close_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			this.Close();
		}

		private void Find_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = regex != null;// && flattener != null;
		}


		private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			var dialog = new OpenFileDialog();
			dialog.Filter = "Text Files (*.txt)|*.txt|CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";

			if (dialog.ShowDialog(this) ?? false)
			{
				var set = new List<string>();

				using (var reader = new StreamReader(File.OpenRead(dialog.FileName)))
				{
					while (!reader.EndOfStream)
					{
						set.Add(reader.ReadLine());
					}
				}

				ViewModel.AddRawSet(set);
			}
		}

		private void ClearScratch_Executed(object sender, ExecutedRoutedEventArgs e)
		{
		}

		private void CreateSetFromMatches_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.AddRawSet(matches);
		}

		private void Move_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = flattener != null && matches != null;
		}

		private void New_Click(object sender, RoutedEventArgs e)
		{
		}

		private void sort_Click(object sender, RoutedEventArgs e)
		{
			this.Sort = !this.Sort;
		}

		private void removeDuplicates_Click(object sender, RoutedEventArgs e)
		{
			this.RemoveDuplicates = !this.RemoveDuplicates;
		}

		private void UnionSets_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.UnionSelectedSets();
		}

		private void IntersectSets_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.IntersectSelectedSets();
		}

		private void DifferenceSets_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.DifferenceSelectedSets();
		}

		private void SymmetricDifferenceSets_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.SymmetricDifferenceSelectedSets();
		}

		private void Subset_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.Subset();
		}

		private void InvertedSubset_Execute(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.InvertedSubset();
		}

		private void RemoveSets_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.RemoveSelectedSets();
		}

        private void RemoveElements_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.RemoveSelectedElements();
        }

        private void Paste_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = ViewModel.CanPaste;
		}

		private void Paste_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.Paste();
		}

		private void BinaryOp_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = ViewModel.CanDoBinaryOp;
		}

		private void UnaryOp_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = ViewModel.CanDoUnaryOp;
		}

        private void ElementOp_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewModel.CanDoElementOp;
        }

        private void Copy_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.Copy(ListFlattener.TextWithLinebreaks, this.isItemFocus);
		}

		private void CopyAs_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.Copy(ListFlattener.All.Single(lf => lf.Name == (string)e.Parameter), this.isItemFocus);
		}

		private void elements_GotFocus(object sender, RoutedEventArgs e)
		{
			this.isItemFocus = true;
		}

		private void setlist_GotFocus(object sender, RoutedEventArgs e)
		{
			this.isItemFocus = false;
		}
	}
}
