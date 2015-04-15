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


namespace Athena
{
    public partial class Form1 : Form
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

        public Form1()
        {

            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            DateTime buildDate = (new DateTime(2000, 1, 1)).AddDays(version.Build).AddSeconds(version.Revision * 2);

            InitializeComponent();
            label4.Text = "Built on " + buildDate.ToString();

            dialog.Filter = DefaultOpenDialogFilter;
            dialog.InitialDirectory = GAPT_LastDirectoryName; dialog.Title = "Open";
            dialog.Multiselect = true;

            label9.Text = Environment.SystemDirectory + "\\drivers\\gm.dls";

        }

        public delegate void DFCADelegate();

        private void ERRCHECK(FMOD.RESULT result)
        {
            if (result != FMOD.RESULT.OK)
            {
                MessageBox(new IntPtr(0), "Exception Raised: " + result + Environment.NewLine + FMOD.Error.String(result), "GSA Exception Trapper", MB_OK | ICON_STOP);
                Environment.Exit(-1);
            }
        }

        protected void TTRFetchTrackFilesize()
        {

            if (radioButton3.Checked)
            {
                GAPT_CurrentFileSize = "HTTP Stream";
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

                if (radioButton3.Checked)
                {
                    result = system.createSound(Argument0, FMOD.MODE.SOFTWARE | FMOD.MODE._2D | FMOD.MODE.CREATESTREAM | FMOD.MODE.NONBLOCKING, ref ex, ref TTRSound);
                }
                else
                {
                    result = system.createSound(Argument0, FMOD.MODE.SOFTWARE | FMOD.MODE._2D | FMOD.MODE.CREATESTREAM, ref ex, ref TTRSound);
                }

                ERRCHECK(result);

                while (radioButton3.Checked)
                {
                    result = TTRSound.getOpenState(ref a, ref b, ref c);
                    ERRCHECK(result);

                    if (a == FMOD.OPENSTATE.READY && TTRChannel == null)
                    {
                        break;
                    }

                }

                result = TTRSound.setLoopPoints(GAPT_CurrentLoopStart, FMOD.TIMEUNIT.MS, GAPT_CurrentLoopDone, FMOD.TIMEUNIT.MS);
                ERRCHECK(result);

                if (radioButton2.Checked)
                {
                    result = TTRSound.setMode(FMOD.MODE.LOOP_NORMAL | FMOD.MODE.ACCURATETIME);
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

                if (checkBox2.Checked)
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
                                if (radioButton3.Checked)
                                {
                                    oogimaflip_001 = "Online Broadcasting";
                                }
                                else
                                {
                                    oogimaflip_001 = Path.GetFileName(Argument0);
                                }
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

                    if (radioButton3.Checked)
                    {
                        GAPT_CurrentTrackTitle = "Ginever Radio: " + oogimaflip_002.Replace("&", "&&") + oogimaflip_001.Replace("&", "&&");
                    }
                    else
                    {
                        GAPT_CurrentTrackTitle = oogimaflip_002.Replace("&", "&&") + oogimaflip_001.Replace("&", "&&");
                    }

                }
                else
                {
                    if (radioButton3.Checked)
                    {
                        GAPT_CurrentTrackTitle = "Ginever Radio";
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
                MessageBox(new IntPtr(0), "Non-critical Exception Raised: GAPI_PLAY_FAILURE" + Environment.NewLine + "Failed to play requested track.", "GSA Exception Trapper", MB_OK | ICON_INFORMATION);
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
            groupBox4.Invoke(new UpdateTextCallback(this.TTRUpdateText_VolTab),
            new object[] { str });
        }

        public void TTRUpdateText_VolTab(string str)
        {
            groupBox4.Text = str;
        }

        public void TTRUpdateTrackbar_Sub(string str)
        {
            trackBar2.Invoke(new UpdateTextCallback(this.TTRUpdateTrackbar),
                           new object[] { str });
        }

        public void TTRUpdateTrackbar(string str)
        {
            if (radioButton3.Checked)
            {
                trackBar2.Enabled = false;
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
				else if (radioButton3.Checked){
					TTRUpdateGUICheckboxStatus(false);
					result = TTRChannel.setPaused(false);
                    ERRCHECK(result);
                    GAPT_CurrentPlaybackState = "Playing";
				}
                else if (GAPT_LastTrackName != string.Empty)
                {
                    TTRPlaySound(GAPT_LastTrackName, GAPT_CurrentLoopStart, GAPT_CurrentLoopDone, false);
                }
            }
            else
            {
                // Attempted fix for issue #94 "Play from Playlist with Play button?" reported by Trevor Sparks

                if (radioButton2.Checked == false)
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

                    if (radioButton2.Checked)
                    {
                        subForm_LoopStart = new Form2();
                        if (subForm_LoopStart.ShowDialog() == DialogResult.OK)
                        {
                            if (subForm_LoopStart.textBox1.Text.ToString() == "")
                            {
                                GAPT_CurrentLoopStart = 0;
                            }
                            else
                            {
                                try
                                {
                                    GAPT_CurrentLoopStart = Convert.ToUInt32(subForm_LoopStart.textBox1.Text.ToString());
                                    if (GAPT_CurrentLoopStart > TTRLength)
                                    {
                                        GAPT_CurrentLoopStart = 0;
                                    }
                                    else if (GAPT_CurrentLoopStart < 0)
                                    {
                                        GAPT_CurrentLoopStart = 0;
                                    }
                                }
                                catch
                                {
                                    MessageBox(new IntPtr(0), "Non-critical Exception Raised: GAPI_DIALOG1_EXCEPTION" + Environment.NewLine + "Sorry, that failed.", "GSA Exception Trapper", MB_OK | ICON_STOP);
                                    GAPT_CurrentLoopStart = 0;
                                }
                            }

                        }

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
                    if(radioButton3.Checked == false) try
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
                        if (radioButton1.Checked)
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
                                            while (newTrackPlayingEntry == TTRTrackPlayingEntryID)
                                            {
                                                TTRTrackPlayingEntryID = rand.Next(listBox1b.Items.Count);
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
                                        TTRPlaySound(listBox1b.Items[TTRTrackPlayingEntryID].ToString(), 0, 0, false);
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        MessageBox(new IntPtr(0), "Non-critical Exception Raised: GAPI_INCREMENT_FAILURE" + Environment.NewLine + "Track incrementation failed.", "GSA Exception Trapper", MB_OK | ICON_INFORMATION);
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
                    if (radioButton2.Checked)
                    {
                        if (GAPT_LastPROCESSEDState == "Playing")
                        {
                            TTRUpdateText_TopTab_Sub("[Looping: " + GAPT_CurrentLoopStart.ToString() + " - " + GAPT_CurrentLoopDone.ToString() + "]");
                            TTRUpdateText_VolTab_Sub("[" + TTRPosition.ToString() + "/" + TTRLength.ToString() + "]");
                        }
                        else
                        {
                            TTRUpdateText_TopTab_Sub("[" + GAPT_LastPROCESSEDState + ": " + GAPT_CurrentLoopStart.ToString() + " - " + GAPT_CurrentLoopDone.ToString() + "]");
                            TTRUpdateText_VolTab_Sub("[" + TTRPosition.ToString() + "/" + TTRLength.ToString() + "]");
                        }
                    }
                    else if (radioButton1.Checked)
                    {
                        TTRUpdateText_TopTab_Sub("[" + GAPT_LastPROCESSEDState + "]");
                        TTRUpdateText_VolTab_Sub("[" + TTRPosition.ToString() + "/" + TTRLength.ToString() + "]");
                    }
                    else if (radioButton3.Checked)
                    {
                        result = TTRSound.getOpenState(ref a, ref b, ref c);
                        ERRCHECK(result);
                        TTRUpdateText_TopTab_Sub("[" + GAPT_LastPROCESSEDState + "]");
                        TTRUpdateText_VolTab_Sub((a == FMOD.OPENSTATE.BUFFERING ? "Buffering..." : (a == FMOD.OPENSTATE.CONNECTING ? "Connecting..." : "Connected")) + " (Buffer: " + b + "%)" + (c ? " STARVING" : "        "));
                    }
                }
                catch
                {
                   // MessageBox(new IntPtr(0), "Non-critical Exception Raised: GAPI_GUIUPDATE1_FAILURE" + Environment.NewLine + "Failed to update GUI state.", "GSA Exception Trapper", MB_OK | ICON_INFORMATION);
                }
                if(radioButton3.Checked == false) try
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

        private void button_playpause_Click(object sender, EventArgs e)
        {
                TTRPauseSound();
        }

        private void button_stop_Click(object sender, EventArgs e)
        {
            TTRStopSound();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

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
            if (radioButton3.Checked)
            {
                if (TTRSound != null)
                {
                    result = TTRSound.setLoopPoints(0, FMOD.TIMEUNIT.MS, 0, FMOD.TIMEUNIT.MS);
                    ERRCHECK(result);
                    result = TTRSound.setMode(FMOD.MODE.LOOP_OFF | FMOD.MODE.ACCURATETIME);
                    ERRCHECK(result);
                }
                groupBox1.Enabled = false;
                button_up.Enabled = false;
                button_down.Enabled = false;
                button_add.Enabled = false;
                button_remove.Enabled = false;
                listBox1.Enabled = false;
                button_of_fun.Enabled = false;
                button_prev.Image = Athena.Properties.Resources.prev;
                button_next.Image = Athena.Properties.Resources.next;
                button_prev.Enabled = false;
                button_next.Enabled = false;
                button_restart.Enabled = false;
                trackBar2.Value = 0;
                trackBar2.Enabled = false;

                TTRTrackPlayingEntryID = -1;
                TTRPlaySound("http://athena-radio-link.ginever.net:8000/", GAPT_CurrentLoopStart, GAPT_CurrentLoopDone, true);
                if (q) { TTRStopSound(); }

            }
            else if (radioButton2.Checked)
            {
                if (TTRSound != null)
                {
                    result = TTRSound.setLoopPoints(GAPT_CurrentLoopStart, FMOD.TIMEUNIT.MS, GAPT_CurrentLoopDone, FMOD.TIMEUNIT.MS);
                    ERRCHECK(result);
                    result = TTRSound.setMode(FMOD.MODE.LOOP_NORMAL | FMOD.MODE.ACCURATETIME);
                    ERRCHECK(result);
                }
                groupBox1.Enabled = false;
                button_up.Enabled = false;
                button_down.Enabled = false;
                button_add.Enabled = false;
                button_remove.Enabled = false;
                listBox1.Enabled = false;
                button_of_fun.Enabled = true;
                button_prev.Image = Athena.Properties.Resources.loop1;
                button_next.Image = Athena.Properties.Resources.loop2;
                button_prev.Enabled = true;
                button_next.Enabled = true;
                button_restart.Enabled = true;
            }
            else if (radioButton1.Checked)
            {
                if (TTRSound != null)
                {
                    result = TTRSound.setLoopPoints(0, FMOD.TIMEUNIT.MS, 0, FMOD.TIMEUNIT.MS);
                    ERRCHECK(result);
                    result = TTRSound.setMode(FMOD.MODE.LOOP_OFF | FMOD.MODE.ACCURATETIME);
                    ERRCHECK(result);
                }
                groupBox1.Enabled = true;
                button_up.Enabled = true;
                button_down.Enabled = true;
                button_add.Enabled = true;
                button_remove.Enabled = true;
                listBox1.Enabled = true;
                button_of_fun.Enabled = false;
                button_prev.Image = Athena.Properties.Resources.prev;
                button_next.Image = Athena.Properties.Resources.next;
                button_prev.Enabled = true;
                button_next.Enabled = true;
                button_restart.Enabled = true;
            }
        }

        private void button_prev_Click(object sender, EventArgs e)
        {
            //FMOD.RESULT result;
            if (radioButton2.Checked)
            {
                if (GAPT_LastTrackName == string.Empty)
                {
                    return;
                }
                subForm_LoopStart = new Form2();
                if (subForm_LoopStart.ShowDialog() == DialogResult.OK)
                {
                    if (subForm_LoopStart.textBox1.Text.ToString() == "")
                    {
                        try
                        {
                            GAPT_CurrentLoopStart = 0;
                            TTRPlaySound(GAPT_LastTrackName, GAPT_CurrentLoopStart, GAPT_CurrentLoopDone, false);
                        }
                        catch
                        {
                            MessageBox(new IntPtr(0), "Non-critical Exception Raised: GAPI_RAD2A_FAILURE" + Environment.NewLine + "Sorry, that failed.", "GSA Exception Trapper", MB_OK | ICON_STOP);
                        }
                    }
                    else
                    {
                        try
                        {
                            GAPT_CurrentLoopStart = Convert.ToUInt32(subForm_LoopStart.textBox1.Text.ToString());
                            if (GAPT_CurrentLoopStart > TTRLength)
                            {
                                GAPT_CurrentLoopStart = 0;
                            }
                            else if (GAPT_CurrentLoopStart < 0)
                            {
                                GAPT_CurrentLoopStart = 0;
                            }
                            TTRPlaySound(GAPT_LastTrackName, GAPT_CurrentLoopStart, GAPT_CurrentLoopDone, false);
                        }
                        catch
                        {
                            MessageBox(new IntPtr(0), "Non-critical Exception Raised: GAPI_RAD2B_FAILURE" + Environment.NewLine + "Sorry, that failed.", "GSA Exception Trapper", MB_OK | ICON_STOP);
                        }
                    }

                }

            }
            else
            {
            }
        }

        private void button_next_Click(object sender, EventArgs e)
        {
            //FMOD.RESULT result;
            if (radioButton2.Checked)
            {
                if (GAPT_LastTrackName == string.Empty)
                {
                    return;
                }
                subForm_LoopEnd = new Form3();
                if (subForm_LoopEnd.ShowDialog() == DialogResult.OK)
                {
                    if (subForm_LoopEnd.textBox1.Text.ToString() == "")
                    {
                        try
                        {
                            GAPT_CurrentLoopDone = 0;
                            TTRPlaySound(GAPT_LastTrackName, GAPT_CurrentLoopStart, GAPT_CurrentLoopDone, false);
                        }
                        catch
                        {
                            MessageBox(new IntPtr(0), "Non-critical Exception Raised: GAPI_RAD2C_FAILURE" + Environment.NewLine + "Sorry, that failed.", "GSA Exception Trapper", MB_OK | ICON_STOP);
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
                            TTRPlaySound(GAPT_LastTrackName, GAPT_CurrentLoopStart, GAPT_CurrentLoopDone, false);
                        }
                        catch
                        {
                            MessageBox(new IntPtr(0), "Non-critical Exception Raised: GAPI_RAD2D_FAILURE" + Environment.NewLine + "Sorry, that failed.", "GSA Exception Trapper", MB_OK | ICON_STOP);
                        }
                    }

                }

            }
            else
            {
            }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                listBox1b.SelectedIndex = listBox1.SelectedIndex;
                TTRTrackPlayingEntryID = listBox1.SelectedIndex;
                // MessageBox(new IntPtr(0), Non-critical Exception Raised: GAPI_DEBUGGER" + Environment.NewLine + listBox1b.SelectedItem.ToString(), "GSA Exception Trapper", MB_OK | ICON_STOP);
                TTRPlaySound(listBox1b.SelectedItem.ToString(), 0, 0, false);

            }
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

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
                    double intensifier = 0.00324;
                    System.Drawing.Color fool1 = Color.Blue;
                    System.Drawing.Color fool2 = Color.Aqua;
                    System.Drawing.Color fool3 = Color.FromArgb(((fool1.A) + (fool2.A)) / 2, ((fool1.R) + (fool2.R)) / 2, ((fool1.G) + (fool2.G)) / 2, ((fool1.B) + (fool2.B)) / 2);
                    GL.Begin(BeginMode.Lines);
                    GL.Color3(fool3);
                    GL.Vertex2(-1, 0);
                    GL.Color3(fool3);
                    GL.Vertex2(1, 0);
                    GL.End();
                    int j = -308;
                    double k = 0;
                    for (double i = -1; i < 1; i += intensifier)
                    {
                        j++;
                        if (j < 0)
                        {
                            k = (SpectrumArrayLeft[j * -1]) * 2;
                            if (k > 0.9) { k = 0.9; }
                        }
                        else
                        {
                            k = (SpectrumArrayRight[j]) * 2;
                            if (k > 0.9) { k = 0.9; }
                        }
                        GL.Begin(BeginMode.Lines);
                        GL.Color3(fool1);
                        GL.Vertex2(i, k);
                        GL.Color3(fool2);
                        GL.Vertex2(i, k * -1);
                        GL.End();
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

        private void listBox1_DragDrop(object sender, DragEventArgs e)
        {
            string[] ThoseSupremeFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (String file in ThoseSupremeFiles)
            {
                if (file != string.Empty)
                {
                    listBox1.Items.Add(Path.GetFileName(file));
                    listBox1b.Items.Add(file);
                }
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

        private void button2_Click(object sender, EventArgs e)
        {
            CurrentDLSFile = "";
            label9.Text = Environment.SystemDirectory + "\\drivers\\gm.dls";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            dialog.Filter = "Downloadable Soundfont (*.DLS)|*.dls|All Files|*.*";
            dialog.Multiselect = false;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if(dialog.FileName != "")
                {
                    CurrentDLSFile = dialog.FileName;
                    label9.Text = CurrentDLSFile;
                }
            }
        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        public bool shuffleCheck { get; set; }
    }
}
