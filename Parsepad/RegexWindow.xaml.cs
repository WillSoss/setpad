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
using System.Windows.Shapes;

namespace Parsepad
{
	/// <summary>
	/// Interaction logic for RegexWindow.xaml
	/// </summary>
	public partial class RegexWindow : Window
	{
		public string RegexName { get; set; }
		public bool SaveFormat { get; set; }
		public bool EnableSaveFormat { get; set; }

		public RegexWindow(bool enableSaveFormat)
		{
			InitializeComponent();

			this.EnableSaveFormat = enableSaveFormat;
			this.Loaded += new RoutedEventHandler(RegexWindow_Loaded);
		}

		void RegexWindow_Loaded(object sender, RoutedEventArgs e)
		{
			name.Text = this.RegexName;
			saveFormat.IsEnabled = EnableSaveFormat;
		}

		private void save_Click(object sender, RoutedEventArgs e)
		{
			this.RegexName = this.name.Text;
			this.SaveFormat = EnableSaveFormat && (saveFormat.IsChecked ?? false);
			
			this.DialogResult = true;
			this.Close();
		}

		private void cancel_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
			this.Close();
		}

	}
}
