using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FrodLib.Commands
{
    public static class CommandHelper
    {
        public static ICommand InitializeCommand(ref ICommand command, Action action, Func<bool> canExecute = null, params INotifyPropertyChanged[] listenOnObjects)
        {
            if(command == null)
            {
                command = new RelayCommand(action, canExecute);
            }
            var relayCommand = command as RelayCommand;
            if (relayCommand != null)
            {
                foreach (var listenOn in listenOnObjects)
                {
                    relayCommand.ListenForNotificationFrom(listenOn);
                }
            }
            return command;
        }

        public static ICommand InitializeCommand<TParam>(ref ICommand command, Action<TParam> action, Predicate<TParam> canExecute = null, params INotifyPropertyChanged[] listenOnObjects)
        {
            if (command == null)
            {
                command = new RelayCommand<TParam>(action, canExecute);
            }
            var relayCommand = command as RelayCommand<TParam>;
            if (relayCommand != null)
            {
                foreach (var listenOn in listenOnObjects)
                {
                    relayCommand.ListenForNotificationFrom(listenOn);
                }
            }
            return command;
        }
    }
}
