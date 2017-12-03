using FrodLib.CQI.Commands;
using FrodLib.Interfaces;
using FrodLib.IoC;
using FrodLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrodLib.CQI
{
    public class CommandManager : ICommandManager
    {
        [IoCResolveMany]
        private IEnumerable<IoCLazy<ICQICommand, ICQICommandData>> m_promptCommands;
        private ICommandManagerOutput m_commandManagerOutput;
        private IoCContainer m_container = new IoCContainer();

        [ThreadStatic]
        private static List<string> s_commandHistory;

        protected CommandManager() : this(null)
        {
        }

        public CommandManager(ICommandManagerOutput commandOutput)
        {
            CommandRegistry.Instance.CommandCatalogsChanged += CommandCatalogs_CommandCatalogsChanged;
            m_container.Configure(c => c.AddRegistry(CommandRegistry.Instance));
            m_container.Fill(this);
            m_promptCommands = new List<IoCLazy<ICQICommand, ICQICommandData>>(m_promptCommands);
            this.m_commandManagerOutput = commandOutput ?? this as ICommandManagerOutput;
            InitializeCommandHistory();
        }

        private void CommandCatalogs_CommandCatalogsChanged(object sender, EventArgs e)
        {
            m_container.Fill(this);
            m_promptCommands = new List<IoCLazy<ICQICommand, ICQICommandData>>(m_promptCommands);
        }

        public void SetOutput(ICommandManagerOutput cmdManagerOutput)
        {
            ArgumentValidator.IsNotNull(cmdManagerOutput, nameof(cmdManagerOutput));
            this.m_commandManagerOutput = cmdManagerOutput;
        }

        public bool CommandHistoryEnabled { get; set; } = true;

        void ICommandManager.PrintAvailableCommands()
        {
            foreach (var command in m_promptCommands.OrderBy(p => p.Metadata.Command))
            {
                if ((command.Value is HistoryCommand || command.Value is ClearHistoryCommand))
                {
                    if (!CommandHistoryEnabled)
                    {
                        continue;
                    }
                }
                m_commandManagerOutput.WriteLine(command.Metadata.Command + " - " + command.Metadata.Description);
            }
        }

        void ICommandManager.PrintCommandHistory()
        {
            if (!CommandHistoryEnabled) return;
            var commandHistory = s_commandHistory;
            if (commandHistory != null)
            {
                foreach (string history in commandHistory)
                {
                    m_commandManagerOutput.WriteLine(history);
                }
            }
        }

        void ICommandManager.ClearCommandHistory()
        {
            if (!CommandHistoryEnabled) return;
            var commandHistory = s_commandHistory;
            if (commandHistory != null) commandHistory.Clear();
        }

        protected void InitializeCommandHistory()
        {
            s_commandHistory = new List<string>();
        }

        public void ExecuteCommand(string commandText)
        {
            if (string.IsNullOrWhiteSpace(commandText))
            {
                return;
            }
            else
            {
                var output = m_commandManagerOutput;
                if (output == null)
                {
                    throw new InvalidOperationException("No output has been defined");
                }

                bool commandFound = false;

                string commandName;
                string[] promptCommandArgs;

                string promptCommand = commandText.Trim();
                ParseCommand(promptCommand, out commandName, out promptCommandArgs);

                foreach (var command in m_promptCommands)
                {
                    if (command.Metadata.Command.Equals(commandName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        OnExecuteCommand(command.Value, output, promptCommandArgs);
                        commandFound = true;
                    }
                }

                if (!commandFound)
                {
                    output.WriteLine("Couldn't find the command '" + commandName + "'");
                }

                if (CommandHistoryEnabled)
                {
                    var commandHistory = s_commandHistory;
                    if (commandHistory != null)
                    {
                        s_commandHistory.Add(commandText);
                    }
                }

            }
        }

        protected virtual void ParseCommand(string commandText, out string commandName, out string[] commandArguments)
        {
            List<string> args = new List<string>();
            bool firstQuoteFound = false;
            for (int i = 0; i < commandText.Length; i++)
            {
                if (commandText[i] == '"')
                {
                    if (!firstQuoteFound)
                    {
                        firstQuoteFound = true;
                    }
                    else
                    {
                        string arg = commandText.Substring(1, i - 1);
                        commandText = commandText.Remove(0, i + 1).TrimStart();
                        args.Add(arg);
                        i = -1;
                        firstQuoteFound = false;
                    }
                }
                else if (!firstQuoteFound && Char.IsWhiteSpace(commandText[i]))
                {
                    string arg = commandText.Substring(0, i);
                    commandText = commandText.Remove(0, i).TrimStart();
                    args.Add(arg);
                    i = -1;
                }
                else if (i == commandText.Length - 1)
                {
                    if (firstQuoteFound)
                    {
                        args.AddRange(commandText.Replace("\"", "").Split(' '));
                    }
                    else
                    {
                        args.Add(commandText);
                    }
                }
            }

            commandName = args.First();
            commandArguments = args.Skip(1).ToArray();
        }

        protected virtual void OnExecuteCommand(ICQICommand command, ICommandManagerOutput output, string[] commandArgs)
        {
            try
            {
                if (command is ICQIInternalCommand)
                {
                    ((ICQIInternalCommand)command).ExecuteCommand(this, output, commandArgs);
                }
                else
                {
                    command.ExecuteCommand(output, commandArgs);
                }
            }
            catch (Exception ex)
            {
                m_commandManagerOutput.WriteLine("Command finished with error: " + ex.Message);
            }
        }
    }
}
