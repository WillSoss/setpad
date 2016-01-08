using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Setpad.UI
{
	public static class Commands
	{
		public static RoutedCommand Copy = new RoutedCommand();
		public static RoutedCommand CopyAs = new RoutedCommand();
		public static RoutedCommand Open = new RoutedCommand();
		public static RoutedCommand Paste = new RoutedCommand();

		public static RoutedCommand UnionSets = new RoutedCommand();
		public static RoutedCommand IntersectSets = new RoutedCommand();
		public static RoutedCommand DifferenceSets = new RoutedCommand();
		public static RoutedCommand SymmetricDifferenceSets = new RoutedCommand();
		public static RoutedCommand RemoveSets = new RoutedCommand();
        public static RoutedCommand RemoveElements = new RoutedCommand();
    }
}
