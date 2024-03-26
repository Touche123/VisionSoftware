using OpenCvSharp;
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
    /// Interaction logic for UserControl_Inspect_Add.xaml
    /// </summary>
    public partial class UserControl_Inspect_Add : UserControl
    {
		//public InspectModel InspectModel
		//{
			
		//	get { return DependencyProperties.InspectModelProperty(this); }
		//	set { SetValue(InspectModelProperty, value); }
		//}

		//public static readonly DependencyProperty InspectModelProperty =
		//DependencyProperty.Register("InspectModel", typeof(InspectModel), typeof(UserControl_Inspect));

		private Mat _destination;
		private InspectModel _inspectModel;
		private UserControl_Inspect _userControlInspect;
		public UserControl_Inspect_Add(UserControl_Inspect userControlInspect, Mat destination, InspectModel inspectModel)
        {
            InitializeComponent();
			DataContext = new InspectViewModel();
			_destination = destination;
			_inspectModel = inspectModel;
			_userControlInspect = userControlInspect;
		}

		private void Button_Click_AddPattern(object sender, RoutedEventArgs e)
		{
            _inspectModel.Tools.Add(new PatternTool());
			_userControlInspect.ContentControl.Content = new UserControl_Inspect_Hierarchy(_userControlInspect, _inspectModel);
			_userControlInspect.ContentControl.Content = new UserControl_Inspect_Add_Pattern(_userControlInspect, _destination, _inspectModel);
		}
	}
}
