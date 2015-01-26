using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pad.UI;

namespace Parsepad
{
	public class MatchOptionsViewModel : ViewModel
	{
		private const string RemoveDuplicatesProperty = "RemoveDuplicates";
		private const string RemoveEmptyStringsProperty = "RemoveEmptyStrings";
		private const string RemoveWhitespaceStringsProperty = "RemoveWhitespaceStrings";
		private const string SortProperty = "Sort";

		private bool removeDuplicates;
		private bool removeEmptyStrings;
		private bool removeWhitespaceStrings;
		private bool sort;

		public MatchOptionsViewModel()
		{
			RemoveDuplicates = GetSetting(RemoveDuplicatesProperty);
			RemoveEmptyStrings = GetSetting(RemoveEmptyStringsProperty);
			RemoveWhitespaceStrings = GetSetting(RemoveWhitespaceStringsProperty);
			Sort = GetSetting(SortProperty);
		}

		public bool RemoveDuplicates
		{
			get { return removeDuplicates; }
			set
			{
				if (removeDuplicates != value)
				{
					removeDuplicates = value;

					OnPropertyChanged(RemoveDuplicatesProperty);

					SetSetting(RemoveDuplicatesProperty, value);
				}
			}
		}

		public bool RemoveEmptyStrings
		{
			get { return removeEmptyStrings; }
			set
			{
				if (removeEmptyStrings != value)
				{
					removeEmptyStrings = value;

					OnPropertyChanged(RemoveEmptyStringsProperty);

					SetSetting(RemoveEmptyStringsProperty, value);
				}
			}
		}

		public bool RemoveWhitespaceStrings
		{
			get { return removeWhitespaceStrings; }
			set
			{
				if (removeWhitespaceStrings != value)
				{
					removeWhitespaceStrings = value;

					OnPropertyChanged(RemoveWhitespaceStringsProperty);

					SetSetting(RemoveWhitespaceStringsProperty, value);
				}
			}
		}

		public bool Sort
		{
			get { return sort; }
			set
			{
				if (sort != value)
				{
					sort = value;

					OnPropertyChanged(SortProperty);

					SetSetting(SortProperty, value);
				}
			}
		}

		private void SetSetting(string key, bool value)
		{
			var cu = RegHelper.GetCurrentUser("Parsepad");

			cu.SetValue(key, Convert.ToInt32(value), Microsoft.Win32.RegistryValueKind.DWord);
		}

		private bool GetSetting(string key)
		{
			var cu = RegHelper.GetCurrentUser("Parsepad");

			return Convert.ToBoolean(((int?)cu.GetValue(key)) ?? 0);
		}
	}
}
