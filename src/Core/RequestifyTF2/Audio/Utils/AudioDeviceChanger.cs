﻿// RequestifyTF2
// Copyright (C) 2018  Villiam Nmerukini
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CSCore.CoreAudioAPI;

namespace RequestifyTF2.Audio.Utils
{
    // Contains Some Code From https://github.com/aifdsc/AudioChanger
    // Thanks Stephan for your work
    internal static class AudioDeviceChanger
    {
        private static bool setDefaultAudioDevice(string deviceId)
        {
            var client = new CPolicyConfigClient();
            client.SetDefaultDevice(deviceId);
            var vclient = new CPolicyConfigVistaClient();
            vclient.SetDefaultDevice(deviceId);
            return true;
        }

        public static bool SetDefaultInputDevice(string deviceId)
        {
            var result = setDefaultAudioDevice(deviceId);
            return result;
        }
    }

    public static class DeviceChanger
    {
        // 0 <- Input; 1 <- Output
        public static bool AutoSetDevice()
        {

            var devices = GetBestAudioDevices();
            if (devices == null)
            {
                Logger.Nlogger.Error(
                    Localization.Localization.CORE_CANNOT_FIND_DEVICES
                  );
                return false;

            }
            try
            {
                AudioDeviceChanger.SetDefaultInputDevice(devices[0].DeviceID);
            }
            catch (Exception e)
            {
                Logger.Nlogger.Error(e,Localization.Localization.CORE_ERROR_WHILE_SETTING_INPUT, devices[0].FriendlyName
                        );
            }
            AudioManager.Extra.SoundOut.Device = devices[1];
            AudioManager.BackGround.SoundOut.Device = devices[1];
            AudioManager.ForeGround.SoundOut.Device = devices[1];
            Logger.Nlogger.Info(Localization.Localization.CORE_USED_DEVICES, devices[1].FriendlyName,
                    devices[1].FriendlyName);
            return true;
        }

        public static MMDevice[] GetBestAudioDevices()
        {
            var goodInputDevices = new List<MMDevice>();
            var goodOutputDevices = new List<MMDevice>();

            using (var deviceEnumerator = new MMDeviceEnumerator())
            {
                using (var deviceoutCollection =
                    deviceEnumerator.EnumAudioEndpoints(DataFlow.Render, DeviceState.Active))
                {
                    foreach (var device in deviceoutCollection)
                        if (device.FriendlyName.Contains("Cable") && device.FriendlyName.Contains("Virtual")
                                                                  && device.FriendlyName.Contains("Audio"))
                            goodOutputDevices.Add(device);
                    using (var deviceinpCollection =
                        deviceEnumerator.EnumAudioEndpoints(DataFlow.Capture, DeviceState.Active))
                    {
                        foreach (var device in deviceinpCollection) goodInputDevices.Add(device);
                    }
                }

                if (goodOutputDevices.Count == 0 || goodInputDevices.Count == 0)
                {
                    Logger.Nlogger.Error(
                        Localization.Localization.CORE_CANNOT_FIND_DEVICES,
                        ConsoleColor.Red);
                    return null;

                }

                foreach (var a in goodInputDevices)
                {
                    Logger.Nlogger.Debug("GoodInputDev " +a.FriendlyName
                      );
                }
                foreach (var a in goodOutputDevices)
                {
                    Logger.Nlogger.Debug("GoodOutputDev " + a.FriendlyName
                    );
                }
                if (goodOutputDevices.Count(n => n.FriendlyName.Contains("VB-Audio")) != 0 &&
                    goodInputDevices.Count(n => n.FriendlyName.Contains("VB-Audio")) != 0)
                {
                    var outp = goodOutputDevices.FirstOrDefault(n => n.FriendlyName.Contains("VB-Audio"));
                    var inp = goodInputDevices.FirstOrDefault(n => n.FriendlyName.Contains("VB-Audio"));

                    return new MMDevice[2]
                    {
                        inp, outp
                    };
                }

                if (goodOutputDevices.Count(n => n.FriendlyName.Contains("Virtual Audio Cable")) != 0 &&
                    goodInputDevices.Count(n => n.FriendlyName.Contains("Virtual Audio Cable")) != 0)
                {
                    var outp = goodOutputDevices.FirstOrDefault(n =>
                        n.FriendlyName.Contains("Virtual Audio Cable"));
                    var inp = goodInputDevices.FirstOrDefault(n => n.FriendlyName.Contains("Virtual Audio Cable"));
                    return new MMDevice[2]
                    {
                        inp, outp
                    };
                }

              
                Logger.Nlogger.Error(
                    Localization.Localization.CORE_CANNOT_FIND_DEVICES,
                    ConsoleColor.Red);
                return null;
            }
        }
    }
}