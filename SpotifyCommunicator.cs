using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spotifyMute
{
  public static class SpotifyCommunicator
  {
    public static string GetSongTitle()
    {

      var procs = Process.GetProcessesByName("Spotify");

      foreach (var p in procs.Where(p => !string.IsNullOrEmpty(p.MainWindowTitle)))
      {
        return p.MainWindowTitle;
      }

      return "SPOTIFY NOT RUNNING";
    }
  }
}
