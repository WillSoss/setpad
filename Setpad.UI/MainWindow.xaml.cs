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

			//scratch.Document.PageWidth = 1000;
		}

		private void Close_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			this.Close();
		}

		private void Find_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = regex != null;// && flattener != null;
		}

		private void Find_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Find();
		}

		private void FindAs_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			flattener = ListFlattener.All.Single(lf => lf.Name == (string)e.Parameter);
			Clipboard.SetText(flattener.GetString(matches));
		}

		private void Find()
		{
			parser = new DataParser(regex, format.Text);

			MatchInfo = "Finding matches";

			var defaultCursor = Cursor;
			Cursor = System.Windows.Input.Cursors.Wait;

			this.matches = parser.Parse(scratch.Document);

			Cursor = defaultCursor;

			if (this.matches != null)
			{
				var matches = this.matches.AsQueryable();

				if (RemoveDuplicates)
					matches = matches.Distinct();

				if (Sort)
					matches = matches.OrderBy(s => s);

				results.Document = new FlowDocument(new Paragraph(new Run(ListFlattener.Default.GetString(matches))));

				MatchInfo = string.Format("{0} matches", matches.Count());
			}
		}

		private void pattern_TextChanged(object sender, TextChangedEventArgs e)
		{
			pattern.Foreground = Brushes.Black;

			if (!string.IsNullOrEmpty(pattern.Text))
			{
				regex = RegexManager.GetRegex(pattern.Text, false);

				if (regex != null)
				{
					pattern.Foreground = Brushes.DarkGreen;

					MatchInfo = "Valid regex";
				}
				else
				{
					pattern.Foreground = Brushes.DarkRed;

					MatchInfo = "Invalid regex";
				}
			}
			else
			{
				MatchInfo = "No pattern";
			}

		}

		private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			var namedRegex = RegexManager.PersistedRegexes.Single(nr => nr.Name == (string)e.Parameter);

			pattern.Text = namedRegex.Regex;
		}

		private void ClearScratch_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			scratch.Document = new FlowDocument();
		}

		private void Move_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = flattener != null && matches != null;
		}

		private void Move_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			scratch.Document = new FlowDocument(new Paragraph(new Run(flattener.GetString(matches))));
			results.Document.Blocks.Clear();
		}

		private void SaveRegex_Click(object sender, RoutedEventArgs e)
		{
			RegexWindow regexWindow = new RegexWindow();

			regexWindow.Regex = pattern.Text;

			if (regexWindow.ShowDialog() ?? false)
				RegexManager.Persist(regexWindow.RegexName, regexWindow.Regex);
		}

		private void New_Click(object sender, RoutedEventArgs e)
		{
			pattern.Text = string.Empty;
		}

		private void sort_Click(object sender, RoutedEventArgs e)
		{
			this.Sort = !this.Sort;
		}

		private void removeDuplicates_Click(object sender, RoutedEventArgs e)
		{
			this.RemoveDuplicates = !this.RemoveDuplicates;
		}
	}
}
