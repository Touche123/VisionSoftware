using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace MainApp
{
	public static class ListBoxHelper
	{
		public static readonly DependencyProperty SelectedItemsProperty =
			DependencyProperty.RegisterAttached("SelectedItems", typeof(ObservableCollection<object>), typeof(ListBoxHelper),
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SelectedItemsPropertyChanged));

		public static ObservableCollection<object> GetSelectedItems(ListBox listBox)
		{
			return (ObservableCollection<object>)listBox.GetValue(SelectedItemsProperty);
		}

		public static void SetSelectedItems(ListBox listBox, ObservableCollection<object> value)
		{
			listBox.SetValue(SelectedItemsProperty, value);
		}

		

		private static void SelectedItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is ListBox listBox)
			{
				var selectedItems = GetSelectedItems(listBox);
				listBox.SelectedItems.Clear();
				foreach (var item in selectedItems)
				{
					listBox.SelectedItems.Add(item);
				}

				if (e.OldValue is ObservableCollection<object> oldCollection)
				{
					oldCollection.CollectionChanged -= SelectedItems_CollectionChanged;
				}

				if (e.NewValue is ObservableCollection<object> newCollection)
				{
					newCollection.CollectionChanged += SelectedItems_CollectionChanged;
				}
			}
		}

		private static void SelectedItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (sender is ObservableCollection<object> selectedItems)
			{
				var parent = VisualTreeHelper.GetParent(selectedItems[0] as DependencyObject);
				while (parent != null && !(parent is ListBox))
				{
					parent = VisualTreeHelper.GetParent(parent);
				}

				if (parent is ListBox listBox)
				{
					listBox.SelectedItems.Clear();
					foreach (var item in selectedItems.Cast<object>())
					{
						listBox.SelectedItems.Add(item);
					}

					// Raise PropertyChanged event for SelectedImages property in view model
					if (listBox.DataContext is MainWindowViewModel viewModel)
					{
						viewModel.SelectedImages = new ObservableCollection<object>(listBox.SelectedItems.Cast<object>());
					}
				}
			}
		}
	}
}
