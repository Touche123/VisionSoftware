using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp
{

    public interface ITool
    {
        string Name { get; }
    }

	public class PatternTool : ITool
	{
		public string Name
		{
			get { return "Pattern"; }
		}
	}

	public class InspectModel 
	{
		private ITool _selectedTool;
		public ITool SelectedTool
		{
			get { return _selectedTool; }
			set 
			{ 
				_selectedTool = value;
				OnPropertyChanged(nameof(SelectedTool));
			}
		}

		public event PropertyChangedEventHandler? PropertyChanged;
        private ObservableCollection<ITool> _tools; 
		public ObservableCollection<ITool> Tools
        {
            get { return _tools; }
            set 
            { 
                _tools = value;
				OnPropertyChanged(nameof(Tools));
			}
        }

        public InspectModel()
        {
            Tools = new ObservableCollection<ITool>();
		}

		public void AddTool()
        {
            Tools.Add(new PatternTool());
		}

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
