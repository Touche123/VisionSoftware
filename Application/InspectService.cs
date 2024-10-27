using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Tensorflow.GraphTransferInfo.Types;

namespace MainApp
{
    public class InspectService : IService, INotifyPropertyChanged
    {
        private InspectModel _inspectModel = new InspectModel();

		public InspectModel InspectModel { get => _inspectModel; set => _inspectModel = value; }

		public event PropertyChangedEventHandler? PropertyChanged;

		private void InspectModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			OnPropertyChanged(e.PropertyName);
		}

		public InspectService()
        {
            InspectModel = new InspectModel();
            InspectModel.Tools.Add(new PatternTool());
            _inspectModel.PropertyChanged += InspectModel_PropertyChanged;
		}


        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
