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
	class KeyUpCommandProperty
	{
		public static DependencyProperty KeyUpCommandDependencyProperty =
			DependencyProperty.RegisterAttached("KeyUpCommand", typeof(ICommand), typeof(KeyUpCommandProperty),
				new FrameworkPropertyMetadata(new PropertyChangedCallback(KeyUpCommandProperty.KeyUp_Changed)));

		public static DependencyProperty KeyUpCommandParameterDependencyProperty =
			DependencyProperty.RegisterAttached("KeyUpCommandParameter", typeof(Object), typeof(KeyUpCommandProperty),
			new FrameworkPropertyMetadata(null));

		public static void SetKeyUpCommand(DependencyObject target, ICommand value)
		{
			target.SetValue(KeyUpCommandProperty.KeyUpCommandDependencyProperty, value);
		}

		public static ICommand GetKeyUpCommand(DependencyObject target)
		{
			return (ICommand)target.GetValue(KeyUpCommandProperty.KeyUpCommandDependencyProperty);
		}

		public static void SetKeyUpCommandParameter(DependencyObject target, Object value)
		{
			target.SetValue(KeyUpCommandParameterDependencyProperty, value);
		}

		public static Object GetKeyUpCommandParameter(DependencyObject target)
		{
			return (Object)target.GetValue(KeyUpCommandParameterDependencyProperty);
		}

		private static void KeyUp_Changed(DependencyObject target, DependencyPropertyChangedEventArgs e)
		{
			UIElement element = target as UIElement;
			if (element != null)
			{
				if ((e.NewValue != null) && (e.OldValue == null))
				{
					element.KeyUp += element_KeyUp;
				}
				else if ((e.NewValue == null) && (e.OldValue != null))
				{
					element.KeyUp -= element_KeyUp;
				}
			}
		}

		static void element_KeyUp(object sender, KeyboardEventArgs e)
		{
			UIElement element = sender as UIElement;
			ICommand command = (ICommand)element.GetValue(KeyUpCommandProperty.KeyUpCommandDependencyProperty);
			Object commandParameter = (Object)element.GetValue(KeyUpCommandParameterDependencyProperty);
			Object[] parameters = new Object[2];
			parameters[0] = sender;
			parameters[1] = commandParameter;
			command.Execute(parameters);
		}
	}
}
