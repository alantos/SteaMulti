namespace SteaMUlti.Commands
{
	using System.Windows.Input;
	using SteaMUlti.ViewModels;

	internal class ChangeSortingCommand : ICommand
	{
		public ChangeSortingCommand(AccountViewModel viewModel)
		{
			_viewModel = viewModel;
		}

		private AccountViewModel _viewModel;

		public bool CanExecute(object parameter)
		{
			return _viewModel.ChangeSortingCanExecute;
		}

		public void Execute(object parameters)
		{
			_viewModel.ChangeSortingExecute(parameters);
		}

		public event System.EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}
	}	
}
