using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;

using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using AlphaForms;
using System.Drawing.Imaging;


namespace Athena
{
    public partial class AthenaPlayerForm1 : AlphaForms.AlphaForm 
    {
        private Form2 subForm_LoopStart;
        private Form3 subForm_LoopEnd;
        private Form4 subForm_About;

        OpenFileDialog dialog = new OpenFileDialog();

        public FMOD.System system = null;
        public FMOD.Sound TTRSound = null;
        public FMOD.Channel TTRChannel = null;

        public uint TTRLength = 0;
        public FMOD.SOUND_TYPE TTRSoundType = new FMOD.SOUND_TYPE();
        public FMOD.SOUND_FORMAT TTRSoundFormat = new FMOD.SOUND_FORMAT();
        public FMOD.TAG TTRDataTag = new FMOD.TAG();

        public int TTRNumTags = 0;
        public int TTRNumTagsUpdated = 0;

        FMOD.OPENSTATE a = 0; uint b = 0; bool c = false;

        public string GAPT_LastDirectoryName = string.Empty;

        public uint GAPT_CurrentLoopStart = 0;
        public uint GAPT_CurrentLoopDone = 0;

        public string GAPT_CurrentPlaybackState = "Stopped"; //stopped paused playing
        public string GAPT_FormerPlaybackState = "Stopped";
        public string GAPT_LastTrackName = string.Empty;
        public string GAPT_CurrentTrackAuthor = string.Empty;
        public string GAPT_CurrentTrackTitle = string.Empty;
        public string GAPT_CurrentFileType = string.Empty;
        public string GAPT_CurrentFileSize = string.Empty;

        public String CurrentDLSFile = "";

        public String DefaultOpenDialogFilter = "All Supported File Types (*.AIFF; *.ASF; *.ASX; *.DAT; *.FLAC; *.GM; *.IT; *.M3U; *.MID; *.MOD; *.MP2; *.MP3; *.OGG; *.RAW; *.S3M; *.VAG; *.WAV; *.WAX; *.WMA; *.XM; *.XMA)|*.aiff;*.asf;*.asx;*.dat;*.flac;*.gm;*.it;*.m3u;*.mid;*.mod;*.mp2;*.mp3;*.ogg;*.raw;*.s3m;*.vag;*.wav;*.wax;*.wma;*.xm;*.xma|Audio Interchange File Format (*.AIFF)|*.aiff|Chris Sawyer Software [CSS] Audio Data File (*.DAT)|*.dat|Free Lossless Audio Codec (*.FLAC)|*.flac|Impulse Tracker (*.IT)|*.it|Musical Instrument Digital Interface (*.GM, *.MID, *.MIDI)|*.gm;*.mid;*.midi|Modular Audio File Format (*.MOD)|*.mod|MPEG-1 Audio Layer 3 (*.MP2, *.MP3)|*.mp2;*.mp3|OGG Vorbis File Format (*.OGG)|*.ogg|Raw Audio File (*.RAW)|*.raw|Scream Tracker (*.S3M)|*.s3m|Wave File (*.WAV)|*.wav|Windows Media File Format (*.WMA)|*.wma|Extended Modular Audio File Format (*.XM, *.XMA)|*.xm;*.xma|All Files|*.*";

        FMOD.DSP keyPlus1 = new FMOD.DSP();
        FMOD.DSP keyPlus2 = new FMOD.DSP();
        FMOD.DSP keyPlus3 = new FMOD.DSP();
        FMOD.DSP keyPlus4 = new FMOD.DSP();
        FMOD.DSP keyPlus5 = new FMOD.DSP();
        FMOD.DSP keyMinus1 = new FMOD.DSP();
        FMOD.DSP keyMinus2 = new FMOD.DSP();
        FMOD.DSP keyMinus3 = new FMOD.DSP();
        FMOD.DSP keyMinus4 = new FMOD.DSP();
        FMOD.DSP keyMinus5 = new FMOD.DSP();
        //FMOD.DSPConnection TTRDSPNull = null;

        public string GAPT_LastPROCESSEDName = string.Empty;
        public string GAPT_LastPROCESSEDState = string.Empty;

        public string GAPT_IssueWorkerCommand = string.Empty;

        public int TTRTrackPlayingEntryID = -1;

        public uint MB_OK = 0;
        public uint ICON_STOP = 0x10;
        public uint ICON_INFORMATION = 0x40;
        public uint ICON_WARNING = 0x30;

        public Random rand = new Random();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern uint MessageBox(IntPtr hWnd, String text, String caption, uint type);

        public AthenaPlayerForm1()
        {

            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            DateTime buildDate = (new DateTime(2000, 1, 1)).AddDays(version.Build).AddSeconds(version.Revision * 2);

            InitializeComponent();
            this.Text+= "Built on " + buildDate.ToString();

            dialog.Filter = DefaultOpenDialogFilter;
            dialog.InitialDirectory = GAPT_LastDirectoryName; dialog.Title = "Open";
            dialog.Multiselect = true;

            //label9.Text = Environment.SystemDirectory + "\\drivers\\gm.dls";

        }

        public delegate void DFCADelegate();

        private void ERRCHECK(FMOD.RESULT result)
        {
            if (result != FMOD.RESULT.OK)
            {
                //MessageBox(new IntPtr(0), "Exception Raised: " + result + Environment.NewLine + FMOD.Error.String(result), "GSA Exception Trapper", MB_OK | ICON_STOP);
                //Environment.Exit(-1);
            }
        }

        protected void TTRFetchTrackFilesize()
        {

            if (false )
            {
                //GAPT_CurrentFileSize = "HTTP Stream";
            }
            else
            {
                try
                {
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(GAPT_LastTrackName);
                    GAPT_CurrentFileSize = GetFileSize(fileInfo.Length);
                }
                catch (Exception e)
                {
                    MessageBox(new IntPtr(0), "Non-critical Exception Raised: GAPI_FILESIZE_FAILURE" + Environment.NewLine + e.ToString(), "GSA Exception Trapper", MB_OK | ICON_STOP);
                }
            }
        }

        protected void TTRPlaySound(string Argument0, uint Argument1, uint Argument2, bool Argument3)
        {
            try
            {
                string oogimaflip_001;
                string oogimaflip_002;
                FMOD.RESULT result;

                int TTRNull = 0;

                GAPT_LastTrackName = Argument0;

                GAPT_CurrentTrackTitle = Path.GetFileName(Argument0);

                GAPT_CurrentLoopStart = Argument1;
                GAPT_CurrentLoopDone = Argument2;

                FMOD.CREATESOUNDEXINFO ex = new FMOD.CREATESOUNDEXINFO();
                ex.cbsize = Marshal.SizeOf(ex);

                if (CurrentDLSFile != "")
                {
                    ex.dlsname = CurrentDLSFile;
                }

                if (false )
                {
                    //result = system.createSound(Argument0, FMOD.MODE.SOFTWARE | FMOD.MODE._2D | FMOD.MODE.CREATESTREAM | FMOD.MODE.NONBLOCKING, ref ex, ref TTRSound);
                }
                else
                {
                    result = system.createSound(Argument0, FMOD.MODE.SOFTWARE | FMOD.MODE._2D | FMOD.MODE.CREATESTREAM, ref ex, ref TTRSound);
                }

                ERRCHECK(result);

                while (false )
                {
                    //result = TTRSound.getOpenState(ref a, ref b, ref c);
                    //ERRCHECK(result);

                    //if (a == FMOD.OPENSTATE.READY && TTRChannel == null)
                    //{
                    //    break;
                    //}

                }

                result = TTRSound.setLoopPoints(GAPT_CurrentLoopStart, FMOD.TIMEUNIT.MS, GAPT_CurrentLoopDone, FMOD.TIMEUNIT.MS);
                ERRCHECK(result);

                if (false )
                {
                    //result = TTRSound.setMode(FMOD.MODE.LOOP_NORMAL | FMOD.MODE.ACCURATETIME);
                }
                else
                {
                    result = TTRSound.setMode(FMOD.MODE.LOOP_OFF | FMOD.MODE.ACCURATETIME);
                }


                result = system.playSound(FMOD.CHANNELINDEX.REUSE, TTRSound, Argument3, ref TTRChannel);
                ERRCHECK(result);

                if (trackBar1.InvokeRequired)
                {
                    trackBar1.Invoke(new DFCADelegate(this.TTRUpdateSoundVolume), null);
                }
                else
                {
                    TTRUpdateSoundVolume();
                }


                result = TTRSound.getLength(ref TTRLength, FMOD.TIMEUNIT.MS);
                ERRCHECK(result);

                result = TTRSound.getNumTags(ref TTRNumTags, ref TTRNumTagsUpdated);
                ERRCHECK(result);
                oogimaflip_001 = "";
                if (DisplayID3)
                {
                    result = TTRSound.getTag("TIT2", -1, ref TTRDataTag);
                    if (result != FMOD.RESULT.ERR_TAGNOTFOUND)
                    {
                        ERRCHECK(result);
                        oogimaflip_001 = Marshal.PtrToStringAnsi(TTRDataTag.data);
                        if (oogimaflip_001.StartsWith("ÿþ"))
                        {
                            oogimaflip_001 = Marshal.PtrToStringUni(TTRDataTag.data);
                        }
                    }
                    else
                    {
                        result = TTRSound.getTag("TIT1", -1, ref TTRDataTag);
                        if (result != FMOD.RESULT.ERR_TAGNOTFOUND)
                        {
                            ERRCHECK(result);
                            oogimaflip_001 = Marshal.PtrToStringAnsi(TTRDataTag.data);
                            if (oogimaflip_001.StartsWith("ÿþ"))
                            {
                                oogimaflip_001 = Marshal.PtrToStringUni(TTRDataTag.data);
                            }
                        }
                        else
                        {
                            result = TTRSound.getTag("TITLE", -1, ref TTRDataTag);
                            if (result != FMOD.RESULT.ERR_TAGNOTFOUND)
                            {
                                ERRCHECK(result);
                                oogimaflip_001 = Marshal.PtrToStringAnsi(TTRDataTag.data);
                                if (oogimaflip_001.StartsWith("ÿþ"))
                                {
                                    oogimaflip_001 = Marshal.PtrToStringUni(TTRDataTag.data);
                                }
                            }
                            else
                            {
                                // Title tag is not present
                                //if (false )
                                //{
                                //    oogimaflip_001 = "Online Broadcasting";
                                //}
                                //else
                                //{
                                //    oogimaflip_001 = Path.GetFileName(Argument0);
                                //}
                            }
                        }
                    }

                    result = TTRSound.getTag("TPE2", -1, ref TTRDataTag);
                    if (result != FMOD.RESULT.ERR_TAGNOTFOUND)
                    {
                        ERRCHECK(result);
                        oogimaflip_002 = Marshal.PtrToStringAnsi(TTRDataTag.data) + " - ";
                        if (oogimaflip_002.StartsWith("ÿþ"))
                        {
                            oogimaflip_002 = Marshal.PtrToStringUni(TTRDataTag.data) + " - ";
                        }
                    }
                    else
                    {
                        result = TTRSound.getTag("TPE1", -1, ref TTRDataTag);
                        if (result != FMOD.RESULT.ERR_TAGNOTFOUND)
                        {
                            ERRCHECK(result);
                            oogimaflip_002 = Marshal.PtrToStringAnsi(TTRDataTag.data) + " - ";
                            if (oogimaflip_002.StartsWith("ÿþ"))
                            {
                                oogimaflip_002 = Marshal.PtrToStringUni(TTRDataTag.data) + " - ";
                            }
                        }
                        else
                        {
                            result = TTRSound.getTag("ARTIST", -1, ref TTRDataTag);
                            if (result != FMOD.RESULT.ERR_TAGNOTFOUND)
                            {
                                ERRCHECK(result);
                                oogimaflip_002 = Marshal.PtrToStringAnsi(TTRDataTag.data) + " - ";
                                if (oogimaflip_002.StartsWith("ÿþ"))
                                {
                                    oogimaflip_002 = Marshal.PtrToStringUni(TTRDataTag.data) + " - ";
                                }
                            }
                            else
                            {
                                oogimaflip_002 = "";
                            }
                        }
                    }

                    if (false )
                    {
                        //GAPT_CurrentTrackTitle = "Ginever Radio: " + oogimaflip_002.Replace("&", "&&") + oogimaflip_001.Replace("&", "&&");
                    }
                    else
                    {
                        GAPT_CurrentTrackTitle = oogimaflip_002.Replace("&", "&&") + oogimaflip_001.Replace("&", "&&");
                    }

                }
                else
                {
                    if (false )
                    {
                        //GAPT_CurrentTrackTitle = "Ginever Radio";
                    }
                    else
                    {
                        GAPT_CurrentTrackTitle = Path.GetFileName(Argument0);
                    }
                }

                result = TTRSound.getFormat(ref TTRSoundType, ref TTRSoundFormat, ref TTRNull, ref TTRNull);
                ERRCHECK(result);


                TTRFetchSoundTypeTextifier();
                TTRFetchTrackFilesize();

                /* try
                 {
                   //  float queenie = 0;                 
                    // TTRChannel.getFrequency(ref queenie);
                    // TTRChannel.setFrequency(queenie*0.95f);

                     TTRChannel.addDSP(keyMinus1, ref TTRDSPNull);
                 }
                 catch
                 {
                     MessageBox(new IntPtr(0), "Non-critical Exception Raised: GAPI_SETFREQ_FAILURE" + Environment.NewLine + "Failed to set track frequency.", "GSA Exception Trapper", MB_OK | ICON_INFORMATION);
                 }*/

                if (Argument3)
                {
                    GAPT_CurrentPlaybackState = "Paused";
                }
                else
                {
                    GAPT_CurrentPlaybackState = "Playing";
                }
            }
            catch
            {
                //MessageBox(new IntPtr(0), "Non-critical Exception Raised: GAPI_PLAY_FAILURE" + Environment.NewLine + "Failed to play requested track.", "GSA Exception Trapper", MB_OK | ICON_INFORMATION);
            }
        }

        protected void TTRFetchSoundTypeTextifier()
        {
            if (TTRSoundType == FMOD.SOUND_TYPE.AAC)
            {
                GAPT_CurrentFileType = "Unsupported";
            }
            else if (TTRSoundType == FMOD.SOUND_TYPE.AIFF)
            {
                GAPT_CurrentFileType = "Audio Interchange File Format";
            }
            else if (TTRSoundType == FMOD.SOUND_TYPE.ASF)
            {
                GAPT_CurrentFileType = "Windows Media File Format";
            }
            else if (TTRSoundType == FMOD.SOUND_TYPE.AT3)
            {
                GAPT_CurrentFileType = "Sony ATRAC 3 Format";
            }
            else if (TTRSoundType == FMOD.SOUND_TYPE.CDDA)
            {
                GAPT_CurrentFileType = "CD Audio Format";
            }
            else if (TTRSoundType == FMOD.SOUND_TYPE.DLS)
            {
                GAPT_CurrentFileType = "Downloadable Sound Bank";
            }
            else if (TTRSoundType == FMOD.SOUND_TYPE.FLAC)
            {
                GAPT_CurrentFileType = "Free Lossless Audio Codec";
            }
            else if (TTRSoundType == FMOD.SOUND_TYPE.FSB)
            {
                GAPT_CurrentFileType = "FMOD Sample Bank";
            }
            else if (TTRSoundType == FMOD.SOUND_TYPE.GCADPCM)
            {
                GAPT_CurrentFileType = "Nintendo Gamecube ADPCM";
            }
            else if (TTRSoundType == FMOD.SOUND_TYPE.IT)
            {
                GAPT_CurrentFileType = "Impulse Tracker";
            }
            else if (TTRSoundType == FMOD.SOUND_TYPE.MIDI)
            {
                GAPT_CurrentFileType = "Musical Instrument Digital Interface";
            }
            else if (TTRSoundType == FMOD.SOUND_TYPE.MOD)
            {
                GAPT_CurrentFileType = "Modular Audio File Format";
            }
            else if (TTRSoundType == FMOD.SOUND_TYPE.MPEG)
            {
                GAPT_CurrentFileType = "MPEG-1 Audio Layer 3";
            }
            else if (TTRSoundType == FMOD.SOUND_TYPE.OGGVORBIS)
            {
                GAPT_CurrentFileType = "OGG Vorbis File Format";
            }
            else if (TTRSoundType == FMOD.SOUND_TYPE.PLAYLIST)
            {
                GAPT_CurrentFileType = "Unsupported";
            }
            else if (TTRSoundType == FMOD.SOUND_TYPE.RAW)
            {
                GAPT_CurrentFileType = "Raw Audio File";
            }
            else if (TTRSoundType == FMOD.SOUND_TYPE.S3M)
            {
                GAPT_CurrentFileType = "Scream Tracker";
            }
            else if (TTRSoundType == FMOD.SOUND_TYPE.SF2)
            {
                GAPT_CurrentFileType = "Unsupported";
            }
            else if (TTRSoundType == FMOD.SOUND_TYPE.USER)
            {
                GAPT_CurrentFileType = "Unsupported";
            }
            else if (TTRSoundType == FMOD.SOUND_TYPE.WAV)
            {
                GAPT_CurrentFileType = "Wave File";
            }
            else if (TTRSoundType == FMOD.SOUND_TYPE.XM)
            {
                GAPT_CurrentFileType = "Extended Modular Audio File Format";
            }
            else if (TTRSoundType == FMOD.SOUND_TYPE.XMA)
            {
                GAPT_CurrentFileType = "Extended Modular Audio File Format";
            }
            else if (TTRSoundType == FMOD.SOUND_TYPE.VAG)
            {
                GAPT_CurrentFileType = "Sony Playstation ADPCM";
            }
            else if (TTRSoundType == FMOD.SOUND_TYPE.AUDIOQUEUE)
            {
                GAPT_CurrentFileType = "iPhone Hardware Decoder Format";
            }
            else if (TTRSoundType == FMOD.SOUND_TYPE.XWMA)
            {
                GAPT_CurrentFileType = "Xbox-Windows Media Audio Format";
            }
            else
            {
                GAPT_CurrentFileType = "Unsupported";
            }

        }

        protected void TTRStopSound()
        {
            FMOD.RESULT result;

            if (TTRChannel != null)
            {

                bool playing = false;

                result = TTRChannel.isPlaying(ref playing);

                if (playing == true)
                {
                    result = TTRChannel.stop();
                    ERRCHECK(result);

                    GAPT_CurrentPlaybackState = "Stopped";
                }
            }


        }

        public delegate void UpdateTextCallback(string text);

        public void TTRUpdateText_TopTab_Sub(string str)
        {
            groupBox2.Invoke(new UpdateTextCallback(this.TTRUpdateText_TopTab),
            new object[] { str });
        }

        public void TTRUpdateText_TopTab(string str)
        {
            groupBox2.Text = str;
        }

        public void TTRUpdateText_VolTab_Sub(string str)
        {
            this.Invoke(new UpdateTextCallback(this.TTRUpdateText_VolTab),
            new object[] { str });
        }

        public void TTRUpdateText_VolTab(string str)
        {
            
        }

        public void TTRUpdateTrackbar_Sub(string str)
        {
            trackBar2.Invoke(new UpdateTextCallback(this.TTRUpdateTrackbar),
                           new object[] { str });
        }

        public void TTRUpdateTrackbar(string str)
        {
            if (false )
            {
                //trackBar2.Enabled = false;
            }
            else
            {
                FMOD.RESULT result;
                uint TTRPosition = 0;
                if (TTRChannel != null)
                {
                    try
                    {
                        result = TTRChannel.getPosition(ref TTRPosition, FMOD.TIMEUNIT.MS);
                        //ERRCHECK(result);
                    }
                    catch
                    {
                        TTRPosition = 0;
                    }
                }
                if (TTRLength == 0)
                {
                    trackBar2.Enabled = false;
                    trackBar2.Maximum = 1;
                    trackBar2.Value = 0;
                }
                else
                {
                    trackBar2.Enabled = true;
                    trackBar2.Maximum = (int)TTRLength;
                    if (TTRPosition > TTRLength - 1)
                    {
                        TTRPosition = TTRLength - 1;
                    }
                    trackBar2.Value = (int)TTRPosition;
                }
            }
        }

        public void TTRUpdateText_Row1_Sub(string str)
        {
            label1.Invoke(new UpdateTextCallback(this.TTRUpdateText_Row1),
            new object[] { str });
        }

        public void TTRUpdateText_Row1(string str)
        {
            label1.Text = str;
        }

        public void TTRUpdateText_VolumeRow_Sub(string str)
        {
            label5.Invoke(new UpdateTextCallback(this.TTRUpdateText_VolumeRow),
            new object[] { str });
        }

        public void TTRUpdateText_VolumeRow(string str)
        {
            label5.Text = str;
        }

        public void TTRUpdateText_Row2_Sub(string str)
        {
            label2.Invoke(new UpdateTextCallback(this.TTRUpdateText_Row2),
            new object[] { str });
        }

        public void TTRUpdateText_Row2(string str)
        {
            label2.Text = str;
        }

        public void TTRUpdateText_Row3_Sub(string str)
        {
            label3.Invoke(new UpdateTextCallback(this.TTRUpdateText_Row3),
            new object[] { str });
        }

        public void TTRUpdateText_Row3(string str)
        {
            label3.Text = str;
        }

        protected void TTRPauseSound()
        {
            FMOD.RESULT result;

            if (TTRChannel != null)
            {
                bool playing = false;

                result = TTRChannel.isPlaying(ref playing);

                if (playing == true)
                {

                    bool pausecheck = false;

                    result = TTRChannel.getPaused(ref pausecheck);

                    if (pausecheck == true)
                    {
                        result = TTRChannel.setPaused(false);
                        ERRCHECK(result);
                        GAPT_CurrentPlaybackState = "Playing";
                    }
                    else
                    {
                        result = TTRChannel.setPaused(true);
                        ERRCHECK(result);
                        GAPT_CurrentPlaybackState = "Paused";
                    }
                }
                else if (false )
                {
                    //TTRUpdateGUICheckboxStatus(false);
                    //result = TTRChannel.setPaused(false);
                    //ERRCHECK(result);
                    //GAPT_CurrentPlaybackState = "Playing";
                }
                else if (GAPT_LastTrackName != string.Empty)
                {
                    TTRPlaySound(GAPT_LastTrackName, GAPT_CurrentLoopStart, GAPT_CurrentLoopDone, false);
                }
            }
            else
            {
                // Attempted fix for issue #94 "Play from Playlist with Play button?" reported by Trevor Sparks

                if (true )
                {
                    if (listBox1.Items.Count > 0)
                    {
                        // Playlist mode is selected and there is something in the playlist

                        /* The following section can be uncommented for an alternate behaviour (NOTE: THIS IS UNTESTED)
                        
                                  int temp_selected = listBox1.SelectedIndex;
                                  if (temp_selected != -1)
                                  {
                                      // Act as doubleclick (play highlighted entry)
                                      listBox1b.SelectedIndex = listBox1.SelectedIndex;
                                      TTRTrackPlayingEntryID = listBox1.SelectedIndex;
                                      TTRPlaySound(listBox1b.SelectedItem.ToString(), 0, 0, false);
                                  }
                                  else
                                  {

                        */
                        // Nothing is selected, so play first track in the list
                        TTRTrackPlayingEntryID = 0;
                        TTRPlaySound(listBox1b.Items[TTRTrackPlayingEntryID].ToString(), 0, 0, false);
                        //        }   // <<----- uncomment this line also
                    }
                }
            }

        }

        private void TTRUnloadSound()
        {
            if (TTRChannel != null)
            {
                // Bugfix: sound now only unloads if stopped
                if (GAPT_CurrentPlaybackState == "Stopped")
                {
                    TTRSound.release();
                    TTRUpdateText_Row3_Sub("No track is currently loaded.");
                    TTRUpdateText_Row1_Sub(" ");
                    TTRUpdateText_Row2_Sub(" ");
                    TTRSound = null;
                    TTRChannel = null;
                    TTRLength = 0;
                    GAPT_LastPROCESSEDName = "";
                    GAPT_LastTrackName = "";
                }
            }
        }

        private void button_add_Click(object sender, EventArgs e)
        {
            //    OpenFileDialog dialog = new OpenFileDialog();
            //   dialog.Filter = "// File Types (*.AIFF; *.ASF; *.ASX; *.DAT; *.FLAC; *.GM; *.IT; *.M3U; *.MID; *.MOD; *.MP2; *.MP3; *.OGG; *.RAW; *.S3M; *.VAG; *.WAV; *.WAX; *.WMA; *.XM; *.XMA)|*.aiff;*.asf;*.asx;*.dat;*.flac;*.gm;*.it;*.m3u;*.mid;*.mod;*.mp2;*.mp3;*.ogg;*.raw;*.s3m;*.vag;*.wav;*.wax;*.wma;*.xm;*.xma|Audio Interchange File Format (*.AIFF)|*.aiff|Chris Sawyer Software [CSS] Audio Data File (*.DAT)|*.dat|Free Lossless Audio Codec (*.FLAC)|*.flac|Impulse Tracker (*.IT)|*.it|Musical Instrument Digital Interface (*.GM, *.MID, *.MIDI)|*.gm;*.mid;*.midi|Modular Audio File Format (*.MOD)|*.mod|MPEG-1 Audio Layer 3 (*.MP2, *.MP3)|*.mp2;*.mp3|OGG Vorbis File Format (*.OGG)|*.ogg|Raw Audio File (*.RAW)|*.raw|Scream Tracker (*.S3M)|*.s3m|Wave File (*.WAV)|*.wav|Windows Media File Format (*.WMA)|*.wma|Extended Modular Audio File Format (*.XM, *.XMA)|*.xm;*.xma|All Files|*.*";
            //   dialog.InitialDirectory = GAPT_LastDirectoryName; dialog.Title = "Open";
            dialog.Filter = DefaultOpenDialogFilter;
            dialog.Multiselect = true;

            if (dialog.ShowDialog() == DialogResult.OK)
            {

                foreach (String file in dialog.FileNames)
                {
                    if (file != string.Empty)
                    {
                        listBox1.Items.Add(Path.GetFileName(file));
                        listBox1b.Items.Add(file);
                    }
                }
            }
        }

        private void button_remove_Click(object sender, EventArgs e)
        {
            int temp_selected = listBox1.SelectedIndex;

            if (temp_selected != -1)
            {
                listBox1b.SelectedIndex = temp_selected;
                listBox1.Items.RemoveAt(temp_selected);
                listBox1b.Items.RemoveAt(temp_selected);
                if ((listBox1.Items.Count - 1) > (temp_selected - 1))
                {
                    listBox1.SelectedIndex = temp_selected;
                    listBox1b.SelectedIndex = temp_selected;
                }
                else if ((listBox1.Items.Count - 1) > (temp_selected - 2))
                {
                    listBox1.SelectedIndex = temp_selected - 1;
                    listBox1b.SelectedIndex = temp_selected - 1;
                }

                if (TTRTrackPlayingEntryID == temp_selected)
                {
                    // MORON! You just DELETED the entry we were playing! There is no alternative but to unload!
                    TTRTrackPlayingEntryID = TTRTrackPlayingEntryID - 1;
                    TTRStopSound();
                    TTRUnloadSound();
                }
                else if (TTRTrackPlayingEntryID > (temp_selected - 1))
                {
                    // The position of the currently playing track has moved backwards. Adjust accordingly.
                    TTRTrackPlayingEntryID = TTRTrackPlayingEntryID - 1;
                }
            }
        }

        



        private void button_of_fun_Click(object sender, EventArgs e)
        {
            //   OpenFileDialog dialog = new OpenFileDialog();
            //    dialog.Filter = "All Supported File Types (*.AIFF; *.ASF; *.ASX; *.DAT; *.FLAC; *.GM; *.IT; *.M3U; *.MID; *.MOD; *.MP2; *.MP3; *.OGG; *.RAW; *.S3M; *.VAG; *.WAV; *.WAX; *.WMA; *.XM; *.XMA)|*.aiff;*.asf;*.asx;*.dat;*.flac;*.gm;*.it;*.m3u;*.mid;*.mod;*.mp2;*.mp3;*.ogg;*.raw;*.s3m;*.vag;*.wav;*.wax;*.wma;*.xm;*.xma|Audio Interchange File Format (*.AIFF)|*.aiff|Chris Sawyer Software [CSS] Audio Data File (*.DAT)|*.dat|Free Lossless Audio Codec (*.FLAC)|*.flac|Impulse Tracker (*.IT)|*.it|Musical Instrument Digital Interface (*.GM, *.MID, *.MIDI)|*.gm;*.mid;*.midi|Modular Audio File Format (*.MOD)|*.mod|MPEG-1 Audio Layer 3 (*.MP2, *.MP3)|*.mp2;*.mp3|OGG Vorbis File Format (*.OGG)|*.ogg|Raw Audio File (*.RAW)|*.raw|Scream Tracker (*.S3M)|*.s3m|Wave File (*.WAV)|*.wav|Windows Media File Format (*.WMA)|*.wma|Extended Modular Audio File Format (*.XM, *.XMA)|*.xm;*.xma|All Files|*.*";
            //   dialog.InitialDirectory = GAPT_LastDirectoryName; dialog.Title = "Open";
            dialog.Filter = DefaultOpenDialogFilter;
            dialog.Multiselect = false;

            if (dialog.ShowDialog() == DialogResult.OK)
            {

                string input = string.Empty;
                input = dialog.FileName;

                if (input != string.Empty)
                {
                    GAPT_LastDirectoryName = Path.GetDirectoryName(input);
                    TTRTrackPlayingEntryID = -1;
                    TTRPlaySound(input, GAPT_CurrentLoopStart, GAPT_CurrentLoopDone, true);
                    TTRStopSound();

                    if (false )
                    {
                        //subForm_LoopStart = new Form2();
                        //if (subForm_LoopStart.ShowDialog() == DialogResult.OK)
                        //{
                        //    if (subForm_LoopStart.textBox1.Text.ToString() == "")
                        //    {
                        //        GAPT_CurrentLoopStart = 0;
                        //    }
                        //    else
                        //    {
                        //        try
                        //        {
                        //            GAPT_CurrentLoopStart = Convert.ToUInt32(subForm_LoopStart.textBox1.Text.ToString());
                        //            if (GAPT_CurrentLoopStart > TTRLength)
                        //            {
                        //                GAPT_CurrentLoopStart = 0;
                        //            }
                        //            else if (GAPT_CurrentLoopStart < 0)
                        //            {
                        //                GAPT_CurrentLoopStart = 0;
                        //            }
                        //        }
                        //        catch
                        //        {
                        //            //MessageBox(new IntPtr(0), "Non-critical Exception Raised: GAPI_DIALOG1_EXCEPTION" + Environment.NewLine + "Sorry, that failed.", "GSA Exception Trapper", MB_OK | ICON_STOP);
                        //            GAPT_CurrentLoopStart = 0;
                        //        }
                        //    }
                        //}

                    }


                    {
                        subForm_LoopEnd = new Form3();
                        if (subForm_LoopEnd.ShowDialog() == DialogResult.OK)
                        {
                            if (subForm_LoopEnd.textBox1.Text.ToString() == "")
                            {
                                try
                                {
                                    GAPT_CurrentLoopDone = 0;
                                }
                                catch
                                {
                                    MessageBox(new IntPtr(0), "Non-critical Exception Raised: GAPI_DIALOG2_EXCEPTION" + Environment.NewLine + "Sorry, that failed.", "GSA Exception Trapper", MB_OK | ICON_STOP);
                                    GAPT_CurrentLoopDone = 0;
                                }
                            }
                            else
                            {
                                try
                                {
                                    GAPT_CurrentLoopDone = Convert.ToUInt32(subForm_LoopEnd.textBox1.Text.ToString());
                                    if (GAPT_CurrentLoopDone > TTRLength)
                                    {
                                        GAPT_CurrentLoopDone = 0;
                                    }
                                    else if (GAPT_CurrentLoopDone < 0)
                                    {
                                        GAPT_CurrentLoopDone = 0;
                                    }
                                }
                                catch
                                {
                                    MessageBox(new IntPtr(0), "Non-critical Exception Raised: GAPI_DIALOG3_EXCEPTION" + Environment.NewLine + "Sorry, that failed.", "GSA Exception Trapper", MB_OK | ICON_STOP);
                                    GAPT_CurrentLoopDone = 0;
                                }
                            }

                        }

                    }

                    TTRPlaySound(input, GAPT_CurrentLoopStart, GAPT_CurrentLoopDone, false);
                }

                dialog.Dispose();
            }
        }

        public static string GetFileSize(long Bytes)
        {
            if (Bytes >= 1073741824)
            {
                Decimal size = Decimal.Divide(Bytes, 1073741824);
                return String.Format("{0:##.##} GB", size);
            }
            else if (Bytes >= 1048576)
            {
                Decimal size = Decimal.Divide(Bytes, 1048576);
                return String.Format("{0:##.##} MB", size);
            }
            else if (Bytes >= 1024)
            {
                Decimal size = Decimal.Divide(Bytes, 1024);
                return String.Format("{0:##.##} KB", size);
            }
            else if (Bytes > 0 & Bytes < 1024)
            {
                Decimal size = Bytes;
                return String.Format("{0:##.##} bytes", size);
            }
            else
            {
                return "0 bytes";
            }


        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                System.Threading.Thread.Sleep(40);

                system.update();

                FMOD.RESULT result;
                uint TTRPosition = 0;
                bool playing = false;
                if (TTRChannel != null)
                {
                    if (true ) try
                        {
                            result = TTRChannel.getPosition(ref TTRPosition, FMOD.TIMEUNIT.MS);
                            //ERRCHECK(result);
                            if (TTRPosition > TTRLength)
                            {
                                try
                                {
                                    TTRChannel.stop();
                                }
                                catch
                                {
                                    // woo
                                }
                            }
                        }
                        catch
                        {
                            TTRPosition = 0;
                        }
                    try
                    {
                        if (true )
                        {
                            if (GAPT_CurrentPlaybackState == "Playing")
                            {
                                playing = false;
                                result = TTRChannel.isPlaying(ref playing);
                                if (playing == false)
                                {

                                    if (listBox1.Items.Count == 0)
                                    {
                                        GAPT_CurrentPlaybackState = "Stopped";
                                    }
                                    else
                                    {
                                        if (shuffleCheck && listBox1b.Items.Count > 1)
                                        {
                                            int newTrackPlayingEntry = TTRTrackPlayingEntryID;
                                            bool finded = false;
                                            double maxd = -9999;
                                            string findedfile = "";
                                            for (int i = 0; i < listBox1b.Items.Count ; i++)
                                            {
                                                string s = (string)listBox1b.Items[i];
                                                var bs = NNH.GetBaseSign(s);
                                                if ((bs.NeuroDays==null )|(bs.NeuroHours == null))
                                                {
                                                    if (bs.Spectr != null)
                                                    {
                                                        NNH.GetNeuroData(ref bs);
                                                    }
                                                }
                                                if (bs.LastPlay ==null )
                                                {
                                                    bs.LastPlay = new DateTime(2001, 01, 01);
                                                }
                                                var ts = DateTime.Now-bs.LastPlay ;
                                                if (ts.TotalDays>6)
                                                {
                                                    if ((bs.NeuroDays != null) & (bs.NeuroHours != null))
                                                    {
                                                        double d = bs.NeuroDays[(int)DateTime.Now.DayOfWeek] + bs.NeuroHours[DateTime.Now.Hour];
                                                        if (d>maxd )
                                                        {
                                                            maxd = d;
                                                            findedfile = bs.filepath;
                                                            finded = true;
                                                        }
                                                    }
                                                }
                                            }
                                            if (finded)
                                            {
                                                for (int i = 0; i < listBox1b.Items.Count; i++)
                                                {
                                                    string s = (string)listBox1b.Items[i];
                                                    if (s==findedfile )
                                                    {
                                                        var bs = NNH.GetBaseSign(s);
                                                        bs.LastPlay = DateTime.Now;
                                                        TTRTrackPlayingEntryID = i;
                                                        break;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                while (newTrackPlayingEntry == TTRTrackPlayingEntryID)
                                                {
                                                    TTRTrackPlayingEntryID = rand.Next(listBox1b.Items.Count);
                                                }
                                                string s = (string)listBox1b.Items[TTRTrackPlayingEntryID];
                                                var bs = NNH.GetBaseSign(s);
                                                bs.LastPlay = DateTime.Now;
                                            }
                                        }
                                        else
                                        {
                                            TTRTrackPlayingEntryID += 1;
                                        }

                                        if (TTRTrackPlayingEntryID < 0)
                                        {
                                            TTRTrackPlayingEntryID = 0;
                                        }
                                        else if (TTRTrackPlayingEntryID > (listBox1.Items.Count - 1))
                                        {
                                            TTRTrackPlayingEntryID = 0;
                                        }
                                        PlaySelected(TTRTrackPlayingEntryID);
//                                      TTRPlaySound(listBox1b.Items[TTRTrackPlayingEntryID].ToString(), 0, 0, false);
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        //MessageBox(new IntPtr(0), "Non-critical Exception Raised: GAPI_INCREMENT_FAILURE" + Environment.NewLine + "Track incrementation failed.", "GSA Exception Trapper", MB_OK | ICON_INFORMATION);
                    }
                }


                if (GAPT_LastPROCESSEDState != GAPT_CurrentPlaybackState)
                {
                    GAPT_LastPROCESSEDState = GAPT_CurrentPlaybackState;

                    if (GAPT_CurrentPlaybackState == "Playing")
                    {
                        button_playpause.Image = Athena.Properties.Resources.pause;
                    }
                    else if (GAPT_CurrentPlaybackState == "Paused")
                    {
                        button_playpause.Image = Athena.Properties.Resources.play;
                    }
                    else
                    {
                        button_playpause.Image = Athena.Properties.Resources.play;
                    }
                }
                try
                {
                    if (false )
                    {
                        //if (GAPT_LastPROCESSEDState == "Playing")
                        //{
                        //    TTRUpdateText_TopTab_Sub("[Looping: " + GAPT_CurrentLoopStart.ToString() + " - " + GAPT_CurrentLoopDone.ToString() + "]");
                        //    TTRUpdateText_VolTab_Sub("[" + TTRPosition.ToString() + "/" + TTRLength.ToString() + "]");
                        //}
                        //else
                        //{
                        //    TTRUpdateText_TopTab_Sub("[" + GAPT_LastPROCESSEDState + ": " + GAPT_CurrentLoopStart.ToString() + " - " + GAPT_CurrentLoopDone.ToString() + "]");
                        //    TTRUpdateText_VolTab_Sub("[" + TTRPosition.ToString() + "/" + TTRLength.ToString() + "]");
                        //}
                    }
                    else if (true )
                    {
                        TTRUpdateText_TopTab_Sub("[" + GAPT_LastPROCESSEDState + "]");
                        TTRUpdateText_VolTab_Sub("[" + TTRPosition.ToString() + "/" + TTRLength.ToString() + "]");
                    }
                    //else if (false )
                    //{
                    //    //result = TTRSound.getOpenState(ref a, ref b, ref c);
                    //    //ERRCHECK(result);
                    //    //TTRUpdateText_TopTab_Sub("[" + GAPT_LastPROCESSEDState + "]");
                    //    //TTRUpdateText_VolTab_Sub((a == FMOD.OPENSTATE.BUFFERING ? "Buffering..." : (a == FMOD.OPENSTATE.CONNECTING ? "Connecting..." : "Connected")) + " (Buffer: " + b + "%)" + (c ? " STARVING" : "        "));
                    //}
                }
                catch
                {
                    // MessageBox(new IntPtr(0), "Non-critical Exception Raised: GAPI_GUIUPDATE1_FAILURE" + Environment.NewLine + "Failed to update GUI state.", "GSA Exception Trapper", MB_OK | ICON_INFORMATION);
                }
                if (true ) try
                    {
                        TTRUpdateTrackbar_Sub("trackPosition");
                    }
                    catch
                    {
                        //   MessageBox(new IntPtr(0), "Non-critical Exception Raised: GAPI_GUIUPDATE2_FAILURE" + Environment.NewLine + "Failed to update trackbar position.", "GSA Exception Trapper", MB_OK | ICON_INFORMATION);
                    }

                if (GAPT_LastPROCESSEDName != GAPT_LastTrackName)
                {
                    try
                    {
                        //MessageBox(new IntPtr(0), "Track has been changed", "Debug", MB_OK | ICON_INFORMATION);
                        GAPT_LastPROCESSEDName = GAPT_LastTrackName;
                        if (GAPT_CurrentTrackAuthor == string.Empty)
                        {
                            TTRUpdateText_Row1_Sub(GAPT_CurrentTrackTitle);
                        }
                        else
                        {
                            TTRUpdateText_Row1_Sub(GAPT_CurrentTrackAuthor + ": " + GAPT_CurrentTrackTitle);
                        }
                        TTRUpdateText_Row2_Sub(GAPT_CurrentFileType + " (" + GAPT_CurrentFileSize + ")");
                        TTRUpdateText_Row3_Sub(Path.GetFileName(GAPT_LastPROCESSEDName));
                    }
                    catch
                    {
                        MessageBox(new IntPtr(0), "Non-critical Exception Raised: GAPI_NAMEPROC_FAILURE" + Environment.NewLine + "Failed to process track name", "GSA Exception Trapper", MB_OK | ICON_INFORMATION);
                    }
                }

                try
                {
                    glControl1.Invalidate();
                }
                catch
                {
                    MessageBox(new IntPtr(0), "Non-critical Exception Raised: GAPI_OPENGL_INVALIDATE" + Environment.NewLine + "Failed to invalidate OpenGL control", "GSA Exception Trapper", MB_OK | ICON_INFORMATION);
                }
            }
        }

        static GLForm glform=new GLForm();
        OpenTK.GLControl glControl1 = glform.glControl1;

        private void button_playpause_Click(object sender, EventArgs e)
        {
            TTRPauseSound();
        }

        private void button_stop_Click(object sender, EventArgs e)
        {
            TTRStopSound();
        }

        DateTime lastclick = DateTime.Now;
        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(System.Windows.Forms.Keys vKey);
        private void WaitKey()
        {
            while (this.IsHandleCreated)
            {
                int res1 = Convert.ToInt32(GetAsyncKeyState(Keys.XButton1).ToString());
                int res2 = Convert.ToInt32(GetAsyncKeyState(Keys.XButton2).ToString());
                if (res1 != 0)
                {
                    var t = DateTime.Now - lastclick;
                    if (t.TotalSeconds > 1)
                    {
                        lastclick = DateTime.Now;
                        NextTrack();
                    }
                }
                if (res2 != 0)
                {
                    var t = DateTime.Now - lastclick;
                    if (t.TotalSeconds > 1)
                    {
                        lastclick = DateTime.Now;
                        PrevTrack();
                    }
                }
                //XButton1 или XButton2 соответственно 4 и 5 кнопки
                Thread.Sleep(1000);
            }
        }

        Thread SpectrScanner;
        private void Form1_Load(object sender, EventArgs e)
        {
            DrawControlBackground(pictureBox1, false);
            UpdateLayeredBackground();
            glform.glControl1.Paint+= new PaintEventHandler(glControl1_Paint);
            glform.Show();
            MethodInvoker mi = new MethodInvoker(WaitKey);
            mi.BeginInvoke(null, null);
            SpectrScanner = new Thread(DoScanSpectrum);
            NNH.Load();
            NNH.LoadLB();
            if (File.Exists("PlaylistFiles.pls"))
            {
                var f = File.ReadAllLines("PlaylistFiles.pls");
                for (int i = 0; i < f.Length ; i++)
                {
                    listBox1b.Items.Add(f[i]);
                }
            }
            if (File.Exists("PlaylistNames.pls"))
            {
                var f = File.ReadAllLines("PlaylistNames.pls");
                for (int i = 0; i < f.Length; i++)
                {
                    listBox1.Items.Add(f[i]);
                }
            }
            this.Location =Athena.Properties.Settings.Default.Pos;
            //alphaFormTransformer1.TransformForm(0);
            uint FMODversion = 0;
            FMOD.RESULT result;
            /*
                Create a System object and initialize.
            */
            result = FMOD.Factory.System_Create(ref system);
            ERRCHECK(result);
            result = system.getVersion(ref FMODversion);
            ERRCHECK(result);
            if (FMODversion < FMOD.VERSION.number)
            {
                MessageBox(new IntPtr(0), "Exception Raised: VERSION_MISMATCH" + Environment.NewLine + "Version detected: " + FMODversion.ToString("X") + Environment.NewLine + "Version expected: " + FMOD.VERSION.number.ToString("X"), "Version Mismatch", MB_OK | ICON_STOP);
                Environment.Exit(-1);
            }
            result = system.init(32, FMOD.INITFLAGS.NORMAL, (IntPtr)null);
            ERRCHECK(result);
            result = system.setStreamBufferSize(64 * 1024, FMOD.TIMEUNIT.RAWBYTES);
            ERRCHECK(result);
            //DSP Pitch shifts - awesome
            system.createDSPByType(FMOD.DSP_TYPE.PITCHSHIFT, ref keyPlus1);
            keyPlus1.setParameter(0, 1.05946f);
            system.createDSPByType(FMOD.DSP_TYPE.PITCHSHIFT, ref keyPlus2);
            keyPlus2.setParameter(0, 1.12245f);
            system.createDSPByType(FMOD.DSP_TYPE.PITCHSHIFT, ref keyPlus3);
            keyPlus3.setParameter(0, 1.18920f);
            system.createDSPByType(FMOD.DSP_TYPE.PITCHSHIFT, ref keyPlus4);
            keyPlus4.setParameter(0, 1.25991f);
            system.createDSPByType(FMOD.DSP_TYPE.PITCHSHIFT, ref keyPlus5);
            keyPlus5.setParameter(0, 1.33482f);
            system.createDSPByType(FMOD.DSP_TYPE.PITCHSHIFT, ref keyMinus1);
            keyMinus1.setParameter(0, 0.94380f);
            system.createDSPByType(FMOD.DSP_TYPE.PITCHSHIFT, ref keyMinus2);
            keyMinus2.setParameter(0, 0.89076f);
            system.createDSPByType(FMOD.DSP_TYPE.PITCHSHIFT, ref keyMinus3);
            keyMinus3.setParameter(0, 0.84069f);
            system.createDSPByType(FMOD.DSP_TYPE.PITCHSHIFT, ref keyMinus4);
            keyMinus4.setParameter(0, 0.79345f);
            system.createDSPByType(FMOD.DSP_TYPE.PITCHSHIFT, ref keyMinus5);
            keyMinus5.setParameter(0, 0.74886f);
            backgroundWorker2.RunWorkerAsync();
            TTRUpdateGUICheckboxStatus(true);
            SpectrScanner.Start();
            MethodInvoker mi2 = new MethodInvoker(delegate
            {
                for (int i = 0; i < listBox1b.Items.Count ; i++)
                {
                    NeedToGenTGI.Push((string)listBox1b.Items[i]);
                }
                do
                {   
                    var file = NeedToGenTGI.Pop();
                    NNH.GetBaseSign(file);
                } while (NeedToGenTGI.Count > 0);
            });
            mi2.BeginInvoke(null, null);
        }

        private void button_restart_Click(object sender, EventArgs e)
        {
            if (TTRChannel != null)
            {
                TTRChannel.setPosition(0, FMOD.TIMEUNIT.MS);
            }
        }

        private void TTRUpdateGUICheckboxStatus(bool q)
        {
            FMOD.RESULT result;
            if (TTRSound != null)
            {
                TTRStopSound();
                TTRUnloadSound();
            }
            if (false )
            {
                //if (TTRSound != null)
                //{
                //    result = TTRSound.setLoopPoints(0, FMOD.TIMEUNIT.MS, 0, FMOD.TIMEUNIT.MS);
                //    ERRCHECK(result);
                //    result = TTRSound.setMode(FMOD.MODE.LOOP_OFF | FMOD.MODE.ACCURATETIME);
                //    ERRCHECK(result);
                //}
                //button_add.Enabled = false;
                //button_remove.Enabled = false;
                //listBox1.Enabled = false;
                //button_prev.Image = Athena.Properties.Resources.prev;
                //button_next.Image = Athena.Properties.Resources.next;
                //button_prev.Enabled = false;
                //button_next.Enabled = false;
                //button_restart.Enabled = false;
                //trackBar2.Value = 0;
                //trackBar2.Enabled = false;

                //TTRTrackPlayingEntryID = -1;
                //TTRPlaySound("http://athena-radio-link.ginever.net:8000/", GAPT_CurrentLoopStart, GAPT_CurrentLoopDone, true);
                //if (q) { TTRStopSound(); }

            }
            else if (false )
            {
                //if (TTRSound != null)
                //{
                //    result = TTRSound.setLoopPoints(GAPT_CurrentLoopStart, FMOD.TIMEUNIT.MS, GAPT_CurrentLoopDone, FMOD.TIMEUNIT.MS);
                //    ERRCHECK(result);
                //    result = TTRSound.setMode(FMOD.MODE.LOOP_NORMAL | FMOD.MODE.ACCURATETIME);
                //    ERRCHECK(result);
                //}
                //button_add.Enabled = false;
                //button_remove.Enabled = false;
                //listBox1.Enabled = false;
                //button_prev.Image = Athena.Properties.Resources.loop1;
                //button_next.Image = Athena.Properties.Resources.loop2;
                //button_prev.Enabled = true;
                //button_next.Enabled = true;
                //button_restart.Enabled = true;
            }
            else if (true )
            {
                if (TTRSound != null)
                {
                    result = TTRSound.setLoopPoints(0, FMOD.TIMEUNIT.MS, 0, FMOD.TIMEUNIT.MS);
                    ERRCHECK(result);
                    result = TTRSound.setMode(FMOD.MODE.LOOP_OFF | FMOD.MODE.ACCURATETIME);
                    ERRCHECK(result);
                }
                button_add.Enabled = true;
                button_remove.Enabled = true;
                listBox1.Enabled = true;
                button_prev.Image = Athena.Properties.Resources.prev;
                button_next.Image = Athena.Properties.Resources.next;
                button_prev.Enabled = true;
                button_next.Enabled = true;
                button_restart.Enabled = true;
            }
        }

        private void button_prev_Click(object sender, EventArgs e)
        {
            PrevTrack();
        }

        void PrevTrack()
        {
            if (curn > 0)
            {
                BeginInvoke(new MethodInvoker(delegate
                {
                    if (listBox1.SelectedIndex - 1 >= 0)
                    {
                        listBox1.SelectedIndex--;
                        PlaySelected(listBox1.SelectedIndex);
                    }
                }));
                
            }
        }

        private void button_next_Click(object sender, EventArgs e)
        {
            NextTrack();
        }

        void NextTrack()
        {
            if (CurTrack != null) CurTrack.Correct(NeuroHelper.BaseSign.corValues.Skip, (byte)(int)DateTime.Now.DayOfWeek, (byte)DateTime.Now.Hour);
            BeginInvoke(new MethodInvoker(delegate
            {
                if (listBox1.SelectedIndex + 1 <= listBox1.Items.Count)
                {
                    listBox1.SelectedIndex++;
                    PlaySelected(listBox1.SelectedIndex);
                }
            }));
        }


        NeuroHelper NNH = new NeuroHelper();
        Athena.NeuroHelper.BaseSign CurTrack;

        float[] DoubleToFloat(double[] data)
        {
            float[] ff = new float[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                float result = (float)data[i];
                if (float.IsPositiveInfinity(result))
                {
                    result = float.MaxValue;
                }
                else if (float.IsNegativeInfinity(result))
                {
                    result = float.MinValue;
                }
                ff[i] = result;
            }
            return ff;
        }

        public Bitmap GrabScreenshot()
        {
            if (GraphicsContext.CurrentContext == null)
                throw new GraphicsContextMissingException();

            Bitmap bmp = new Bitmap(glControl1.ClientSize.Width, glControl1.ClientSize.Height/2);
            System.Drawing.Imaging.BitmapData data =
                bmp.LockBits(new Rectangle(0,0,glControl1.ClientSize.Width,glControl1.ClientSize.Height/2)   , System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            GL.ReadPixels(0, 0, glControl1.ClientSize.Width, glControl1.ClientSize.Height/2, OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return bmp;
        }

        void PlaySelected(int index)
        {
            if (index != -1)
            {
                if (CurTrack!=null )
                {
                    foreach (var item in NNH.Signes )
                    {

                        if (item.filename == CurTrack.filename )
                        {
                            item.Spectr = GetDoubAr(RawSpectr);
                            //TTRPauseSound();
                            //for (int i = 0; i < 10; i++)
                            //{
                            //    Application.DoEvents();
                            //    Thread.Sleep(10);
                            //}
                            //Application.DoEvents();
                            //glControl1.Invalidate();
                            //Application.DoEvents();
                            //pictureBox2.BackgroundImage = GrabScreenshot();
                            //pictureBox2.BackgroundImageLayout = ImageLayout.Stretch  ;
                        }
                    }
                    MethodInvoker mi = new MethodInvoker(delegate
                    {
                        NNH.Save();
                    });
                    mi.BeginInvoke(null, null);
                   
                }
                
                curn = index;
                TTRTrackPlayingEntryID = index;
                // MessageBox(new IntPtr(0), Non-critical Exception Raised: GAPI_DEBUGGER" + Environment.NewLine + listBox1b.SelectedItem.ToString(), "GSA Exception Trapper", MB_OK | ICON_STOP);
                TTRPlaySound(listBox1b.Items[index].ToString(), 0, 0, false);
                if (curn > 0)
                {
                   CurTrack= NNH.GetBaseSign(listBox1b.Items[curn].ToString());
                   if (CurTrack.Spectr!=null )
                   {
                       RawSpectr = DoubleToFloat(CurTrack.Spectr);
                   }
                    MethodInvoker mi = new MethodInvoker(delegate
                       {
                           var nn = TTRTrackPlayingEntryID;
                           BassGetSpectrum.Spectrum spec = new BassGetSpectrum.Spectrum();
                           var specc = spec.GetRawSpectrData(CurTrack.filepath, BassGetSpectrum.Spectrum.FFTSize.FFT1024, 100, 10000);
                           if (nn==curn)
                           {
                               RawSpectr = specc;
                               spec = null;
                           }
                       });
                       mi.BeginInvoke(null, null);
                   if (CurTrack.TGI != null)
                   {
                       if (CurTrack.Spectr != null)
                       {
                           NNH.GetNeuroData(ref CurTrack);
                           if ((CurTrack.NeuroDays != null) & (CurTrack.NeuroHours != null))
                           {
                               double d = CurTrack.NeuroDays[(int)DateTime.Now.DayOfWeek] + CurTrack.NeuroHours[DateTime.Now.Hour];
                               d = d / 2;
                               d = d - 0.4;
                               d = d * 10;
                               d = d * 100;
                               d = d -90;
                               d = d * d;
                               d = d / 4;
                               if (d>450)
                               {
                                   d = d - 450;
                               }
                               d = Math.Round(d, 1);
                               double d2 = CurTrack.NeuroDays.Average() + CurTrack.NeuroHours.Average();
                               d2 = d2 / 2;
                               d2 = d2 - 0.4;
                               d2 = d2 * 10;
                               d2 = d2 * 100;
                               d2 = d2 - 90;
                               d2 = d2 * d2;
                               d2 = d2 /4;
                               d2 = Math.Round(d2, 1);
                               this.BeginInvoke(new MethodInvoker(delegate
                               {
                                   label4.Text = "Рейтинг нейросети:" + d.ToString() + "/" + d2.ToString() + " pts.";
                               }));
                           }
                           else
                           {
                               this.BeginInvoke(new MethodInvoker(delegate
                               {
                                   label4.Text = "Рейтинг недоступен...";
                               }));
                           }
                       }
                       else
                       {
                           this.BeginInvoke(new MethodInvoker(delegate
                           {
                               label4.Text = "Рейтинг недоступен...";
                           }));
                       }
                   }
                   else
                   {
                       this.BeginInvoke(new MethodInvoker(delegate
                       {
                           label4.Text = "Рейтинг недоступен...";
                       }));
                   }
                }
            }
        }

        bool clearspectrum = false;
        bool ScanSpectrum = true;
        int scncnt = 0;
        Stack<float[]> SpectrumLog = new Stack<float[]>();
        float[] RawSpectr=new float[1024];
        void DoScanSpectrum()
        {
            do
            {
                if (clearspectrum)
                {
                    clearspectrum = false;
                    scncnt = 0;
                    for (int i = 0; i < RawSpectr.Length; i++)
                    {
                        RawSpectr[i] = 0F;
                    }
                }
                scncnt++;
                float ffmax = 0;
                if (SpectrumLog.Count>0)
                {
                    var SpectrumArrayRight = SpectrumLog.Pop();
                    if (SpectrumArrayRight!=null )
                    {
                        //int pico = 0;
                        //float ff = 0;
                        //for (int i = 0; i < SpectrumArrayRight.Length - 50; i += 50)
                        //{
                        //    ff = 0;
                        //    for (int j = 0; j < 50; j++)
                        //    {
                        //        ff += RawSpectr[i + j];
                        //    }
                        //    if (ff > ffmax)
                        //    {
                        //        ffmax = ff;
                        //        pico = i + 25;
                        //    }
                        //}
                        //ffmax = 0;
                        //if (ffmax>3)
                        //{
                        //    BeginInvoke(new MethodInvoker(delegate
                        //    {
                        //        if (trackBar1.Value - 1 > 0)
                        //        {
                        //            trackBar1.Value -= 1;
                        //        }
                        //    }));
                        //    if (TTRChannel != null)
                        //    {
                        //        if (TTRSound != null)
                        //        {
                        //            TTRChannel.setVolume((float)NewVolume); // no longer checking for an exception here, as it crashes the program for some reason; who knows why!
                        //        }
                        //    }
                        //    TTRUpdateSoundVolume();
                        //}
                        //if (ffmax < 1)
                        //{
                        //    BeginInvoke(new MethodInvoker(delegate
                        //    {
                        //        if (trackBar1.Value + 1<trackBar1.Maximum )
                        //        {
                        //            trackBar1.Value += 1;
                        //        }
                        //    }));
                           
                        //    TTRUpdateSoundVolume();
                        //}
                        float pik = 1F;
                        for (int i = 0; i < SpectrumArrayRight.Length; i++)
                        {
                            float ddd = (float)i / 1023F;
                            pik = pik + (ddd/3) / (pik / 2F);
                            float k = (SpectrumArrayRight[i]) * 4 * pik;
                            if (k > 0.9) { k = 0.9F; }
                            if (k > 0.0005)
                            {
                                RawSpectr[i] = (RawSpectr[i] * 1500 + k) / 1501;
                            }
                            if (k > 0.1)
                            {
                                RawSpectr[i] = (RawSpectr[i] * 100 + k) / 101;
                            }
                            if (k > 0.3)
                            {
                                RawSpectr[i] = (RawSpectr[i] * 70 + k) / 71;
                            }
                            if (k > 0.5)
                            {
                                RawSpectr[i] = (RawSpectr[i] * 30 + k) / 31;
                            }
                            if (k > 0.7)
                            {
                                RawSpectr[i] = (RawSpectr[i] * 15 + k) / 16;
                            }
                            if (k > 0.9)
                            {
                                RawSpectr[i] = (RawSpectr[i] * 7 + k) / 8;
                            }
                        }
                        float[] ffts = new float[5];
                        for (int i = 2; i < RawSpectr.Length - 3; i += 2)
                        {
                            ffts[0] = RawSpectr[i - 2] / 4F;
                            ffts[1] = RawSpectr[i + 2] / 4F;
                            ffts[2] = RawSpectr[i - 1] / 2F;
                            ffts[3] = RawSpectr[i + 1] / 2F;
                            ffts[4] = RawSpectr[i];
                            RawSpectr[i] = ffts[0] + ffts[1] + ffts[2] + ffts[3] + ffts[4];
                            RawSpectr[i] = RawSpectr[i] / 3F;
                        }  
                    }
                }
                Thread.Sleep(50);
            } while (ScanSpectrum);
        }
        int curn;
        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            PlaySelected(listBox1.SelectedIndex);
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Ortho(-1.0, 1.0, -1.0, 1.0, -1.0, 0.8);
                //GL.MatrixMode(MatrixMode.Projection);
                //GL.LoadIdentity();
                //GL.Ortho(-1.0, 1.0, -1.0, 1.0, -1.0, 1.0);
                //GL.MatrixMode(MatrixMode.Modelview);
                //GL.LoadIdentity();

                //GL.Enable(EnableCap.Texture2D);
                //GL.BindTexture(TextureTarget.Texture2D, backid);

                //GL.Begin(BeginMode.Quads);
                //GL.TexCoord2(0.0f, 1.5f); GL.Vertex2(-1.0f, -1.0f);
                //GL.TexCoord2(1.5f, 1.5f); GL.Vertex2(1.5f, -1.5f);
                //GL.TexCoord2(1.5f, 0.0f); GL.Vertex2(1.0f, 1.0f);
                //GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(-1.5f, 1.5f);
                //GL.End();
                //GL.Disable(EnableCap.Texture2D);
                //GL.ClearColor(Color.Black);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();


            try
            {
                float[] SpectrumArrayLeft;
                SpectrumArrayLeft = new float[1024];
                float[] SpectrumArrayRight;
                SpectrumArrayRight = new float[1024];
                if (TTRSound != null)
                {
                    system.getSpectrum(SpectrumArrayLeft, 1024, 0, FMOD.DSP_FFT_WINDOW.RECT);
                    system.getSpectrum(SpectrumArrayRight, 1024, 1, FMOD.DSP_FFT_WINDOW.RECT);
                    if (SpectrumLog.Count<5)
                    {
                        SpectrumLog.Push(SpectrumArrayRight);
                    }
                    double intensifier = 0.00324;//0.00324
                    System.Drawing.Color fool1 = Color.Violet;
                    System.Drawing.Color fool2 = Color.Aqua;
                    System.Drawing.Color fool3 = Color.FromArgb(((fool1.A) + (fool2.A)) / 2, ((fool1.R) + (fool2.R)) / 2, ((fool1.G) + (fool2.G)) / 2, ((fool1.B) + (fool2.B)) / 2);
                    System.Drawing.Color fool4 = Color.FromArgb(64, Color.Violet .R , Color.DarkViolet.G , Color.DarkViolet.B);
                    System.Drawing.Color fool5 = Color.FromArgb(64, Color.Black.R, Color.Black.G, Color.Black.B);
                    System.Drawing.Color fool6 = Color.Red ;
                    GL.Begin(BeginMode.Points);
                    GL.Color3(fool1);
                    GL.Vertex2(-1, 0);
                    GL.Color3(fool2);
                    GL.Vertex2(1, 0);
                    GL.End();
                    int j = -308;
                    double k = 0;
                    //int p = 0;
                    //int pmax=5;
                    //float[] f1=new float[pmax];
                    //float av;
                    int p = 0;
                    for (double i = -1; i < 1; i += intensifier)
                    {
                        j++;
                        if (j < 0)
                        {
                            //p++; if (p >= pmax) p = 0; f1[p] = SpectrumArrayLeft[j * -1] * 100; av = f1.Average();
                            k = (SpectrumArrayLeft[j * -1])*4;
                            k = k * Math.Sqrt(k+2);
                            if (k > 0.9) { k = 0.9; }
                        }
                        else
                        {
                            //p++; if (p >= pmax) p = 0; f1[p] = SpectrumArrayLeft[j] * 100; av = f1.Average();
                            k = (SpectrumArrayRight[j]) * 4;
                            k = k * Math.Sqrt(k + 2);
                            if (k > 0.9) { k = 0.9; }
                        }


                        GL.Begin(BeginMode.Lines);
                        GL.Color3(fool5);
                        GL.Vertex2(i, RawSpectr[p] );
                        GL.Color3(fool4);
                        GL.Vertex2(i, RawSpectr[p] + RawSpectr[1023 - p] + RawSpectr[p]/5);
                        GL.End();

                        GL.Begin(BeginMode.Lines);
                        GL.Color3(fool5);
                        GL.Vertex2(i, RawSpectr[p] *-1);
                        GL.Color3(fool4);
                        GL.Vertex2(i, (RawSpectr[p] + RawSpectr[1023 - p] + RawSpectr[p] / 5) * -1);
                        GL.End();

                           

                        GL.Begin(BeginMode.Lines);
                        GL.Color3(fool5);
                        GL.Vertex2(i, RawSpectr[p]*-1);
                        GL.Color3(fool4);
                        GL.Vertex2(i, k * -1);
                        GL.End();

                        GL.Begin(BeginMode.Lines);
                        GL.Color3(fool5);
                        GL.Vertex2(i, RawSpectr[p] );
                        GL.Color3(fool4);
                        GL.Vertex2(i, k );
                        GL.End();

                        GL.Begin(BeginMode.Lines);
                        GL.Color3(fool4);
                        GL.Vertex2(i, k);
                        GL.Color3(fool5);
                        GL.Vertex2(i, 0);
                        GL.End();

                        GL.Begin(BeginMode.Lines);
                        GL.Color3(fool5);
                        GL.Vertex2(i, 0);
                        GL.Color3(fool4);
                        GL.Vertex2(i, k * -1);
                        GL.End();
                        
                        p++;
                    }

                }
                glControl1.SwapBuffers();
            }
            catch
            {
                // bah
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            subForm_About = new Form4();
            subForm_About.ShowDialog();

        }

        private void listBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
            {
                e.Effect = DragDropEffects.All;
            }
        }

        int backid;
        int LoadTexture(string filename)
        {
            if (String.IsNullOrEmpty(filename))
                throw new ArgumentException(filename);

            int id = GL.GenTexture();
            backid = id;
            GL.BindTexture(TextureTarget.Texture2D, id);

            Bitmap bmp = new Bitmap(filename);
            BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

            bmp.UnlockBits(bmp_data);

            // We haven't uploaded mipmaps, so disable mipmapping (otherwise the texture will not appear).
            // On newer video cards, we can use GL.GenerateMipmaps() or GL.Ext.GenerateMipmaps() to create
            // mipmaps automatically. In that case, use TextureMinFilter.LinearMipmapLinear to enable them.
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            return id;
        }
        private void listBox1_DragDrop(object sender, DragEventArgs e)
        {
            string[] ThoseSupremeFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (String file in ThoseSupremeFiles)
            {
                if (file != string.Empty)
                {
                    if (Directory.Exists(file))
                    {
                        var files = Directory.GetFiles(file, "*.mp3", SearchOption.AllDirectories);
                        foreach (var item in files)
                        {
                            AddFileToList(item);
                            NeedToGenTGI.Push (item);
                        }
                    }
                    else
                    {
                        if (Path.GetExtension(file)==".mp3")
                        {
                            AddFileToList(file);
                            NeedToGenTGI.Push(file);
                        }
                    }
                }
            }
            MethodInvoker mi = new MethodInvoker(delegate
            {
                do
                {
                    var file = NeedToGenTGI.Pop();
                    NNH.GetBaseSign(file);
                } while (NeedToGenTGI.Count>0);
            });
            mi.BeginInvoke(null, null);
            
        }

        Stack<string> NeedToGenTGI = new Stack<string>();

        void AddFileToList(string filename)
        {
            if (Path.GetExtension(filename)==".mp3")
            {
                listBox1.Items.Add(Path.GetFileName(filename));
                listBox1b.Items.Add(filename);
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            TTRUpdateSoundVolume();

        }

        private void TTRUpdateSoundVolume()
        {
            double NewVolume = trackBar1.Value / 100.0;
            if (TTRChannel != null)
            {
                if (TTRSound != null)
                {
                    TTRChannel.setVolume((float)NewVolume); // no longer checking for an exception here, as it crashes the program for some reason; who knows why!
                }
            }
            TTRUpdateText_VolumeRow("" + NewVolume * 100 + "%");
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            FMOD.RESULT result;
            if (TTRChannel != null)
            {
                if (TTRSound != null)
                {
                    if (GAPT_LastPROCESSEDState != "Stopped")
                    {
                        if (trackBar2.Value > (TTRLength - 1))
                        {
                            trackBar2.Value = (int)(TTRLength - 1);
                        }
                        result = TTRChannel.setPosition((uint)trackBar2.Value, FMOD.TIMEUNIT.MS);
                        ERRCHECK(result);
                    }
                }
            }
        }

        private void trackBar2_MouseUp(object sender, MouseEventArgs e)
        {
            if (GAPT_FormerPlaybackState == "Playing")
            {
                TTRPauseSound();
            }
        }

        private void trackBar2_MouseDown(object sender, MouseEventArgs e)
        {

            GAPT_FormerPlaybackState = GAPT_CurrentPlaybackState;
            if (GAPT_CurrentPlaybackState == "Stopped")
            {
            }
            else if (GAPT_CurrentPlaybackState == "Paused")
            {
            }
            else
            {
                // Playing
                TTRPauseSound();
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            TTRUpdateGUICheckboxStatus(true);
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            TTRUpdateGUICheckboxStatus(true);
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            TTRUpdateGUICheckboxStatus(true);
        }

        //private void button2_Click(object sender, EventArgs e)
        //{
        //    CurrentDLSFile = "";
        //    label9.Text = Environment.SystemDirectory + "\\drivers\\gm.dls";
        //}

        //private void button3_Click(object sender, EventArgs e)
        //{
        //    dialog.Filter = "Downloadable Soundfont (*.DLS)|*.dls|All Files|*.*";
        //    dialog.Multiselect = false;

        //    if (dialog.ShowDialog() == DialogResult.OK)
        //    {
        //        if (dialog.FileName != "")
        //        {
        //            CurrentDLSFile = dialog.FileName;
        //            label9.Text = CurrentDLSFile;
        //        }
        //    }
        //}

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void alphaFormTransformer1_Paint(object sender, PaintEventArgs e)
        {

        }

        //protected override void OnPaintBackground(PaintEventArgs e) { }
        //protected override void OnClosing(CancelEventArgs e)
        //{
        //    // On XP, the speed at which the main form fades is 
        //    // distractingly slow because of its size (see my note in OnShown below),
        //    // so we just fade out the layered window frame. 
        //    // NOTE: If you have the main form with a background image,
        //    // that matches for AFT's background image (i.e., controls on top),
        //    // you'll always want to fade in/out both windows.

        //    //alphaFormTransformer1.Fade(FadeType.FadeOut, true,
        //    //  System.Environment.OSVersion.Version.Major < 6, 500);

        //    base.OnClosing(e);
        //}

        //protected override void OnShown(EventArgs e)
        //{
        //    // I'm not real pleased with the speed that XP fades in
        //    // the main form when the form itself is somewhat large like
        //    // this one. Apparently when you have a Region, changing
        //    // the layered window opacity attribute doesn't draw very fast
        //    // for large regions. So here we only fade in the layered window.
        //    // NOTE: If you have the main form with a background image,
        //    // that matches for AFT's background image (sans alpha channel),
        //    // you'll always want to fade in/out both, windows otherwise 
        //    // it will look ugly. Look at the ControlsOnTopOfSkin project
        //    // where we've added calls to Fade().

        //    //alphaFormTransformer1.Fade(FadeType.FadeIn, false,
        //    // System.Environment.OSVersion.Version.Major < 6, 400);
        //    base.OnShown(e);
        //}

        public bool shuffleCheck = true ;
        private bool DisplayID3 = true;

        private void button1_Click_1(object sender, EventArgs e)
        {
            Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ScanSpectrum = false;
            Athena.Properties.Settings.Default.Pos = this.Location;
            Athena.Properties.Settings.Default.Save();
            
            List<string> PlaylistFiles = new List<string>();
            for (int i = 0; i < listBox1b.Items.Count ; i++)
            {
                string s = listBox1b.Items[i].ToString();
                PlaylistFiles.Add(s);
            }
            File.WriteAllLines("PlaylistFiles.pls", PlaylistFiles.ToArray());

            List<string> PlaylistFiles2 = new List<string>();
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                string s = listBox1.Items[i].ToString();
                PlaylistFiles2.Add(s);
            }
            File.WriteAllLines("PlaylistNames.pls", PlaylistFiles2.ToArray());
            NNH.NET.SaveNW("Brain.db");
            NNH.Save();
            NNH.SaveLB();
            Close();
        }

        private void button_up_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Thread Lth = new Thread(DoLearn);
            Lth.Priority = ThreadPriority.Lowest;
            Lth.IsBackground = true;
            Lth.Start();

            //foreach (var item in NNH.Signes )
            //{
            //    NNH.GetNeuroData(item);
            //}
        }

        void DoLearn()
        {
            //int[] layers = new int[7];
            //layers[0] = (31 * 31);
            //layers[1] = (31 * 31);
            //layers[2] = 31 * 31;
            //layers[3] = 31 * 8;
            //layers[4] = 31 * 4;
            //layers[5] = 31 * 2;
            //layers[6] = 31;
            ////layers[0] = 1120*2;
            ////layers[1] = 31 * 1120/4;
            ////layers[2] = 31*4;
            ////layers[3] = 31*2;
            ////layers[4] = 31;
            //NNH.CreateNW(1120, layers);
            NNH.CreateLearnBook();
            NNH.LearnAll();
            DoScan();
        }

        void DoScan()
        {
            GetAllSignes();
            GetNeuroData();
        }
        void GetAllSignes()
        {
            Form frm = new Form();
            ProgressBar pb = new ProgressBar();
            frm.Controls.Add(pb);
            frm.Width = 200;
            frm.Height = 50;
            frm.TopMost = true;
            frm.Show();
            frm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            pb.Visible = true;
            pb.Dock = DockStyle.Fill;
            pb.Maximum = listBox1b.Items.Count ;
            pb.Value = 0;
            int n = 0;
            foreach (var item in listBox1b.Items)
            {
                pb.Value = n;
                frm.Invalidate();
                Application.DoEvents();
                pb.Refresh();
                Application.DoEvents();
                NNH.GetBaseSign((string)item);
                n++;
            }
            frm.Close();
        }

        BassGetSpectrum.Spectrum SpecA = new BassGetSpectrum.Spectrum();
        void GetNeuroData()
        {
            Form frm = new Form();
            ProgressBar pb = new ProgressBar();
            frm.Controls.Add(pb);
            frm.Width = 300;
            frm.Height = 50;
            frm.TopMost = true;
            frm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            pb.Visible = true;
            pb.Maximum = NNH.Signes.Count;
            pb.Value = 0;
            pb.Height = 50;
            pb.Dock = DockStyle.Fill;
            pb.Invalidate();
            Application.DoEvents();
            frm.Show();
            int n = 0;
            for (int i = 0; i < NNH.Signes.Count ; i++)
            {
                if (i%5==0)
                {
                    pb.Value = n;
                    pb.Refresh();
                    frm.Invalidate();
                    Application.DoEvents();
                }
                if (NNH.Signes[i].Spectr == null)
                {
                    NNH.Signes[i].Spectr = (GetDoubAr(SpecA.GetRawSpectrData(NNH.Signes[i].filepath, BassGetSpectrum.Spectrum.FFTSize.FFT1024, 30, 10000)));
                }
                var item = NNH.Signes[i];
                NNH.GetNeuroData(ref item);
                NNH.Signes[i].NeuroDays = item.NeuroDays;
                NNH.Signes[i].NeuroHours = item.NeuroHours;
                n++;
            }
            frm.Close();
        }

        double[] GetDoubAr(float[] data)
        {
            double[] dd = new double[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                dd[i] = data[i];
            }
            return dd;
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            byte Day = (byte)DateTime.Now.DayOfWeek;
            byte Hour = (byte)DateTime.Now.Hour;
            CurTrack.Spectr = GetDoubAr(RawSpectr);
            if (comboBox1.Text == "VeryGood")
            {
                CurTrack.Correct(NeuroHelper.BaseSign.corValues.VeryGood, Day, Hour);
            }
            if (comboBox1.Text == "Good")
            {
                CurTrack.Correct(NeuroHelper.BaseSign.corValues.Good , Day, Hour);
            }
            if (comboBox1.Text == "Norm")
            {
                CurTrack.Correct(NeuroHelper.BaseSign.corValues.Norm , Day, Hour);
            }
            if (comboBox1.Text == "Skip")
            {
                CurTrack.Correct(NeuroHelper.BaseSign.corValues.Skip , Day, Hour);
            }
            if (comboBox1.Text == "Bad")
            {
                CurTrack.Correct(NeuroHelper.BaseSign.corValues.Bad , Day, Hour);
            }
            if (comboBox1.Text == "VeryBad")
            {
                CurTrack.Correct(NeuroHelper.BaseSign.corValues.VeryBad , Day, Hour);
            }
            comboBox1.SelectedIndex = -1;
            comboBox1.Text = "OK";
            NNH.AddItemToLearnBook(CurTrack);
            MethodInvoker mi = new MethodInvoker(delegate
            {
                NNH.Save();
            });
            mi.BeginInvoke(null, null);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Thread Lth = new Thread(DoScan);
            Lth.Priority = ThreadPriority.Lowest;
            Lth.IsBackground = true;
            Lth.Start();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
