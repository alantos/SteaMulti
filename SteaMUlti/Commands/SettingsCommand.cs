namespace SteaMUlti.Commands
{
	using System.Windows.Input;
	using SteaMUlti.ViewModels;

	internal class SettingsCommand : ICommand
	{
		public SettingsCommand(AccountViewModel viewModel)
		{
			_viewModel = viewModel;
		}

		private AccountViewModel _viewModel;

		public bool CanExecute(object parameter)
		{
			return _viewModel.SettingsCanExecute;
		}

		public void Execute(object parameter)
		{
			_viewModel.SettingsExecute();
		}

		public event System.EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}
	}	
}
