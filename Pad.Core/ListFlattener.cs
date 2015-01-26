using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Pad.Core
{
	public abstract class ListFlattener
	{
		private static readonly ReaderWriterLockSlim initLock = new ReaderWriterLockSlim();
		private static ListFlattener[] flatteners;

		public static ListFlattener Default
		{
			get { return All.Single(f => f is ListFlatteners.TextWithLinebreaks); }
		}

		public static ListFlattener[] All
		{
			get
			{
				if (flatteners == null)
				{
					initLock.EnterWriteLock();

					try
					{
						if (flatteners == null)
						{
							List<ListFlattener> list = new List<ListFlattener>();

							foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
								foreach (var type in asm.GetTypes())
									if (!type.IsAbstract && typeof(ListFlattener).IsAssignableFrom(type))
									{
										var ctor = type.GetConstructor(Type.EmptyTypes);

										if (ctor != null)
											list.Add((ListFlattener)ctor.Invoke(new object[0]));
									}

							flatteners = list.OrderBy(f => f.Name).ToArray();
						}
					}
					finally
					{
						initLock.ExitWriteLock();
					}

				}

				return flatteners;
			}
		}

		public string Name { get; private set; }
		public string Description { get; private set; }

		protected ListFlattener(string name, string description)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException("Name is required");

			if (name.Length > 30)
				throw new ArgumentOutOfRangeException("Name cannot be longer than 30 characters");

			description = description ?? string.Empty;

			if (description.Length > 500)
				throw new ArgumentOutOfRangeException("Description cannot be longer than 500 characters");

			this.Name = name;
			this.Description = description;
		}

		public abstract string GetString(IEnumerable<string> list);

		protected string SimpleFlatten(IEnumerable<string> list, string qualifier, string delimeter, string prefix, string suffix)
		{
			return SimpleFlatten(list, qualifier, delimeter, prefix, suffix, null, -1);
		}

		protected string SimpleFlatten(IEnumerable<string> list, string qualifier, string delimeter, string prefix, string suffix, string altDelim, int altDelimEvery)
		{
			if (list == null)
				throw new ArgumentNullException("List is required");

			qualifier = qualifier ?? string.Empty;
			delimeter = delimeter ?? string.Empty;
			prefix = prefix ?? string.Empty;
			suffix = suffix ?? string.Empty;
			altDelim = altDelim ?? string.Empty;

			StringBuilder sb = new StringBuilder();

			sb.Append(prefix);

			int count = 0;

			foreach (string item in list)
			{
				if (count > 0)
				{
					if (altDelimEvery > -1 && count % altDelimEvery == 0)
						sb.Append(altDelim);
					else
						sb.Append(delimeter);
				}

				sb.Append(string.Format("{0}{1}{0}", qualifier, item));

				count++;
			}

			sb.Append(suffix);

			return sb.ToString();
		}

		public override string ToString()
		{
			return Name;
		}

	}
}
