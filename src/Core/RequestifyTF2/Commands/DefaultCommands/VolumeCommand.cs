using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RequestifyTF2.API;

namespace RequestifyTF2.Commands
{
    class VolumeCommand : IRequestifyCommand
    {
        public string Help => "Volume commands";
        public string Name => "volume";
        public bool OnlyAdmin => true;
        public List<string> Alias => new List<string>();
        public void Execute(User executor, List<string> arguments)
        {
            if (executor.Name != Instance.Config.Admin)
            {
                ConsoleSender.SendCommand("Current volume: " + Instance.Config.BaseVolume + "%", ConsoleSender.Command.Chat);
                return;
            }
            Thread.Sleep(1600);
            if (arguments.Count <= 0)
            {
                ConsoleSender.SendCommand("Usage: !volume [percentage]", ConsoleSender.Command.Chat);
                ConsoleSender.SendCommand("Current volume: " + Instance.Config.BaseVolume + "%", ConsoleSender.Command.Chat);
                return;
            }

            int VolumePercent;
            try
            {
                VolumePercent = Int32.Parse(arguments[0]);
            }
            catch (Exception)
            {
                ConsoleSender.SendCommand("Invalid argument", ConsoleSender.Command.Chat);
                return;
            }

            if (VolumePercent > 100 || VolumePercent < 0)
            {
                ConsoleSender.SendCommand("Invalid percentage", ConsoleSender.Command.Chat);
                return;
            }

            Instance.Config.BaseVolume = VolumePercent;
            ConsoleSender.SendCommand("Music volume has been set to " + Instance.Config.BaseVolume + "%", ConsoleSender.Command.Chat);

        }
    }
}
