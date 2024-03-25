using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MainApp
{
	public class DependencyProperties : DependencyObject
	{
		public static readonly DependencyProperty InspectModelPropertyProperty =
		DependencyProperty.Register("InspectModel", typeof(InspectModel), typeof(DependencyProperties), new PropertyMetadata());

		public InspectModel InspectModelProperty
		{
			get { return (InspectModel)GetValue(InspectModelPropertyProperty); }
			set { SetValue(InspectModelPropertyProperty, value); }
		}
	}
}
