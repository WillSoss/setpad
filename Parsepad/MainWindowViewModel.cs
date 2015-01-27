﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Pad.Core;
using Pad.UI;
using RegexOpts = System.Text.RegularExpressions.RegexOptions;

namespace Parsepad
{
	public class MainWindowViewModel : ViewModel
	{
		private readonly RegexOptionsViewModel regexOptions = new RegexOptionsViewModel();
		private readonly MatchOptionsViewModel matchOptions = new MatchOptionsViewModel();
		private readonly ObservableCollection<string> patterns = new ObservableCollection<string>();
		private readonly ObservableCollection<PatternMatch> matches = new ObservableCollection<PatternMatch>();

		private string pattern;
		private bool isPatternValid;
		private string format;
		private string searchText;

		private Brush patternForeground;
		private string matchInfo;

		public RegexOptionsViewModel RegexOptions
		{
			get { return regexOptions; }
		}

		public MatchOptionsViewModel MatchOptions
		{
			get { return matchOptions; }
		}

		public ObservableCollection<string> Patterns
		{
			get { return patterns; }
		}

		public ObservableCollection<PatternMatch> Matches
		{
			get { return matches; }
		}

		public string Pattern
		{
			get { return pattern; }
			set
			{
				if (pattern != value)
				{
					pattern = value;

					OnPatternChanged();
					OnPropertyChanged("Pattern");
				}
			}
		}

		public bool IsPatternValid
		{
			get { return isPatternValid; }
			set
			{
				if (isPatternValid != value)
				{
					isPatternValid = value;

					OnPropertyChanged("IsPatternValid");
				}
			}
		}

		public string Format
		{
			get { return format; }
			set
			{
				if (format != value)
				{
					format = value;

					OnPropertyChanged("Format");
				}
			}
		}

		public string SearchText
		{
			get { return searchText; }
			set
			{
				if (searchText != value)
				{
					searchText = value;

					ClearMatches();
					OnPropertyChanged("SearchText");
				}
			}
		}

		public Brush PatternForeground
		{
			get { return patternForeground; }
			set
			{
				if (patternForeground != value)
				{
					patternForeground = value;

					OnPropertyChanged("PatternForeground");
				}
			}
		}

		public string MatchInfo
		{
			get { return matchInfo; }
			set
			{
				if (matchInfo != value)
				{
					matchInfo = value;

					OnPropertyChanged("MatchInfo");
				}
			}
		}

		public RegexOpts PatternOptions
		{
			get
			{
				return (RegexOptions.IgnoreCase ? RegexOpts.IgnoreCase : 0) |
					(RegexOptions.Multiline ? RegexOpts.Multiline : 0) |
					(RegexOptions.Singleline ? RegexOpts.Singleline : 0) |
					(RegexOptions.RightToLeft ? RegexOpts.RightToLeft : 0);
			}
		}

		public MainWindowViewModel()
		{
			PatternForeground = Brushes.Black;

			foreach (var s in RegexManager.GetRecent())
				Patterns.Add(s);
		}

		private void OnPatternChanged()
		{
			PatternForeground = Brushes.Black;

			if (!string.IsNullOrEmpty(Pattern))
			{
				var regex = RegexManager.GetRegex(Pattern, PatternOptions);

				if (regex != null)
				{
					IsPatternValid = true;

					PatternForeground = Brushes.DarkGreen;

					MatchInfo = "Valid regex";
				}
				else
				{
					IsPatternValid = false;

					PatternForeground = Brushes.DarkRed;

					MatchInfo = "Invalid regex";
				}
			}
			else
			{
				MatchInfo = "No pattern";
			}
		}

		public void ClearMatches()
		{
			MatchInfo = string.Empty;
			Matches.Clear();
		}

		public void Parse()
		{
			MatchInfo = "Finding matches";

			var parser = new DataParser(RegexManager.GetRegex(Pattern, PatternOptions), Format, SearchText);

			//var defaultCursor = Cursor;
			//Cursor = System.Windows.Input.Cursors.Wait;

			var unfiltered = parser.GetMatches();

			//Cursor = defaultCursor;

			if (unfiltered != null)
			{
				var query = unfiltered.AsQueryable();

				if (MatchOptions.RemoveEmptyStrings)
					query = query.Where(s => !string.IsNullOrEmpty(s.FormattedText));

				if (MatchOptions.RemoveWhitespaceStrings)
					query = query.Where(s => !string.IsNullOrWhiteSpace(s.FormattedText));

				if (MatchOptions.RemoveDuplicates)
					query = query.Distinct();

				if (matchOptions.Sort)
					query = query.OrderBy(s => s.FormattedText);

				matches.Clear();

				foreach (var m in query)
					matches.Add(m);

				//Results = ListFlattener.TextWithLinebreaks.GetString(matches);

				MatchInfo = string.Format("{0} matches", matches.Count());
			}

			RegexManager.AddRecent(Pattern);
		}

		public void Copy(ListFlattener flattener)
		{
			Clipboard.SetText(flattener.GetString(matches.Select(m => m.FormattedText)));
		}

		public void MoveResultsToInput()
		{
			SearchText = ListFlattener.TextWithLinebreaks.GetString(matches.Select(m => m.FormattedText));
			Matches.Clear();
			MatchInfo = string.Empty;
		}
	}
}