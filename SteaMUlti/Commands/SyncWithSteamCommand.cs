namespace SteaMUlti.Commands
{
	using System.Windows.Input;
	using SteaMUlti.ViewModels;
	using System.Windows.Controls;

	class SyncWithSteamCommand : ICommand
	{
		public SyncWithSteamCommand(AccountViewModel viewModel)
		{
			_viewModel = viewModel;
		}

		private AccountViewModel _viewModel;

		public bool CanExecute(object parameter)
		{
			return _viewModel.SyncWithSteamCanExecute;
		}

		public void Execute(object parameter)
		{
			string param = (string)parameter;
			_viewModel.SyncWithSteamExecute(param);
		}

		public event System.EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}
	}
}
