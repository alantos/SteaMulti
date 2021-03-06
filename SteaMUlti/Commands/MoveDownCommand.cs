﻿namespace SteaMUlti.Commands
{
	using System.Windows.Input;
	using SteaMUlti.ViewModels;

	internal class MoveDownCommand : ICommand
	{
        public MoveDownCommand(AccountViewModel viewModel)
		{
			_viewModel = viewModel;
		}

		private AccountViewModel _viewModel;

		public bool CanExecute(object parameter)
		{
            return _viewModel.MoveDownCanExecute;
		}

		public void Execute(object parameter)
		{
            _viewModel.MoveDownExecute();
		}

		public event System.EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}
	}	
}
