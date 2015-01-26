using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace Pad.UI
{
	public class ViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private readonly Dispatcher dispatcher;

		protected ViewModel()
		{
			dispatcher = Dispatcher.CurrentDispatcher;
		}

		protected Dispatcher Dispatcher
		{
			get
			{
				return dispatcher;
			}
		}

		protected void Execute(Action action)
		{
			if (this.Dispatcher.CheckAccess())
			{
				action.Invoke();
			}
			else
			{
				this.Dispatcher.Invoke(DispatcherPriority.DataBind, action);
			}
		}

		protected void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

	}
}
