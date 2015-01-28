using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
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
		private readonly PatternsViewModel patterns = new PatternsViewModel();
		private readonly ObservableCollection<PatternMatch> matches = new ObservableCollection<PatternMatch>();

		private string pattern;
		private bool isPatternValid;
		private string format;
		private string searchText;

		private Brush patternForeground;
		private string textInfo;
		private string regexInfo;
		private string matchInfo;

		private Cursor cursor;

		public RegexOptionsViewModel RegexOptions
		{
			get { return regexOptions; }
		}

		public MatchOptionsViewModel MatchOptions
		{
			get { return matchOptions; }
		}

		public PatternsViewModel Patterns
		{
			get { return patterns; }
		}

		public ObservableCollection<PatternMatch> Matches
		{
			get { return matches; }
		}

		public Cursor Cursor
		{
			get { return cursor; }
			set
			{
				if (cursor != value)
				{
					cursor = value;

					OnPropertyChanged("Cursor");
				}
			}
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

		public string TextInfo
		{
			get { return textInfo; }
			set
			{
				if (textInfo != value)
				{
					textInfo = value;

					OnPropertyChanged("TextInfo");
				}
			}
		}

		public string RegexInfo
		{
			get { return regexInfo; }
			set
			{
				if (regexInfo != value)
				{
					regexInfo = value;

					OnPropertyChanged("RegexInfo");
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

					RegexInfo = "Valid regex";
				}
				else
				{
					IsPatternValid = false;

					PatternForeground = Brushes.DarkRed;

					RegexInfo = "Invalid regex";
				}
			}
			else
			{
				RegexInfo = string.Empty;
			}
		}

		public void ClearMatches()
		{
			MatchInfo = string.Empty;
			Matches.Clear();
		}

		public bool CanParse()
		{
			return IsPatternValid && !string.IsNullOrEmpty(SearchText);
		}

		public void Parse()
		{
			MatchInfo = "Finding matches";

			var parser = new DataParser(RegexManager.GetRegex(Pattern, PatternOptions), Format, SearchText);

			var defaultCursor = Cursor;
			Cursor = System.Windows.Input.Cursors.Wait;

			var unfiltered = parser.GetMatches();


			if (unfiltered != null)
			{
				var query = unfiltered.AsQueryable();

				if (MatchOptions.RemoveEmptyStrings)
					query = query.Where(s => !string.IsNullOrEmpty(s.FormattedText));

				if (MatchOptions.RemoveWhitespaceStrings)
					query = query.Where(s => !string.IsNullOrWhiteSpace(s.FormattedText));

				if (MatchOptions.RemoveDuplicates)
					query = query.Distinct(new PatternMatchEqualityComparer());

				if (matchOptions.Sort)
					query = query.OrderBy(s => s.FormattedText);

				matches.Clear();

				foreach (var m in query)
					matches.Add(m);

				//Results = ListFlattener.TextWithLinebreaks.GetString(matches);

				MatchInfo = string.Format("{0} matches", matches.Count());
			}

			var pattern = Pattern;
			var format = Format;

			Patterns.AddRecent(Pattern, Format);

			Pattern = pattern;
			Format = format;

			Cursor = defaultCursor;
		}

		public void Copy(ListFlattener flattener)
		{
			StaClipboard.SetText(flattener.GetString(matches.Select(m => m.FormattedText)));
		}

		public void MoveResultsToInput()
		{
			SearchText = ListFlattener.TextWithLinebreaks.GetString(matches.Select(m => m.FormattedText));
			Matches.Clear();
			MatchInfo = string.Empty;
		}
	}
}
