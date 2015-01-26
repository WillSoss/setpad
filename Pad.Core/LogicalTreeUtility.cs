using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections;

namespace Pad.Core
{
	public static class LogicalTreeUtility
	{
		public static IEnumerable GetChildren(DependencyObject obj, Boolean allChildrenInHierachy)
		{
			if (!allChildrenInHierachy)
			{
				return LogicalTreeHelper.GetChildren(obj);
			}
			else
			{
				List<object> ReturnValues = new List<object>();

				RecursionReturnAllChildren(obj, ReturnValues);

				return ReturnValues;
			}
		}

		private static void RecursionReturnAllChildren(DependencyObject obj, List<object> returnValues)
		{
			foreach (object curChild in LogicalTreeHelper.GetChildren(obj))
			{
				returnValues.Add(curChild);

				if (curChild is DependencyObject)
					RecursionReturnAllChildren((DependencyObject)curChild, returnValues);
			}
		}

		public static IEnumerable<ReturnType> GetChildren<ReturnType>(DependencyObject obj, Boolean allChildrenInHierachy)
		{
			foreach (object child in GetChildren(obj, allChildrenInHierachy))
				if (child is ReturnType)
					yield return (ReturnType)child;
		}
	}
}
