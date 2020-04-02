using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SM200Bx64
{
    internal class MMSYSERR
    {
        // Token: 0x0400000E RID: 14
        public const int NOERROR = 0;

        // Token: 0x0400000F RID: 15
        public const int ERROR = 1;

        // Token: 0x04000010 RID: 16
        public const int BADDEVICEID = 2;

        // Token: 0x04000011 RID: 17
        public const int NOTENABLED = 3;

        // Token: 0x04000012 RID: 18
        public const int ALLOCATED = 4;

        // Token: 0x04000013 RID: 19
        public const int INVALHANDLE = 5;
    }
    // Token: 0x02000004 RID: 4
    internal class WavConstants
    {
        // Token: 0x04000014 RID: 20
        public const int MM_WOM_OPEN = 955;

        // Token: 0x04000015 RID: 21
        public const int MM_WOM_CLOSE = 956;

        // Token: 0x04000016 RID: 22
        public const int MM_WOM_DONE = 957;

        // Token: 0x04000017 RID: 23
        public const int MM_WIM_OPEN = 958;

        // Token: 0x04000018 RID: 24
        public const int MM_WIM_CLOSE = 959;

        // Token: 0x04000019 RID: 25
        public const int MM_WIM_DATA = 960;

        // Token: 0x0400001A RID: 26
        public const int CALLBACK_FUNCTION = 196608;

        // Token: 0x0400001B RID: 27
        public const int WAVERR_STILLPLAYING = 33;

        // Token: 0x0400001C RID: 28
        public const int WHDR_DONE = 1;

        // Token: 0x0400001D RID: 29
        public const int WHDR_PREPARED = 2;

        // Token: 0x0400001E RID: 30
        public const int WHDR_BEGINLOOP = 4;

        // Token: 0x0400001F RID: 31
        public const int WHDR_ENDLOOP = 8;

        // Token: 0x04000020 RID: 32
        public const int WHDR_INQUEUE = 16;
    }
    // Token: 0x02000007 RID: 7
    [StructLayout(LayoutKind.Sequential)]
    internal class WAVEFORMATEX
    {
        // Token: 0x0600001D RID: 29 RVA: 0x00002650 File Offset: 0x00000850
        public override string ToString()
        {
            StringBuilder retVal = new StringBuilder();
            retVal.Append("wFormatTag: " + this.wFormatTag + "\r\n");
            retVal.Append("nChannels: " + this.nChannels + "\r\n");
            retVal.Append("nSamplesPerSec: " + this.nSamplesPerSec + "\r\n");
            retVal.Append("nAvgBytesPerSec: " + this.nAvgBytesPerSec + "\r\n");
            retVal.Append("nBlockAlign: " + this.nBlockAlign + "\r\n");
            retVal.Append("wBitsPerSample: " + this.wBitsPerSample + "\r\n");
            retVal.Append("cbSize: " + this.cbSize + "\r\n");
            return retVal.ToString();
        }

        // Token: 0x0400002B RID: 43
        public ushort wFormatTag;

        // Token: 0x0400002C RID: 44
        public ushort nChannels;

        // Token: 0x0400002D RID: 45
        public uint nSamplesPerSec;

        // Token: 0x0400002E RID: 46
        public uint nAvgBytesPerSec;

        // Token: 0x0400002F RID: 47
        public ushort nBlockAlign;

        // Token: 0x04000030 RID: 48
        public ushort wBitsPerSample;

        // Token: 0x04000031 RID: 49
        public ushort cbSize;
    }
    // Token: 0x02000005 RID: 5
    internal struct WAVEHDR
    {
        // Token: 0x04000021 RID: 33
        public IntPtr lpData;

        // Token: 0x04000022 RID: 34
        public uint dwBufferLength;

        // Token: 0x04000023 RID: 35
        public uint dwBytesRecorded;

        // Token: 0x04000024 RID: 36
        public IntPtr dwUser;

        // Token: 0x04000025 RID: 37
        public uint dwFlags;

        // Token: 0x04000026 RID: 38
        public uint dwLoops;

        // Token: 0x04000027 RID: 39
        public IntPtr lpNext;

        // Token: 0x04000028 RID: 40
        public uint reserved;
    }
    // Token: 0x02000002 RID: 2
    public class WaveOut : IDisposable
    {
        // Token: 0x17000001 RID: 1
        // (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        // (set) Token: 0x06000002 RID: 2 RVA: 0x00002058 File Offset: 0x00000258
        public WaveOut.PlayStatus Status { get; set; } = WaveOut.PlayStatus.Stop;

        // Token: 0x17000002 RID: 2
        // (get) Token: 0x06000003 RID: 3 RVA: 0x00002061 File Offset: 0x00000261
        // (set) Token: 0x06000004 RID: 4 RVA: 0x00002069 File Offset: 0x00000269
        public int m_SamplesPerSec { get; set; } = 30382;

        // Token: 0x17000003 RID: 3
        // (get) Token: 0x06000005 RID: 5 RVA: 0x00002072 File Offset: 0x00000272
        // (set) Token: 0x06000006 RID: 6 RVA: 0x0000207A File Offset: 0x0000027A
        public int m_BitsPerSample { get; set; } = 32;

        // Token: 0x17000004 RID: 4
        // (get) Token: 0x06000007 RID: 7 RVA: 0x00002083 File Offset: 0x00000283
        // (set) Token: 0x06000008 RID: 8 RVA: 0x0000208B File Offset: 0x0000028B
        public int m_Channels { get; set; } = 1;

        // Token: 0x17000005 RID: 5
        // (get) Token: 0x06000009 RID: 9 RVA: 0x00002094 File Offset: 0x00000294
        // (set) Token: 0x0600000A RID: 10 RVA: 0x0000209C File Offset: 0x0000029C
        public int m_AvgBytesPerSec { get; set; } = 0;

        // Token: 0x17000006 RID: 6
        // (get) Token: 0x0600000B RID: 11 RVA: 0x000020A5 File Offset: 0x000002A5
        // (set) Token: 0x0600000C RID: 12 RVA: 0x000020AD File Offset: 0x000002AD
        private int m_BlockSize { get; set; } = 0;

        // Token: 0x17000007 RID: 7
        // (get) Token: 0x0600000D RID: 13 RVA: 0x000020B6 File Offset: 0x000002B6
        // (set) Token: 0x0600000E RID: 14 RVA: 0x000020BE File Offset: 0x000002BE
        private int m_BytesBuffered { get; set; } = 0;

        // Token: 0x17000008 RID: 8
        // (get) Token: 0x0600000F RID: 15 RVA: 0x000020C7 File Offset: 0x000002C7
        // (set) Token: 0x06000010 RID: 16 RVA: 0x000020CF File Offset: 0x000002CF
        private int m_MinBuffer { get; set; } = 1200;

        // Token: 0x06000011 RID: 17 RVA: 0x000020D8 File Offset: 0x000002D8
        public WaveOut()
        {
            bool flag = WavOutMethods.waveOutGetNumDevs() <= 0;
            if (flag)
            {
                throw new Exception("Cannot find the your computer audio output device.");
            }
            this.m_BlockSize = this.m_Channels * (this.m_BitsPerSample / 8);
            this.m_AvgBytesPerSec = this.m_SamplesPerSec * this.m_Channels * (this.m_BitsPerSample / 8);
            this.m_pPlayItems = new List<WaveOut.PlayItem>();
            WAVEFORMATEX format = new WAVEFORMATEX();
            format.wFormatTag = 3;
            format.nChannels = (ushort)this.m_Channels;
            format.nSamplesPerSec = (uint)this.m_SamplesPerSec;
            format.nAvgBytesPerSec = (uint)this.m_AvgBytesPerSec;
            format.nBlockAlign = (ushort)this.m_BlockSize;
            format.wBitsPerSample = (ushort)this.m_BitsPerSample;
            format.cbSize = 0;
            this.m_pWaveOutProc = new waveOutProc(this.OnWaveOutProc);
            int result = WavOutMethods.waveOutOpen(out this.m_pWavDevHandle, -1, format, this.m_pWaveOutProc, 0, 196608);
            bool flag2 = result != 0;
            if (flag2)
            {
                throw new Exception("Failed to open wav device, error: " + result.ToString() + ".");
            }
        }

        // Token: 0x06000012 RID: 18 RVA: 0x00002254 File Offset: 0x00000454
        ~WaveOut()
        {
            this.Dispose();
        }

        // Token: 0x06000013 RID: 19 RVA: 0x00002284 File Offset: 0x00000484
        public void Dispose()
        {
            bool isDisposed = this.m_IsDisposed;
            if (!isDisposed)
            {
                this.m_IsDisposed = true;
                try
                {
                    WavOutMethods.waveOutReset(this.m_pWavDevHandle);
                    foreach (WaveOut.PlayItem item in this.m_pPlayItems)
                    {
                        WavOutMethods.waveOutUnprepareHeader(this.m_pWavDevHandle, item.HeaderHandle.AddrOfPinnedObject(), Marshal.SizeOf<WAVEHDR>(item.Header));
                        item.Dispose();
                    }
                    WavOutMethods.waveOutClose(this.m_pWavDevHandle);
                    this.m_pWavDevHandle = IntPtr.Zero;
                    this.m_pPlayItems = null;
                    this.m_pWaveOutProc = null;
                }
                catch
                {
                }
            }
        }

        // Token: 0x06000014 RID: 20 RVA: 0x00002360 File Offset: 0x00000560
        private void OnWaveOutProc(IntPtr hdrvr, int uMsg, int dwUser, int dwParam1, int dwParam2)
        {
            try
            {
                bool flag = uMsg == 957;
                if (flag)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(this.OnCleanUpFirstBlock));
                }
            }
            catch
            {
            }
        }

        // Token: 0x06000015 RID: 21 RVA: 0x000023A8 File Offset: 0x000005A8
        private void OnCleanUpFirstBlock(object state)
        {
            try
            {
                List<WaveOut.PlayItem> pPlayItems = this.m_pPlayItems;
                lock (pPlayItems)
                {
                    WaveOut.PlayItem item = this.m_pPlayItems[0];
                    WavOutMethods.waveOutUnprepareHeader(this.m_pWavDevHandle, item.HeaderHandle.AddrOfPinnedObject(), Marshal.SizeOf<WAVEHDR>(item.Header));
                    this.m_pPlayItems.Remove(item);
                    this.m_BytesBuffered -= item.DataSize;
                    item.Dispose();
                }
            }
            catch
            {
            }
        }

        // Token: 0x06000016 RID: 22 RVA: 0x00002458 File Offset: 0x00000658
        public void Play(float[] audioData)
        {
            int offset = 0;
            int count = audioData.Length;
            bool isDisposed = this.m_IsDisposed;
            if (isDisposed)
            {
                throw new ObjectDisposedException("WaveOut");
            }
            bool flag = audioData == null;
            if (flag)
            {
                throw new ArgumentNullException("audioData");
            }
            bool flag2 = count % this.m_BlockSize != 0;
            if (flag2)
            {
                throw new ArgumentException("Audio data is not n * BlockSize.");
            }
            float[] data = new float[count];
            Array.Copy(audioData, offset, data, 0, count);
            GCHandle dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            WAVEHDR wavHeader = new WAVEHDR
            {
                lpData = dataHandle.AddrOfPinnedObject(),
                dwBufferLength = (uint)(data.Length * 4),
                dwBytesRecorded = 0u,
                dwUser = IntPtr.Zero,
                dwFlags = 0u,
                dwLoops = 0u,
                lpNext = IntPtr.Zero,
                reserved = 0u
            };
            GCHandle headerHandle = GCHandle.Alloc(wavHeader, GCHandleType.Pinned);
            int result = WavOutMethods.waveOutPrepareHeader(this.m_pWavDevHandle, headerHandle.AddrOfPinnedObject(), Marshal.SizeOf<WAVEHDR>(wavHeader));
            bool flag3 = result == 0;
            if (flag3)
            {
                WaveOut.PlayItem item = new WaveOut.PlayItem(ref headerHandle, ref dataHandle, data.Length);
                this.m_pPlayItems.Add(item);
                this.Status = WaveOut.PlayStatus.Playing;
                this.m_BytesBuffered += data.Length;
                result = WavOutMethods.waveOutWrite(this.m_pWavDevHandle, headerHandle.AddrOfPinnedObject(), Marshal.SizeOf<WAVEHDR>(wavHeader));
            }
            else
            {
                dataHandle.Free();
                headerHandle.Free();
            }
        }

        // Token: 0x06000017 RID: 23 RVA: 0x000025CC File Offset: 0x000007CC
        public void Pause()
        {
            bool flag = WavOutMethods.waveOutPause(this.m_pWavDevHandle) != 0;
            if (flag)
            {
                throw new Exception("Unable to pause output waveform data block to device.");
            }
        }

        // Token: 0x06000018 RID: 24 RVA: 0x000025F8 File Offset: 0x000007F8
        public void Resume()
        {
            bool flag = WavOutMethods.waveOutRestart(this.m_pWavDevHandle) != 0;
            if (flag)
            {
                throw new Exception("Unable to restart suspended waveform output device.");
            }
        }

        // Token: 0x06000019 RID: 25 RVA: 0x00002624 File Offset: 0x00000824
        public void Stop()
        {
            WavOutMethods.waveOutReset(this.m_pWavDevHandle);
        }

        // Token: 0x04000009 RID: 9
        private bool m_IsPaused = false;

        // Token: 0x0400000A RID: 10
        private IntPtr m_pWavDevHandle = IntPtr.Zero;

        // Token: 0x0400000B RID: 11
        private List<WaveOut.PlayItem> m_pPlayItems = null;

        // Token: 0x0400000C RID: 12
        private waveOutProc m_pWaveOutProc = null;

        // Token: 0x0400000D RID: 13
        private bool m_IsDisposed = false;

        // Token: 0x0200000B RID: 11
        internal class PlayItem
        {
            // Token: 0x06000031 RID: 49 RVA: 0x00002767 File Offset: 0x00000967
            public PlayItem(ref GCHandle headerHandle, ref GCHandle dataHandle, int dataSize)
            {
                this.m_HeaderHandle = headerHandle;
                this.m_DataHandle = dataHandle;
                this.m_DataSize = dataSize;
            }

            // Token: 0x06000032 RID: 50 RVA: 0x00002797 File Offset: 0x00000997
            public void Dispose()
            {
                this.m_HeaderHandle.Free();
                this.m_DataHandle.Free();
            }

            // Token: 0x17000009 RID: 9
            // (get) Token: 0x06000033 RID: 51 RVA: 0x000027B4 File Offset: 0x000009B4
            public GCHandle HeaderHandle
            {
                get
                {
                    return this.m_HeaderHandle;
                }
            }

            // Token: 0x1700000A RID: 10
            // (get) Token: 0x06000034 RID: 52 RVA: 0x000027CC File Offset: 0x000009CC
            public WAVEHDR Header
            {
                get
                {
                    return (WAVEHDR)this.m_HeaderHandle.Target;
                }
            }

            // Token: 0x1700000B RID: 11
            // (get) Token: 0x06000035 RID: 53 RVA: 0x000027F0 File Offset: 0x000009F0
            public GCHandle DataHandle
            {
                get
                {
                    return this.m_DataHandle;
                }
            }

            // Token: 0x1700000C RID: 12
            // (get) Token: 0x06000036 RID: 54 RVA: 0x00002808 File Offset: 0x00000A08
            public int DataSize
            {
                get
                {
                    return this.m_DataSize;
                }
            }

            // Token: 0x0400003A RID: 58
            private GCHandle m_HeaderHandle;

            // Token: 0x0400003B RID: 59
            private GCHandle m_DataHandle;

            // Token: 0x0400003C RID: 60
            private int m_DataSize = 0;
        }

        // Token: 0x0200000C RID: 12
        public enum PlayStatus
        {
            // Token: 0x0400003E RID: 62
            Playing = 1,
            // Token: 0x0400003F RID: 63
            Pause,
            // Token: 0x04000040 RID: 64
            Stop
        }
    }
    // Token: 0x02000008 RID: 8
    internal struct WAVEOUTCAPS
    {
        // Token: 0x04000032 RID: 50
        public ushort wMid;

        // Token: 0x04000033 RID: 51
        public ushort wPid;

        // Token: 0x04000034 RID: 52
        public uint vDriverVersion;

        // Token: 0x04000035 RID: 53
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string szPname;

        // Token: 0x04000036 RID: 54
        public uint dwFormats;

        // Token: 0x04000037 RID: 55
        public ushort wChannels;

        // Token: 0x04000038 RID: 56
        public ushort wReserved1;

        // Token: 0x04000039 RID: 57
        public uint dwSupport;
    }
    // Token: 0x02000009 RID: 9
    // (Invoke) Token: 0x06000020 RID: 32
    internal delegate void waveOutProc(IntPtr hdrvr, int uMsg, int dwUser, int dwParam1, int dwParam2);
    // Token: 0x02000006 RID: 6
    internal class WavFormat
    {
        // Token: 0x04000029 RID: 41
        public const int PCM = 1;

        // Token: 0x0400002A RID: 42
        public const int IEEE_FLOAT = 3;
    }
    // Token: 0x0200000A RID: 10
    internal class WavOutMethods
    {
        // Token: 0x06000023 RID: 35
        [DllImport("winmm.dll")]
        public static extern int waveOutClose(IntPtr hWaveOut);

        // Token: 0x06000024 RID: 36
        [DllImport("winmm.dll")]
        public static extern uint waveOutGetDevCaps(uint hwo, ref WAVEOUTCAPS pwoc, int cbwoc);

        // Token: 0x06000025 RID: 37
        [DllImport("winmm.dll")]
        public static extern int waveOutGetNumDevs();

        // Token: 0x06000026 RID: 38
        [DllImport("winmm.dll")]
        public static extern int waveOutGetPosition(IntPtr hWaveOut, out int lpInfo, int uSize);

        // Token: 0x06000027 RID: 39
        [DllImport("winmm.dll")]
        public static extern int waveOutGetVolume(IntPtr hWaveOut, out int dwVolume);

        // Token: 0x06000028 RID: 40
        [DllImport("winmm.dll")]
        public static extern int waveOutOpen(out IntPtr hWaveOut, int uDeviceID, WAVEFORMATEX lpFormat, waveOutProc dwCallback, int dwInstance, int dwFlags);

        // Token: 0x06000029 RID: 41
        [DllImport("winmm.dll")]
        public static extern int waveOutPause(IntPtr hWaveOut);

        // Token: 0x0600002A RID: 42
        [DllImport("winmm.dll")]
        public static extern int waveOutPrepareHeader(IntPtr hWaveOut, IntPtr lpWaveOutHdr, int uSize);

        // Token: 0x0600002B RID: 43
        [DllImport("winmm.dll")]
        public static extern int waveOutReset(IntPtr hWaveOut);

        // Token: 0x0600002C RID: 44
        [DllImport("winmm.dll")]
        public static extern int waveOutRestart(IntPtr hWaveOut);

        // Token: 0x0600002D RID: 45
        [DllImport("winmm.dll")]
        public static extern int waveOutSetVolume(IntPtr hWaveOut, int dwVolume);

        // Token: 0x0600002E RID: 46
        [DllImport("winmm.dll")]
        public static extern int waveOutUnprepareHeader(IntPtr hWaveOut, IntPtr lpWaveOutHdr, int uSize);

        // Token: 0x0600002F RID: 47
        [DllImport("winmm.dll")]
        public static extern int waveOutWrite(IntPtr hWaveOut, IntPtr lpWaveOutHdr, int uSize);
    }
}
