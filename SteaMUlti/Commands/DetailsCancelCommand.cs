namespace SteaMUlti.Commands
{
	using System.Windows.Input;
	using SteaMUlti.ViewModels;

	class DetailsCancelCommand : ICommand
	{
		public DetailsCancelCommand(AccountViewModel viewModel)
		{
			_viewModel = viewModel;
		}

		private AccountViewModel _viewModel;

		public bool CanExecute(object parameter)
		{
			return _viewModel.DetailsCancelCanExecute;
		}

		public void Execute(object parameter)
		{
			string param = (string)parameter;
			_viewModel.DetailsCancelExecute(param);
		}

		public event System.EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}
	}
}
