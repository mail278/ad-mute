using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using adMute.Properties;
using Microsoft.Win32;

namespace adMute
{
  public partial class Form1 : Form
  {
    private bool isMuted = false;
    private IDisposable timer;

    public Form1()
    {
      InitializeComponent();

      timer = Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(_ => TestAds());

      SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;

      AudioManager.Mute(false);
    }

    void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
    {
      if (e.Reason == SessionSwitchReason.SessionLock)
      {
        Pause();
      }
      else if (e.Reason == SessionSwitchReason.SessionUnlock)
      {
        Play();
      }
    }

    private void TestAds()
    {
      try
      {
        var title = SpotifyCommunicator.GetSongTitle();
        if (!title.Contains(" - "))
        {
          if (!isMuted)
          {
            isMuted = true;
            AudioManager.Mute(true);
          }
          notifyIcon1.Icon = Resources.Custom_Icon_Design_Pretty_Office_8_Sound_off;
          notifyIcon1.Text = "adMute auto-mute (muted)";

        }
        else
        {
          if (isMuted)
          {
            isMuted = false;
            AudioManager.Mute(false);
          }
          notifyIcon1.Icon = Resources.Custom_Icon_Design_Pretty_Office_8_Sound_on;
          notifyIcon1.Text = "adMute auto-mute (playing)";
        }

      }
      catch (Exception)
      {
      }
    }

    private bool wasPausedByMe = false;
    private void Pause()
    {
      var title = SpotifyCommunicator.GetSongTitle();
      if (string.IsNullOrEmpty(title)) return;
      if (title == "Spotify")
      {
        wasPausedByMe = false;
        return;
      };

      //keybd_event(VK_MEDIA_PLAY_PAUSE, 0, KEYEVENTF_EXTENDEDKEY, IntPtr.Zero);  
      SendMessageW(this.Handle, WM_APPCOMMAND, this.Handle, APPCOMMAND_MEDIA_PAUSE);
      wasPausedByMe = true;
    }

    private void Play()
    {
      var title = SpotifyCommunicator.GetSongTitle();
      if (string.IsNullOrEmpty(title)) return;
      if (title != "Spotify" || !wasPausedByMe) return;

      SendMessageW(this.Handle, WM_APPCOMMAND, this.Handle, APPCOMMAND_MEDIA_PLAY);
      //keybd_event(VK_MEDIA_PLAY_PAUSE, 0, KEYEVENTF_EXTENDEDKEY, IntPtr.Zero);
      wasPausedByMe = false;
    }

    #region private

    private void closeToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Application.Exit();
    }

    private void Form1_FormClosed(object sender, FormClosedEventArgs e)
    {
      timer.Dispose();
    }

    protected override void SetVisibleCore(bool value)
    {
      base.SetVisibleCore(false);
    }

    #endregion

    private const int WM_APPCOMMAND = 0x319;

    private const int APPCOMMAND_MEDIA_PAUSE = 0x2f0000;
    private const int APPCOMMAND_MEDIA_PLAY = 0x2e0000;

    [DllImport("user32.dll")]
    public static extern IntPtr SendMessageW(IntPtr hWnd, int Msg, IntPtr wParam, int lParam);




public const int KEYEVENTF_EXTENDEDKEY = 1;
public const int KEYEVENTF_KEYUP = 2;
public const int VK_MEDIA_PLAY_PAUSE = 0xB3;

[DllImport("user32.dll", SetLastError = true)]
public static extern void keybd_event(byte virtualKey, byte scanCode, uint flags, IntPtr extraInfo);



  }
}
