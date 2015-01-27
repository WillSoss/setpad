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

namespace Parsepad
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindowViewModel ViewModel { get; private set; }

		public MainWindow()
		{
			InitializeComponent();

			this.ViewModel = new MainWindowViewModel();
			this.DataContext = this.ViewModel;

			//ViewModel.Scratch = scratch.Document;
		}

		private void Close_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			this.Close();
		}

		private void Find_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = ViewModel.IsPatternValid;
		}

		private void Find_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Find();
		}

		private void Copy_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.Copy(ListFlattener.TextWithLinebreaks);
		}

		private void CopyAs_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.Copy(ListFlattener.All.Single(lf => lf.Name == (string)e.Parameter));
		}

		private void Find()
		{
			searchText.GetBindingExpression(TextBox.TextProperty).UpdateSource();
			ViewModel.Parse();
		}

		private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			var namedRegex = RegexManager.PersistedRegexes.Single(nr => nr.Name == (string)e.Parameter);

			pattern.Text = namedRegex.Regex;
		}

		private void ClearScratch_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			searchText.Clear();
		}

		private void HasResults(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = ViewModel.Matches.Count > 0;
		}

		private void Move_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.MoveResultsToInput();
		}

		private void SaveRegex_Click(object sender, RoutedEventArgs e)
		{
			//RegexWindow regexWindow = new RegexWindow();

			//regexWindow.Regex = pattern.Text;

			//if (regexWindow.ShowDialog() ?? false)
			//	RegexManager.Persist(regexWindow.RegexName, regexWindow.Regex);
		}

		private void ignoreCase_Click(object sender, RoutedEventArgs e)
		{
			ViewModel.RegexOptions.IgnoreCase = !ViewModel.RegexOptions.IgnoreCase;
		}

		private void multiline_Click(object sender, RoutedEventArgs e)
		{
			ViewModel.RegexOptions.Multiline = !ViewModel.RegexOptions.Multiline;
		}

		private void singleline_Click(object sender, RoutedEventArgs e)
		{
			ViewModel.RegexOptions.Singleline = !ViewModel.RegexOptions.Singleline;
		}

		private void rightToLeft_Click(object sender, RoutedEventArgs e)
		{
			ViewModel.RegexOptions.RightToLeft = !ViewModel.RegexOptions.RightToLeft;
		}

		private void removeDuplicates_Click(object sender, RoutedEventArgs e)
		{
			ViewModel.MatchOptions.RemoveDuplicates = !ViewModel.MatchOptions.RemoveDuplicates;
		}

		private void removeEmptyStrings_Click(object sender, RoutedEventArgs e)
		{
			ViewModel.MatchOptions.RemoveEmptyStrings = !ViewModel.MatchOptions.RemoveEmptyStrings;
		}

		private void removeWhitespaceStrings_Click(object sender, RoutedEventArgs e)
		{
			ViewModel.MatchOptions.RemoveWhitespaceStrings = !ViewModel.MatchOptions.RemoveWhitespaceStrings;
		}

		private void sort_Click(object sender, RoutedEventArgs e)
		{
			ViewModel.MatchOptions.Sort = !ViewModel.MatchOptions.Sort;
		}

		private void MatchSelected(object sender, SelectionChangedEventArgs e)
		{
			var match = results.SelectedItem as PatternMatch;

			if (match != null)
			{
				searchText.ScrollToLine(searchText.GetLineIndexFromCharacterIndex(match.Position));
				searchText.Select(match.Position, match.Length);
			}
		}

		private void searchText_TextChanged(object sender, TextChangedEventArgs e)
		{
			ViewModel.ClearMatches();
		}
	}
}
