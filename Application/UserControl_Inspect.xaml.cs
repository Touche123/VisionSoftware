﻿using System;
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
        private InspectModel _inspectModel;

        public InspectModel InspectModel
        {
            get { return (InspectModel)GetValue(InspectModelProperty); }
			set { SetValue(InspectModelProperty, value); }
		}

		public static readonly DependencyProperty InspectModelProperty =
		DependencyProperty.Register("InspectModel", typeof(InspectModel), typeof(UserControl_Inspect));

		public UserControl_Inspect(InspectModel inspectModel)
        {
            InitializeComponent();
			InspectModel = inspectModel;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
            InspectModel.Tools.Add(new PatternTool());
		}
	}
}
