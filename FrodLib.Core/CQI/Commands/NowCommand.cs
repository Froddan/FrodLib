using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrodLib.CQI;

namespace FrodLib.CQI.Commands
{
    [Metadata("Command", "Now")]
    [Metadata("Description", "Prints the current date and time")]
    class NowCommand : ICQICommand
    {
        private const string HelpKey = "-help";
        private const string UTCKey = "-utc";
        private const string FormatKey = "-format";

        public void ExecuteCommand(ICommandManagerOutput commandPrompt, string[] args)
        {
            if (args.Length == 0)
            {
                commandPrompt.WriteLine(Clock.Now.ToString());
            }
            else if (args.Length > 0)
            {
                bool useUTC = false;
                string format = null;
                bool showHelp = false;
                for (int i = 0; i < args.Length; i++)
                {
                    string key = args[i].ToLower();
                    switch (key)
                    {
                        case HelpKey: showHelp = true; break;
                        case UTCKey: useUTC = true; break;
                        case FormatKey: if (args.Length > i + 1) format = args[i + 1]; break;
                        default:
                            break;
                    }
                }
                if (showHelp)
                {
                    commandPrompt.WriteLine(HelpKey + "\t Prints the help menu");
                    commandPrompt.WriteLine(UTCKey + "\t Writes the time in UTC");
                    commandPrompt.WriteLine(FormatKey + "\t applies custom format to datetime");
                    commandPrompt.WriteLine("\t Ex: Now -format \"yyyy-MM-dd HH:mm\" writes the time with 2014-02-18 20:11");
                }
                else
                {
                    DateTime dt;
                    if (useUTC)
                    {
                        dt = Clock.UtcNow;
                    }
                    else
                    {
                        dt = Clock.Now;
                    }

                    if(string.IsNullOrWhiteSpace(format))
                    {
                        commandPrompt.WriteLine(dt.ToString());
                    }
                    else
                    {
                        commandPrompt.WriteLine(dt.ToString(format));
                    }
                }
            }
        }
    }
}
