using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pad.Core;
using Pad.UI;

namespace Regexpad
{
	public class PatternsViewModel : ViewModel
	{
		private readonly ObservableCollection<string> recent = new ObservableCollection<string>();
		private readonly ObservableCollection<NamedRegex> saved = new ObservableCollection<NamedRegex>();
		private readonly ObservableCollection<string> recentFormats = new ObservableCollection<string>();

		public ObservableCollection<string> Recent
		{
			get { return recent; }
		}

		public ObservableCollection<NamedRegex> Saved
		{
			get { return saved; }
		}

		public ObservableCollection<string> RecentFormats
		{
			get { return recentFormats; }
		}

		public PatternsViewModel()
		{
			foreach (var s in RegexManager.GetRecent())
				recent.Add(s);

			foreach (var s in RegexManager.GetRecentFormats())
				recentFormats.Add(s);

			RefreshNamed();
		}

		private void RefreshNamed()
		{
			saved.Clear();

			foreach (var s in RegexManager.GetNamed().OrderBy(n => n.Name))
				saved.Add(s);
		}

		public void AddRecent(string regex, string format)
		{
			RegexManager.AddRecent(regex, format);

			if (recent.Contains(regex))
				recent.Move(recent.IndexOf(regex), 0);
			else
				recent.Insert(0, regex);

			if (!string.IsNullOrWhiteSpace(format))
			{
				if (recentFormats.Contains(format))
					recentFormats.Move(recentFormats.IndexOf(format), 0);
				else
					recentFormats.Insert(0, format);
			}
		}

		public void AddNamed(string name, string regex, string format)
		{
			RegexManager.AddNamed(name, regex, format);

			RefreshNamed();
		}
	}
}
