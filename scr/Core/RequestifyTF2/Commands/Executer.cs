﻿using System.Collections.Generic;
using System.Linq;
using RequestifyTF2.Api;

namespace RequestifyTF2.Commands
{
    internal class Executer
    {
        public static void Execute(string caller, string command, List<string> arguments)
        {
            var calledcommand = Instances.ActivePlugins.FirstOrDefault(n => n.Command == command);
            if (calledcommand == null)
            {
                var argstostring = "";
                if (arguments.Count > 0)
                    argstostring = string.Join(" ", argstostring.ToArray());
                Events.PlayerChat.Invoke(caller, command + " " + argstostring);
            }
            else
            {
                if (!Instances.Config.Ignored.Contains(caller))
                {
                    if (!Instances.Config.IgnoredReversed)
                        calledcommand.Execute(caller, arguments);
                }
                else
                {
                    if (Instances.Config.IgnoredReversed)
                        calledcommand.Execute(caller, arguments);
                }
            }
        }
    }
}