using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp
{
    public class InspectViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
		

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

		private InspectService _inspectService;

		public InspectViewModel()
        {
			_inspectService = ServiceLocator.ResolveSingleton<InspectService>();
		}
    }
}
