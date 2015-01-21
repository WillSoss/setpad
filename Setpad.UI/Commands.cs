using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Setpad.UI
{
	public static class Commands
	{
		public static RoutedCommand Find = new RoutedCommand();
		public static RoutedCommand FindAs = new RoutedCommand();
		public static RoutedCommand Open = new RoutedCommand();
		public static RoutedCommand Move = new RoutedCommand();
		public static RoutedCommand ClearScratch = new RoutedCommand();
	}
}
