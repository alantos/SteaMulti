using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace SteaMUlti.Attached_Properties
{
	public class SelectionChangedCommandProperty
	{
		public static DependencyProperty SelectionChangedCommandDependencyProperty =
			DependencyProperty.RegisterAttached("SelectionChangedCommand", typeof(ICommand), typeof(SelectionChangedCommandProperty),
				new FrameworkPropertyMetadata(new PropertyChangedCallback(SelectionChangedCommandProperty.SelectionChanged_Changed)));

		public static DependencyProperty SelectionChangedCommandParameterDependencyProperty =
			DependencyProperty.RegisterAttached("SelectionChangedCommandParameter", typeof(Object), typeof(SelectionChangedCommandProperty),
			new FrameworkPropertyMetadata(null));

		public static void SetSelectionChangedCommand(DependencyObject target, ICommand value)
		{
			target.SetValue(SelectionChangedCommandProperty.SelectionChangedCommandDependencyProperty, value);
		}

		public static ICommand GetSelectionChangedCommand(DependencyObject target)
		{
			return (ICommand)target.GetValue(SelectionChangedCommandProperty.SelectionChangedCommandDependencyProperty);
		}

		public static void SetSelectionChangedCommandParameter(DependencyObject target, Object value)
		{
			target.SetValue(SelectionChangedCommandParameterDependencyProperty, value);
		}

		public static Object GetSelectionChangedCommandParameter(DependencyObject target)
		{
			return (Object)target.GetValue(SelectionChangedCommandParameterDependencyProperty);
		}

		private static void SelectionChanged_Changed(DependencyObject target, DependencyPropertyChangedEventArgs e)
		{
			Selector element = target as Selector;
			if (element != null)
			{
				if ((e.NewValue != null) && (e.OldValue == null))
				{
					element.SelectionChanged += element_SelectionChanged;
				}
				else if ((e.NewValue == null) && (e.OldValue != null))
				{
					element.SelectionChanged -= element_SelectionChanged;
				}
			}
		}

		static void element_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Selector element = sender as Selector;
			ICommand command = (ICommand)element.GetValue(SelectionChangedCommandProperty.SelectionChangedCommandDependencyProperty);
			Object commandParameter = (Object)element.GetValue(SelectionChangedCommandParameterDependencyProperty);
			Object[] parameters = new Object[2];
			parameters[0] = sender;
			parameters[1] = commandParameter;
			command.Execute(parameters);
		}
	}
}
