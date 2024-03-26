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
        
        private Mat _destination = new Mat();

        /// <summary>
		/// Image to be processed
		/// </summary>
		public Mat Destination
        {
            get { return _destination; }
            set
            {
                _destination = value;
                OnPropertyChanged(nameof(Destination));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
