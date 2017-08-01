﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RequestifyTF2.Api;

namespace RequestifyTF2.Commands
{
    class Executer
    {
        public static void Execute(string caller,string command,List<string> arguments )
        {
            var calledcommand = Instances.ActivePlugins.FirstOrDefault(n => n.Command == command);
            if (calledcommand == null)
            {
                return;
            }
            else
            {
                if (!Instances.Config.Ignored.Contains(caller)&&!Instances.Config.IgnoredReversed)
                {   
                    calledcommand.Execute(caller, arguments);
                 //   Logger.Write(Logger.Status.Info, $"{caller} executed command {command} with arguments {String.Join(" ", arguments.ToArray())}");
                }
                else
                {
                    if (Instances.Config.Ignored.Contains(caller) && Instances.Config.IgnoredReversed)
                    {
                        calledcommand.Execute(caller, arguments);
                     //   Logger.Write(Logger.Status.Info, $"{caller} executed command {command} with arguments {String.Join(" ", arguments.ToArray())}");
                    }
                   
                }
            }
        }
}
}