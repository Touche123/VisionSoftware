using MainApp.UserControls;
using OpenCvSharp;
using Prism.Commands;
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

namespace MainApp
{
    /// <summary>
    /// Interaction logic for UserControl_Inspect.xaml
    /// </summary>
    public partial class UserControl_Inspect : UserControl
	{
		private Mat _destination;

		UserControl _inspectContent;
		public UserControl InspectContent
		{
			get { return _inspectContent; }
			set
			{
				_inspectContent = value;
			}
		}

        public InspectModel InspectModel
        {
            get { return (InspectModel)GetValue(InspectModelProperty); }
			set { SetValue(InspectModelProperty, value); }
		}

		public ContentControl ContentControl
		{
			get { return (ContentControl)GetValue(ContentControlProperty); }
			set { SetValue(ContentControlProperty, value); }
		}

		public static readonly DependencyProperty InspectModelProperty =
		DependencyProperty.Register("InspectModel", typeof(InspectModel), typeof(UserControl_Inspect));
		public static readonly DependencyProperty ContentControlProperty =
		DependencyProperty.Register("ContentControl", typeof(ContentControl), typeof(UserControl_Inspect));

		public UserControl_Inspect(Mat destination, InspectModel inspectModel)
        {
            InitializeComponent();
			_destination = destination;
			InspectModel = inspectModel;
			ContentControl = new UserControl_Inspect_Hierarchy(this, InspectModel);
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			ContentControl = new UserControl_Inspect_Add(this, _destination, InspectModel);
            //InspectModel.Tools.Add(new PatternTool());
		}

	}
}
