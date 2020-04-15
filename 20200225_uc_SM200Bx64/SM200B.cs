using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using System.Collections;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using SM200Bx64.Class;
using System.Timers;

namespace SM200Bx64
{
    public partial class SM200B : UserControl
    {
        public SM200B()
        {

            if (!DesignMode)
            {
                InitializeComponent();

                this.DoubleBuffered = true;//设置本窗体
                this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint, true);
                this.UpdateStyles();
            }
            else {
                设计模式显示();
            }
        }

        #region 控件事件响应
        private void SM200B_Load(object sender, EventArgs e)
        {
            菜单定位();
            panel_菜单.BringToFront();
            panel_MenuTable.Location = new Point(菜单最小显示宽度, 0);
            显示界面(界面类型.主要设置);
            默认值加载及初始化();

        }
        private void SM200B_SizeChanged(object sender, EventArgs e)
        {
            菜单定位();
            定位label左上信息显示();
        }
        private void label_按钮1_Click(object sender, EventArgs e)
        {
            菜单显示();
        }
        private void chart1_MouseMove(object sender, MouseEventArgs e)
        {
            new Thread(() =>
            {
                if (chart1.IsHandleCreated)
                {
                    chart1.BeginInvoke(new 无参(() =>
                    {
                        chart1.ChartAreas[0].CursorX.SetCursorPixelPosition(new Point(e.X, e.Y), true);
                        chart1.ChartAreas[0].CursorY.SetCursorPixelPosition(new Point(e.X, e.Y), true);
                        label_显示区_坐标轴数值显示.Text = "X:" + chart1.ChartAreas[0].CursorX.Position.ToString("0.000") + Y轴单位
                        + "    Y:" + chart1.ChartAreas[0].CursorY.Position.ToString("0.000") + X轴单位;
                    }));
                }
            })
            { IsBackground = true }.Start();

        }
        private void 还原比例ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chart1.ChartAreas[0].AxisX.ScaleView.ZoomReset();
            chart1.ChartAreas[0].AxisY.ScaleView.ZoomReset();
        }
        private void toolStripMenuItem_还原_Click(object sender, EventArgs e)
        {
            chart1.ChartAreas[0].AxisX.ScaleView.ZoomReset(200);
            chart1.ChartAreas[0].AxisY.ScaleView.ZoomReset(200);
        }
        #endregion

        #region 控件属性

        private bool IsShowMenu = true;
        [Category("SM200B控件属性"), Browsable(true), Description("菜单栏是否显示")]
        public bool 是否显示菜单
        {
            get => IsShowMenu;
            set { IsShowMenu = value; }
        }

        [Category("SM200B控件属性"), Browsable(true), Description("自动连接启用")]
        public bool 自动连接启用
        {
            get => 自动连接;
            set
            {
                自动连接 = checkBox_设置_自动连接.Checked = value;

            }
        }
        [Category("SM200B控件属性"), Browsable(true), Description("调用C++API的位数,true为x64;false为x86")]
        public bool 调用类库位数
        {
            get => Isx64;
            set
            {
                Isx64 = value;

            }
        }
        #endregion

        #region 弹出样式
        delegate void 无参();
        int 菜单固定宽度 = 268;
        int 菜单最小显示宽度 = 25;
        private static int FlashInterval = 10;
        private static Size temp_destSize;
        private static Control temp_control = null;
        private static readonly int temp_MoveStep = 70;//动画移动步进

        private async Task temp_showMenuFlash() {
            temp_control.BeginInvoke(new 无参(() => {
                temp_control.Enabled = false;
                temp_destSize = new Size(菜单固定宽度, temp_control.Height); ;
                temp_control.Left += (temp_control.Width - 菜单最小显示宽度);
                temp_control.Width = 菜单最小显示宽度;
                temp_control.Show();
            }));
            int newValue = 0;
            int offSet = 0;
            new Thread(() => {
                if (temp_control.IsHandleCreated)
                {
                    temp_control.BeginInvoke(new 无参(async () => {
                        while (true) {
                            newValue = temp_control.Width + temp_MoveStep;
                            if (newValue > temp_destSize.Width)
                            {
                                temp_control.Enabled = true;
                                newValue = temp_destSize.Width;
                                offSet = newValue - temp_control.Width;
                                temp_control.Width += offSet;
                                temp_control.Left -= offSet;
                                temp_control.Refresh();
                                break;
                            }
                            offSet = newValue - temp_control.Width;
                            temp_control.Width += offSet;
                            temp_control.Left -= offSet;
                            temp_control.Refresh();
                            await Task.Delay(FlashInterval);
                        }
                    }));
                }
            })
            { IsBackground = true }.Start();
        }

        bool 菜单是否显示 = false;
        private async Task 菜单显示()
        {
            显示其他菜单按钮(菜单是否显示);
            if (菜单是否显示)
            {
                toolTip1.SetToolTip(label_显示菜单按钮, "收起菜单");
                temp_control = panel_菜单;
                await temp_showMenuFlash();
            }
            else {
                toolTip1.SetToolTip(label_显示菜单按钮, "显示菜单");
                new Thread(() => {
                    if (panel_菜单.IsHandleCreated) {
                        panel_菜单.BeginInvoke(new 无参(() => {
                            panel_菜单.Size = new Size(菜单最小显示宽度, this.Height);
                            panel_菜单.Location = new Point(this.Width - panel_菜单.Width, 0);
                        }));
                    } }) { IsBackground = true }.Start();
            }
            菜单是否显示 = !菜单是否显示;
        }
        private void 显示其他菜单按钮(bool v)
        {
            panel_MenuTable.Visible = v;
            label_主界面按钮.Visible = v;
            label_扫频按钮.Visible = v;
            label_IQ采集按钮.Visible = v;
            label_其他相关按钮.Visible = v;
            label_临时数据按钮.Visible = v;
        }
        #endregion

        #region 菜单定位
        private void 菜单定位()
        {
            new Thread(() => {
                if (panel_菜单.IsHandleCreated) {
                    panel_菜单.BeginInvoke(new 无参(() => {
                        panel_菜单.Size = new Size(panel_菜单.Width, this.Height);
                        panel_菜单.Location = new Point(this.Width - panel_菜单.Width, 0);
                    }));
                }
                if (panel_MenuTable.IsHandleCreated) {
                    panel_MenuTable.BeginInvoke(new 无参(() => {
                        panel_MenuTable.Size = new Size(菜单固定宽度 - 菜单最小显示宽度, panel_菜单.Height);

                    }));
                }

            })
            { IsBackground = true }.Start();
        }

        private void 定位label左上信息显示()
        {
            new Thread(() => {
                if (label_显示区_左上信息显示.IsHandleCreated)
                {
                    label_显示区_左上信息显示.BeginInvoke(new 无参(() => {
                        try
                        {
                            label_显示区_左上信息显示.Parent = chart1;
                            label_显示区_左上信息显示.Location = new Point((int)(chart1.Width / 11.622), (int)(chart1.Height / 49.571));
                        }
                        catch { }
                    }));
                }
            })
            { IsBackground = true }.Start();
        }

        #endregion

        #region 菜单层级显示
        private enum 界面类型
        {
            实时扫频 = 0,
            扫频相关 = 1,
            IQ采集相关 = 2,
            主要设置 = 3,
            临时数据显示 = 9
        }
        private void label5_Click(object sender, EventArgs e)
        {
            显示界面(界面类型.临时数据显示);
        }
        private void label1_Click(object sender, EventArgs e)
        {
            显示界面(界面类型.主要设置);
        }
        private void label2_Click(object sender, EventArgs e)
        {
            显示界面(界面类型.扫频相关);

        }
        private void label3_Click(object sender, EventArgs e)
        {
            显示界面(界面类型.IQ采集相关);
        }
        private void label4_Click(object sender, EventArgs e)
        {
            显示界面(界面类型.实时扫频);
        }
        界面类型 界面当前类型 = 界面类型.临时数据显示;
        private void 显示界面(界面类型 temp_j) {
            if (界面当前类型 != temp_j)
            {
                groupBox_IQ采集.Visible = groupBox_实时扫频.Visible =
               groupBox_主要设置.Visible = groupBox_扫频.Visible =
               groupBox_临时显示点.Visible = false;
                switch (temp_j)
                {
                    case 界面类型.主要设置:
                        //panel_MenuTable.BackColor = Color.FromArgb(228,228,228);
                        groupBox_主要设置.Visible = true;
                        break;
                    case 界面类型.扫频相关:
                        //panel_MenuTable.BackColor = Color.FromArgb(224, 250, 237);
                        groupBox_扫频.Visible = true;
                        break;
                    case 界面类型.IQ采集相关:
                        //panel_MenuTable.BackColor = Color.FromArgb(156, 212, 227);
                        groupBox_IQ采集.Visible = true;
                        break;
                    case 界面类型.实时扫频:
                        //panel_MenuTable.BackColor = Color.FromArgb(239,170,193);
                        groupBox_实时扫频.Visible = true;
                        break;
                    case 界面类型.临时数据显示:
                        //panel_MenuTable.BackColor = Color.FromArgb(254, 252, 203);
                        groupBox_临时显示点.Visible = true;
                        break;
                    default:
                        break;
                }
                界面当前类型 = temp_j;
            }

        }


        #endregion

        #region 按钮样式改变
        private void 按钮默认样式() {
            label_主界面按钮.Image = Properties.Resources.按钮_设备信息_00;
            label_扫频按钮.Image = Properties.Resources.按钮_频谱扫描_00;
            label_IQ采集按钮.Image = Properties.Resources.按钮_IQ采集_00;
            label_临时数据按钮.Image = Properties.Resources.按钮_音频处理_00;
            label_其他相关按钮.Image = Properties.Resources.按钮_实时扫描_00;
        }
        private void label_按钮1_MouseEnter(object sender, EventArgs e)
        {
            if (!菜单是否显示) {
                label_显示菜单按钮.Image = Properties.Resources.收起_01;
            }
            else {
                label_显示菜单按钮.Image = Properties.Resources.展开_01;
            }
        }

        private void label_按钮1_MouseDown(object sender, MouseEventArgs e)
        {
            //label_显示菜单按钮.BackColor = Color.Brown;
        }

        private void label_按钮1_MouseLeave(object sender, EventArgs e)
        {
            if (!菜单是否显示)
            {
                label_显示菜单按钮.Image = Properties.Resources.收起_00;
            }
            else
            {
                label_显示菜单按钮.Image = Properties.Resources.展开_00;
            }
        }

        private void label_按钮1_MouseUp(object sender, MouseEventArgs e)
        {
            //label_显示菜单按钮.BackColor = Color.IndianRed;
        }


        private void label_主界面按钮_MouseDown(object sender, MouseEventArgs e)
        {
            按钮默认样式();
            label_主界面按钮.Image = Properties.Resources.按钮_设备信息_00_按下;
        }

        private void label_主界面按钮_MouseEnter(object sender, EventArgs e)
        {
            if (界面当前类型 == 界面类型.主要设置)
            {
                label_主界面按钮.Image = Properties.Resources.按钮_设备信息_00_按下;
            }
            else {
                label_主界面按钮.Image = Properties.Resources.按钮_设备信息_00_放上;
            }
        }

        private void label_主界面按钮_MouseLeave(object sender, EventArgs e)
        {
            if (界面当前类型 == 界面类型.主要设置)
            {
                label_主界面按钮.Image = Properties.Resources.按钮_设备信息_00_按下;
            }
            else
            {
                label_主界面按钮.Image = Properties.Resources.按钮_设备信息_00;
            }
        }




        private void label_扫频按钮_MouseDown(object sender, MouseEventArgs e)
        {
            按钮默认样式();
            label_扫频按钮.Image = Properties.Resources.按钮_频谱扫描_02;
        }

        private void label_扫频按钮_MouseEnter(object sender, EventArgs e)
        {
            if (界面当前类型 == 界面类型.扫频相关)
            {
                label_扫频按钮.Image = Properties.Resources.按钮_频谱扫描_02;
            }
            else
            {
                label_扫频按钮.Image = Properties.Resources.按钮_频谱扫描_01;
            }
        }

        private void label_扫频按钮_MouseLeave(object sender, EventArgs e)
        {
            if (界面当前类型 == 界面类型.扫频相关)
            {
                label_扫频按钮.Image = Properties.Resources.按钮_频谱扫描_02;
            }
            else
            {
                label_扫频按钮.Image = Properties.Resources.按钮_频谱扫描_00;
            }
        }


        private void label_IQ采集按钮_MouseDown(object sender, MouseEventArgs e)
        {
            按钮默认样式();
            label_IQ采集按钮.Image = Properties.Resources.按钮_IQ采集_02;
        }
        private void label_IQ采集按钮_MouseEnter(object sender, EventArgs e)
        {
            if (界面当前类型 == 界面类型.IQ采集相关)
            {
                label_IQ采集按钮.Image = Properties.Resources.按钮_IQ采集_02;
            }
            else
            {
                label_IQ采集按钮.Image = Properties.Resources.按钮_IQ采集_01;
            }
        }

        private void label_IQ采集按钮_MouseLeave(object sender, EventArgs e)
        {
            if (界面当前类型 == 界面类型.IQ采集相关)
            {
                label_IQ采集按钮.Image = Properties.Resources.按钮_IQ采集_02;
            }
            else
            {
                label_IQ采集按钮.Image = Properties.Resources.按钮_IQ采集_00;
            }
        }


        private void label_其他相关按钮_MouseDown(object sender, MouseEventArgs e)
        {
            按钮默认样式();
            label_其他相关按钮.Image = Properties.Resources.按钮_实时扫描_02;
        }


        private void label_其他相关按钮_MouseEnter(object sender, EventArgs e)
        {
            if (界面当前类型 == 界面类型.实时扫频)
            {
                label_其他相关按钮.Image = Properties.Resources.按钮_实时扫描_02;
            }
            else
            {
                label_其他相关按钮.Image = Properties.Resources.按钮_实时扫描_01;
            }
        }

        private void label_其他相关按钮_MouseLeave(object sender, EventArgs e)
        {
            if (界面当前类型 == 界面类型.实时扫频)
            {
                label_其他相关按钮.Image = Properties.Resources.按钮_实时扫描_02;
            }
            else
            {
                label_其他相关按钮.Image = Properties.Resources.按钮_实时扫描_00;
            }
        }


        private void label_临时数据按钮_MouseDown(object sender, MouseEventArgs e)
        {
            按钮默认样式();
            label_临时数据按钮.Image = Properties.Resources.按钮_音频处理_02;
        }

        private void label_临时数据按钮_MouseEnter(object sender, EventArgs e)
        {
            if (界面当前类型 == 界面类型.临时数据显示)
            {
                label_临时数据按钮.Image = Properties.Resources.按钮_音频处理_02;
            }
            else
            {
                label_临时数据按钮.Image = Properties.Resources.按钮_音频处理_01;
            }
        }
        private void label_临时数据按钮_MouseLeave(object sender, EventArgs e)
        {
            if (界面当前类型 == 界面类型.临时数据显示)
            {
                label_临时数据按钮.Image = Properties.Resources.按钮_音频处理_02;
            }
            else
            {
                label_临时数据按钮.Image = Properties.Resources.按钮_音频处理_00;
            }
        }

        #endregion

        #region 信息显示
        String Message_DefaultString = "暂无消息提示";
        ArrayList Message_ArrayList = new ArrayList();
        System.Timers.Timer Message_Timer = new System.Timers.Timer(1000);
        private void showMessage(int MessageLeve, string MessageString) {
            MessageFormat mf = new MessageFormat(MessageString, MessageLeve);
            if (Message_ArrayList.Count > 10)
            {
                int temp = Message_ArrayList.Count - 10;
                for (int i = 0; i<temp;i++) {
                    try { Message_ArrayList.RemoveAt(9 + i); } catch { }
                }
                Message_ArrayList.Add(mf);
            }
            else {
                for (int i = 0; i < Message_ArrayList.Count; i++)
                {
                    if (MessageLeve > ((MessageFormat)Message_ArrayList[i]).Num)
                    {
                        Message_ArrayList.Insert(i, mf);
                        break;
                    }
                    else
                    {
                        ((MessageFormat)Message_ArrayList[i]).Num--;
                    }
                }
            }
        }

        private class MessageFormat {
            public MessageFormat() { }
            public MessageFormat(string s,int i) {
                Content = s;
                Num = i;
            }
            public string Content;
            public int Num;
        }
        private void MessageTimerTicked(object sender, ElapsedEventArgs e)
        {
            temp_flag:
            if (Message_ArrayList.Count<1) {
                MessageFormat mf = new MessageFormat(Message_DefaultString,1);
                Message_ArrayList.Add(mf);
            }
            else
            {
                for (int i = 0;i<Message_ArrayList.Count;i++) {
                    if (((MessageFormat)Message_ArrayList[i]).Num < 0) {
                        Message_ArrayList.RemoveAt(i);
                        if (Message_ArrayList.Count<1) {
                            goto temp_flag;
                        }
                    }
                    else {
                        ((MessageFormat)Message_ArrayList[i]).Num -= 1;
                    }
                }
            }
            

            if (scrollingText_消息提示.IsHandleCreated) {
                scrollingText_消息提示.Invoke(new 无参(()=> {
                    try {
                        scrollingText_消息提示.ScrollText = ((MessageFormat)Message_ArrayList[0]).Content;
                    } catch { scrollingText_消息提示.ScrollText = "(0x00)消息提示解析问题"; }
                }));
            }
        }

        #endregion

        //目前没有输入值判断\带宽计算检测\chart上下边界值\轴单位,记得改==========================================================

        #region 界面相关参数
        bool Isx64 = true;

        private int Equipment_Num = -1;
        int chart_固定长度 = 2000;
        const int GCClear_num = 5;
        int GCClear_tempNum = 0;
        string X轴单位 = "";
        string Y轴单位 = "";

        public void 方法_连接中断()
        {
            停止任何活动();
            清空chart();
            MineSM200B.方法_状态操作_断开设备(Isx64, Equipment_Num);
            连接状态 = false;
            button_设置_手动连接.Invoke(new 无参(() => { button_设置_手动连接.Text = "连接"; }));
            showMessage(2,"中断设备连接");
        }
        public void 方法_设为空闲()
        {
            MineSM200B.方法_状态操作_置为空闲(Isx64, Equipment_Num);
            showMessage(2, "设备置为空闲");
        }


        private void 默认值加载及初始化()
        {

            panel_菜单.Visible = IsShowMenu;
            timer_信息查询及连接检测.Start();
            连接指示灯(false);
            连接信息显示();
            toolTip1.SetToolTip(label_GPS状态, "GPS未锁定");
            toolTip1.SetToolTip(label_显示菜单按钮, "收起菜单");
            toolTip1.SetToolTip(label_其他相关按钮, "实时扫描");
            toolTip1.SetToolTip(label_临时数据按钮, "音频处理");
            toolTip1.SetToolTip(label_IQ采集按钮, "IQ采集");
            toolTip1.SetToolTip(label_扫频按钮, "扫频设置");
            toolTip1.SetToolTip(label_主界面按钮, "设备连接");
            //label定位======
            定位label左上信息显示();
            //===============
            ForAutoLink = new Thread(link_equipment);
            //扫频值默认
            try
            {
                comboBox_扫频_衰减值.SelectedIndex = 0; 扫频_参考电平 = -20; numericUpDown_设置_参考电平.Value = -20;
                comboBox_扫频_中心频率单位.SelectedIndex = 1;
                comboBox_扫频_扫频宽度单位.SelectedIndex = 1;
                扫频_中心频率 = 8160000; numericUpDown_扫频_中心频率.Value = 8160;
                扫频_扫宽 = 2e9; numericUpDown_扫频_扫宽.Value = 2000;

                comboBox_扫频_终止频率单位.SelectedIndex = 1;
                comboBox_扫频_起始频率单位.SelectedIndex = 1;

                comboBox_扫频_RBW形状.SelectedIndex = 0;
                comboBox_扫频_视频单位.SelectedIndex = 0;
                comboBox_扫频_检波器.SelectedIndex = 0;
                comboBox_扫频_返回单位.SelectedIndex = 0;

                comboBox_扫频_RBW单位.SelectedIndex = 2;
                comboBox_扫频_VBW单位.SelectedIndex = 2;
                扫频_频率设置_界面切换(true);
            }
            catch { showMessage(5, "(0x01)设备扫频界面配置失败"); }
            //IQ默认值
            try
            {
                IQ_记录回放界面(true);
                comboBox_IQ_衰减值.SelectedIndex = 0;
                IQ_参考电平 = 0; numericUpDown_IQ_参考电平.Value = 0;
                comboBox_IQ_中心频率单位.SelectedIndex = 1;
                comboBox_IQ_中心频率单位.SelectedIndex = 1;
                numericUpDown_IQ_中心频率.Value = 8160;
                comboBox_IQ_采样率.SelectedIndex = 1;
                numericUpDown_IQ_带宽.Value = 41.5M;
                comboBox_IQ_触发沿.SelectedIndex = 0;
                IQ_保存路径 = textBox_iq记录_保存路径.Text = Application.StartupPath.ToString();
            }
            catch { showMessage(5, "(0x02)设备IQ界面配置失败"); }
            //音频默认
            try
            {
                comboBox_音频处理_中心频率单位.SelectedIndex = 1;
                numericUpDown_音频处理_中心频率.Value = (decimal)101.7;
                comboBox_音频处理_音频类型.SelectedIndex = 1;
            }
            catch { showMessage(5, "(0x03)设备音频处理界面配置失败"); }

            //信息加载
            try {
                Message_Timer.Elapsed += new System.Timers.ElapsedEventHandler(MessageTimerTicked);
                Message_Timer.AutoReset = true;
                Message_Timer.Enabled = true;
            }
            catch { showMessage(5, "(0x04)消息提示计时器加载失败"); }
            //==
            //== 临时 ==
            //await Task.Run(new Action(() => {

            //}));
        }

        private void 停止任何活动()
        {
            停止扫描();
            停止IQ();
            停止音频播放();
            label_显示区_左上信息显示.Text = "";
            X轴单位 = "";
            Y轴单位 = "";
            清空chart();
        }
        private void 清空chart()
        {
            if (chart1.IsHandleCreated)
            {
                chart1.Invoke(new 无参(() => {
                    chart1.Series[0].Points.Clear();
                }));
            }
        }

        #endregion

        #region 主要设置
        Thread ForAutoLink;
        public bool 连接状态 = false;
        private bool 界面显示_连接状态 = false;
        private bool 自动连接 = true;
        private void button_设置_手动连接_Click(object sender, EventArgs e)
        {
            if (连接状态)
            {
                方法_连接中断();
                checkBox_设置_自动连接.Checked = false;
            }
            else
            {
                new Thread(() => {
                    link_equipment();
                }) { IsBackground = true}.Start();
            }
        }
        private void checkBox_设置_自动连接_CheckedChanged(object sender, EventArgs e)
        {
            自动连接 = checkBox_设置_自动连接.Checked;
            if (自动连接) {
                showMessage(4,"用户启用自动连接");
            }
            else {
                showMessage(4, "用户关闭自动连接");
            }
        }
        private void link_equipment() {
            progressBar_设置_连接等待.BeginInvoke(new 无参(() =>
            {
               progressBar_设置_连接等待.Visible = true;
                button_设置_手动连接.Enabled = false;
            }));
            Thread temp_thread = new Thread(() =>{
                if (MineSM200B.方法_状态操作_连接设备(Isx64, ref Equipment_Num))
                {showMessage(3, "设备连接成功");
                }else { showMessage(3, "设备连接失败"); }
            }){ IsBackground = true };temp_thread.Start();
            temp_thread.Join();
            new Thread(() =>
            {
                progressBar_设置_连接等待.BeginInvoke(new 无参(() =>
                {
                    progressBar_设置_连接等待.Visible = false;
                    if (MineSM200B.方法_状态操作_置为空闲(Isx64,Equipment_Num) >= 0)
                    {
                        连接状态 = true;
                        button_设置_手动连接.Text = "断开";
                    }
                    else
                    {
                        连接状态 = false;
                        button_设置_手动连接.Text = "连接";
                    }
                    button_设置_手动连接.Enabled = true;
                }));
            })
            { IsBackground = true }.Start();
        }

        private void timer_信息查询及连接检测_Tick(object sender, EventArgs e)
        {
            连接信息显示();
            if (连接状态){
                连接信息查询(界面显示_连接状态);
            }
            else {
                if (自动连接)
                {
                    try {
                        if (!ForAutoLink.IsAlive)
                        {
                            ForAutoLink = new Thread(link_equipment);
                            ForAutoLink.IsBackground = true;
                            ForAutoLink.Start();
                        }
                    } catch { showMessage(5, "(0x05)自动连接过程失败"); }
                }
            }
            if (界面显示_连接状态 != 连接状态)
            {
                界面显示_连接状态 = 连接状态;
                连接指示灯(界面显示_连接状态);
                连接信息查询(界面显示_连接状态);
                

            }

            if (GCClear_tempNum > GCClear_num)
            {
                ClearMemory();
                GCClear_tempNum = 0;
            }
            GCClear_tempNum++;
        }

        private void 连接指示灯(bool v) {
            if (v) {label_设备状态指示器.Image = Properties.Resources.confirm_16px_1232396_easyicon_net;}
            else {label_设备状态指示器.Image = Properties.Resources.cancel_16px_1232349_easyicon_net;}

        }

        private void 连接信息显示() {
            if (连接状态) {
                switch (MineSM200B.方法_设备查询_工作状态(Isx64,Equipment_Num)) {
                    case SmMode.smModeAudio:
                        label_连接状态显示.Text = "音频处理";
                        break;
                    case SmMode.smModeIdle:
                        label_连接状态显示.Text = "空闲";
                        break;
                    case SmMode.smModeIQ:
                        label_连接状态显示.Text = "IQ采集";
                        break;
                    case SmMode.smModeIQSegmentedCapture:
                        label_连接状态显示.Text = "IQ分段采集";
                        break;
                    case SmMode.smModeSweeping:
                        label_连接状态显示.Text = "扫描";
                        break;
                    case SmMode.smModeRealTime:
                        label_连接状态显示.Text = "实时";
                        break;
                    default:
                        label_连接状态显示.Text = "存在连接";
                        break;
                }
            }
            else {label_连接状态显示.Text = "未连接";}
        }

        private void 连接信息查询(bool v) {
            switch (MineSM200B.方法_设备查询_GPS状态(Isx64,Equipment_Num)) {
                case SmGPSState.smGPSStateNotPresent:
                    label_GPS状态.Image = Properties.Resources.un_locked_13_473684210526px_1130403_easyicon_net;
                    break;
                default:
                    label_GPS状态.Image = Properties.Resources.locked_13_473684210526px_1130402_easyicon_net;
                    break;
            }


            if (!菜单是否显示) {
                if (v) {
                    int Etemp = 0;
                    long ETime = 0;
                    double ELon = 0, ELat = 0, EH = 0;
                    float Ev =0,Ea = 0,Et = 0;
                    SmStatus temp_sms = MineSM200B.方法_设备查询_设备诊断(Isx64,Equipment_Num, ref Ev, ref Ea, ref Et); 
                    if (temp_sms<0) {
                        方法_连接中断();
                        label_设置_信息显示.Text = "无设备连接";
                        return;
                    }
                    MineSM200B.方法_设备查询_查询风扇启用温度(Isx64,Equipment_Num, ref Etemp);
                    if (MineSM200B.方法_设备查询_获取GPS信息(Isx64,Equipment_Num, SmBool.smFalse, ref ETime, ref ELon, ref ELat, ref EH)
                       == SmStatus.smGpsNotLockedErr)
                    {
                        ELat = ELon = EH = 0;
                        ETime = 0;
                    }
                    string s_gps = null;
                    switch (MineSM200B.方法_设备查询_GPS状态(Isx64,Equipment_Num))
                    {
                        case SmGPSState.smGPSStateNotPresent:
                            toolTip1.SetToolTip(label_GPS状态, "GPS未锁定");
                            s_gps = "未锁定";
                            break;
                        case SmGPSState.smGPSStateLocked:
                            toolTip1.SetToolTip(label_GPS状态, "GPS锁定");
                            s_gps = "锁定";
                            break;
                        case SmGPSState.smGPSStateDisciplined:
                            toolTip1.SetToolTip(label_GPS状态, "GPSState = Disciplined");
                            s_gps = "受训";
                            break;
                        default:
                            toolTip1.SetToolTip(label_GPS状态, "GPS未锁定");
                            s_gps = "未锁定";
                            break;
                    }
                    string temp_Ss = MineSM200B.方法_设备查询_设备类型(Isx64,Equipment_Num);
                    if (temp_Ss !=null) {
                        temp_Ss = temp_Ss.Substring(temp_Ss.Length - 6, 6);
                        string temp_s = null;
                        temp_s = "设备类型:" + temp_Ss
                            + "\n校准时间:" + Loc_GMTTimeToString_s(MineSM200B.方法_设备查询_校准日期(Isx64,Equipment_Num))
                            + "\nAPI 版本:" + MineSM200B.方法_设备查询_API版本(Isx64)
                            + "\n固件版本:" + MineSM200B.方法_设备查询_固件版本(Isx64,Equipment_Num)
                            + "\n设备电压:" + Ev
                            + "V\n设备电流:" + Ea
                            + "A\n设备温度:" + Et
                            + "℃\nGPS 状态:"+s_gps
                            +"\nGPS 时间:" + Loc_GMTTimeToString_s(ETime)
                            + "\nGPS 经度:" + ELon
                            + "\nGPS 纬度:" + ELat
                            + "\nGPS 高度:" + EH
                            + "\n风扇启用:" + Etemp + "℃";
                        label_设置_信息显示.Text = temp_s;
                    }


                }
                else { label_设置_信息显示.Text = "无设备连接"; }
            }
        }
        #endregion

        #region 扫频
        ArrayList temp_SweepResult = new ArrayList();
        int 扫频_衰减 = -1;
        double 扫频_中心频率 = 0, 扫频_扫宽 = 0,
            扫频_RBW = 0, 扫频_VBW = 0, 扫频_参考电平 = 0;
        SmBool 扫频_启用软拒 = SmBool.smFalse;
        SmDetector 扫频_检波器 = SmDetector.smDetectorAverage;
        SmScale 扫频_返回单位 = SmScale.smScaleFullScale;
        SmVideoUnits 扫频_视频单位 = SmVideoUnits.smVideoLog;
        SmWindowType 扫频_RBW形状 = SmWindowType.smWindowBlackman;
        double 扫频_Y轴最小 = double.NaN, 扫频_Y轴最大 = double.NaN;

        //是否在自动扫描
        private bool 扫频_TimerAutoGetSweep = false;
        #region 控件响应
        private void comboBox_扫频_衰减值_SelectedIndexChanged(object sender, EventArgs e)
        {
            try { switch (comboBox_扫频_衰减值.SelectedIndex) {
                    case 1:
                        扫频_衰减 = 0;
                        break;
                    case 2:
                        扫频_衰减 =1;
                        break;
                    case 3:
                        扫频_衰减 =2;
                        break;
                    case 4:
                        扫频_衰减 =3;
                        break;
                    case 5:
                        扫频_衰减 =4;
                        break;
                    case 6:
                        扫频_衰减 = 5;
                        break;
                    case 7:
                        扫频_衰减 = 6;
                        break;
                    default:
                        扫频_衰减 =-1;
                        break;
                }
                扫频_Y轴标数值还原();
            } catch { showMessage(5, "(0x07)扫频衰减值错误"); }
        }
        private void numericUpDown_设置_参考电平_ValueChanged(object sender, EventArgs e)
        {
            扫频_参考电平 = (double)numericUpDown_设置_参考电平.Value; 扫频_Y轴标数值还原();
        }

        #region 频率界面切换
        private void 扫频_频率设置_界面切换(bool v)
        {
            panel_扫频_频率设置_中心.Visible = v;
            panel_扫频_频率设置_始末.Visible = !v;
        }
        private void button_扫频_频率设置_中心扫宽_Click(object sender, EventArgs e)
        {
            扫频_频率设置_界面切换(true);
        }

        private void button_扫频_频率设置_起始终止_Click(object sender, EventArgs e)
        {
            扫频_频率设置_界面切换(false);
        }
        #endregion
        private void button_扫频_频率设置_全扫宽_Click(object sender, EventArgs e)
        {
            扫频_中心频率 = 10e9;
            扫频_扫宽 = 20e9;
            comboBox_扫频_中心频率单位.SelectedIndex = comboBox_扫频_扫频宽度单位.SelectedIndex = comboBox_扫频_起始频率单位.SelectedIndex
                = comboBox_扫频_终止频率单位.SelectedIndex = 1;
            numericUpDown_扫频_终止频率.Value = 20000;
            numericUpDown_扫频_起始频率.Value = 0;
            numericUpDown_扫频_中心频率.Value = 10000;
            numericUpDown_扫频_扫宽.Value = 20000;
            扫频_Y轴标数值还原();
        }

        #region 中心频率
        private void numericUpDown_扫频_中心频率_ValueChanged(object sender, EventArgs e)
        {
            中心频率同步数据();
        }
        private void 中心频率同步数据()
        {
            switch (comboBox_扫频_中心频率单位.SelectedIndex)
            {
                case 0:
                    扫频_中心频率 = (double)numericUpDown_扫频_中心频率.Value * 1e9;
                    break;
                case 2:
                    扫频_中心频率 = (double)numericUpDown_扫频_中心频率.Value * 1e3;
                    break;
                case 3:
                    扫频_中心频率 = (int)numericUpDown_扫频_中心频率.Value;
                    break;
                default:
                    扫频_中心频率 = (double)numericUpDown_扫频_中心频率.Value * 1e6;
                    break;
                    
            }
            扫频_Y轴标数值还原();
        }
        private void comboBox_扫频_中心频率单位_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_扫频_中心频率单位.SelectedIndex)
            {
                case 0:
                    numericUpDown_扫频_中心频率.Maximum = 20;
                    numericUpDown_扫频_中心频率.Minimum = 0;
                    numericUpDown_扫频_中心频率.Increment = 0.01M;
                    numericUpDown_扫频_中心频率.DecimalPlaces = 9;
                    break;
                case 1:
                    numericUpDown_扫频_中心频率.Maximum = 20e3M;
                    numericUpDown_扫频_中心频率.Minimum = 0;
                    numericUpDown_扫频_中心频率.Increment = 1;
                    numericUpDown_扫频_中心频率.DecimalPlaces = 6;
                    break;
                case 2:
                    numericUpDown_扫频_中心频率.Maximum = 20e6M;
                    numericUpDown_扫频_中心频率.Minimum = 0;
                    numericUpDown_扫频_中心频率.Increment = 10;
                    numericUpDown_扫频_中心频率.DecimalPlaces = 3;
                    break;
                default:
                    numericUpDown_扫频_中心频率.Maximum = 20e9M;
                    numericUpDown_扫频_中心频率.Minimum = 0;
                    numericUpDown_扫频_中心频率.Increment = 1;
                    numericUpDown_扫频_中心频率.DecimalPlaces = 0;
                    break;
            }
            中心频率同步数据();
        }
        #endregion
        
        #region 扫描宽度
        private void numericUpDown_扫频_扫宽_ValueChanged(object sender, EventArgs e)
        {
            扫描宽度同步数据();
        }
        private void 扫描宽度同步数据()
        {
            switch (comboBox_扫频_扫频宽度单位.SelectedIndex)
            {
                case 0:
                    扫频_扫宽 = (double)numericUpDown_扫频_扫宽.Value * 1e9;
                    break;
                case 2:
                    扫频_扫宽 = (double)numericUpDown_扫频_扫宽.Value * 1e3;
                    break;
                case 3:
                    扫频_扫宽 = (int)numericUpDown_扫频_扫宽.Value;
                    break;
                default:
                    扫频_扫宽 = (double)numericUpDown_扫频_扫宽.Value * 1e6;
                    break;
            }
            扫频_Y轴标数值还原();
        }
        private void comboBox_扫频_扫描宽度单位_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_扫频_扫频宽度单位.SelectedIndex)
            {
                case 0:
                    numericUpDown_扫频_扫宽.Maximum = 20;
                    numericUpDown_扫频_扫宽.Minimum = 0;
                    numericUpDown_扫频_扫宽.Increment = 0.01M;
                    numericUpDown_扫频_扫宽.DecimalPlaces = 9;
                    break;
                case 1:
                    numericUpDown_扫频_扫宽.Maximum = 20e3M;
                    numericUpDown_扫频_扫宽.Minimum = 0;
                    numericUpDown_扫频_扫宽.Increment = 1;
                    numericUpDown_扫频_扫宽.DecimalPlaces = 6;
                    break;
                case 2:
                    numericUpDown_扫频_扫宽.Maximum = 20e6M;
                    numericUpDown_扫频_扫宽.Minimum = 0;
                    numericUpDown_扫频_扫宽.Increment = 10;
                    numericUpDown_扫频_扫宽.DecimalPlaces = 3;
                    break;
                default:
                    numericUpDown_扫频_扫宽.Maximum = 20e9M;
                    numericUpDown_扫频_扫宽.Minimum = 0;
                    numericUpDown_扫频_扫宽.Increment = 1;
                    numericUpDown_扫频_扫宽.DecimalPlaces = 0;
                    break;
            }
            扫描宽度同步数据();
        }
        #endregion
        
        #region 起始频率
        private void numericUpDown_扫频_起始频率_ValueChanged(object sender, EventArgs e)
        {
            始末同步数据();
        }
        private void 始末同步数据()
        {
            double 起始频率 = 0, 终止频率 =0;
            switch (comboBox_扫频_起始频率单位.SelectedIndex)
            {
                case 0:
                    起始频率 = (double)numericUpDown_扫频_起始频率.Value * 1e9;
                    break;
                case 2:
                    起始频率 = (double)numericUpDown_扫频_起始频率.Value * 1e3;
                    break;
                case 3:
                    起始频率 = (int)numericUpDown_扫频_起始频率.Value;
                    break;
                default:
                    起始频率 = (double)numericUpDown_扫频_起始频率.Value * 1e6;
                    break;
            }
            switch (comboBox_扫频_终止频率单位.SelectedIndex)
            {
                case 0:
                    终止频率 = (double)numericUpDown_扫频_终止频率.Value * 1e9;
                    break;
                case 2:
                    终止频率 = (double)numericUpDown_扫频_终止频率.Value * 1e3;
                    break;
                case 3:
                    终止频率 = (int)numericUpDown_扫频_终止频率.Value;
                    break;
                default:
                    终止频率 = (double)numericUpDown_扫频_终止频率.Value * 1e6;
                    break;
            }
            if (起始频率<终止频率) {
                扫频_扫宽 = 终止频率-起始频率;
                扫频_中心频率 = (起始频率 +终止频率)/2;
            }
            扫频_Y轴标数值还原();
        }
        private void comboBox_扫频_起始频率单位_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_扫频_起始频率单位.SelectedIndex)
            {
                case 0:
                    numericUpDown_扫频_起始频率.Maximum = 20;
                    numericUpDown_扫频_起始频率.Minimum = 0;
                    numericUpDown_扫频_起始频率.Increment = 0.01M;
                    numericUpDown_扫频_起始频率.DecimalPlaces = 9;
                    break;
                case 1:
                    numericUpDown_扫频_起始频率.Maximum = 20e3M;
                    numericUpDown_扫频_起始频率.Minimum = 0;
                    numericUpDown_扫频_起始频率.Increment = 1;
                    numericUpDown_扫频_起始频率.DecimalPlaces = 6;
                    break;
                case 2:
                    numericUpDown_扫频_起始频率.Maximum = 20e6M;
                    numericUpDown_扫频_起始频率.Minimum = 0;
                    numericUpDown_扫频_起始频率.Increment = 10;
                    numericUpDown_扫频_起始频率.DecimalPlaces = 3;
                    break;
                default:
                    numericUpDown_扫频_起始频率.Maximum = 20e9M;
                    numericUpDown_扫频_起始频率.Minimum = 0;
                    numericUpDown_扫频_起始频率.Increment = 1;
                    numericUpDown_扫频_起始频率.DecimalPlaces = 0;
                    break;
            }
            始末同步数据();
        }
        #endregion

        #region 终止频率
        private void numericUpDown_扫频_终止频率_ValueChanged(object sender, EventArgs e)
        {
            始末同步数据();
        }
        private void comboBox_扫频_终止频率单位_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_扫频_终止频率单位.SelectedIndex)
            {
                case 0:
                    numericUpDown_扫频_终止频率.Maximum = 20;
                    numericUpDown_扫频_终止频率.Minimum = 0;
                    numericUpDown_扫频_终止频率.Increment = 0.01M;
                    numericUpDown_扫频_终止频率.DecimalPlaces = 9;
                    break;
                case 1:
                    numericUpDown_扫频_终止频率.Maximum = 20e3M;
                    numericUpDown_扫频_终止频率.Minimum = 0;
                    numericUpDown_扫频_终止频率.Increment = 1;
                    numericUpDown_扫频_终止频率.DecimalPlaces = 6;
                    break;
                case 2:
                    numericUpDown_扫频_终止频率.Maximum = 20e6M;
                    numericUpDown_扫频_终止频率.Minimum = 0;
                    numericUpDown_扫频_终止频率.Increment = 10;
                    numericUpDown_扫频_终止频率.DecimalPlaces = 3;
                    break;
                default:
                    numericUpDown_扫频_终止频率.Maximum = 20e9M;
                    numericUpDown_扫频_终止频率.Minimum = 0;
                    numericUpDown_扫频_终止频率.Increment = 1;
                    numericUpDown_扫频_终止频率.DecimalPlaces = 0;
                    break;
            }
            始末同步数据();
        }
        #endregion

        #region RBW
        private void numericUpDown_扫频_RBW_ValueChanged(object sender, EventArgs e)
        {
            RBW数据同步();
        }

        private void comboBox_扫频_RBW单位_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_扫频_RBW单位.SelectedIndex)
            {
                case 0:
                    numericUpDown_扫频_RBW.Maximum = 3;
                    numericUpDown_扫频_RBW.Minimum = 3e-4M;
                    numericUpDown_扫频_RBW.Increment = 1e-6M;
                    numericUpDown_扫频_RBW.DecimalPlaces = 9;
                    break;
                case 1:
                    numericUpDown_扫频_RBW.Maximum = 3e3M;
                    numericUpDown_扫频_RBW.Minimum = 3e-1M;
                    numericUpDown_扫频_RBW.Increment = 1e-3M;
                    numericUpDown_扫频_RBW.DecimalPlaces = 6;
                    break;
                case 2:
                    numericUpDown_扫频_RBW.Maximum = 3e6M;
                    numericUpDown_扫频_RBW.Minimum = 300;
                    numericUpDown_扫频_RBW.Increment = 1;
                    numericUpDown_扫频_RBW.DecimalPlaces = 3;
                    break;
                default:
                    numericUpDown_扫频_RBW.Maximum = 3e9M;
                    numericUpDown_扫频_RBW.Minimum = 3e5M;
                    numericUpDown_扫频_RBW.Increment = 1;
                    numericUpDown_扫频_RBW.DecimalPlaces = 0;
                    break;
            }
            RBW数据同步();
        }

        private void RBW数据同步() {
            switch (comboBox_扫频_RBW单位.SelectedIndex)
            {
                case 0:
                    扫频_RBW = (double)numericUpDown_扫频_RBW.Value * 1e9;
                    break;
                case 2:
                    扫频_RBW = (double)numericUpDown_扫频_RBW.Value * 1e3;
                    break;
                case 3:
                    扫频_RBW = (int)numericUpDown_扫频_RBW.Value;
                    break;
                default:
                    扫频_RBW = (double)numericUpDown_扫频_RBW.Value * 1e6;
                    break;
            }
            扫频_Y轴标数值还原();
        }
        #endregion

        #region VBW
        private void numericUpDown_扫频_VBW_ValueChanged(object sender, EventArgs e)
        {
            VBW数据同步();
        }

        private void comboBox_扫频_VBW单位_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_扫频_VBW单位.SelectedIndex)
            {
                case 0:
                    numericUpDown_扫频_VBW.Maximum = 3;
                    numericUpDown_扫频_VBW.Minimum = 3e-4M;
                    numericUpDown_扫频_VBW.Increment = 1e-6M;
                    numericUpDown_扫频_VBW.DecimalPlaces = 9;
                    break;
                case 1:
                    numericUpDown_扫频_VBW.Maximum = 3e3M;
                    numericUpDown_扫频_VBW.Minimum = 3e-1M;
                    numericUpDown_扫频_VBW.Increment = 1e-3M;
                    numericUpDown_扫频_VBW.DecimalPlaces = 6;
                    break;
                case 2:
                    numericUpDown_扫频_VBW.Maximum = 3e6M;
                    numericUpDown_扫频_VBW.Minimum = 300;
                    numericUpDown_扫频_VBW.Increment = 1;
                    numericUpDown_扫频_VBW.DecimalPlaces = 3;
                    break;
                default:
                    numericUpDown_扫频_VBW.Maximum = 3e9M;
                    numericUpDown_扫频_VBW.Minimum = 3e5M;
                    numericUpDown_扫频_VBW.Increment = 1;
                    numericUpDown_扫频_VBW.DecimalPlaces = 0;
                    break;
            }
            VBW数据同步();
        }

        private void VBW数据同步()
        {
            switch (comboBox_扫频_VBW单位.SelectedIndex)
            {
                case 0:
                    扫频_VBW = (double)numericUpDown_扫频_VBW.Value * 1e9;
                    break;
                case 2:
                    扫频_VBW = (double)numericUpDown_扫频_VBW.Value * 1e3;
                    break;
                case 3:
                    扫频_VBW = (int)numericUpDown_扫频_VBW.Value;
                    break;
                default:
                    扫频_VBW = (double)numericUpDown_扫频_VBW.Value * 1e6;
                    break;
            }
            扫频_Y轴标数值还原();
        }
        #endregion
        private void comboBox_扫频_RBW形状_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_扫频_RBW单位.SelectedIndex) {
                case 1:
                    扫频_RBW形状 = SmWindowType.smWindowNutall;
                    break;
                case 2:
                    扫频_RBW形状 = SmWindowType.smWindowBlackman;
                    break;
                case 3:
                    扫频_RBW形状 = SmWindowType.smWindowHamming;
                    break;
                case 4:
                    扫频_RBW形状 = SmWindowType.smWindowGaussian6dB;
                    break;
                case 5:
                    扫频_RBW形状 = SmWindowType.smWindowRect;
                    break;
                default:
                    扫频_RBW形状 = SmWindowType.smWindowFlatTop;
                    break;
            }
            扫频_Y轴标数值还原();
        }
        private void comboBox_扫频_视频单位_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_扫频_视频单位.SelectedIndex) {
                case 1:
                    扫频_视频单位 = SmVideoUnits.smVideoVoltage;
                    break;
                case 2:
                    扫频_视频单位 = SmVideoUnits.smVideoPower;
                    break;
                case 3:
                    扫频_视频单位 = SmVideoUnits.smVideoSample;
                    break;
                default:
                    扫频_视频单位 = SmVideoUnits.smVideoLog;
                    break;
            }
            扫频_Y轴标数值还原();
        }
        private void comboBox_扫频_检波器_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_扫频_检波器.SelectedIndex == 0)
            {
                扫频_检波器 = SmDetector.smDetectorMinMax;
            }
            else
            {
                扫频_检波器 = SmDetector.smDetectorAverage;
            }
            扫频_Y轴标数值还原();
        }
        private void comboBox_扫频_返回单位_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_扫频_返回单位.SelectedIndex == 0) {
                扫频_返回单位 = SmScale.smScaleLog;
            }
            else {
                扫频_返回单位 = SmScale.smScaleLin;
            }
            扫频_Y轴标数值还原();
        }
        #region 图软拒
        private void radioButton_扫频_图像拒绝算法_启用_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_扫频_图像拒绝算法_启用.Checked) {
                扫频_启用软拒 = SmBool.smTrue;
            }
            else {
                扫频_启用软拒 = SmBool.smFalse;
            }
            扫频_Y轴标数值还原();
        }

        private void radioButton_扫频_图像拒绝算法_禁用_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_扫频_图像拒绝算法_禁用.Checked)
            {
                扫频_启用软拒 = SmBool.smFalse;
            }
            else
            {
                扫频_启用软拒 = SmBool.smTrue;
            }
            扫频_Y轴标数值还原();
        }

        #endregion

        #endregion
        private SmStatus 扫频_加载参数()
        {
            MineSM200B.方法_配置设备_衰减与参考电平(Isx64,Equipment_Num, 扫频_衰减, 扫频_参考电平);
            return MineSM200B.方法_配置设备_扫频配置_统一配置_中心(Isx64,Equipment_Num,
                SmSweepSpeed.smSweepSpeedAuto, 扫频_中心频率, 扫频_扫宽, 扫频_RBW, 扫频_VBW, 0.001,
                扫频_检波器, 扫频_视频单位, 扫频_返回单位, 扫频_RBW形状, 扫频_启用软拒);
        }

        private void 停止扫描()
        {
            扫频_TimerAutoGetSweep = false;
        }
        private void button_扫频停止_Click(object sender, EventArgs e)
        {
            停止任何活动();
            方法_设为空闲(); 清空chart();
        }
        private void button_扫频单次_Click(object sender, EventArgs e)
        {
            停止任何活动();
            try
            {
                设置并查询();
            } catch { showMessage(5, "(0x09)扫频获取失败"); }
        }
        private void button_扫频自动_Click(object sender, EventArgs e)
        {
            if (IQ_TimerAutoGet||音频_播放) {
                停止任何活动();
            }
            if (!扫频_TimerAutoGetSweep) {
                扫频_TimerAutoGetSweep = true;
                 循环扫描();

            }

        }
        private async Task 循环扫描()
        {
            while (true){
                if (扫频_TimerAutoGetSweep)
                {
                    await 设置并查询();
                    await Task.Delay(10);
                }else{break;}
            }
        }
        private async Task 设置并查询() {
            if (扫频_加载参数() >= 0)
            {
                await 扫频_获取结果();
            }
            else {
                //提示:参数错误
                showMessage(2, "设备可能未连接");
            }
        }
        private async Task 扫频_获取结果() {
            Stopwatch sw = new Stopwatch(); sw.Start();
            double rbw = 0, vbw = 0, sf = 0, b = 0; long eee = 0;
            int sweepLength = 0;
            try
            {
                if (chart1.IsHandleCreated)
                {
                    temp_SweepResult = MineSM200B.方法_获取结果_扫频_获取结果(Isx64,Equipment_Num, ref rbw, ref vbw, ref sf, ref b, ref eee);
                    float[] sweepMin = (float[])temp_SweepResult[0];
                    float[] sweepMax = (float[])temp_SweepResult[1];
                    sweepLength = sweepMax.Length;
                    if (sweepLength > 0)
                    {
                        if (sweepLength > chart_固定长度)
                        {
                            float[] sweepResult = new float[chart_固定长度 * 2];
                            double[] sweepResultY = new double[chart_固定长度 * 2];
                            double 间隔 = (double)sweepLength / (double)chart_固定长度;
                            for (int i = 0; i < chart_固定长度; i++)
                            {
                                sweepResultY[i * 2] = (间隔 * i * b + sf) / 1e6;//1e6 转MHz
                                sweepResultY[i * 2 + 1] = (间隔 * (i + 1) * b + sf) / 1e6;
                                sweepResult[i * 2] = sweepMin[(int)(间隔 * i)];
                                sweepResult[i * 2 + 1] = sweepMax[(int)(间隔 * i)];
                            }
                            chart结果显示(sweepResult, sweepResultY,sf/1e6,(sf+sweepLength*b) / 1e6);
                        }
                        else
                        {
                            float[] sweepResult = new float[sweepLength * 2];
                            double[] sweepResultY = new double[sweepLength * 2];
                            for (int i = 0; i < sweepLength; i++)
                            {
                                sweepResultY[i * 2] = (i * b + sf) / 1e6;
                                sweepResultY[i * 2 + 1] = ((i + 1) * b + sf) / 1e6;
                                sweepResult[i * 2] = sweepMin[i];
                                sweepResult[i * 2 + 1] = sweepMax[i];
                            }
                            chart结果显示(sweepResult, sweepResultY, sf / 1e6, (sf + sweepLength * b) / 1e6);
                        }
                    }
                    else { chart1.Series[0].Points.Clear(); }
                    sw.Stop();
                }
            }
            catch
            {
                showMessage(5, "(0x15)设备扫频获取结果错误");
            }
            await Task.Run(new Action(() => {
                if (label_显示区_扫描间隔.IsHandleCreated) {
                    label_显示区_扫描间隔.Invoke(new 无参(()=> {
                        label_显示区_扫描间隔.Text = "扫描间隔:" + sw.Elapsed.TotalMilliseconds.ToString() + "ms";
                        label_显示区_左上信息显示.Text = "RBW:" + (rbw / 1000) + "KHz"
                        + "\nVBW:" + (vbw / 1000) + "KHz"
                        + "\nPts:" + ((double)sweepLength * 2) / 1e3 + "K"
                        + "\nTime:" + Loc_GMTTimeToString_us(eee);
                    }));
                    if (扫频_返回单位 == SmScale.smScaleLin)
                    {
                        X轴单位 = "mV";
                    }
                    else {
                        X轴单位 = "dBm";
                    }
                    Y轴单位 = "MHz";

                }
            }));
        }
        private void chart结果显示(float[] temp, double[] tempX,double Sp,double Ep)
        {
            double temp_max = temp[0], temp_min = temp[0];
            for (int i=0;i<temp.Length;i++) {
                if (temp[i] > temp_max) {
                    temp_max = temp[i];
                }
                if (temp[i]<temp_min) {
                    temp_min = temp[i];
                }

            }

            if (double.IsNaN(扫频_Y轴最大))
            {
                扫频_Y轴最大 = temp_max;
            }
            else {
                if (扫频_Y轴最大 < temp_max)
                {
                    扫频_Y轴最大 = temp_max;
                }
            }
            if (double.IsNaN(扫频_Y轴最小)) {
                扫频_Y轴最小 = temp_min;
            }
            else {
                if (扫频_Y轴最小 > temp_min)
                {
                    扫频_Y轴最小 = temp_min;
                }
            }
            double 差值 = 扫频_Y轴最大 - 扫频_Y轴最小;

            chart1.ChartAreas[0].AxisY.Maximum = (扫频_Y轴最大+差值/5);
            chart1.ChartAreas[0].AxisY.Minimum = (扫频_Y轴最小- 差值 / 5);
            if (Ep>Sp) {
                chart1.ChartAreas[0].AxisX.Maximum = Ep;
                chart1.ChartAreas[0].AxisX.Minimum = Sp;
            }

            chart1.Series[0].Points.DataBindXY(tempX, temp);
        }
        private void 扫频_Y轴标数值还原() {
            扫频_Y轴最大 = double.NaN;
            扫频_Y轴最小 = double.NaN;
        }
        #endregion

        #region IQ采集
        //设置参数
        int IQ_衰减 = -1,IQ_采样率 = 0;
        double IQ_中心频率 = 8160e6, IQ_参考电平 = 0, IQ_带宽 = 41.5e6;
        SmTriggerEdge IQ_触发沿 = SmTriggerEdge.smTriggerEdgeRising;
        ArrayList temp_IQResult = new ArrayList();
        double IQ_自动采集时长_s = 1e-3;
        private bool IQ_TimerAutoGet = false;
        FFT fft = new FFT();
        double IQ_Y轴最小 = double.NaN, IQ_Y轴最大 = double.NaN;
        //记录======
        string IQ_保存路径 = "",IQ_文件前缀 = "IQD";
        double IQ_记录时长_s = 1;
        bool IQ_正在记录 = false;
        //==========


        //获取参数
        double iqSampleRate = 0, iqBandwidth = 0, avg_p = 0;
        long iqTimeStamp = 0, GetIQLen = 0;
        int sampleLoss = 0, samplesRemaining = 0,CAPTURE_LEN = 0;
        #region 控件响应

        private void comboBox_IQ_衰减值_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                switch (comboBox_IQ_衰减值.SelectedIndex)
                {
                    case 1:
                        IQ_衰减 = 0;
                        break;
                    case 2:
                        IQ_衰减 = 1;
                        break;
                    case 3:
                        IQ_衰减 = 2;
                        break;
                    case 4:
                        IQ_衰减 = 3;
                        break;
                    case 5:
                        IQ_衰减 = 4;
                        break;
                    case 6:
                        IQ_衰减 = 5;
                        break;
                    case 7:
                        IQ_衰减 = 6;
                        break;
                    default:
                        IQ_衰减 = -1;
                        break;
                }
                IQ_Y轴标数值还原();
            }
            catch { showMessage(5, "(0x16)设备IQ衰减配置错误"); }
        }
        private void numericUpDown_IQ_参考电平_ValueChanged(object sender, EventArgs e)
        {
            IQ_参考电平 = (double)numericUpDown_IQ_参考电平.Value; IQ_Y轴标数值还原();
        }
        #region 中心频率
        private void IQ_中心频率同步数据()
        {
            switch (comboBox_IQ_中心频率单位.SelectedIndex)
            {
                case 0:
                    IQ_中心频率 = (double)numericUpDown_IQ_中心频率.Value * 1e9;
                    break;
                case 2:
                    IQ_中心频率 = (double)numericUpDown_IQ_中心频率.Value * 1e3;
                    break;
                case 3:
                    IQ_中心频率 = (int)numericUpDown_IQ_中心频率.Value;
                    break;
                default:
                    IQ_中心频率 = (double)numericUpDown_IQ_中心频率.Value * 1e6;
                    break;
            }
            IQ_Y轴标数值还原();
        }
        private void comboBox_IQ_中心频率单位_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_IQ_中心频率单位.SelectedIndex)
            {
                case 0:
                    numericUpDown_IQ_中心频率.Maximum = 20;
                    numericUpDown_IQ_中心频率.Minimum = 0;
                    numericUpDown_IQ_中心频率.Increment = 0.01M;
                    numericUpDown_IQ_中心频率.DecimalPlaces = 9;
                    break;
                case 1:
                    numericUpDown_IQ_中心频率.Maximum = 20e3M;
                    numericUpDown_IQ_中心频率.Minimum = 0;
                    numericUpDown_IQ_中心频率.Increment = 1;
                    numericUpDown_IQ_中心频率.DecimalPlaces = 6;
                    break;
                case 2:
                    numericUpDown_IQ_中心频率.Maximum = 20e6M;
                    numericUpDown_IQ_中心频率.Minimum = 0;
                    numericUpDown_IQ_中心频率.Increment = 10;
                    numericUpDown_IQ_中心频率.DecimalPlaces = 3;
                    break;
                default:
                    numericUpDown_IQ_中心频率.Maximum = 20e9M;
                    numericUpDown_IQ_中心频率.Minimum = 0;
                    numericUpDown_IQ_中心频率.Increment = 1;
                    numericUpDown_IQ_中心频率.DecimalPlaces = 0;
                    break;
            }
            IQ_中心频率同步数据();
        }
        private void numericUpDown_IQ_中心频率_ValueChanged(object sender, EventArgs e)
        {
            IQ_中心频率同步数据();
        }
        #endregion
        private void numericUpDown_IQ_带宽_ValueChanged(object sender, EventArgs e)
        {
            IQ_带宽 = (double)numericUpDown_IQ_带宽.Value*1e6; IQ_Y轴标数值还原();
        }
        private void comboBox_IQ_触发沿_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_IQ_触发沿.SelectedIndex) {
                case 0:
                    IQ_触发沿 = SmTriggerEdge.smTriggerEdgeRising;
                    break;
                case 1:
                    IQ_触发沿 = SmTriggerEdge.smTriggerEdgeFalling;
                    break;
                default:
                    IQ_触发沿 = SmTriggerEdge.smTriggerEdgeRising;
                    break;
            }
            IQ_Y轴标数值还原();
        }
        private void numericUpDown_IQ_采集大小_ValueChanged(object sender, EventArgs e)
        {
            IQ_自动采集时长_s = (double)numericUpDown_IQ_采集大小.Value*1e-3; IQ_Y轴标数值还原();
        }
        private void comboBox_IQ_采样率_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (comboBox_IQ_采样率.SelectedIndex > 0)
            {
                if (comboBox_IQ_采样率.SelectedIndex > 1)
                {
                    IQ_采样率 = (int)(Math.Pow(2, comboBox_IQ_采样率.SelectedIndex - 1));
                    numericUpDown_IQ_带宽.Maximum = (decimal)(20 / Math.Pow(2, comboBox_IQ_采样率.SelectedIndex - 2));
                    numericUpDown_IQ_带宽.Minimum = 0;
                }
                else {
                    IQ_采样率 = (int)(Math.Pow(2, comboBox_IQ_采样率.SelectedIndex - 1));
                    numericUpDown_IQ_带宽.Maximum = 41.5M;
                    numericUpDown_IQ_带宽.Minimum = 0;
                }
            }
            else {//250Ms
                IQ_采样率 = (int)250e6;
                numericUpDown_IQ_带宽.Maximum = 160;
                numericUpDown_IQ_带宽.Minimum = 160;
                numericUpDown_IQ_采集大小.Maximum = 1;
                numericUpDown_IQ_采集大小.Minimum = 0.01M;

            }
            IQ_Y轴标数值还原();
        }


        //=================================记录==============================
        private  void button_iq记录_路径选择_Click(object sender, EventArgs e)
        {
            FolderSelectDialog fbd = new FolderSelectDialog();
            if (fbd.ShowDialog(this.Handle))
            {
                textBox_iq记录_保存路径.Invoke(new 无参(() =>
                {
                    IQ_保存路径 =  textBox_iq记录_保存路径.Text = fbd.FileName;
                }));
            }

        }
        private void textBox_iq记录_文件名_TextChanged(object sender, EventArgs e)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"^[A-Za-z0-9]+$");
            if (reg.IsMatch(textBox_iq记录_文件名.Text))
            {
                IQ_文件前缀 = textBox_iq记录_文件名.Text;
            }
            else {
                MessageBox.Show("目前只支持英文字母与数字");
                IQ_文件前缀 = textBox_iq记录_文件名.Text= "IQD";
            }

        }

        private void numericUpDown_iq记录_ValueChanged(object sender, EventArgs e)
        {
            IQ_记录时长_s = (double)numericUpDown_iq记录.Value/1000;
        }
        private void button_iq记录_Click(object sender, EventArgs e)
        {
            停止任何活动();
            清空chart();
            if (IQ_正在记录) {
                
                IQ_取消记录();
            }
            else { 停止任何活动(); IQ_记录文件(); }
        }
        #endregion

        private SmStatus IQ_加载参数()
        {
            MineSM200B.方法_配置设备_衰减与参考电平(Isx64,Equipment_Num, IQ_衰减, IQ_参考电平);
            return MineSM200B.方法_配置设备_IQ_建议配置(Isx64,Equipment_Num,IQ_中心频率,IQ_采样率,IQ_带宽,IQ_触发沿);
        }
        private SmStatus 分段IQ_加载参数(double 采集时间_s)
        {
            CAPTURE_LEN = (int)(250e6 * 采集时间_s);
            return MineSM200B.方法_配置设备_IQ分段_建议配置(Isx64,Equipment_Num,IQ_衰减,IQ_参考电平 ,SmDataType.smDataType32fc,IQ_中心频率
                ,SmTriggerType.smTriggerTypeImmediate,0
                , CAPTURE_LEN, 0.0);
        }
        private void 停止IQ()
        {
            IQ_TimerAutoGet = false;
        }
        private void button_IQ单次_Click(object sender, EventArgs e)
        {
            停止任何活动();
            try
            {
                IQ_设置并查询();
            }
            catch { showMessage(5, "(0x19)IQ配置查询发生错误"); }
        }
        private void button_IQ自动_Click(object sender, EventArgs e)
        {
            if (扫频_TimerAutoGetSweep||音频_播放) {
                停止任何活动();
            }
            if (!IQ_TimerAutoGet)
            {
                IQ_TimerAutoGet = true;
                IQ_循环扫描();

            }
        }
        private void button_IQ停止_Click(object sender, EventArgs e)
        {
            停止任何活动();
            方法_设为空闲(); 清空chart();
        }
        private async Task IQ_循环扫描()
        {
            while (true)
            {
                if (IQ_TimerAutoGet)
                {
                    await IQ_设置并查询();
                    await Task.Delay(50);
                }
                else
                {break; }
            }
        }
        private async Task IQ_设置并查询()
        {
            if (IQ_采样率 == 250e6)
            {
                if (分段IQ_加载参数(IQ_自动采集时长_s) >= 0)
                {
                    await 分段IQ_获取结果();
                }
                else {
                    //提示:参数错误}
                    showMessage(2, "设备可能未连接");
                }
            }
            else
            {
                if (IQ_加载参数() >= 0)
                {
                    await IQ_获取结果(IQ_自动采集时长_s);
                }
                else
                {
                    //提示:参数错误}
                    showMessage(2, "设备可能未连接");
                }
            }

        }

        private async Task IQ_获取结果(double 采集时长)
        {
            Stopwatch sw = new Stopwatch(); sw.Start();
            int iqRLen = 0;
            int temp_i = 0;
            float avg_P = 0;
            if (chart1.IsHandleCreated) {
                    sw.Start();
                    temp_IQResult = MineSM200B.方法_获取结果_IQ_默认数据(Isx64,Equipment_Num, 采集时长, ref iqSampleRate, ref iqBandwidth
                    , ref iqTimeStamp, ref sampleLoss, ref samplesRemaining);
                    sw.Stop();

                #region normal
                try
                {
                    float[] temp_iqR = (float[])(temp_IQResult[0]);
                    iqRLen = temp_iqR.Length / 2;
                    GetIQLen = iqRLen;
                    if (iqRLen > 0)
                    {
                        if (iqRLen > chart_固定长度)
                        {
                            double[] iqRX = new double[chart_固定长度];
                            float[] iqRY = new float[chart_固定长度];
                            double 时间间隔 = (采集时长 * 1000000) / chart_固定长度;
                            int 获取间隔 = iqRLen / chart_固定长度;
                            for (int i = 0; i < chart_固定长度; i++)
                            {
                                float f_r = temp_iqR[i * 2 * 获取间隔];
                                float f_i = temp_iqR[(i * 2 + 1) * 获取间隔];


                                iqRY[i] = 10 * (float)Math.Log10(Math.Pow(f_i, 2) + Math.Pow(f_r, 2));
                                if (float.IsInfinity(iqRY[i]))
                                {
                                    iqRY[i] = float.NaN;
                                }
                                else
                                {
                                    avg_P += iqRY[i];
                                    temp_i++;
                                }
                                iqRX[i] = i * 时间间隔;
                            }
                            IQ_chart结果显示(iqRY, iqRX, iqRX[0], iqRX[iqRX.Length - 1]);
                        }
                        else
                        {
                            double[] iqRX = new double[iqRLen];
                            float[] iqRY = new float[iqRLen];
                            double 时间间隔 = (采集时长 * 1000000) / iqRLen;
                            for (int i = 0; i < iqRLen; i++)
                            {
                                float f_r = temp_iqR[i * 2];
                                float f_i = temp_iqR[i * 2 + 1];
                                iqRY[i] = 10 * (float)Math.Log10(Math.Pow(f_i, 2) + Math.Pow(f_r, 2));
                                if (float.IsInfinity(iqRY[i]))
                                {
                                    iqRY[i] = float.NaN;
                                }
                                else
                                {
                                    avg_P += iqRY[i];
                                    temp_i++;
                                }
                                iqRX[i] = i * 时间间隔;
                            }
                            IQ_chart结果显示(iqRY, iqRX, iqRX[0], iqRX[iqRX.Length - 1]);
                        }
                    }
                    await Task.Run(new Action(() => {
                        if (label_显示区_扫描间隔.IsHandleCreated)
                        {
                            X轴单位 = "dBm";
                            Y轴单位 = "μs";
                            label_显示区_扫描间隔.Invoke(new 无参(() => {
                                label_显示区_扫描间隔.Text = "扫描间隔:" + sw.Elapsed.TotalMilliseconds.ToString() + "ms";
                                label_显示区_左上信息显示.Text = "SampleRate:" + (iqSampleRate / 1e3).ToString("0.000") + "KS/s"
                                + "\nBandWidth:" + (iqBandwidth / 1e3).ToString("0.000") + "KHz"
                                + "\nPoints:" + GetIQLen
                                + "\nTime:" + Loc_GMTTimeToString_us(iqTimeStamp)
                                + "\nLoss:" + sampleLoss
                                + "\nRemaining:" + samplesRemaining
                                + "\nAvg_P:" + avg_P / temp_i + "dBm";
                            }));
                        }
                    }));
                }
                catch
                {
                    IQ_TimerAutoGet = false;
                    showMessage(5, "(0x0A)IQ获取结果错误");
                }
                #endregion
            }
        }
        private async Task 分段IQ_获取结果()
        {
            Stopwatch sw = new Stopwatch(); sw.Start();
            int iqRLen = 0;
            int temp_i = 0;
            float avg_P = 0;
            try {
                #region normal
                sw.Start();
                ArrayList temp_segIQResult = MineSM200B.方法_获取结果_IQ分段(Isx64,Equipment_Num, CAPTURE_LEN);
                sw.Stop();
                if (temp_segIQResult.Count > 0)
                {

                    for (int i = 0; i < temp_segIQResult.Count; i++)
                    {
                        iqRLen += ((float[])temp_segIQResult[i]).Length;
                    }
                    float[] iqR = new float[iqRLen];
                    int j = 0, k = 0;
                    for (int i = 0; i < iqRLen; i++)
                    {
                        if (j < ((float[])temp_segIQResult[k]).Length)
                        {
                            iqR[i] = ((float[])temp_segIQResult[k])[j];
                            j++;
                        }
                        else
                        {
                            k++;
                            j = 0;
                        }

                    }
                    if (iqRLen / 2 > chart_固定长度)
                    {
                        double[] iqRX = new double[chart_固定长度];
                        float[] iqRY = new float[chart_固定长度];
                        double 时间间隔 = (iqRLen / 2 / 250e6) * 1000000 / chart_固定长度;
                        int 获取间隔 = iqRLen / 2 / chart_固定长度;
                        for (int i = 0; i < chart_固定长度; i++)
                        {
                            float f_r = iqR[i * 2 * 获取间隔];
                            float f_i = iqR[(i * 2 + 1) * 获取间隔];


                            iqRY[i] = 10 * (float)Math.Log10(Math.Pow(f_i, 2) + Math.Pow(f_r, 2));
                            if (float.IsInfinity(iqRY[i]))
                            {
                                iqRY[i] = float.NaN;
                            }
                            else
                            {
                                avg_P += iqRY[i];
                                temp_i++;
                            }
                            iqRX[i] = i * 时间间隔;
                        }
                        IQ_chart结果显示(iqRY, iqRX, iqRX[0], iqRX[iqRX.Length - 1]);
                    }
                    else
                    {
                        double[] iqRX = new double[iqRLen / 2];
                        float[] iqRY = new float[iqRLen / 2];
                        double 时间间隔 = (double)(iqRLen / 2 / 250e6) * 1000000 / iqRLen;
                        for (int i = 0; i < iqRLen/2; i++)
                        {
                            float f_r = iqR[i * 2];
                            float f_i = iqR[i * 2 + 1];
                            iqRY[i] = 10 * (float)Math.Log10(Math.Pow(f_i, 2) + Math.Pow(f_r, 2));
                            if (float.IsInfinity(iqRY[i]))
                            {
                                iqRY[i] = float.NaN;
                            }
                            else
                            {
                                avg_P += iqRY[i];
                                temp_i++;
                            }
                            iqRX[i] = i * 时间间隔;
                        }
                        IQ_chart结果显示(iqRY, iqRX, iqRX[0], iqRX[iqRX.Length - 1]);
                    }
                    await Task.Run(new Action(() =>
                    {
                        X轴单位 = "dBm";
                        Y轴单位 = "μs";
                        if (label_显示区_扫描间隔.IsHandleCreated)
                        {
                            label_显示区_扫描间隔.Invoke(new 无参(() =>
                            {
                                label_显示区_扫描间隔.Text = "扫描间隔:" + sw.Elapsed.TotalMilliseconds.ToString("0.000ms");
                                label_显示区_左上信息显示.Text = "SampleRate:250MS/s"
                                + "\nBandWidth:160MHz"
                                + "\nPoints:" + iqRLen/2
                                + "\nAvg_P:" + avg_P / temp_i + "dBm";
                            }));
                        }
                    }));
                }
                #endregion
            }
            catch { IQ_TimerAutoGet = false;
                showMessage(5, "(0x0B)IQ分段获取结果错误");
            }

        }

        private void IQ_Y轴标数值还原()
        {
            IQ_Y轴最大 = double.NaN;
            IQ_Y轴最小 = double.NaN;
        }
        private void IQ_chart结果显示(float[] temp, double[] tempX,double st,double et)
        {
            double temp_max = temp[0], temp_min = temp[0];
            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i] > temp_max)
                {
                    temp_max = temp[i];
                }
                if (temp[i] < temp_min)
                {
                    temp_min = temp[i];
                }

            }

            if (double.IsNaN(IQ_Y轴最大))
            {
                IQ_Y轴最大 = temp_max;
            }
            else
            {
                if (IQ_Y轴最大 < temp_max)
                {
                    IQ_Y轴最大 = temp_max;
                }
            }
            if (double.IsNaN(IQ_Y轴最小))
            {
                IQ_Y轴最小 = temp_min;
            }
            else
            {
                if (IQ_Y轴最小 > temp_min)
                {
                    IQ_Y轴最小 = temp_min;
                }
            }
            
            double 差值 = IQ_Y轴最大 - IQ_Y轴最小;
            //double x差值 = et - st;
            chart1.ChartAreas[0].AxisY.Maximum = (IQ_Y轴最大 + 差值 / 5);
            chart1.ChartAreas[0].AxisY.Minimum = (IQ_Y轴最小 - 差值 / 5);
            if (et > st)
            {
                chart1.ChartAreas[0].AxisX.Maximum = et ;
                chart1.ChartAreas[0].AxisX.Minimum = st ;
            }

            chart1.Series[0].Points.DataBindXY(tempX, temp);
        }

        //======================================IQ记录文件==================
        private void 记录时控件取消使用(bool v) {
            if (button_iq记录.IsHandleCreated) {
                button_iq记录.Invoke(new 无参(() =>
                {
                    label_主界面按钮.Enabled = label_扫频按钮.Enabled = label_其他相关按钮.Enabled = label_临时数据按钮.Enabled = numericUpDown_iq记录.Enabled = textBox_iq记录_文件名.Enabled = button_iq记录_路径选择.Enabled = button_IQ停止.Enabled = button_IQ自动.Enabled = button_IQ单次.Enabled = !v;
                }));
            }   
        }
        private async void IQ_记录文件()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            ManualResetEvent resetEvent = new ManualResetEvent(true);
            String 路径 = IQ_保存路径 + "//" + IQ_文件前缀 + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".SMD";
            IQ_记录显示(true);
            if (IQ_采样率 == 250e6) {
                CancellationToken token_transcribe = tokenSource.Token;
                await Task.Run(() => {
                    if (分段IQ_加载参数(IQ_记录时长_s) < 0)
                    {
                        //警告：设备未连接
                        showMessage(4, "设备可能未连接");
                        tokenSource.Cancel();
                        return;
                    }
                    int iqRLen = 0;
                    ArrayList temp_segIQResult = MineSM200B.方法_获取结果_IQ分段(Isx64, Equipment_Num,CAPTURE_LEN);
                    //250MSa/s缺少部分信息
                    IQData iqd = new IQData(); 
                    iqd.Attenuator = IQ_衰减; 
                    iqd.BandWidth = (int)160e6; iqd.CaptureSize = IQ_记录时长_s; iqd.CenterF = IQ_中心频率; 
                     iqd.RefLevel = IQ_参考电平; 
                    iqd.SampleRate = 250e6;
                    iqd.Is250 = true;iqd.segIQResult_32f = temp_segIQResult;

                    try
                    {

                        using (FileStream fs = new FileStream(路径, FileMode.Create))
                        {
                            BinaryFormatter bf = new BinaryFormatter(); bf.Serialize(fs, iqd);
                        }

                    }
                    catch (Exception Err)
                    {
                        showMessage(5, "(0x20)IQ流记录文件过程发生错误,可能需要管理员权限。" + Err.Message);
                    }
                    try
                    {
                        if (label_iq记录_文本_文件大小.IsHandleCreated)
                        {
                            label_iq记录_文本_文件大小.Invoke(new 无参(() =>
                            {
                                label_iq记录_文本_文件大小.Text = "文件大小:" + (((double)FileSize(路径)) / 1000 / 1000) + "MB"; ;
                            }));
                        }
                    }
                    catch { }
                });
                await Task.Run(() =>
                {
                    IQ_记录显示(false);
                    showMessage(2, "记录完成");
                });
            }
            else {
                CancellationToken token_transcribe = tokenSource.Token;
                await Task.Run(() =>
                {
                    if (IQ_加载参数() < 0)
                    {
                        //警告：设备未连接
                        showMessage(4, "设备可能未连接");
                        tokenSource.Cancel();
                        return;
                    }
                    int iqRLen = 0;
                    temp_IQResult = MineSM200B.方法_获取结果_IQ_默认数据(Isx64, Equipment_Num, IQ_记录时长_s, ref iqSampleRate, ref iqBandwidth
                        , ref iqTimeStamp, ref sampleLoss, ref samplesRemaining);
                    float[] temp_iqR = (float[])(temp_IQResult[0]); iqRLen = temp_iqR.Length / 2; GetIQLen = iqRLen;
                    IQData iqd = new IQData(); iqd.Attenuator = IQ_衰减; iqd.BandWidth = (int)iqBandwidth; iqd.CaptureSize = IQ_记录时长_s; iqd.CenterF = IQ_中心频率; iqd.IQResult_32f = temp_iqR;
                    iqd.IQTime = iqTimeStamp; iqd.RefLevel = IQ_参考电平; iqd.SampleLoss = sampleLoss; iqd.SampleRate = iqSampleRate; iqd.SampleRemaining = samplesRemaining;
                    iqd.Is250 = false;
                    try
                    {

                        using (FileStream fs = new FileStream(路径, FileMode.Create))
                        {
                            BinaryFormatter bf = new BinaryFormatter(); bf.Serialize(fs, iqd);

                        }

                    }
                    catch (Exception Err)
                    {
                        showMessage(5, "(0x20)IQ流记录文件过程发生错误,可能需要管理员权限。" + Err.Message);
                    }
                    try
                    {
                        if (label_iq记录_文本_文件大小.IsHandleCreated)
                        {
                            label_iq记录_文本_文件大小.Invoke(new 无参(() =>
                            {
                                label_iq记录_文本_文件大小.Text = "文件大小:" + (((double)FileSize(路径)) / 1000 / 1000) + "MB"; ;
                            }));
                        }
                    }
                    catch { }
                });
                await Task.Run(() =>
                {
                    IQ_记录显示(false);
                    showMessage(2, "记录完成");
                });
            }
        }
        private void IQ_记录显示(bool v) {
            IQ_正在记录 = v;
            if (button_iq记录.IsHandleCreated)
            {
                progressBar_iq记录.Invoke(new 无参(() => {
                    progressBar_iq记录.Visible = v; if (v) { button_iq记录.Text = "取消记录";
                    } else { button_iq记录.Text = "开始记录"; }
                }));
            }
            记录时控件取消使用(v);
        }
        private void IQ_取消记录() {
            IQ_记录显示(false);
        }

        #endregion

        #region 回放
        IQData temp_iqd;
        bool 当前界面 = false;
        string 回放_IQ文件信息 = null, 回放_扫频文件信息 = null;
        /// <summary>
        /// true为记录界面，false为回放界面
        /// </summary>
        /// <param name="v"></param>
        private void IQ_记录回放界面(bool v)
        {
            if (当前界面 != v)
            {
                当前界面 = v;
                panel_扫频回放界面.Visible = v;
                panel_IQ回放界面.Visible = !v;
                if (!当前界面)
                {
                    label_回放_文件信息.Text = 回放_IQ文件信息;
                }
                else
                {
                    label_回放_文件信息.Text = 回放_扫频文件信息;
                }
            }
        }
        private void 回放_界面设置_读取(bool v) {
            if(groupBox_实时扫频.IsHandleCreated)
            {
                groupBox_实时扫频.Invoke(new 无参(()=> {
                    groupBox_实时扫频.Enabled = !v;
                    progressBar_回放.Visible = v;
                }));
            }
        }
        private void button_IQ采集_记录界面按钮_Click_1(object sender, EventArgs e)
        {
            IQ_记录回放界面(true);
        }
        private void button_IQ采集_回放界面按钮_Click(object sender, EventArgs e)
        {
            IQ_记录回放界面(false);
        }
        private async void button_IQ采集_回放文件选择_Click(object sender, EventArgs e)
        {
            回放_界面设置_读取(true);
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    await Task.Run(() =>
                    {
                        string s = ofd.FileName;
                        using (FileStream fs = new FileStream(s, FileMode.Open))
                        {
                            BinaryFormatter bf = new BinaryFormatter();
                            try
                            {
                                Class.IQData iqd = bf.Deserialize(fs) as Class.IQData;
                                temp_iqd = iqd;
                            }
                            catch { showMessage(5, "无法正常解析文件"); 回放_界面设置_读取(false); return; }
                            showMessage(2, "正在读取文件");
                            if (label_回放_文件信息.IsHandleCreated)
                            {
                                label_回放_文件信息.Invoke(new 无参(() =>
                                {
                                    textBox_回放_IQ路径.Text = s;
                                    回放_IQ文件信息 = label_回放_文件信息.Text = "衰减:" + temp_iqd.Attenuator
                                        + "\n采样率:" + (temp_iqd.SampleRate) / 1e6 + "MS/s";
                                }));
                            }
                        }
                    });
                    回放_界面设置_读取(false);
                }
                catch { showMessage(5, "无法正常解析文件"); 回放_界面设置_读取(false); }
            }
            else { 回放_界面设置_读取(false); }
        }

        int NFFT = 2048;
        private void IQ播放() {
            if (temp_iqd != null)
            {
                if (!temp_iqd.Is250) {
                    Complex[] c = new Complex[temp_iqd.IQResult_32f.Length / 2];
                    for (int i = 0; i < NFFT; i++)
                    {
                        c[i] = new Complex();
                        c[i].Real = temp_iqd.IQResult_32f[2*i];
                        c[i].Imaginary = temp_iqd.IQResult_32f[2 * i+1];
                    }
                    FFT t = new FFT();
                    Complex[] ttt =  t.DFT(c, NFFT);
                    double[] fi = new double[NFFT];
                    double sweep_interval = temp_iqd.BandWidth / NFFT;
                    double[] x = new double[NFFT];
                    for (int i = 0;i< NFFT; i++) {
                    fi[i] = 10 * Math.Log10(ttt[i].Real * ttt[i].Real+ ttt[i].Imaginary * ttt[i].Imaginary);
                        x[i] = temp_iqd.CenterF - temp_iqd.BandWidth / 2 + i * sweep_interval;
                    }
                    double tempd = 0;
                    for (int i = 0;i<NFFT/4;i++) {
                        tempd = fi[i];
                        fi[i] = fi[NFFT/2-i];
                        fi[NFFT / 2-i] = tempd;
                    }
                    for (int i = 0; i < NFFT / 4-1; i++)
                    {
                        tempd = fi[NFFT/2+i+1];
                        fi[NFFT / 2 + i+1] = fi[NFFT - i-1];
                        fi[NFFT - i-1] = tempd;
                    }

                    chart1.Series[0].Points.DataBindXY(x,fi);

                }
            
            }
            else {
                showMessage(4,"未检测到文件");
            }
        
        }

        private void button7_Click(object sender, EventArgs e)
        {
            IQ播放();
        }

        #endregion

        #region 收音机
        CancellationTokenSource source;
        private float[] audioData = new float[1000];
        public Queue<CancellationTokenSource> Cancells = new Queue<CancellationTokenSource>(2);
        bool IsStarPlay = false;
        const int BandsCount = 128;
        int[] FFTPeacks = new int[BandsCount];
        int[] FFTFall = new int[BandsCount];
        int rate = 1500;
        double 音频_中心频率 = 101.7e6, 音频_带宽 = 120000,
            音频_低通 = 3000, 音频_高通 = 20, 音频_去加重 = 75, 音频_Y轴最大 = double.NaN,音频_Y轴最小 = double.NaN;
        SmAudioType 音频_类型 = SmAudioType.smAudioTypeFM;
        bool 音频_播放 = false, 音频_参数变化 = false;
        private void 音频_Y轴标数值还原()
        {
            音频_Y轴最大 = double.NaN;
            音频_Y轴最小 = double.NaN;
        }
        #region 控件响应
        #region 中心频率
        private void numericUpDown_音频处理_中心频率_ValueChanged(object sender, EventArgs e)
        {
            音频_中心频率同步数据();
        }
        private void comboBox_音频处理_中心频率单位_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_音频处理_中心频率单位.SelectedIndex)
            {
                case 0:
                    numericUpDown_音频处理_中心频率.Maximum = 20;
                    numericUpDown_音频处理_中心频率.Minimum = 0;
                    numericUpDown_音频处理_中心频率.Increment = 0.01M;
                    numericUpDown_音频处理_中心频率.DecimalPlaces = 9;
                    break;
                case 1:
                    numericUpDown_音频处理_中心频率.Maximum = 20e3M;
                    numericUpDown_音频处理_中心频率.Minimum = 0;
                    numericUpDown_音频处理_中心频率.Increment = 1;
                    numericUpDown_音频处理_中心频率.DecimalPlaces = 6;
                    break;
                case 2:
                    numericUpDown_音频处理_中心频率.Maximum = 20e6M;
                    numericUpDown_音频处理_中心频率.Minimum = 0;
                    numericUpDown_音频处理_中心频率.Increment = 10;
                    numericUpDown_音频处理_中心频率.DecimalPlaces = 3;
                    break;
                default:
                    numericUpDown_音频处理_中心频率.Maximum = 20e9M;
                    numericUpDown_音频处理_中心频率.Minimum = 0;
                    numericUpDown_音频处理_中心频率.Increment = 1;
                    numericUpDown_音频处理_中心频率.DecimalPlaces = 0;
                    break;
            }
            音频_中心频率同步数据();
        }
        private void 音频_中心频率同步数据()
        {
            switch (comboBox_音频处理_中心频率单位.SelectedIndex)
            {
                case 0:
                    音频_中心频率 = (double)numericUpDown_音频处理_中心频率.Value * 1e9;
                    break;
                case 2:
                    音频_中心频率 = (double)numericUpDown_音频处理_中心频率.Value * 1e3;
                    break;
                case 3:
                    音频_中心频率 = (int)numericUpDown_音频处理_中心频率.Value;
                    break;
                default:
                    音频_中心频率 = (double)numericUpDown_音频处理_中心频率.Value * 1e6;
                    break;

            }
            音频_参数变化 = true;
            音频_Y轴标数值还原();
        }











        #endregion

        private void comboBox_音频处理_音频类型_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_音频处理_音频类型.SelectedIndex) {
                case 0:
                    音频_类型 = SmAudioType.smAudioTypeAM;
                    break;
                case 1:
                    音频_类型 = SmAudioType.smAudioTypeFM;
                    break;
                case 2:
                    音频_类型 = SmAudioType.smAudioTypeCW;
                    break;
                case 3:
                    音频_类型 = SmAudioType.smAudioTypeUSB;
                    break;
                case 4:
                    音频_类型 = SmAudioType.smAudioTypeLSB;
                    break;
                default:
                    音频_类型 = SmAudioType.smAudioTypeFM;
                    break;
            }
            音频_参数变化 = true;
        }

        private void numericUpDown_音频处理_带宽_ValueChanged(object sender, EventArgs e)
        {
            音频_带宽 = (double)numericUpDown_音频处理_带宽.Value * 1e3; 音频_参数变化 = true;
        } 

        private void numericUpDown_音频处理_高通_ValueChanged(object sender, EventArgs e)
        {
            音频_高通 = (double)numericUpDown_音频处理_高通.Value; 音频_参数变化 = true;
        }

        private void numericUpDown_音频处理_低通_ValueChanged(object sender, EventArgs e)
        {
            音频_低通 = (double)numericUpDown_音频处理_低通.Value * 1e3; 音频_参数变化 = true;

        }

        private void numericUpDown_音频处理_去加重_ValueChanged(object sender, EventArgs e)
        {
            音频_去加重 = (double)numericUpDown_音频处理_去加重.Value; 音频_参数变化 = true;
        }


        #endregion





        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button_音频处理_停止_Click(object sender, EventArgs e)
        {
            停止任何活动();
            方法_设为空闲();
            清空chart();
        }

 

        private void button_音频处理_自动_Click(object sender, EventArgs e)
        {
            if (扫频_TimerAutoGetSweep || IQ_TimerAutoGet)
            {
                停止任何活动();
            }
            if (!音频_播放)
            {
                音频_播放 = true;
                音频_循环获取();

            }
        }

        private async void 音频_循环获取()
        {
            try
            {

                WaveOut waveOut = new WaveOut();
                if (Cancells.Count > 0) Cancells.Dequeue().Cancel();
                source = new CancellationTokenSource();
                音频_加载参数();
                await Task.Run(new Action(() => {
                    while (!source.IsCancellationRequested)
                    {
                        if (!音频_播放)
                        {
                            break;
                        }
                        if (音频_参数变化)
                        {
                            Cancells.Enqueue(source);
                            source = new CancellationTokenSource();
                            音频_参数变化 = false;
                            音频_加载参数();
                        }

                        音频_获取结果(waveOut);

                    }
                }));

                //Task.Run(() => {
                   
                //});
                Cancells.Enqueue(source);

            }
            catch (Exception ex)
            {
                showMessage(5, "(0x21)"+ex.Message);
            }
        }

   
        private void 停止音频播放()
        {
            音频_播放 = false;
        }
        private async Task 音频_获取结果(WaveOut waveOut)
        {
            try {
                if (MineSM200B.方法_获取结果_音频(Isx64, Equipment_Num, audioData)
                > 0)
                {
                    waveOut.Play(audioData);
                    if (chart1.IsHandleCreated)
                    {
                        chart1.Invoke(new 无参(() =>
                        {

                            chart1.ChartAreas[0].AxisY.Maximum = 1.5;
                            chart1.ChartAreas[0].AxisY.Minimum = -1.5;
                            chart1.ChartAreas[0].AxisX.Maximum = 1001;
                            chart1.ChartAreas[0].AxisX.Minimum = 0;
                            chart1.Series[0].Points.DataBindY(audioData);
                        }
                        ));
                    }
                }
                else {
                    音频_播放 = false; 停止任何活动();
                    //提示：未连接
                    showMessage(3, "连接或许发生了异常"); 

                }
            } catch { showMessage(5, "(0x22)播放过程错误"); }
        }
        private SmStatus 音频_加载参数()
        {
           return MineSM200B.方法_配置设备_音频_统一配置(Isx64,Equipment_Num,音频_中心频率,音频_类型,音频_带宽, 音频_低通, 音频_高通, 音频_去加重);
        }

        private void datatoFFT(float[] af) {
            if (af.Length<1024) { 
            
            }

        }

        #endregion

        #region 其他方法
        public static long FileSize(string filePath)
        {
            long temp = 0;
            try
            {
                if (File.Exists(filePath) == false)
                {
                    string[] str1 = Directory.GetFileSystemEntries(filePath);
                    foreach (string s1 in str1)
                    {
                        temp += FileSize(s1);
                    }
                }
                else
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    return fileInfo.Length;
                }
            } catch { return 0; }
            return temp;
        }
        protected new bool DesignMode
        {
            get
            {
                bool returnFlag = false;
#if DEBUG
                if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
                    returnFlag = true;
                else if (System.Diagnostics.Process.GetCurrentProcess().ProcessName.ToUpper().Equals("DEVENV"))
                    returnFlag = true;
#endif
                return returnFlag;
            }
        }
        private void 设计模式显示()
        {
            this.panel_设计显示 = new System.Windows.Forms.Panel();
            this.checkBox_设置_自动连接 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel_设计显示.BackColor = System.Drawing.Color.Green;
            this.panel_设计显示.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_设计显示.Location = new System.Drawing.Point(0, 0);
            this.panel_设计显示.Name = "this.panel_设计显示";
            this.panel_设计显示.Size = new System.Drawing.Size(239, 133);
            this.panel_设计显示.TabIndex = 0;
            // 
            // SM200B
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel_设计显示);
            this.Name = "SM200B";
            this.Size = new System.Drawing.Size(239, 133);
            this.ResumeLayout(false);
            // 
            // checkBox_设置_自动连接
            // 
            this.checkBox_设置_自动连接.AutoSize = true;
            this.checkBox_设置_自动连接.Location = new System.Drawing.Point(340, 234);
            this.checkBox_设置_自动连接.Name = "checkBox_设置_自动连接";
            this.checkBox_设置_自动连接.Size = new System.Drawing.Size(78, 16);
            this.checkBox_设置_自动连接.TabIndex = 0;
            this.checkBox_设置_自动连接.Text = "checkBox1";
            this.checkBox_设置_自动连接.UseVisualStyleBackColor = true;
        }
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0014) // 禁掉清除背景消息
                return;
            base.WndProc(ref m);
        }

        private string Loc_GMTTimeToString_s(long time_s)
        {
            string s = null;
            if (time_s > 0)
            {
                s = new DateTime(1970, 1, 1).AddSeconds(time_s).ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss");
            }
            return s;
        }
        private string Loc_GMTTimeToString_s(ulong time_s)
        {
            string s = null;
            if (time_s > 0)
            {
                s = new DateTime(1970, 1, 1).AddSeconds(time_s).ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss");
            }
            return s;
        }
        private string Loc_GMTTimeToString_ms(long time_s)
        {
            string s = null;
            if (time_s > 0)
            {
                s = new DateTime(1970, 1, 1).AddMilliseconds(time_s).ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss");
            }
            return s;
        }
        private string Loc_GMTTimeToString_ms(ulong time_s)
        {
            string s = null;
            if (time_s > 0)
            {
                s = new DateTime(1970, 1, 1).AddMilliseconds(time_s).ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss");
            }
            return s;
        }
        private string Loc_GMTTimeToString_us(long time_s)
        {
            string s = null;
            if (time_s > 0)
            {
                s = new DateTime(1970, 1, 1).AddMilliseconds(time_s / 1e6).ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss");
            }
            return s;
        }
        private string Loc_GMTTimeToString_us(ulong time_s)
        {
            string s = null;
            if (time_s > 0)
            {
                s = new DateTime(1970, 1, 1).AddMilliseconds(time_s / 1e6).ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss");
            }
            return s;
        }


        [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize")]
        public static extern int SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);
        /// <summary>
        /// 释放内存
        /// </summary>
        public static void ClearMemory()

        {

            GC.Collect();

            GC.WaitForPendingFinalizers();

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)

            {

                SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);

            }

        }
        #endregion



        #region 临时测试

        private void button_IQ采集_生成文件_Click(object sender, EventArgs e)
        {
            test();
        }
        private void button_IQ采集_选择路径_Click(object sender, EventArgs e)
        {
            test2();
        }

    
    
        private void test()
        {
            Class.IQData iqd = new Class.IQData();
            iqd.Attenuator = -1;
            iqd.BandWidth = (int)20e6;
            iqd.CaptureSize = 1e-3;
            iqd.CenterF = 8.16e9;
            iqd.IQResult_32f = new float[300000];
            iqd.IQTime = (long)1e10;
            iqd.RefLevel = -20;
            iqd.SampleLoss = 0;
            iqd.SampleRate = (int)250e6;
            iqd.SampleRemaining = 0;
            

            using (FileStream fs = new FileStream("1.txt",FileMode.Create)) {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs,iqd);
            }
            //timer_chart控件使用.
        }

    
        private void test2()
        {
            //IQD_20200414140804.SMD
            //IQD_20200414141101.SMD
            using (FileStream fs = new FileStream("C:/Users/Linda/Desktop/IQD_20200414140804.SMD", FileMode.Open)) {
                BinaryFormatter bf = new BinaryFormatter();
                Class.IQData iqd = bf.Deserialize(fs) as Class.IQData;
                temp_iqd = iqd;
                //if (iqd != null) {
                //    MessageBox.Show(iqd.IQResult_32f.Length.ToString());

                //    if (iqd.IQResult_16s == null)
                //    {
                //        MessageBox.Show("暂无16型");
                //    }
                //}


            }
        }

        public void test3() {
            float[] temp_f = temp_iqd.IQResult_32f;
            if (temp_f.Length > 4000)
            {
                int ti = temp_f.Length;
                ti = ti / 4000;

                float[] temppf = new float[4000];

                for (int i = 0; i < 4000; i++)
                {
                    temppf[i] = temp_f[ti * i];
                }
                chart1.Series[0].Points.DataBindY(temppf);
            }
            else
            {
                chart1.Series[0].Points.DataBindY(temp_iqd.IQResult_32f);
            }
        }
        #endregion
    }


}
