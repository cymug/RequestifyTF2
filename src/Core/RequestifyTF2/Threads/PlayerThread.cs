using System;
using System.Threading;
using System.Threading.Tasks;
using CSCore;
using CSCore.SoundOut;
using RequestifyTF2.API;

namespace RequestifyTF2.Threads
{
    
  internal static  class PlayerThread
  {

      private static Thread thread;

      public static void StopThread()
      {
          if (thread.IsAlive)
          {
              thread.Abort();
          }
      }
        public static void StartThread()
        {
            thread = new Thread(Play) {IsBackground = true};
            thread.Start();

            Logger.Write(Logger.Status.Info, Localization.Localization.CORE_STARTED_PLAYER_THREAD);
        }

        private static void Play()
        {
            while (true)
            {
                // Background stuff (music)
                if (Instance.SoundOutBackground.PlaybackState == PlaybackState.Playing)
                {
                    Instance.SoundOutBackground.Volume =
                        Instance.SoundOutForeground.PlaybackState == PlaybackState.Playing ? ((float)Instance.Config.BaseVolume * 0.005f) : ((float)Instance.Config.BaseVolume * 0.01f);
                    if (Instance.SoundOutBackground.WaveSource != null)
                    {
                        if (Instance.SoundOutBackground.WaveSource.Length
                            - Instance.SoundOutBackground.WaveSource.Position
                            < Instance.SoundOutBackground.WaveSource.WaveFormat.BytesPerSecond / 100)
                        {
                            Instance.SoundOutBackground.Stop();
                            Logger.Write(Logger.Status.Info, "Stopped at Position: " + Instance.SoundOutBackground.WaveSource.Position.ToString() + " Length: " + Instance.SoundOutBackground.WaveSource.Length.ToString());
                        }
                    }
                }

                if (Instance.BackGroundQueue.GetQueueLenght() > 0)
                {
                    if (Instance.SoundOutBackground.PlaybackState == PlaybackState.Stopped && Instance.NewSongLock == false)
                    {
                        Instance.Song s;
                        if (Instance.BackGroundQueue.PlayList.TryDequeue(out s))
                        {
                            Task.Run(
                                () =>
                                {
                                    Thread.Sleep(1600);
                                    ConsoleSender.SendCommand(
                                        string.Format(Localization.Localization.CORE_PLAYING_TITLE_FROM, s.Title, s.RequestedBy.Name),
                                        ConsoleSender.Command.Chat);
                                    Logger.Write(Logger.Status.Info, string.Format(Localization.Localization.CORE_PLAYING_TITLE_FROM, s.Title, s.RequestedBy.Name));
                                    Player(s.Source, Instance.SoundOutBackground);
                                    Instance.NewSongLock = true;
                                    Instance.CurrentTitle = s.Title;
                                    Instance.CurrentSongFrom = s.RequestedBy.Name;
                                    Logger.Write(Logger.Status.Info, "Started at Position: " + Instance.SoundOutBackground.WaveSource.Position.ToString() + " Length: " + Instance.SoundOutBackground.WaveSource.Length.ToString());
                                });
                        }
                    }
                }

                // Check to make sure sound output has actually started (prevent queue skipping)
                if (Instance.SoundOutBackground.PlaybackState == PlaybackState.Playing && Instance.NewSongLock == true)
                {
                    Instance.NewSongLock = false;
                }

                // Foreground stuff (tts)
                if (Instance.SoundOutForeground.PlaybackState == PlaybackState.Playing)
                {
                    if (Instance.SoundOutForeground.WaveSource != null)
                    {
                        if (Instance.SoundOutForeground.WaveSource.Length
                            - Instance.SoundOutForeground.WaveSource.Position
                            < Instance.SoundOutForeground.WaveSource.WaveFormat.BytesPerSecond / 1000)
                        {
                            Instance.SoundOutForeground.Stop();
                        }
                    }
                }

                if (Instance.QueueForeGround.Count > 0)
                {
                    if (Instance.SoundOutForeground.PlaybackState == PlaybackState.Stopped)
                    {
                        IWaveSource s;
                        if (Instance.QueueForeGround.TryDequeue(out s))
                        {
                            Task.Run(() => { Player(s, Instance.SoundOutForeground); });
                        }
                    }
                }

                Thread.Sleep(60);
            }
        }

        private static Task Player(IWaveSource decoder, WasapiOut device)
        { 
            //todo: this shit can fuck up!
            while (device.PlaybackState != PlaybackState.Stopped)
            {
                device.Stop();
            }

            Logger.Write(Logger.Status.Info, "Current device.PlaybackState is stopped: " + ((device.PlaybackState == PlaybackState.Stopped) ? "True" : device.PlaybackState.ToString()));
            try
            {
                device.Initialize(decoder.ToMono()); //Mono > Stereo in micspams
            }
            catch (InvalidOperationException e)
            {
                device = new WasapiOut();
                device.Initialize(decoder.ToMono());
                Logger.Write(Logger.Status.Error, "device.PlaybackState was not equal to PlaybackState.Stopped. Error: " + e.ToString());
            }
            
            device.Play();
            return Task.FromResult<object>(null);
        }
    }
}