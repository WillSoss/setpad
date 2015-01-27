using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pad.Core
{
	public class NamedRegex
	{
		public string Name { get; set; }
		public string Regex { get; set; }
		public string Format { get; set; }

		public NamedRegex(string name, string regex, string format)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException("Name is required");

			if (string.IsNullOrEmpty(regex))
				throw new ArgumentNullException("Regex is required");

			//var validate = RegexManager.GetRegex(regex, false);

			//if (validate == null)
			//	throw new ArgumentException(string.Format("The regex '{0}' is invalid", regex));

			this.Name = name;
			this.Regex = regex;
			this.Format = format ?? string.Empty;
		}

		public override string ToString()
		{
			return string.Format("{0}: {1}", Name, Regex); 
		}
	}
}
