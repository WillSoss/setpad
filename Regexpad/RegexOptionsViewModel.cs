using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pad.UI;

namespace Regexpad
{
	public class RegexOptionsViewModel : ViewModel
	{
		private const string IgnoreCaseProperty = "IgnoreCase";
		private const string MultilineProperty = "Multiline";
		private const string SinglelineProperty = "Singleline";
		private const string RightToLeftProperty = "RightToLeft";

		private bool ignoreCase;
		private bool multiline;
		private bool singleline;
		private bool rightToLeft;

		public RegexOptionsViewModel()
		{
			IgnoreCase = GetSetting(IgnoreCaseProperty);
			Multiline = GetSetting(MultilineProperty);
			Singleline = GetSetting(SinglelineProperty);
			RightToLeft = GetSetting(RightToLeftProperty);
		}

		public bool IgnoreCase
		{
			get { return ignoreCase; }
			set
			{
				if (ignoreCase != value)
				{
					ignoreCase = value;

					OnPropertyChanged(IgnoreCaseProperty);

					SetSetting(IgnoreCaseProperty, value);
				}
			}
		}

		public bool Multiline
		{
			get { return multiline; }
			set
			{
				if (multiline != value)
				{
					multiline = value;

					if (Multiline && Singleline)
						Singleline = false;

					OnPropertyChanged(MultilineProperty);

					SetSetting(MultilineProperty, value);
				}
			}
		}

		public bool Singleline
		{
			get { return singleline; }
			set
			{
				if (singleline != value)
				{
					singleline = value;

					if (Multiline && Singleline)
						Multiline = false;

					OnPropertyChanged(SinglelineProperty);

					SetSetting(SinglelineProperty, value);
				}
			}
		}

		public bool RightToLeft
		{
			get { return rightToLeft; }
			set
			{
				if (rightToLeft != value)
				{
					rightToLeft = value;

					OnPropertyChanged(RightToLeftProperty);

					SetSetting(RightToLeftProperty, value);
				}
			}
		}

		private void SetSetting(string key, bool value)
		{
			var cu = RegHelper.GetCurrentUser("Regexpad");

			cu.SetValue(key, Convert.ToInt32(value), Microsoft.Win32.RegistryValueKind.DWord);
		}

		private bool GetSetting(string key)
		{
			var cu = RegHelper.GetCurrentUser("Regexpad");

			return Convert.ToBoolean(((int?)cu.GetValue(key)) ?? 0);
		}
	}
}
