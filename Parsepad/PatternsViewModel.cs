using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pad.Core;
using Pad.UI;

namespace Parsepad
{
	public class PatternsViewModel : ViewModel
	{
		private readonly ObservableCollection<string> recent = new ObservableCollection<string>();
		private readonly ObservableCollection<NamedRegex> saved = new ObservableCollection<NamedRegex>();

		public ObservableCollection<string> Recent
		{
			get { return recent; }
		}

		public ObservableCollection<NamedRegex> Saved
		{
			get { return saved; }
		}

		public PatternsViewModel()
		{
			foreach (var s in RegexManager.GetRecent())
				recent.Add(s);

			RefreshNamed();
		}

		private void RefreshNamed()
		{
			saved.Clear();

			foreach (var s in RegexManager.GetNamed().OrderBy(n => n.Name))
				saved.Add(s);
		}

		public void AddRecent(string regex)
		{
			RegexManager.AddRecent(regex);

			if (recent.Contains(regex))
				recent.Move(recent.IndexOf(regex), 0);
			else
				recent.Insert(0, regex);
		}

		public void AddNamed(string name, string regex, string format)
		{
			RegexManager.AddNamed(name, regex, format);

			RefreshNamed();
		}
	}
}
