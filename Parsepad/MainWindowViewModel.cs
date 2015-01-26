using System;
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

		private string pattern;
		private bool isPatternValid;
		private string format;
		private FlowDocument scratch;
		private string results;
		private List<string> matches;

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

		public FlowDocument Scratch
		{
			get { return scratch; }
			set
			{
				if (scratch != value)
				{
					scratch = value;

					OnPropertyChanged("Scratch");
				}
			}
		}

		public string Results
		{
			get { return results; }
			set
			{
				if (results != value)
				{
					results = value;

					OnPropertyChanged("Results");
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

		public void Parse()
		{
			MatchInfo = "Finding matches";

			var parser = new DataParser(RegexManager.GetRegex(Pattern, PatternOptions), Format, new TextRange(Scratch.ContentStart, Scratch.ContentEnd).Text);

			//var defaultCursor = Cursor;
			//Cursor = System.Windows.Input.Cursors.Wait;

			var unfiltered = parser.GetMatches();

			//Cursor = defaultCursor;

			if (unfiltered != null)
			{
				var query = unfiltered.AsQueryable();

				if (MatchOptions.RemoveEmptyStrings)
					query = query.Where(s => !string.IsNullOrEmpty(s));

				if (MatchOptions.RemoveWhitespaceStrings)
					query = query.Where(s => !string.IsNullOrWhiteSpace(s));

				if (MatchOptions.RemoveDuplicates)
					query = query.Distinct();

				if (matchOptions.Sort)
					query = query.OrderBy(s => s);

				matches = query.ToList();

				Results = ListFlattener.Default.GetString(matches);

				MatchInfo = string.Format("{0} matches", matches.Count());
			}

			RegexManager.AddRecent(Pattern);
		}

		public void Copy(ListFlattener flattener)
		{
			Clipboard.SetText(flattener.GetString(matches));
		}
	}
}
