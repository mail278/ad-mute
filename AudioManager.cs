using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSCore.CoreAudioAPI;

namespace spotifyMute
{
  public static class AudioManager
  {
    public static void Mute(bool mute)
    {
      try
      {
        using (var sessionManager = GetDefaultAudioSessionManager2(DataFlow.Render))
        {
          using (var sessionEnumerator = sessionManager.GetSessionEnumerator())
          {
            foreach (var session in sessionEnumerator)
            {
              if (session.SessionState != AudioSessionState.AudioSessionStateActive) continue;
              using (var session2 = session.QueryInterface<AudioSessionControl2>())
              {
                if (session2.Process != null && session2.Process.ProcessName != "Spotify") continue;
              }
              using (var simpleVolume = session.QueryInterface<SimpleAudioVolume>())
              {
                if (session.SessionState != AudioSessionState.AudioSessionStateActive) continue;
                simpleVolume.IsMuted = mute;
              }
            }
          }
        }

        //var mmde = new NAudio.CoreAudioApi.MMDeviceEnumerator();
        //var devices = mmde.EnumerateAudioEndPoints(NAudio.CoreAudioApi.DataFlow.All, NAudio.CoreAudioApi.DeviceState.All);

        
        //foreach (var dev in devices)
        //{
        //  try
        //  {
        //    dev.AudioEndpointVolume.Mute = mute;
        //  }
        //  catch (Exception ex)
        //  {
        //    //Do something with exception when an audio endpoint could not be muted
        //  }
        //}
      }
      catch (Exception ex)
      {
        //When something happend that prevent us to iterate through the devices
      }
    }

    private static AudioSessionManager2 GetDefaultAudioSessionManager2(DataFlow dataFlow)
    {
      using (var enumerator = new MMDeviceEnumerator())
      {
        using (var device = enumerator.GetDefaultAudioEndpoint(dataFlow, Role.Multimedia))
        {
          Debug.WriteLine("DefaultDevice: " + device.FriendlyName);
          var sessionManager = AudioSessionManager2.FromMMDevice(device);
          return sessionManager;
        }
      }
    }

  }
}
