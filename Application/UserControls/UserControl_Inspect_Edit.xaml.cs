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
        private ITool _selecetedTool;

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

		public UserControl_Inspect_Edit(ITool selectedTool)
        {
            InitializeComponent();
            SelectedTool = selectedTool;
            SelectedToolName = selectedTool.Name;
        }
    }
}
