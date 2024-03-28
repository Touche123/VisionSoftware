using MainApp.UserControls;
using MainApp.ViewModel;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tensorflow.Contexts;

namespace MainApp
{
    /// <summary>
    /// Interaction logic for UserControl_Inspect_Hierarchy.xaml
    /// </summary>
    public partial class UserControl_Inspect_Hierarchy : UserControl
    {
		private UserControl_Inspect _userControlInspect;
		public ICommand EditCommand { get; private set; }
		public readonly InspectService _inspectService;

		private void InspectService_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			
		}
		public UserControl_Inspect_Hierarchy(UserControl_Inspect userControlInspect)
        {
            InitializeComponent();
			_inspectService = ServiceLocator.ResolveSingleton<InspectService>();
			_userControlInspect = userControlInspect;
			EditCommand = new DelegateCommand(EditCommandExecute);

			_inspectService.PropertyChanged += InspectService_PropertyChanged;
		}

		public void EditCommandExecute()
		{
			_inspectService.InspectModel.Tools.RemoveAt(0);
			var bla = 2;
			return;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			
		}

		private void Button_Edit_Click(object sender, RoutedEventArgs e)
		{
			// Find the parent ListViewItem of the clicked child item
			ListViewItem parentListViewItem = FindParent<ListViewItem>((DependencyObject)sender);

			if (parentListViewItem != null)
			{
				// Select the parent ListViewItem
				//parentListViewItem.IsSelected = true;

				ITool tool = parentListViewItem.DataContext as ITool;

				if (tool != null)
				{
					_inspectService.InspectModel.SelectedTool = tool;
				}

			}

			_userControlInspect.ContentControl.Content = new UserControl_Inspect_Edit(_userControlInspect);
		}

		private void Button_Click_Delete(object sender, RoutedEventArgs e)
		{
			// Find the parent ListViewItem of the clicked child item
			ListViewItem parentListViewItem = FindParent<ListViewItem>((DependencyObject)sender);

			if (parentListViewItem != null)
			{
				// Select the parent ListViewItem
				//parentListViewItem.IsSelected = true;

				ITool tool = parentListViewItem.DataContext as ITool;

				if (tool != null)
				{
					_inspectService.InspectModel.Tools.Remove(tool);
				}
				
			}
			
		}

		// Helper method to find the parent ListViewItem
		private T FindParent<T>(DependencyObject child) where T : DependencyObject
		{
			DependencyObject parentObject = VisualTreeHelper.GetParent(child);

			if (parentObject == null)
				return null;

			T parent = parentObject as T;
			return parent ?? FindParent<T>(parentObject);
		}
	}
}
