namespace SteaMUlti.Commands
{
	using System.Windows.Input;
	using SteaMUlti.ViewModels;
	using System.Windows.Controls;

	class DetailsOKCommand : ICommand
	{
		public DetailsOKCommand(AccountViewModel viewModel)
		{
			_viewModel = viewModel;
		}

		private AccountViewModel _viewModel;

		public bool CanExecute(object parameter)
		{
			return _viewModel.DetailsOKCanExecute;
		}

		public void Execute(object parameter)
		{
			string param = (string)parameter;
			_viewModel.DetailsOKExecute(param);
		}

		public event System.EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}
	}
}
