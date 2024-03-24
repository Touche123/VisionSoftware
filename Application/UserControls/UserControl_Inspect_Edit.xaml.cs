using System;
using System.Collections.Generic;
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

namespace MainApp.UserControls
{
    /// <summary>
    /// Interaction logic for UserControl_Inspect_Edit.xaml
    /// </summary>
    public partial class UserControl_Inspect_Edit : UserControl
    {
		private InspectModel _inspectModel;
		private ITool _selecetedTool;
		private UserControl_Inspect _userControlInspect;
		public ITool SelectedTool
        {
            get { return _selecetedTool; }
            set { _selecetedTool = value; }
        }

		public string SelectedToolName
		{
			get { return (string)GetValue(SelectedToolNameProperty); }
			set { SetValue(SelectedToolNameProperty, value); }
		}

		public static readonly DependencyProperty SelectedToolNameProperty =
		DependencyProperty.Register("SelectedToolName", typeof(string), typeof(UserControl_Inspect_Edit));

		public UserControl_Inspect_Edit(UserControl_Inspect userControlInspect, InspectModel inspectModel)
        {
            InitializeComponent();
			_inspectModel = inspectModel;
			_userControlInspect = userControlInspect;
			SelectedTool = _inspectModel.SelectedTool;
            SelectedToolName = SelectedTool.Name;
        }

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			_userControlInspect.ContentControl.Content = new UserControl_Inspect_Hierarchy(_userControlInspect, _inspectModel);
		}

		private void Button_Click_Delete(object sender, RoutedEventArgs e)
		{
			_inspectModel.Tools.Remove(SelectedTool);
			_userControlInspect.ContentControl.Content = new UserControl_Inspect_Hierarchy(_userControlInspect, _inspectModel);
		}
	}
}
