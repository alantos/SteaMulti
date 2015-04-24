namespace SteaMUlti.Commands
{
	using System.Windows.Input;
	using SteaMUlti.ViewModels;

	internal class SearchGamesCommand : ICommand
	{
		public SearchGamesCommand(AccountViewModel viewModel)
		{
			_viewModel = viewModel;
		}

		private AccountViewModel _viewModel;

		public bool CanExecute(object parameter)
		{
			return _viewModel.SearchGamesCanExecute;
		}

		public void Execute(object parameter)
		{
			_viewModel.SearchGamesExecute(parameter);
		}

		public event System.EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}
	}	
}
