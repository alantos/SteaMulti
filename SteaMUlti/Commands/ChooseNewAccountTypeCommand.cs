namespace SteaMUlti.Commands
{
	using System.Windows.Input;
	using SteaMUlti.ViewModels;

	internal class ChooseNewAccountTypeCommand : ICommand
	{
		public ChooseNewAccountTypeCommand(AccountViewModel viewModel)
		{
			_viewModel = viewModel;
		}

		private AccountViewModel _viewModel;

		public bool CanExecute(object parameter)
		{
			return _viewModel.ChooseNewAccountTypeCanExecute;
		}

		public void Execute(object parameter)
		{
			_viewModel.ChooseNewAccountTypeExecute(parameter);
		}

		public event System.EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}
	}	
}
