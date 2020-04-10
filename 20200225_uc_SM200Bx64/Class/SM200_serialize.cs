using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM200Bx64.Class
{

    [Serializable]
    class IQData
    {
        public IQData() { }
        public IQData(int attenuator, double refLevel, double centerF, int sampleRate, long bandwidth
            , double captureSize, float[] IQR_32, short[] IQR_16, long IQT, int sampleLoss, int sampleRemaining)
        {
            Attenuator = attenuator;
            RefLevel = refLevel;
            CenterF = centerF;
            SampleRate = sampleRate;
            BandWidth = bandwidth;
            CaptureSize = captureSize;
            IQResult_32f = IQR_32;
            IQResult_16s = IQR_16;
            IQTime = IQT;
            SampleLoss = sampleLoss;
            SampleRemaining = sampleRemaining;
        }
        [NonSerialized]
        /// <summary>
        /// 范围[-1,6]分别为：自动与[0,30]dB,其中-1为自动。
        /// </summary>
        public int Attenuator;

        /// <summary>
        /// 参考电平,单位dBm
        /// </summary>
        public double RefLevel;

        /// <summary>
        /// 中心频率,单位Hz
        /// </summary>
        public double CenterF;

        /// <summary>
        /// 采样率,单位Sa/s
        /// </summary>
        public double SampleRate;

        /// <summary>
        /// 带宽,单位Hz
        /// </summary>
        public long BandWidth;

        /// <summary>
        /// 捕获大小,单位ms
        /// </summary>
        public double CaptureSize;

        /// <summary>
        /// IQ结果，32float型
        /// </summary>
        public float[] IQResult_32f;

        /// <summary>
        /// IQ结果，16short型
        /// </summary>
        public short[] IQResult_16s;

        /// <summary>
        /// IQ结果的时间,格林威治时的微秒
        /// </summary>
        public long IQTime;

        public int SampleLoss;

        public int SampleRemaining;
    }

    [Serializable]
    class SweepData
    {
        public SweepData() { }
        public SweepData(int attenuator, double refLevel, double startF, int binSize, double rbw, double vbw,
            float[] srMax,float[] srMin,long sweepT,SmDetector detector,SmSweepSpeed sp,SmWindowType wt,
            SmVideoUnits vu,SmScale scale)
        {
            Attenuator = attenuator;
            RefLevel = refLevel;
            StartF = startF;
            BinSize = binSize;
            RBW = rbw;
            VBW = vbw;
            SweepResultMax = srMax;
            SweepResultMin = srMin;
            SweepTime = sweepT;
            SweepScale = scale;
            SweepWindowsType = wt;
            Detector = detector;
            VideoUnits = vu;
            SweepSpeed = sp;
        }
        [NonSerialized]
        /// <summary>
        /// 范围[-1,6]分别为：自动与[0,30]dB,其中-1为自动。
        /// </summary>
        public int Attenuator;

        /// <summary>
        /// 参考电平,单位dBm
        /// </summary>
        public double RefLevel;

        /// <summary>
        /// 起始频率,单位Hz
        /// </summary>
        public double StartF;

        /// <summary>
        /// 终止频率,单位Hz
        /// </summary>
        public int BinSize;

        /// <summary>
        /// RBW,单位Hz
        /// </summary>
        public double RBW;

        /// <summary>
        /// VBW,单位Hz
        /// </summary>
        public double VBW;

        /// <summary>
        /// 返回结果大值
        /// </summary>
        public float[] SweepResultMax;

        /// <summary>
        /// 返回结果小值
        /// </summary>
        public float[] SweepResultMin;

        /// <summary>
        /// 扫频结果的时间,格林威治时的微秒
        /// </summary>
        public long SweepTime;

        /// <summary>
        /// 返回单位
        /// </summary>
        public SmScale SweepScale;

        /// <summary>
        /// 窗口函数Specify the FFT window function
        /// </summary>
        public SmWindowType SweepWindowsType;

        /// <summary>
        /// 检波器
        /// </summary>
        public SmDetector Detector;

        /// <summary>
        /// 视频单位
        /// </summary>
        public SmVideoUnits VideoUnits;

        /// <summary>
        /// 扫描速度
        /// </summary>
        public SmSweepSpeed SweepSpeed;
    }


}
