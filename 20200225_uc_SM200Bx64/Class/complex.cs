using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM200Bx64
{
    /// <summary>
    /// 快速傅里叶变换
    /// </summary>
    public class FFT
    {
        // <summary>
        /// 对给定的序列进行指定长度的离散傅里叶变换DFT
        /// 内部将使用快速傅里叶变换FFT
        /// </summary>
        /// <param name="sourceData">待变换的序列</param>
        /// <param name="countN">变换的长度N</param>
        /// <returns>返回变换后的结果（复数数组）</returns>
        public Complex[] DFT(Complex[] sourceData, int countN)
        {
            if (countN > sourceData.Length || countN < 0)
                throw new Exception("指定的傅立叶变换长度越界！");

            //求出r,2的r次幂为N
            double dr = Math.Log(countN, 2);
            int r = Convert.ToInt32(dr);//获取整数部分

            //初始化存储变换结果的数组
            Complex[] result = new Complex[countN];

            //判断选择合适的算法进行快速傅里叶变换FFT
            if ((dr - r) != 0)
            {
                //待变换序列长度不是基2的
            }
            else
            {
                //待变换序列长度是基2的
                //使用一维频率抽取基2快速傅里叶变换
                result = fft_frequency(sourceData, countN);
            }

            return result;

        }


        /// <summary>
        /// 一维频率抽取基2快速傅里叶逆变换
        /// </summary>
        /// <param name="sourceData">待反变换的序列（复数数组）</param>
        /// <param name="countN">序列长度,可以指定[0,sourceData.Length-1]区间内的任意数值</param>
        /// <returns>返回逆变换后的序列（复数数组）</returns>
        private Complex[] ifft_frequency(Complex[] sourceData, int countN)
        {
            //将待逆变换序列取共轭，再调用正变换得到结果，对结果统一再除以变换序列的长度N

            for (int i = 0; i < countN; i++)
            {
                sourceData[i] = sourceData[i].Conjugate();
            }

            Complex[] interVar = new Complex[countN];

            interVar = fft_frequency(sourceData, countN);

            for (int i = 0; i < countN; i++)
            {
                interVar[i] = new Complex(interVar[i].Real / countN, -interVar[i].Imaginary / countN);
            }

            return interVar;
        }

        /// <summary>
        /// 一维频率抽取基2快速傅里叶变换
        /// 频率抽取：输入为自然顺序，输出为码位倒置顺序
        /// 基2：待变换的序列长度必须为2的整数次幂
        /// </summary>
        /// <param name="sourceData">待变换的序列(复数数组)</param>
        /// <param name="countN">序列长度,可以指定[0,sourceData.Length-1]区间内的任意数值</param>
        /// <returns>返回变换后的序列（复数数组）</returns>
        private Complex[] fft_frequency(Complex[] sourceData, int countN)
        {
            //2的r次幂为N，求出r.r能代表fft算法的迭代次数
            int r = Convert.ToInt32(Math.Log(countN, 2));


            //分别存储蝶形运算过程中左右两列的结果
            Complex[] interVar1 = new Complex[countN];
            Complex[] interVar2 = new Complex[countN];
            interVar1 = (Complex[])sourceData.Clone();

            //w代表旋转因子
            Complex[] w = new Complex[countN / 2];
            //为旋转因子赋值。（在蝶形运算中使用的旋转因子是已经确定的，提前求出以便调用）
            //旋转因子公式 \  /\  /k __
            //              \/  \/N  --  exp(-j*2πk/N)
            //这里还用到了欧拉公式
            for (int i = 0; i < countN / 2; i++)
            {
                double angle = -i * Math.PI * 2 / countN;
                w[i] = new Complex(Math.Cos(angle), Math.Sin(angle));
            }

            //蝶形运算
            for (int i = 0; i < r; i++)
            {
                //i代表当前的迭代次数，r代表总共的迭代次数.
                //i记录着迭代的重要信息.通过i可以算出当前迭代共有几个分组，每个分组的长度

                //interval记录当前有几个组
                // <<是左移操作符，左移一位相当于*2
                //多使用位运算符可以人为提高算法速率^_^
                int interval = 1 << i;

                //halfN记录当前循环每个组的长度N
                int halfN = 1 << (r - i);

                //循环，依次对每个组进行蝶形运算
                for (int j = 0; j < interval; j++)
                {
                    //j代表第j个组

                    //gap=j*每组长度，代表着当前第j组的首元素的下标索引
                    int gap = j * halfN;

                    //进行蝶形运算
                    for (int k = 0; k < halfN / 2; k++)
                    {
                        interVar2[k + gap] = interVar1[k + gap] + interVar1[k + gap + halfN / 2];
                        interVar2[k + halfN / 2 + gap] = (interVar1[k + gap] - interVar1[k + gap + halfN / 2]) * w[k * interval];
                    }
                }

                //将结果拷贝到输入端，为下次迭代做好准备
                interVar1 = (Complex[])interVar2.Clone();
            }

            //将输出码位倒置
            for (uint j = 0; j < countN; j++)
            {
                //j代表自然顺序的数组元素的下标索引

                //用rev记录j码位倒置后的结果
                uint rev = 0;
                //num作为中间变量
                uint num = j;

                //码位倒置（通过将j的最右端一位最先放入rev右端，然后左移，然后将j的次右端一位放入rev右端，然后左移...）
                //由于2的r次幂=N，所以任何j可由r位二进制数组表示，循环r次即可
                for (int i = 0; i < r; i++)
                {
                    rev <<= 1;
                    rev |= num & 1;
                    num >>= 1;
                }
                interVar2[rev] = interVar1[j];
            }
            return interVar2;

        }

        /// <summary>
        /// 获取FFT数列
        /// </summary>
        /// <param name="chdata">数字信号数组</param>
        /// <param name="result_lenth">获取结果的长度，数字信号的数组长度需要不小于该值的两倍，否则将无法正常计算</param>
        /// <returns></returns>
        public double[] GetFftValueFromChData(short[] chdata, int result_lenth)
        {
            double[] result = new double[0];
            double[] allvalue = new double[0];//快速计算结果为双倍长度
            int n = result_lenth * 2;// 470;// this.allChanData.Length;
            short[] x = chdata;//被计算的数组 
            complex[] y = new complex[n];//接收复数结果的数组 
            if (chdata.Length >= n)
            {
                result = new Double[n];//接收幅值结果的数组 
                y = airthm.dft(x, n);//重点耗时项
                allvalue = airthm.amplitude(y, n);

                result = new double[allvalue.Length / 2];
                for (int i = 0; i < result.Length; i++)
                { result[i] = allvalue[i]; }
                //result[0] = 0.0d;
            }
            else { }
            return result;
        }
        //double [,]X_sn;
        double pi = System.Math.PI;
        public double[,] Wcreat(int N, int FFT_IFFT_elect)
        {
            double[,] Wp = new double[2, N / 2];
            if (FFT_IFFT_elect == 0)
            {
                for (int i = 0; i < N / 2; i++)
                {
                    Wp[0, i] = System.Math.Cos(2 * pi / N * i);
                    Wp[1, i] = System.Math.Sin(-2 * pi / N * i);
                }
            }
            else
            {
                for (int i = 0; i < N / 2; i++)
                {
                    Wp[0, i] = System.Math.Cos(2 * pi / N * i);
                    Wp[1, i] = System.Math.Sin(2 * pi / N * i);
                }
            }
            return Wp;
        }
        /// <summary>
        /// 进行傅里叶变换
        /// </summary>
        /// <param name="X_sn">数列/原始信号</param>
        /// <param name="N">数列的长度</param>
        /// <param name="FFT_IFFT_elect"></param>
        /// <returns></returns>
        public double[,] FFT_T(double[,] X_sn, int N, int FFT_IFFT_elect)
        {
            double[,] Wp = new double[2, N / 2];
            FFT Xn = new FFT();
            Wp = Xn.Wcreat(N, FFT_IFFT_elect);
            //测试


            //
            double tem = System.Math.Log(N, 2);
            int M = (int)tem;
            for (int L = 1; L <= M; L++)
            {
                double M_Ld = System.Math.Pow(2, M - L);//计算2的M-L次方
                int M_L = (int)M_Ld;
                double L_1d = System.Math.Pow(2, L - 1);
                int L_1 = (int)L_1d;
                for (int j = 0; j < M_L; j++)
                {
                    int J = j * (int)System.Math.Pow(2, L);
                    for (int k = 0; k < L_1; k++)
                    {
                        double[] T = new double[2];
                        double p_k = k * (int)System.Math.Pow(2, (M - L));
                        int P = (int)p_k;
                        T = Xn.C_multi(X_sn[0, J + L_1 + k], X_sn[1, J + L_1 + k], Wp[0, P], Wp[1, P]);
                        X_sn[0, J + L_1 + k] = X_sn[0, J + k] - T[0];
                        X_sn[1, J + L_1 + k] = X_sn[1, J + k] - T[1];
                        X_sn[0, J + k] = X_sn[0, J + k] + T[0];
                        X_sn[1, J + k] = X_sn[1, J + k] + T[1];
                    }
                }
            }
            return X_sn;
        }
        public double[] C_multi(double a, double b, double c, double d)
        {
            double[] R_value = new double[2];
            R_value[0] = a * c - b * d;
            R_value[1] = a * d + b * c;
            return R_value;
        }
        public int R_value(int num, int M)//将一个数取反
        {
            double R_num = 0;
            for (int i = M; i > 0; i--)
            {
                double tem = System.Math.Pow(2, i);
                int t = (int)tem;
                int j = (num % t) / (t / 2);
                R_num = R_num + j * System.Math.Pow(2, M - i);
            }
            return (int)R_num;
        }
        public double[,] R_Xn(double[,] xn, int N)
        {
            double tem = System.Math.Log(N, 2);//求得序列点数对2为底的对数 例：N=4则tem=2，N=8则tem=3；
            int M = (int)tem;//将该对数取整
            FFT R_fft = new FFT();
            for (int i = 0; i < N / 2; i++)
            {
                int tem1 = R_fft.R_value(i, M);//把索引各种倒换，得出一个换位置的索引，还没看懂
                double[] tem2 = new double[2];
                if (tem1 != i)//防止计算出的索引值与i重合造成计算的浪费
                {
                    tem2[0] = xn[0, i];//将索引位置的值互换
                    tem2[1] = xn[1, i];
                    xn[0, i] = xn[0, tem1];
                    xn[1, i] = xn[1, tem1];
                    xn[0, tem1] = tem2[0];
                    xn[1, tem1] = tem2[1];
                }
            }
            return xn;
        }
    }

    /// <summary>
    /// 复数类
    /// </summary>
    public class Complex
    {
        #region 字段

        //复数实部
        private double real = 0.0;

        //复数虚部
        private double imaginary = 0.0;

        #endregion

        #region 属性

        /// <summary>
        /// 获取或设置复数的实部
        /// </summary>
        public double Real
        {
            get
            {
                return real;
            }
            set
            {
                real = value;
            }
        }

        /// <summary>
        /// 获取或设置复数的虚部
        /// </summary>
        public double Imaginary
        {
            get
            {
                return imaginary;
            }
            set
            {
                imaginary = value;
            }
        }

        #endregion


        #region 构造函数

        /// <summary>
        /// 默认构造函数，得到的复数为0
        /// </summary>
        public Complex()
            : this(0, 0)
        {

        }

        /// <summary>
        /// 只给实部赋值的构造函数，虚部将取0
        /// </summary>
        /// <param name="dbreal">实部</param>
        public Complex(double dbreal)
            : this(dbreal, 0)
        {

        }

        /// <summary>
        /// 一般形式的构造函数
        /// </summary>
        /// <param name="dbreal">实部</param>
        /// <param name="dbImage">虚部</param>
        public Complex(double dbreal, double dbImage)
        {
            real = dbreal;
            imaginary = dbImage;
        }

        /// <summary>
        /// 以拷贝另一个复数的形式赋值的构造函数
        /// </summary>
        /// <param name="other">复数</param>
        public Complex(Complex other)
        {
            real = other.real;
            imaginary = other.imaginary;
        }

        #endregion

        #region 重载

        //加法的重载
        public static Complex operator +(Complex comp1, Complex comp2)
        {
            return comp1.Add(comp2);
        }

        //减法的重载
        public static Complex operator -(Complex comp1, Complex comp2)
        {
            return comp1.Substract(comp2);
        }

        //乘法的重载
        public static Complex operator *(Complex comp1, Complex comp2)
        {
            return comp1.Multiply(comp2);
        }

        //==的重载
        public static bool operator ==(Complex z1, Complex z2)
        {
            return ((z1.real == z2.real) && (z1.imaginary == z2.imaginary));
        }

        //!=的重载
        public static bool operator !=(Complex z1, Complex z2)
        {
            if (z1.real == z2.real)
            {
                return (z1.imaginary != z2.imaginary);
            }
            return true;
        }

        /// <summary>
        /// 重载ToString方法,打印复数字符串
        /// </summary>
        /// <returns>打印字符串</returns>
        public override string ToString()
        {
            if (Real == 0 && imaginary == 0)
            {
                return string.Format("{0}", 0);
            }
            if (Real == 0 && (imaginary != 1 && imaginary != -1))
            {
                return string.Format("{0} i", imaginary);
            }
            if (imaginary == 0)
            {
                return string.Format("{0}", Real);
            }
            if (imaginary == 1)
            {
                return string.Format("i");
            }
            if (imaginary == -1)
            {
                return string.Format("- i");
            }
            if (imaginary < 0)
            {
                return string.Format("{0} - {1} i", Real, -imaginary);
            }
            return string.Format("{0} + {1} i", Real, imaginary);
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 复数加法
        /// </summary>
        /// <param name="comp">待加复数</param>
        /// <returns>返回相加后的复数</returns>
        public Complex Add(Complex comp)
        {
            double x = real + comp.real;
            double y = imaginary + comp.imaginary;

            return new Complex(x, y);
        }

        /// <summary>
        /// 复数减法
        /// </summary>
        /// <param name="comp">待减复数</param>
        /// <returns>返回相减后的复数</returns>
        public Complex Substract(Complex comp)
        {
            double x = real - comp.real;
            double y = imaginary - comp.imaginary;

            return new Complex(x, y);
        }

        /// <summary>
        /// 复数乘法
        /// </summary>
        /// <param name="comp">待乘复数</param>
        /// <returns>返回相乘后的复数</returns>
        public Complex Multiply(Complex comp)
        {
            double x = real * comp.real - imaginary * comp.imaginary;
            double y = real * comp.imaginary + imaginary * comp.real;

            return new Complex(x, y);
        }

        /// <summary>
        /// 获取复数的模/幅度
        /// </summary>
        /// <returns>返回复数的模</returns>
        public double GetModul()
        {
            return Math.Sqrt(real * real + imaginary * imaginary);
        }

        /// <summary>
        /// 获取复数的相位角，取值范围（-π，π]
        /// </summary>
        /// <returns>返回复数的相角</returns>
        public double GetAngle()
        {
            #region 原先求相角的实现，后发现Math.Atan2已经封装好后注释
            ////实部和虚部都为0
            //if (real == 0 && imaginary == 0)
            //{
            //    return 0;
            //}
            //if (real == 0)
            //{
            //    if (imaginary > 0)
            //        return Math.PI / 2;
            //    else
            //        return -Math.PI / 2;
            //}
            //else
            //{
            //    if (real > 0)
            //    {
            //        return Math.Atan2(imaginary, real);
            //    }
            //    else
            //    {
            //        if (imaginary >= 0)
            //            return Math.Atan2(imaginary, real) + Math.PI;
            //        else
            //            return Math.Atan2(imaginary, real) - Math.PI;
            //    }
            //}
            #endregion

            return Math.Atan2(imaginary, real);
        }

        /// <summary>
        /// 获取复数的共轭复数
        /// </summary>
        /// <returns>返回共轭复数</returns>
        public Complex Conjugate()
        {
            return new Complex(this.real, -this.imaginary);
        }

        #endregion
    }
    struct complex//定义复数 
    {
        //复数a+bi中 
        //a为实部，b为虚部 
        public double a;
        public double b;
        public static complex commul(double x, complex y)
        {
            complex c = new complex();
            c.a = x * y.a;
            c.b = x * y.b;

            return c;
        }
        public static complex commulcc(complex x, complex y)
        {
            complex c = new complex();
            c.a = x.a * y.a - x.b * y.b;
            c.b = x.a * y.b + x.b * y.a;

            return c;
        }
        public static complex comsum(complex x, complex y)
        {
            complex c = new complex();
            c.a = x.a + y.a;
            c.b = x.b + y.b;

            return c;
        }
        public static complex comsum1(double x, complex y)
        {
            complex c = new complex();
            c.a = x + y.a;
            c.b = y.b;

            return c;
        }
        public static complex decrease(complex x, complex y)
        {
            complex c = new complex();
            c.a = x.a * y.a - x.b * y.b;
            c.b = x.a * y.b + x.b * y.a;

            return c;
        }
        public static complex powcc(complex x, double n)
        {
            int k;
            complex xout;
            xout.a = 1;
            xout.b = 0;
            for (k = 1; k <= n; k++)
            {
                xout = complex.commulcc(xout, x);
            }
            return xout;
        }
        public void show()
        {
            //Console.Write(a+&quot; + &quot;+b+&quot;i      &quot;); 
        }
    }
    class airthm
    {
        //计算ω=exp（j*2*pi/n） 
        public static complex omega(int n)
        {
            complex x;
            x.a = Math.Cos(0 - 2 * Math.PI / n);
            x.b = Math.Sin(0 - 2 * Math.PI / n);
            return x;
        }
        public static complex[] dft(short[] signal, int n)  //(信号，计算结果的长度) 
        {
            int i, j;
            complex w1;
            w1 = omega(n);
            complex[] w = new complex[n];
            for (i = 0; i < n; i++)
            {
                w[i] = complex.powcc(w1, i);
            }
            complex[] f = new complex[n];
            complex temp;  //w[i]的次方 
            complex temp1; //f中单项的值 
            //耗时部分
            for (i = 0; i < n; i++)
            {
                f[i].a = 0;
                f[i].b = 0;
                for (j = 0; j < n; j++)
                {
                    temp = complex.powcc(w[i], j);
                    temp1 = complex.commul(signal[j], temp);
                    f[i] = complex.comsum(f[i], temp1);
                }
            }
            return f;
        }
        //求幅值  x  信号  n  信号长度  返回 幅值数组 
        public static double[] amplitude(complex[] x, int n)
        {
            int i;
            double temp;
            double[] amp = new double[n];
            for (i = 0; i < n; i++)
            {
                temp = x[i].a * x[i].a + x[i].b * x[i].b;
                amp[i] = Math.Sqrt(temp);
            }
            return amp;
        }
    } 
}
