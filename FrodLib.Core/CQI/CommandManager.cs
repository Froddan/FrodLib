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

        public CommandManager() : this(null)
        {
        }

        public CommandManager(ICommandManagerOutput commandOutput)
        {
            CommandRegistry.Instance.CommandCatalogsChanged += CommandCatalogs_CommandCatalogsChanged;
            m_container.Configure(c => c.AddRegistry(CommandRegistry.Instance));
            m_container.Fill(this);
            m_promptCommands = new List<IoCLazy<ICQICommand, ICQICommandData>>(m_promptCommands);
            this.m_commandManagerOutput = commandOutput ?? this as ICommandManagerOutput;
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

        public virtual void ExecuteCommand(string commandText)
        {
            if (string.IsNullOrWhiteSpace(commandText))
            {
                return;
            }
            else
            {
                List<string> args = new List<string>();
                string promptCommand = commandText.Trim();
                bool firstQuoteFound = false;
                for (int i = 0; i < promptCommand.Length; i++)
                {
                    if (promptCommand[i] == '"')
                    {
                        if (!firstQuoteFound)
                        {
                            firstQuoteFound = true;
                        }
                        else
                        {
                            string arg = promptCommand.Substring(1, i - 1);
                            promptCommand = promptCommand.Remove(0, i + 1).TrimStart();
                            args.Add(arg);
                            i = -1;
                            firstQuoteFound = false;
                        }
                    }
                    else if (!firstQuoteFound && Char.IsWhiteSpace(promptCommand[i]))
                    {
                        string arg = promptCommand.Substring(0, i);
                        promptCommand = promptCommand.Remove(0, i).TrimStart();
                        args.Add(arg);
                        i = -1;
                    }
                    else if (i == promptCommand.Length - 1)
                    {
                        if (firstQuoteFound)
                        {
                            args.AddRange(promptCommand.Replace("\"", "").Split(' '));
                        }
                        else
                        {
                            args.Add(promptCommand);
                        }
                    }
                }
                string proptCommand = args.First();

                // EchoPromptCommand();

                bool commandFound = false;

                string[] proptCommandArgs = args.Skip(1).ToArray();
                foreach (var command in m_promptCommands)
                {
                    if (command.Metadata.Command.Equals(proptCommand, StringComparison.CurrentCultureIgnoreCase))
                    {
                        try
                        {
                            command.Value.ExecuteCommand(m_commandManagerOutput, proptCommandArgs);
                        }
                        catch (Exception ex)
                        {
                            m_commandManagerOutput.WriteLine("Command finished with error: " + ex.Message);
                        }
                        commandFound = true;
                    }
                }

                if (!commandFound)
                {
                    m_commandManagerOutput.WriteLine("Couldn't find the command '" + proptCommand + "'");
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
    }
}
