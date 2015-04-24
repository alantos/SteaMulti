namespace SteaMUlti.Commands
{
	using System.Windows.Input;
	using SteaMUlti.ViewModels;

	internal class BrowseSteamPathCommand : ICommand
	{
		public BrowseSteamPathCommand(AccountViewModel viewModel)
		{
			_viewModel = viewModel;
		}

		private AccountViewModel _viewModel;

		public bool CanExecute(object parameter)
		{
			return _viewModel.BrowseSteamPathCanExecute;
		}

		public void Execute(object parameter)
		{
			_viewModel.BrowseSteamPathExecute();
		}

		public event System.EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}
	}	
}
