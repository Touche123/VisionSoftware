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
		private InspectModel _inspectModel;
		private UserControl_Inspect _userControlInspect;
		public UserControl_Inspect_Add(UserControl_Inspect userControlInspect, InspectModel inspectModel)
        {
            InitializeComponent();
			_inspectModel = inspectModel;
			_userControlInspect = userControlInspect;
		}

		private void Button_Click_AddPattern(object sender, RoutedEventArgs e)
		{
			_inspectModel.Tools.Add(new PatternTool());
			_userControlInspect.ContentControl.Content = new UserControl_Inspect_Hierarchy(_userControlInspect, _inspectModel);
		}
	}
}
