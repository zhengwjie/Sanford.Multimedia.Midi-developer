using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Sanford.Multimedia.Midi;
using Sanford.Multimedia.Midi.UI;
namespace SequencerDemo
{
    public partial class Form1 : Form
    {
        private bool scrolling = false;

        private bool playing = false;

        private bool closing = false;

        private OutputDevice outDevice;

        private int outDeviceID = 0;
        public delegate void playNextDelegate();
        private OutputDeviceDialog outDialog = new OutputDeviceDialog();
        //声明一个list,存储列表中的路径
        List<string> midiList = new List<string>();
        private float x;
        private float y;
        public Form1()
        {
            try
            {
                InitializeComponent();
                x = this.Width;
                y = this.Height;
                setTag(this);
            }
            catch (Exception e)
            {
                MessageBox.Show("运行错误！！");
            }
        }
        private void setTag(Control cons)
        {
            foreach (Control con in cons.Controls)
            {
                con.Tag = con.Width + ";" + con.Height + ";" + con.Left + ";" + con.Top + ";" + con.Font.Size;
                if (con.Controls.Count > 0)
                {
                    setTag(con);
                }
            }
        }
        private void setControls(float newx, float newy, Control cons)
        {
            foreach (Control con in cons.Controls)
            {
                if (con.Tag != null)
                {
                    String[] myTag = con.Tag.ToString().Split(new char[] { ';' });
                    con.Width = Convert.ToInt32(System.Convert.ToSingle(myTag[0]) * newx);
                    con.Height = Convert.ToInt32(System.Convert.ToSingle(myTag[1]) * newy);
                    con.Left = Convert.ToInt32(System.Convert.ToSingle(myTag[2]) * newx);
                    con.Top = Convert.ToInt32(System.Convert.ToSingle(myTag[3]) * newy);
                    Single currentSize = System.Convert.ToSingle(myTag[4]) * newy;
                    con.Font = new Font(con.Font.Name, currentSize, con.Font.Style, con.Font.Unit);
                    if (con.Controls.Count > 0)
                        setControls(newx, newy, con);
                }
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            if(OutputDevice.DeviceCount == 0)
            {
                MessageBox.Show("No MIDI output devices available.", "Error!",
                    MessageBoxButtons.OK, MessageBoxIcon.Stop);

                Close();
            }
            else
            {
                try
                {
                    outDevice = new OutputDevice(outDeviceID);

                    sequence1.LoadProgressChanged += HandleLoadProgressChanged;
                    sequence1.LoadCompleted += HandleLoadCompleted;
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error!",
                        MessageBoxButtons.OK, MessageBoxIcon.Stop);

                    Close();
                }
            }

            base.OnLoad(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            pianoControl1.PressPianoKey(e.KeyCode);

            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            pianoControl1.ReleasePianoKey(e.KeyCode);

            base.OnKeyUp(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            closing = true;

            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            sequence1.Dispose();

            if(outDevice != null)
            {
                outDevice.Dispose();
            }

            outDialog.Dispose();

            base.OnClosed(e);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openMidiFileDialog.Title = "请选择midi文件";
            
            //让选择器多选文件
            openMidiFileDialog.Multiselect = true;
            if(openMidiFileDialog.ShowDialog() == DialogResult.OK)
            {
                string[] fileNames = openMidiFileDialog.FileNames;
                foreach(string fileName in fileNames)
                {
                    
                    string file = Path.GetFileNameWithoutExtension(fileName);
                    bool exists=false;
                    foreach(string str in listBox1.Items)
                    {
                        if (str.Equals(file))
                            exists = true;
                    }
                    if (exists)
                        continue;
                    listBox1.Items.Add(file);
                    midiList.Add(Path.GetFullPath(fileName));
                }
                // string fileName = openMidiFileDialog.FileName;
                // Open(fileName);
            }
        }

        public void Open(string fileName)
        {
            try
            {
                sequencer1.Stop();
                playing = false;
                sequence1.LoadAsync(fileName);
                this.Cursor = Cursors.WaitCursor;
                startButton.Enabled = false;
                continueButton.Enabled = false;
                stopButton.Enabled = false;
                openToolStripMenuItem.Enabled = false;
                //MessageBox.Show(Thread.CurrentThread.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void outputDeviceToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutDialog dlg = new AboutDialog();

            dlg.ShowDialog();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            try
            {
                playing = false;
                sequencer1.Stop();
                timer1.Stop();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }
        public void start_Play()
        {
            try
            {
                playing = true;
                sequencer1.Start();
                timer1.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }
        private void startButton_Click(object sender, EventArgs e)
        {  if (listBox1.SelectedIndex == -1)
                listBox1.SelectedIndex = 0;
            try
            {
                playing = true;
                sequencer1.Start();
                timer1.Start();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            try
            {
                playing = true;
                sequencer1.Continue();
                timer1.Start();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void positionHScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            if(e.Type == ScrollEventType.EndScroll)
            {
                sequencer1.Position = e.NewValue;

                scrolling = false;
            }
            else
            {
                scrolling = true;
            }
        }

        private void HandleLoadProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            toolStripProgressBar1.Value = e.ProgressPercentage;
        }

        private void HandleLoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
            startButton.Enabled = true;
            continueButton.Enabled = true;
            stopButton.Enabled = true;
            openToolStripMenuItem.Enabled = true;
            toolStripProgressBar1.Value = 0;
            try
            {
                //sequencer1.Stop();
                sequencer1.Start();
                timer1.Start();
                playing = true;

            }catch(Exception e1)
            {
                MessageBox.Show("错误");
            }
            if(e.Error == null)
            {
                positionHScrollBar.Value = 0;
                positionHScrollBar.Maximum = sequence1.GetLength();
            }
            else
            {
                MessageBox.Show(e.Error.Message);
            }
        }

        private void HandleChannelMessagePlayed(object sender, ChannelMessageEventArgs e)
        {
            if(closing)
            {
                return;
            }

            outDevice.Send(e.Message);
            pianoControl1.Send(e.Message);
        }

        private void HandleChased(object sender, ChasedEventArgs e)
        {
            foreach(ChannelMessage message in e.Messages)
            {
                outDevice.Send(message);
            }
        }

        private void HandleSysExMessagePlayed(object sender, SysExMessageEventArgs e)
        {
       //     outDevice.Send(e.Message); Sometimes causes an exception to be thrown because the output device is overloaded.
        }

        private void HandleStopped(object sender, StoppedEventArgs e)
        {
            foreach(ChannelMessage message in e.Messages)
            {
                outDevice.Send(message);
                pianoControl1.Send(message);
            }
        }
        public void Random_play()
        {
            
            Random random = new Random();
            int k = Math.Abs(random.Next()) % midiList.Count;
            listBox1.SelectedIndex = k;
        }
        private void HandlePlayingCompleted(object sender, EventArgs e)
        {
            sequencer1.Stop();
            timer1.Stop();
            playing = false;
            if (radioButton3.Checked)
            {
                BeginInvoke(new playNextDelegate(this.start_Play));
            }
            else if (radioButton1.Checked)
            {
                BeginInvoke(new playNextDelegate(playNext));
            }
            else if (radioButton2.Checked)
            {
                BeginInvoke(new playNextDelegate(Random_play));
            }
            else
            {
                
                BeginInvoke(new playNextDelegate(playNext));
            }

            //start_Play();
            //timer1.Start();
        }

        private void pianoControl1_PianoKeyDown(object sender, PianoKeyEventArgs e)
        {
            #region Guard

            if(playing)
            {
                return;
            }

            #endregion

            outDevice.Send(new ChannelMessage(ChannelCommand.NoteOn, 0, e.NoteID, 127));
        }

        private void pianoControl1_PianoKeyUp(object sender, PianoKeyEventArgs e)
        {
            #region Guard

            if(playing)
            {
                return;
            }

            #endregion

            outDevice.Send(new ChannelMessage(ChannelCommand.NoteOff, 0, e.NoteID, 0));
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(!scrolling)
            {
                positionHScrollBar.Value = Math.Min(sequencer1.Position, positionHScrollBar.Maximum);
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {

                float newx = (this.Width) / x;
                float newy = this.Height / y;
                setControls(newx, newy, this);

        }

        private void Select_Midi_File(object sender, EventArgs e)
        {
            try
            {
                timer1.Stop();
                sequencer1.Stop();
                string fileName = midiList[listBox1.SelectedIndex];
                Open(fileName);
                //BeginInvoke(new startPlay(this.start_Play));
            }catch(Exception e1)
            {
                MessageBox.Show("加载选中文件时出错");
            }
        }

        private void last_Click(object sender, EventArgs e)
        {
            try
            {
                sequencer1.Stop();
                timer1.Stop();
                int selectIndex = listBox1.SelectedIndex == 0 ? listBox1.Items.Count - 1 : listBox1.SelectedIndex - 1;
                listBox1.SelectedIndex = selectIndex;
                //timer1.Stop();
                //start_Play();
               // timer1.Start();
            }catch(Exception e1)
            {
                MessageBox.Show("加载下上一曲时出错");
            }

        }

        private void Next_Click(object sender, EventArgs e)
        {
            try
            {
                sequencer1.Stop();
                timer1.Stop();
                int selectIndex = listBox1.SelectedIndex == listBox1.Items.Count - 1 ? 0 : listBox1.SelectedIndex + 1;
                listBox1.SelectedIndex = selectIndex;
                //timer1.Stop();
                //start_Play();
                //timer1.Start();
            }catch(Exception e1)
            {
                MessageBox.Show("加载下一曲时出错");
            }

        }
        public void playNext()
        {
            try
            {
                sequencer1.Stop();
                timer1.Stop();
                playing = false;
                int selectIndex = listBox1.SelectedIndex == listBox1.Items.Count - 1 ? 0 : listBox1.SelectedIndex + 1;
                listBox1.SelectedIndex = selectIndex;
                start_Play();
                timer1.Start();
                playing = true;
            }
            catch (Exception e1)
            {
                MessageBox.Show("加载下一曲时出错");
            }
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            
        }
    }
}