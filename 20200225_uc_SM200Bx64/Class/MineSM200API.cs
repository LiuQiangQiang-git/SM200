﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

/// <summary>
/// 原方法为中文，后因字典无法显示中文,目前改为蹩脚的英文.可能存在Bug,目前暂未测试.
/// ||目前没有编写GPIO、SPI、RealTime相关内容
/// ||主要功能包含：基础查询、状态操作、频谱扫描、IQ采集、SegIQ采集、音频处理
/// ||
/// ||DSO:Device Status Operation(设备状态操作)
/// ||此类方法会改变设备当前的状态
/// ||
/// ||DMS:Device Message Search(设备信息查询)
/// ||此类方法不会修改设备当前状态
/// ||
/// ||CD:Configure Device(设备配置)
/// ||此类方法将用户需要的操作配置入设备,如:扫频/IQ采集等
/// ||
/// ||GR:Get Result(获取结果) 
/// ||此类方法获取对应配置的结果
/// ||
/// </summary>
public class MineSM200API
{
    #region 设备连接
    private static int[] SMLink_EN = new int[8];

    private static SmStatus SMLink_Status = SmStatus.smDeviceNotFoundErr;

    private static string SMLink_StatusString = null;

    #endregion

    #region 状态操作
    /// <summary>
    /// 调用此方法进行设备连接(调用API中:smGetDeviceList/smOpenDeviceBySerial/smAbort)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200b_num">规定当前设备号，获取后用于调用其他方法</param>
    /// <returns>连接成功后返回true</returns>
    public static bool M_DSO_ConnectDevice(bool ISx64, ref int sm200b_num)
    {
        bool temp = false;
        try
        {
            int temp_设备连接数 = 0;
            if (ISx64)
            {
                //获取当前可连接的设备
                SMLink_Status = sm_api.smGetDeviceList(SMLink_EN, ref temp_设备连接数);
                //检测连接设备数量
                temp = !(temp_设备连接数 < 1);
                if (temp)
                {
                    //打开首个设备
                    SMLink_Status = sm_api.smOpenDeviceBySerial(ref sm200b_num, SMLink_EN[0]);
                    temp = !(SMLink_Status < 0);
                    //将设备当前至于空闲状态
                    sm_api.smAbort(sm200b_num);
                }
                //无可用连接
                else
                {
                    SMLink_Status = SmStatus.smDeviceNotFoundErr;
                }
            }
            else
            {
                //获取当前可连接的设备
                SMLink_Status = sm_api.smGetDeviceListx86(SMLink_EN, ref temp_设备连接数);
                //检测连接设备数量
                temp = !(temp_设备连接数 < 1);
                if (temp)
                {
                    //打开首个设备
                    SMLink_Status = sm_api.smOpenDeviceBySerialx86(ref sm200b_num, SMLink_EN[0]);
                    temp = !(SMLink_Status < 0);
                    //将设备当前至于空闲状态
                    sm_api.smAbortx86(sm200b_num);
                }
                //无可用连接
                else
                {
                    SMLink_Status = SmStatus.smDeviceNotFoundErr;
                }
            }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { temp = false; throw new Exception("sm_api错误,注意程序位数."); }
        return temp;
    }

    /// <summary>
    /// 调用此方断开设备(调用API中;smCloseDevice)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <returns>当前设备状态</returns>
    public static SmStatus M_DSO_DisconnectDevice(bool ISx64, int sm200_num)
    {
        try
        {
            if (ISx64)
            {
                //获取断开配置的状态
                SMLink_Status = sm_api.smCloseDevice(sm200_num);
                //重置该参数
                sm200_num = -1;
            }
            else
            {
                //获取断开配置的状态
                SMLink_Status = sm_api.smCloseDevicex86(sm200_num);
                //重置该参数
                sm200_num = -1;
            }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
            return SMLink_Status;
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
    }

    /// <summary>
    /// 调用此方法设备设置为空闲(调用API中;smAbort)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_DSO_AbortDevice(bool ISx64, int sm200_num)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            if (ISx64)
            {
                //配置为空闲的状态
                temp_s = SMLink_Status = sm_api.smAbort(sm200_num);
            }
            else
            {
                //配置为空闲的状态
                temp_s = SMLink_Status = sm_api.smAbortx86(sm200_num);
            }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { temp_s = SmStatus.smDeviceNotFoundErr; throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }

    /// <summary>
    /// 调用此方法进行设备重置配置(通过设备号;调用API中;smPreset)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_DSO_ResetDevice_EN(bool ISx64, int sm200_num)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            if (ISx64)
            {
                //重置设备配置
                temp_s = SMLink_Status = sm_api.smPreset(sm200_num);
            }
            else
            {
                //重置设备配置
                temp_s = SMLink_Status = sm_api.smPresetx86(sm200_num);
            }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }

    /// <summary>
    /// 调用此方法进行设备重置配置(不建议使用,通过序列号;调用API中;smPresetSerial)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_DSO_ResetDevice_SerialNumber(bool ISx64)
    {
        try
        {
            if (ISx64)
            {                 //重置设备配置
                SMLink_Status = sm_api.smPresetSerial(SMLink_EN[0]);
            }
            else
            {                 //重置设备配置
                SMLink_Status = sm_api.smPresetSerialx86(SMLink_EN[0]);
            }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
            return SMLink_Status;
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
    }

    /// <summary>
    /// 调用此方法进行配置时基配置.注意:调用此方法将会将设备置于空闲状态(调用API中;smSetGPSTimebaseUpdate)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>  
    /// <param name="IsStart">true为开启自动更新,false为关闭自动更新;.默认关闭</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_DSO_GPSTimebaseUpdate(bool ISx64, int sm200_num, SmBool IsStart)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            if (ISx64)
            {                 //配置为空闲的状态
                sm_api.smAbort(sm200_num);
                //配置时基更新
                temp_s = SMLink_Status = sm_api.smSetGPSTimebaseUpdate(sm200_num, IsStart);
            }
            else
            {                 //配置为空闲的状态
                sm_api.smAbort(sm200_num);
                //配置时基更新
                temp_s = SMLink_Status = sm_api.smSetGPSTimebaseUpdatex86(sm200_num, IsStart);
            }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { temp_s = SmStatus.smDeviceNotFoundErr; throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }

    /// <summary>
    /// 调用此方法进行设备电源配置配置.注意:调用此方法将会将设备置于空闲状态(调用API中;smSetPowerState)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="IsSave">true为启用省电;false为不启用..默认不启用</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_DSO_PowerState(bool ISx64, int sm200_num, bool IsSave)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            if (ISx64)
            {
                //配置为空闲的状态
                sm_api.smAbort(sm200_num);
                //设置是否开启省电
                if (IsSave)
                {
                    temp_s = SMLink_Status = sm_api.smSetPowerState(sm200_num, SmPowerState.smPowerStateStandby);
                }
                else
                {
                    temp_s = SMLink_Status = sm_api.smSetPowerState(sm200_num, SmPowerState.smPowerStateOn);
                }
            }
            else
            {  //配置为空闲的状态
                sm_api.smAbort(sm200_num);
                //设置是否开启省电
                if (IsSave)
                {
                    temp_s = SMLink_Status = sm_api.smSetPowerStatex86(sm200_num, SmPowerState.smPowerStateStandby);
                }
                else
                {
                    temp_s = SMLink_Status = sm_api.smSetPowerStatex86(sm200_num, SmPowerState.smPowerStateOn);
                }
            }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }

    /// <summary>
    /// (10MHz输出口状态)调用此方法进行设备10MHz输出口的启用或关闭.注意:调用此方法将会将设备置于空闲状态(调用API中;smSetExternalReference)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="IsStart">true为启用输出口;false为关闭输出口.. 默认不启用</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_DSO_ExternalReference(bool ISx64, int sm200_num, bool IsStart)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            if (ISx64)
            { //配置为空闲的状态
                sm_api.smAbort(sm200_num);
                //设置是否开10Mhz输出口
                if (IsStart)
                {
                    temp_s = SMLink_Status = sm_api.smSetExternalReference(sm200_num, SmBool.smTrue);
                }
                else
                {
                    temp_s = SMLink_Status = sm_api.smSetExternalReference(sm200_num, SmBool.smFalse);
                }
            }
            else
            {
                //配置为空闲的状态
                sm_api.smAbortx86(sm200_num);
                //设置是否开10Mhz输出口
                if (IsStart)
                {
                    temp_s = SMLink_Status = sm_api.smSetExternalReferencex86(sm200_num, SmBool.smTrue);
                }
                else
                {
                    temp_s = SMLink_Status = sm_api.smSetExternalReferencex86(sm200_num, SmBool.smFalse);
                }
            }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }

    /// <summary>
    /// (10MHz端口状态)调用此方法进行设备10MHz端口的外部或内部配置.注意:调用此方法将会将设备置于空闲状态(调用API中;smSetReference)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="port">true为使用外部;false为使用内部.默认内部</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_DSO_Reference(bool ISx64, int sm200_num, bool port)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            if (ISx64)
            {  //配置为空闲的状态
                sm_api.smAbort(sm200_num);
                //设置是否开10Mhz输出口
                if (port)
                {
                    temp_s = SMLink_Status = sm_api.smSetReference(sm200_num, SmReference.smReferenceUseExternal);
                }
                else
                {
                    temp_s = SMLink_Status = sm_api.smSetReference(sm200_num, SmReference.smReferenceUseInternal);
                }
            }
            else
            {  //配置为空闲的状态
                sm_api.smAbortx86(sm200_num);
                //设置是否开10Mhz输出口
                if (port)
                {
                    temp_s = SMLink_Status = sm_api.smSetReferencex86(sm200_num, SmReference.smReferenceUseExternal);
                }
                else
                {
                    temp_s = SMLink_Status = sm_api.smSetReferencex86(sm200_num, SmReference.smReferenceUseInternal);
                }
            }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }

    /// <summary>
    /// (设置风扇启用温度)调用此方法进行设备风扇启用的温度设置.注意:调用此方法将会将设备置于空闲状态(调用API中;smSetFanThreshold)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="temp">输入温度[10-90℃]</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_DSO_FanThreshold(bool ISx64, int sm200_num, int temp)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            if (ISx64)
            {
                if (temp >= 10 && temp <= 90)
                {
                    temp_s = sm_api.smSetFanThreshold(sm200_num, temp);
                }
            }
            else
            {
                if (temp >= 10 && temp <= 90)
                {
                    temp_s = sm_api.smSetFanThresholdx86(sm200_num, temp);
                }
            }
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }
    #endregion

    #region  加载配置
    /// <summary>
    ///  (加载配置)调用此方法进行设备对应的工作状态加载.注意:调用此方法将会将设备置于空闲状态(调用API中;smAbort/smConfigure)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="WorkMode">工作状态:选择需要加载的工作状态,加载前必须加载相关配置</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_DSO_Configure(bool ISx64, int sm200_num, SmMode WorkMode)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            if (ISx64)
            { //配置为空闲的状态
                sm_api.smAbort(sm200_num);
                temp_s = SMLink_Status = sm_api.smConfigure(sm200_num, WorkMode);
            }
            else
            { //配置为空闲的状态
                sm_api.smAbortx86(sm200_num);
                temp_s = SMLink_Status = sm_api.smConfigurex86(sm200_num, WorkMode);
            }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }


    #endregion

    #region 设备查询
    /// <summary>
    /// (返回设备状态字符串)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <returns>此函数返回设备状态字符串</returns>
    public static string M_DMS_StatusString(bool ISx64, SmStatus ss)
    {
        string s = SMLink_StatusString = sm_api.smGetStatusString(ss, ISx64);
        return s;
    }

    /// <summary>
    /// (设备当前状态)获取静态的字段
    /// </summary>
    /// <returns>此函数返回设备状</returns>
    public static SmStatus M_DMS_SMLink_Status()
    {
        SmStatus s = SMLink_Status;
        return s;
    }

    /// <summary>
    /// （固件版本）调用此方法进行固件版本查询(调用API中;smGetFirmwareVersion)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <returns>返回设备固件版本</returns>
    public static string M_DMS_FirmwareVersion(bool ISx64, int sm200_num)
    {
        string temp_s = null;
        try
        {
            if (ISx64)
            {//固件版本号
                int major = 0, minor = 0, revision = 0;
                SMLink_Status = sm_api.smGetFirmwareVersion(sm200_num, ref major, ref minor, ref revision);
                if (SMLink_Status < 0) { temp_s = null; }
                else { temp_s = major + "." + minor + "." + revision; }
            }
            else
            { //固件版本号
                int major = 0, minor = 0, revision = 0;
                SMLink_Status = sm_api.smGetFirmwareVersionx86(sm200_num, ref major, ref minor, ref revision);
                if (SMLink_Status < 0) { temp_s = null; }
                else { temp_s = major + "." + minor + "." + revision; }
            }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }

    /// <summary>
    /// （设备类型）调用此方法进行设备类型查询(调用API中;smGetDeviceInfo)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <returns>返回设备类型</returns>
    public static string M_DMS_DeviceType(bool ISx64, int sm200_num)
    {
        string temp_s = null;
        try
        {
            if (ISx64)
            {  //设备类型获取
                SmDeviceType deviceType = SmDeviceType.smDeviceTypeSM200A;
                SMLink_Status = sm_api.smGetDeviceInfo(sm200_num, ref deviceType, ref SMLink_EN[0]);
                if (SMLink_Status < 0) { temp_s = null; }
                else { temp_s = "" + deviceType; }
            }
            else
            {  //设备类型获取
                SmDeviceType deviceType = SmDeviceType.smDeviceTypeSM200A;
                SMLink_Status = sm_api.smGetDeviceInfox86(sm200_num, ref deviceType, ref SMLink_EN[0]);
                if (SMLink_Status < 0) { temp_s = null; }
                else { temp_s = "" + deviceType; }
            }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }

    /// <summary>
    /// (设备诊断)调用此方法进行设备查询电压、电流、温度查询(调用API中;smGetDeviceDiagnostics)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <returns>返回电压、电流、温度字符串带换行</returns>
    public static string M_DMS_DeviceDiagnostics(bool ISx64, int sm200_num)
    {
        string temp_s = null;
        try
        {
            //设备电压、电流、温度
            float voltage = 0, current = 0, temperature = 0;
            if (ISx64)
            {
                SMLink_Status = sm_api.smGetDeviceDiagnostics(sm200_num, ref voltage, ref current, ref temperature);
                if (SMLink_Status < 0) { temp_s = null; }
                else
                {
                    temp_s = "设备电压:" + voltage.ToString("0.000")
                     + "V\n设备电流:" + current.ToString("0.000")
                     + "A\n设备温度:" + temperature.ToString("0.000") + "℃";
                }
            }
            else
            {
                SMLink_Status = sm_api.smGetDeviceDiagnosticsx86(sm200_num, ref voltage, ref current, ref temperature);
                if (SMLink_Status < 0) { temp_s = null; }
                else
                {
                    temp_s = "设备电压:" + voltage.ToString("0.000")
                     + "V\n设备电流:" + current.ToString("0.000")
                     + "A\n设备温度:" + temperature.ToString("0.000") + "℃";
                }
            }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }

    /// <summary>
    /// (设备诊断)调用此方法进行设备查询电压、电流、温度查询(调用API中;smGetDeviceDiagnostics)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="vol">伏</param>
    /// <param name="cur">安培</param>
    /// <param name="temp">摄氏度</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_DMS_DeviceDiagnostics(bool ISx64, int sm200_num, ref float vol, ref float cur, ref float temp)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            //设备电压、电流、温度
            float voltage = 0, current = 0, temperature = 0; if (ISx64)
            {
                temp_s = SMLink_Status = sm_api.smGetDeviceDiagnostics(sm200_num, ref voltage, ref current, ref temperature);
            }
            else
            {
                temp_s = SMLink_Status = sm_api.smGetDeviceDiagnosticsx86(sm200_num, ref voltage, ref current, ref temperature);
            }
            temp = temperature;
            vol = voltage;
            cur = current;
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }

    /// <summary>
    /// (工作状态)获取设备当前工作状态(调用API中;smGetCurrentMode)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <returns>返回工作状态</returns>
    public static SmMode M_DMS_CurrentMode(bool ISx64, int sm200_num)
    {
        SmMode 工作状态 = SmMode.smModeIdle;
        try
        {
            if (ISx64)
            {  //获取状态
                SMLink_Status = sm_api.smGetCurrentMode(sm200_num, ref 工作状态);
            }
            else
            {  //获取状态
                SMLink_Status = sm_api.smGetCurrentModex86(sm200_num, ref 工作状态);
            }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return 工作状态;
    }

    /// <summary>
    /// (获取GPS状态)获取设备当前设备GPS状态(调用API中;smGetGPSState)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <returns>返回GPS状态</returns>
    public static SmGPSState M_DMS_GPSState(bool ISx64, int sm200_num)
    {
        SmGPSState GPS状态 = SmGPSState.smGPSStateNotPresent;
        try
        {
            if (ISx64)
            {//获取状态
                SMLink_Status = sm_api.smGetGPSState(sm200_num, ref GPS状态);
            }
            else
            { //获取状态
                SMLink_Status = sm_api.smGetGPSStatex86(sm200_num, ref GPS状态);
            }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return GPS状态;
    }

    /// <summary>
    /// (GPS延期信息)返回关于GPS延迟校正的信息(调用API中;smGetGPSHoldoverInfo)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="usingGPSHoldover"></param>
    /// <param name="lastHoldoverTime"></param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_DMS_GPSHoldoverInfo(bool ISx64, int sm200_num, ref SmBool usingGPSHoldover, ref ulong lastHoldoverTime)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            if (ISx64)
            { //获取状态
                temp_s = SMLink_Status = sm_api.smGetGPSHoldoverInfo(sm200_num, ref usingGPSHoldover, ref lastHoldoverTime);
            }
            else
            { //获取状态
                temp_s = SMLink_Status = sm_api.smGetGPSHoldoverInfox86(sm200_num, ref usingGPSHoldover, ref lastHoldoverTime);
            }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }

    /// <summary>
    /// (API版本)查询返回关于API版本的信息(调用API中;smGetAPIString)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <returns>API版本信息字符串</returns>
    public static string M_DMS_APIString(bool ISx64)
    {
        string temp_s = null;
        try
        {
            temp_s = sm_api.smGetAPIString(ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }

    /// <summary>
    /// (校准日期)此函数将校准日期作为纪元以来的秒数返回(调用API中;smGetCalDate)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <returns>此函数将校准日期作为纪元以来的秒数返回</returns>
    public static ulong M_DMS_CalDate(bool ISx64, int sm200_num)
    {
        ulong temp_l = 0;
        try
        {
            if (ISx64) { sm_api.smGetCalDate(sm200_num, ref temp_l); } else { sm_api.smGetCalDatex86(sm200_num, ref temp_l); }
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_l;
    }

    /// <summary>
    /// (查询风扇启用温度)调用此方法进行设备风扇启用的温度获取.(调用API中;smGetFanThreshold)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="temp">返回温度</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_DMS_GetFanThreshold(bool ISx64, int sm200_num, ref int temp)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            if (ISx64) { temp_s = SMLink_Status = sm_api.smGetFanThreshold(sm200_num, ref temp); } else { temp_s = SMLink_Status = sm_api.smGetFanThresholdx86(sm200_num, ref temp); }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }

    /// <summary>
    /// (GPS时基更新)调用此方法获取时基配置.(调用API中;smGetGPSTimebaseUpdate)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="IsStart">获取是否开启信息;.默认关闭</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_DSO_GPSTimebaseUpdate(bool ISx64, int sm200_num, ref SmBool IsStart)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            if (ISx64)
            { //配置时基更新
                temp_s = SMLink_Status = sm_api.smGetGPSTimebaseUpdate(sm200_num, ref IsStart);
            }
            else
            {
                //配置时基更新
                temp_s = SMLink_Status = sm_api.smGetGPSTimebaseUpdatex86(sm200_num, ref IsStart);
            }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }

    /// <summary>
    /// (获取GPS信息)调用此方法进行设备GPS信息获取.(调用API中;smGetGPSInfo)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="IsFresh">ture将会刷新后获取GPS信息;设置时请确保没在使用流模式下</param>
    /// <param name="Times"></param>
    /// <param name="Lon"></param>
    /// <param name="Lat"></param>
    /// <param name="H"></param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_DMS_GPSInfo(bool ISx64, int sm200_num, SmBool IsFresh, ref long Times, ref double Lon, ref double Lat, ref double H)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            SmBool temp_sb = SmBool.smFalse;
            int temp_i = 0;
            if (ISx64)
            {
                temp_s = SMLink_Status = sm_api.smGetGPSInfo(sm200_num, IsFresh, ref temp_sb, ref Times, ref Lat
   , ref Lon, ref H, null, ref temp_i);
            }
            else
            {
                temp_s = SMLink_Status = sm_api.smGetGPSInfox86(sm200_num, IsFresh, ref temp_sb, ref Times, ref Lat
, ref Lon, ref H, null, ref temp_i);
            }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }

    #endregion

    #region  配置设备

    #region 基础配置

    /// <summary>
    /// (衰减与参考电平)调用此方法进行设备设备参数配置操作(衰减、参考电平.调用API中;smSetAttenuator/smSetRefLevel)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="Attenuator">衰减:int型[-1,6].对应步进为5dBm的[0,30]的范围.-1为设置为自动衰减</param>
    /// <param name="RefLevel">参考电平:double型[-130,20]dbm</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_CD_AttenuatorAndRefLevel(bool ISx64, int sm200_num, int Attenuator, double RefLevel)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            if (ISx64)
            { //设置衰减
                sm_api.smSetAttenuator(sm200_num, Attenuator);
                //设置参考电平
                temp_s = SMLink_Status = sm_api.smSetRefLevel(sm200_num, RefLevel);
            }
            else
            { //设置衰减
                sm_api.smSetAttenuatorx86(sm200_num, Attenuator);
                //设置参考电平
                temp_s = SMLink_Status = sm_api.smSetRefLevelx86(sm200_num, RefLevel);
            }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }

    /// <summary>
    /// 调用此方法进行设备设备参数配置操作(衰减.调用API中;smSetAttenuator)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="Attenuator">衰减:int型[-1,6].对应步进为5dBm的[0,30]的范围.-1为设置为自动衰减</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_CD_Attenuator(bool ISx64, int sm200_num, int Attenuator)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            if (ISx64)
            {//设置衰减
                temp_s = SMLink_Status = sm_api.smSetAttenuator(sm200_num, Attenuator);
            }
            else
            { //设置衰减
                temp_s = SMLink_Status = sm_api.smSetAttenuatorx86(sm200_num, Attenuator);
            }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }

    /// <summary>
    /// 调用此方法进行设备设备参数配置操作(参考电平.调用API中;smSetRefLevel)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="RefLevel">参考电平:double型[-130,20]dbm</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_CD_RefLevel(bool ISx64, int sm200_num, double RefLevel)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            if (ISx64)
            { //设置参考电平
                SMLink_Status = sm_api.smSetRefLevel(sm200_num, RefLevel);
            }
            else
            { //设置参考电平
                SMLink_Status = sm_api.smSetRefLevel(sm200_num, RefLevel);
            }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }

    #endregion

    #region 扫频配置

    /// <summary>
    /// (扫频_统一配置_中心扫宽)调用此方法进行扫频的参数以中心频率来进行统一配置(调用API中;smSetSweepSpeed/smSetSweepCenterSpan/smSetSweepCoupling/smSetSweepDetector
    /// smSetSweepScale/smSetSweepWindow/smSetSweepSpurReject/smConfigure)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="SweepSpeed">速度选择:指定要使用的设备获取速度.分为:自动、普通、快速</param>
    /// <param name="CenterF">中心频率：指定扫描的中心频率,单位Hz</param>
    /// <param name="Span">扫描跨度:指定扫描的跨度,单位Hz</param>
    /// <param name="RBW">分辨率带宽,单位Hz</param>
    /// <param name="VBW">视频带宽,不能大于RBW,单位Hz</param>
    /// <param name="St">扫频时间:建议扫描的总获取时间。在几秒钟内指定。这个参数是一个建议，将确保在增加扫描时间之前先满足RBW和VBW。</param>
    /// <param name="Detector">检波器:指定扫描的检波器设置</param>
    /// <param name="VideoUnits">视频单位:指定视频处理单元为对数(log)、电压、功率或样本</param>
    /// <param name="Scale">规模:指定返回扫描的单位。可用的单元是dBm或mV</param>
    /// <param name="WindowType">窗口函数:Specify the FFT window function</param>
    /// <param name="IsStart">启用图像拒绝算法:Enable/disable the software image rejection algorithm</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_CD_Sweep_UC_CenterF(bool ISx64, int sm200_num, SmSweepSpeed SweepSpeed
        , double CenterF, double Span
        , double RBW, double VBW, double St
        , SmDetector Detector, SmVideoUnits VideoUnits, SmScale Scale, SmWindowType WindowType, SmBool IsStart)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            if (ISx64)
            {
                sm_api.smSetSweepSpeed(sm200_num, SweepSpeed);
                sm_api.smSetSweepCenterSpan(sm200_num, CenterF, Span);
                sm_api.smSetSweepCoupling(sm200_num, RBW, VBW, St);
                sm_api.smSetSweepDetector(sm200_num, Detector, VideoUnits);
                sm_api.smSetSweepScale(sm200_num, Scale);
                sm_api.smSetSweepWindow(sm200_num, WindowType);
                sm_api.smSetSweepSpurReject(sm200_num, IsStart);
                temp_s = M_DSO_Configure(ISx64, sm200_num, SmMode.smModeSweeping);
            }
            else
            {
                sm_api.smSetSweepSpeedx86(sm200_num, SweepSpeed);
                sm_api.smSetSweepCenterSpanx86(sm200_num, CenterF, Span);
                sm_api.smSetSweepCouplingx86(sm200_num, RBW, VBW, St);
                sm_api.smSetSweepDetectorx86(sm200_num, Detector, VideoUnits);
                sm_api.smSetSweepScalex86(sm200_num, Scale);
                sm_api.smSetSweepWindowx86(sm200_num, WindowType);
                sm_api.smSetSweepSpurRejectx86(sm200_num, IsStart);
                temp_s = M_DSO_Configure(ISx64, sm200_num, SmMode.smModeSweeping);
            }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }

    /// <summary>
    /// （扫频_统一配置_始末频率）调用此方法进行扫频的参数以始末频率来进行统一配置(调用API中;smSetSweepSpeed/smSetSweepStartStop/smSetSweepCoupling/smSetSweepDetector
    /// smSetSweepScale/smSetSweepWindow/smSetSweepSpurReject/smConfigure)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="SweepSpeed">速度选择:指定要使用的设备获取速度.分为:自动、普通、快速</param>
    /// <param name="StartF">起始频率:指定扫频的开始频率,单位Hz</param>
    /// <param name="EndF">终止频率:指定扫频的停止频率,单位Hz</param>
    /// <param name="RBW">分辨率带宽,单位Hz</param>
    /// <param name="VBW">视频带宽,不能大于RBW,单位Hz</param>
    /// <param name="St">扫频时间:建议扫描的总获取时间。在几秒钟内指定。这个参数是一个建议，将确保在增加扫描时间之前先满足RBW和VBW。</param>
    /// <param name="Detector">检波器:指定扫描的检波器设置</param>
    /// <param name="VideoUnits">视频单位:指定视频处理单元为对数(log)、电压、功率或样本</param>
    /// <param name="Scale">规模:指定返回扫描的单位。可用的单元是dBm或mV</param>
    /// <param name="WindowType">窗口函数:Specify the FFT window function</param>
    /// <param name="IsStart">启用图像拒绝算法:Enable/disable the software image rejection algorithm</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_CD_Sweep_UC_SEF(bool ISx64, int sm200_num, SmSweepSpeed SweepSpeed
, double StartF, double EndF
, double RBW, double VBW, double St
        , SmDetector Detector, SmVideoUnits VideoUnits, SmScale Scale, SmWindowType WindowType, SmBool IsStart)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            if (ISx64)
            {
                sm_api.smSetSweepSpeed(sm200_num, SweepSpeed);
                sm_api.smSetSweepStartStop(sm200_num, StartF, EndF);
                sm_api.smSetSweepCoupling(sm200_num, RBW, VBW, St);
                sm_api.smSetSweepDetector(sm200_num, Detector, VideoUnits);
                sm_api.smSetSweepScale(sm200_num, Scale);
                sm_api.smSetSweepWindow(sm200_num, WindowType);
                sm_api.smSetSweepSpurReject(sm200_num, IsStart);
                temp_s = M_DSO_Configure(ISx64, sm200_num, SmMode.smModeSweeping);
            }
            else
            {
                sm_api.smSetSweepSpeedx86(sm200_num, SweepSpeed);
                sm_api.smSetSweepStartStopx86(sm200_num, StartF, EndF);
                sm_api.smSetSweepCouplingx86(sm200_num, RBW, VBW, St);
                sm_api.smSetSweepDetectorx86(sm200_num, Detector, VideoUnits);
                sm_api.smSetSweepScalex86(sm200_num, Scale);
                sm_api.smSetSweepWindowx86(sm200_num, WindowType);
                sm_api.smSetSweepSpurRejectx86(sm200_num, IsStart);
                temp_s = M_DSO_Configure(ISx64, sm200_num, SmMode.smModeSweeping);
            }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }

    /// <summary>
    /// 调用此方法进行扫频的扫频速度参数配置(调用API中;smSetSweepSpeed)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="SweepSpeed">指定要使用的设备获取速度.分为:自动、普通、快速</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_CD_Sweep_SweepSpeed(bool ISx64, int sm200_num, SmSweepSpeed SweepSpeed)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            if (ISx64) { temp_s = SMLink_Status = sm_api.smSetSweepSpeed(sm200_num, SweepSpeed); } else { temp_s = SMLink_Status = sm_api.smSetSweepSpeedx86(sm200_num, SweepSpeed); }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }

    /// <summary>
    /// 调用此方法进行扫频的中心频率与扫描跨度配置(调用API中;smSetSweepCenterSpan)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="CenterF">指定扫描的中心频率,单位Hz</param>
    /// <param name="span">指定扫描的跨度,单位Hz</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_CD_Sweep_CenterSpan(bool ISx64, int sm200_num, double CenterF, double span)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            if (ISx64) { temp_s = SMLink_Status = sm_api.smSetSweepCenterSpan(sm200_num, CenterF, span); } else { temp_s = SMLink_Status = sm_api.smSetSweepCenterSpanx86(sm200_num, CenterF, span); }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }

    /// <summary>
    /// 调用此方法进行扫频的起始频率与终止频率配置(调用API中;smSetSweepStartStop)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="startF">起始频率：指定扫频的开始频率,单位Hz</param>
    /// <param name="endF">终止频率：指定扫频的停止频率,单位Hz</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_CD_Sweep_StartStop(bool ISx64, int sm200_num, double startF, double endF)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            if (ISx64) { temp_s = SMLink_Status = sm_api.smSetSweepStartStop(sm200_num, startF, endF); } else { temp_s = SMLink_Status = sm_api.smSetSweepStartStopx86(sm200_num, startF, endF); }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }

    /// <summary>
    /// 调用此方法进行扫频的RBW、VBW、扫频参考时间配置(调用API中;smSetSweepCoupling)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="RBW">分辨率带宽,单位Hz</param>
    /// <param name="VBW">视频带宽,不能大于RBW,单位Hz</param>
    /// <param name="sweepTime">建议扫描的总获取时间。在几秒钟内指定。这个参数是一个建议，将确保在增加扫描时间之前先满足RBW和VBW。</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_CD_Sweep_Coupling(bool ISx64, int sm200_num, double RBW, double VBW, double sweepTime)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            if (ISx64) { temp_s = SMLink_Status = sm_api.smSetSweepCoupling(sm200_num, RBW, VBW, sweepTime); } else { temp_s = SMLink_Status = sm_api.smSetSweepCouplingx86(sm200_num, RBW, VBW, sweepTime); }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }

    /// <summary>
    /// 调用此方法进行扫频的检波器、视频单位配置(调用API中;smSetSweepDetector)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="Detector">检波器:指定扫描的检波器设置</param>
    /// <param name="VideoUnits">视频单位:指定视频处理单元为对数(log)、电压、功率或样本</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_CD_Sweep_Detector(bool ISx64, int sm200_num, SmDetector Detector, SmVideoUnits VideoUnits)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            if (ISx64) { temp_s = SMLink_Status = sm_api.smSetSweepDetector(sm200_num, Detector, VideoUnits); } else { temp_s = SMLink_Status = sm_api.smSetSweepDetectorx86(sm200_num, Detector, VideoUnits); }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }

    /// <summary>
    /// 调用此方法进行扫频的Scale配置(调用API中;smSetSweepScale)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="Scale">规模:指定返回扫描的单位。可用的单元是dBm或mV</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_CD_Sweep_Scale(bool ISx64, int sm200_num, SmScale Scale)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            if (ISx64) { temp_s = SMLink_Status = sm_api.smSetSweepScale(sm200_num, Scale); } else { temp_s = SMLink_Status = sm_api.smSetSweepScalex86(sm200_num, Scale); }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }

    /// <summary>
    /// 调用此方法进行扫频的窗口配置(调用API中;smSetSweepWindow)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="WindowType">窗口:Specify the FFT window function</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_CD_Sweep_WindowType(bool ISx64, int sm200_num, SmWindowType WindowType)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            if (ISx64) { temp_s = SMLink_Status = sm_api.smSetSweepWindow(sm200_num, WindowType); } else { temp_s = SMLink_Status = sm_api.smSetSweepWindowx86(sm200_num, WindowType); }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }

    /// <summary>
    /// Enable/disable the software image rejection algorithm.
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="IsStart">Enable/disable the software image rejection algorithm.</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_CD_Sweep_SweepSpurReject(bool ISx64, int sm200_num, SmBool IsStart)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            if (ISx64) { temp_s = SMLink_Status = sm_api.smSetSweepSpurReject(sm200_num, IsStart); } else { temp_s = SMLink_Status = sm_api.smSetSweepSpurRejectx86(sm200_num, IsStart); }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }
    #endregion

    #region 实时配置

    /// <summary>
    /// （实时扫频_统一配置）调用此方法进行实时模式的参数统一配置配置
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="CenterF">指定扫描的中心频率,单位Hz</param>
    /// <param name="span">扫描跨度:指定扫描的跨度,单位Hz</param>
    /// <param name="RBW">分辨率带宽,单位Hz</param>
    /// <param name="Detector">检波器:指定扫描的检波器设置</param>
    /// <param name="Scale">规模：指定返回扫描的单位。可用的单元是dBm或mV</param>
    /// <param name="FRF">帧参考电平:设置实时帧的参考电平</param>
    /// <param name="FH">帧高度:指定帧的高度,单位dB;一个常见的值是100dB</param>
    /// <param name="WindowType">Specify the FFT window function</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_CD_RealTime_UC(bool ISx64, int sm200_num, double CenterF, double span
, double RBW, SmDetector Detector, SmScale Scale, double FRF, double FH, SmWindowType WindowType)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            if (ISx64)
            {
                sm_api.smSetRealTimeCenterSpan(sm200_num, CenterF, span);
                sm_api.smSetRealTimeRBW(sm200_num, RBW);
                sm_api.smSetRealTimeDetector(sm200_num, Detector);
                sm_api.smSetRealTimeScale(sm200_num, Scale, FRF, FH);
                SMLink_Status = sm_api.smSetRealTimeWindow(sm200_num, WindowType);
                temp_s = M_DSO_Configure(ISx64, sm200_num, SmMode.smModeRealTime);
            }
            else
            {
                sm_api.smSetRealTimeCenterSpanx86(sm200_num, CenterF, span);
                sm_api.smSetRealTimeRBWx86(sm200_num, RBW);
                sm_api.smSetRealTimeDetectorx86(sm200_num, Detector);
                sm_api.smSetRealTimeScalex86(sm200_num, Scale, FRF, FH);
                SMLink_Status = sm_api.smSetRealTimeWindowx86(sm200_num, WindowType);
                temp_s = M_DSO_Configure(ISx64, sm200_num, SmMode.smModeRealTime);
            }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }

    /// <summary>
    /// 调用此方法进行扫频的中心频率与扫描跨度配置(调用API中;smSetRealTimeCenterSpan)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="CenterF">指定扫描的中心频率,单位Hz</param>
    /// <param name="span">指定扫描的跨度,单位Hz</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_CD_RealTime_CenterSpan(bool ISx64, int sm200_num, double CenterF, double span)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            if (ISx64) { temp_s = SMLink_Status = sm_api.smSetRealTimeCenterSpan(sm200_num, CenterF, span); } else { temp_s = SMLink_Status = sm_api.smSetRealTimeCenterSpanx86(sm200_num, CenterF, span); }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }

    /// <summary>
    /// 调用此方法进行扫频的分辨率带宽配置(调用API中;smSetRealTimeRBW)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="RBW">分辨率带宽,单位Hz</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_CD_RealTime_RBW(bool ISx64, int sm200_num, double RBW)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            if (ISx64) { temp_s = SMLink_Status = sm_api.smSetRealTimeRBW(sm200_num, RBW); } else { temp_s = SMLink_Status = sm_api.smSetRealTimeRBWx86(sm200_num, RBW); }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }

    /// <summary>
    /// 调用此方法进行扫频的检波器配置(调用API中;smSetRealTimeDetector)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="Detector">指定扫描的检波器设置</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_CD_RealTime_Detector(bool ISx64, int sm200_num, SmDetector Detector)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            if (ISx64) { temp_s = SMLink_Status = sm_api.smSetRealTimeDetector(sm200_num, Detector); } else { temp_s = SMLink_Status = sm_api.smSetRealTimeDetectorx86(sm200_num, Detector); }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }

    /// <summary>
    /// 调用此方法进行扫频的返回单位与帧配置(调用API中;smSetRealTimeScale)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="Scale">指定返回扫描的单位。可用的单元是dBm或mV</param>
    /// <param name="FRF">设置实时帧的参考电平</param>
    /// <param name="FH">指定帧的高度,单位dB;一个常见的值是100dB</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_CD_RealTime_tRealTimeScale(bool ISx64, int sm200_num, SmScale Scale, double FRF, double FH)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            if (ISx64) { temp_s = SMLink_Status = sm_api.smSetRealTimeScale(sm200_num, Scale, FRF, FH); } else { temp_s = SMLink_Status = sm_api.smSetRealTimeScalex86(sm200_num, Scale, FRF, FH); }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }

    /// <summary>
    /// 调用此方法进行扫频的窗口函数配置(调用API中;smSetRealTimeWindow)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="WindowType">Specify the FFT window function</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_CD_RealTime_WindowType(bool ISx64, int sm200_num, SmWindowType WindowType)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            if (ISx64) { temp_s = SMLink_Status = sm_api.smSetRealTimeWindow(sm200_num, WindowType); } else { temp_s = SMLink_Status = sm_api.smSetRealTimeWindowx86(sm200_num, WindowType); }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }

    #endregion

    #region IQ采集
    #region IQ流

    /// <summary>
    /// 调用此方法进行IQ模式的参数统一配置配置(调用API中;smSetIQBaseSampleRate/smSetIQDataType/smSetIQCenterFreq/smSetIQSampleRate
    /// smSetIQBandwidth/smSetIQExtTriggerEdge/smSetIQUSBQueueSize)
    /// </summary>  
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="BaseSampleRate">指定I/Q采集的基本采样率(50M或61.44M)</param>
    /// <param name="DataType">指定返回32位复杂float还是16位复杂short</param>
    /// <param name="CenterF">指定I/Q采集的中心频率</param>
    /// <param name="SampleRate">指定I/Q数据的抽取,取值为0-4096中2的幂</param>
    /// <param name="IsStart">设置为true以启用软件筛选器,当抽取大于16后总是启用</param>
    /// <param name="BandWidth">指定软件过滤器的带宽</param>
    /// <param name="edge">触发枚举值.指定SM200将检测外部触发器输入端口上的触发器的边缘</param>
    /// <param name="ms">主动请求的数据队列的大小.如果您希望快速更改频率,可以选择较小的队列大小</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_CD_IQStream_UC(bool ISx64, int sm200_num, SmIQStreamSampleRate BaseSampleRate, SmDataType DataType
        , double CenterF, int SampleRate, SmBool IsStart, double BandWidth, SmTriggerEdge edge, float ms)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            if (ISx64)
            {
                sm_api.smSetIQBaseSampleRate(sm200_num, BaseSampleRate);
                sm_api.smSetIQDataType(sm200_num, DataType);
                sm_api.smSetIQCenterFreq(sm200_num, CenterF);
                sm_api.smSetIQSampleRate(sm200_num, SampleRate);
                sm_api.smSetIQBandwidth(sm200_num, IsStart, BandWidth);
                sm_api.smSetIQExtTriggerEdge(sm200_num, edge);
                SMLink_Status = sm_api.smSetIQUSBQueueSize(sm200_num, ms);
                sm_api.smConfigure(sm200_num, SmMode.smModeIQ);
            }
            else
            {
                sm_api.smSetIQBaseSampleRatex86(sm200_num, BaseSampleRate);
                sm_api.smSetIQDataTypex86(sm200_num, DataType);
                sm_api.smSetIQCenterFreqx86(sm200_num, CenterF);
                sm_api.smSetIQSampleRatex86(sm200_num, SampleRate);
                sm_api.smSetIQBandwidthx86(sm200_num, IsStart, BandWidth);
                sm_api.smSetIQExtTriggerEdgex86(sm200_num, edge);
                SMLink_Status = sm_api.smSetIQUSBQueueSizex86(sm200_num, ms);
                sm_api.smConfigurex86(sm200_num, SmMode.smModeIQ);
            }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }


    /// <summary>
    /// (IQ配置_RC)调用此方法进行IQ模式的参数建议配置配置,针对统一配置中的部分参数进行了固定配置(调用API中;smSetIQBaseSampleRate/smSetIQDataType/smSetIQCenterFreq/smSetIQSampleRate
    /// smSetIQBandwidth/smSetIQExtTriggerEdge)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="CenterF">指定I/Q采集的中心频率</param>
    /// <param name="SampleRate">指定I/Q数据的抽取,取值为0-4096中2的幂,使采集样本率在[12.207k-50MS/s]中选择</param>
    /// <param name="BandWidth">指定软件过滤器的带宽,对应SampleRate存在对应的极值</param>
    /// <param name="TriggerEdge">触发枚举值.指定SM200将检测外部触发器输入端口上的触发器的边缘，可选上升沿或是下降沿</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_CD_IQStream_RC(bool ISx64, int sm200_num, double CenterF, int SampleRate, double BandWidth, SmTriggerEdge TriggerEdge)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            if (ISx64)
            {
                sm_api.smSetIQBaseSampleRate(sm200_num, SmIQStreamSampleRate.smIQStreamSampleRate50M);
                sm_api.smSetIQDataType(sm200_num, SmDataType.smDataType32fc);
                sm_api.smSetIQCenterFreq(sm200_num, CenterF);
                sm_api.smSetIQSampleRate(sm200_num, SampleRate);
                sm_api.smSetIQBandwidth(sm200_num, SmBool.smTrue, BandWidth);
                SMLink_Status = sm_api.smSetIQExtTriggerEdge(sm200_num, TriggerEdge);
                sm_api.smConfigure(sm200_num, SmMode.smModeIQ);
            }
            else
            {
                sm_api.smSetIQBaseSampleRatex86(sm200_num, SmIQStreamSampleRate.smIQStreamSampleRate50M);
                sm_api.smSetIQDataTypex86(sm200_num, SmDataType.smDataType32fc);
                sm_api.smSetIQCenterFreqx86(sm200_num, CenterF);
                sm_api.smSetIQSampleRatex86(sm200_num, SampleRate);
                sm_api.smSetIQBandwidthx86(sm200_num, SmBool.smTrue, BandWidth);
                SMLink_Status = sm_api.smSetIQExtTriggerEdgex86(sm200_num, TriggerEdge);
                sm_api.smConfigurex86(sm200_num, SmMode.smModeIQ);
            }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }

    /// <summary>
    ///  调用此方法进行IQ模式的基本采样率配置(调用API中;smSetIQBaseSampleRate)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="SampleRate">指定I/Q采集的基本采样率(50M或61.44M)</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_CD_IQStream_BaseSampleRate(bool ISx64, int sm200_num, SmIQStreamSampleRate SampleRate)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            if (ISx64) { temp_s = SMLink_Status = sm_api.smSetIQBaseSampleRate(sm200_num, SampleRate); } else { temp_s = SMLink_Status = sm_api.smSetIQBaseSampleRatex86(sm200_num, SampleRate); }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }

    /// <summary>
    ///  调用此方法进行IQ模式的基本采样率配置(调用API中;smSetIQDataType)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="DataType">数据类型:指定返回32位复杂float还是16位复杂short</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_CD_IQStream_DataType(bool ISx64, int sm200_num, SmDataType DataType)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            if (ISx64) { temp_s = SMLink_Status = sm_api.smSetIQDataType(sm200_num, DataType); } else { temp_s = SMLink_Status = sm_api.smSetIQDataTypex86(sm200_num, DataType); }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }

    /// <summary>
    ///  调用此方法进行IQ模式的中心频率配置(调用API中;smSetIQCenterFreq)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="CenterF">指定I/Q采集的中心频率</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_CD_IQStream_CenterF(bool ISx64, int sm200_num, double CenterF)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            if (ISx64) { temp_s = SMLink_Status = sm_api.smSetIQCenterFreq(sm200_num, CenterF); } else { temp_s = SMLink_Status = sm_api.smSetIQCenterFreqx86(sm200_num, CenterF); }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }

    /// <summary>
    ///  调用此方法进行IQ模式的抽取配置(调用API中;smSetIQSampleRate)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="SampleRate">指定I/Q数据的抽取,取值为0-4096中2的幂</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_CD_IQStream_SampleRate(bool ISx64, int sm200_num, int SampleRate)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            if (ISx64) { temp_s = SMLink_Status = sm_api.smSetIQSampleRate(sm200_num, SampleRate); } else { temp_s = SMLink_Status = sm_api.smSetIQSampleRatex86(sm200_num, SampleRate); }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }

    /// <summary>
    ///  调用此方法进行IQ模式的带宽配置(调用API中;smSetIQBandwidth)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="IsStart">启用软件过滤:设置为true以启用软件筛选器,当抽取大于16后总是启用</param>
    /// <param name="BandWidth">指定软件过滤器的带宽</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_CD_IQStream_BandWidth(bool ISx64, int sm200_num, SmBool IsStart, double BandWidth)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            if (ISx64) { temp_s = SMLink_Status = sm_api.smSetIQBandwidth(sm200_num, SmBool.smTrue, BandWidth); } else { temp_s = SMLink_Status = sm_api.smSetIQBandwidthx86(sm200_num, SmBool.smTrue, BandWidth); }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }

    /// <summary>
    ///  调用此方法进行IQ模式的触发沿配置(调用API中;smSetIQExtTriggerEdge)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="edge">触发枚举值.指定SM200将检测外部触发器输入端口上的触发器的边缘</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_CD_IQStream_TriggerEdge(bool ISx64, int sm200_num, SmTriggerEdge edge)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            if (ISx64) { temp_s = SMLink_Status = sm_api.smSetIQExtTriggerEdge(sm200_num, edge); } else { temp_s = SMLink_Status = sm_api.smSetIQExtTriggerEdgex86(sm200_num, edge); }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }

    /// <summary>
    ///  调用此方法进行IQ模式的数据队列配置(调用API中;smSetIQUSBQueueSize)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="ms">毫秒:主动请求的数据队列的大小.如果您希望快速更改频率,可以选择较小的队列大小</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_CD_IQStream_USBQueueSize(bool ISx64, int sm200_num, float ms)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            if (ISx64) { temp_s = SMLink_Status = sm_api.smSetIQUSBQueueSize(sm200_num, ms); } else { temp_s = SMLink_Status = sm_api.smSetIQUSBQueueSizex86(sm200_num, ms); }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }
    #endregion

    #region SegIQ捕获

    /// <summary>
    /// (IQ分段捕获_统一配置)仅限SM200B中，设置设备进行250mbps进行2秒内的IQ采集
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="Attenuator"></param>
    /// <param name="RefLevel"></param>
    /// <param name="DataType">指定返回32位复杂float还是16位复杂short</param>
    /// <param name="CenterF">分段I/Q捕获的中心频率</param>
    /// <param name="TriggerLevel">触发级别：dBm中用于视频触发捕获的视频触发级别</param>
    /// <param name="TriggerEdge">视频触发边缘类型</param>
    /// <param name="fftSize">用于FMT触发的FFT的大小,512和16384之间的2的幂;值小,时间分辨率高，频率分辨率低</param>
    /// <param name="frequencies">计数频率的数组,单位Hz;指定FMT掩码的频率点</param>
    /// <param name="ampls">计数振幅的数组,单位dBm;指定FMT掩码的振幅阈值限制</param>
    /// <param name="count">频率和振幅阵列中FMT点的数目.</param>
    /// <param name="segmentcount">指定分段I/Q捕获中的段数</param>
    /// <param name="segment">指定要修改的段。必须大于或等于0，并且小于segmentCount</param>
    /// <param name="triggerType">指定触发器段的触发器类型</param>
    /// <param name="preTrigger">在触发事件之前捕获的样本数量.这是对捕获大小的补充.对于即时触发器,将预触发器添加到捕获大小,然后将其设置为零</param>
    /// <param name="capturesize">在触发事件之后要捕获的样本数量.对于即时触发器,将预触发器添加到此值,并将预触发器设置为零。</param>
    /// <param name="ToP">等待触发器返回之前的时间量.如果发生超时,捕获仍然在超时的时刻发生</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_CD_SegIQ_UC(bool ISx64, int sm200_num, int Attenuator, int RefLevel, SmDataType DataType
      , double CenterF, double TriggerLevel, SmTriggerEdge TriggerEdge
      , int fftSize, double frequencies, double ampls, int count
        , int segmentcount, int segment, SmTriggerType triggerType
        , int preTrigger, int capturesize, double ToP)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            if (ISx64)
            {
                M_CD_AttenuatorAndRefLevel(ISx64, sm200_num, Attenuator, RefLevel);
                sm_api.smSetSegIQDataType(sm200_num, DataType);
                sm_api.smSetSegIQCenterFreq(sm200_num, CenterF);
                sm_api.smSetSegIQVideoTrigger(sm200_num, TriggerLevel, TriggerEdge);
                sm_api.smSetSegIQFMTParams(sm200_num, fftSize, ref frequencies, ref ampls, count);
                sm_api.smSetSegIQSegmentCount(sm200_num, segmentcount);
                SMLink_Status = sm_api.smSetSegIQSegment(sm200_num, segment, triggerType, preTrigger, capturesize, ToP);
            }
            else
            {
                M_CD_AttenuatorAndRefLevel(ISx64, sm200_num, Attenuator, RefLevel);
                sm_api.smSetSegIQDataTypex86(sm200_num, DataType);
                sm_api.smSetSegIQCenterFreqx86(sm200_num, CenterF);
                sm_api.smSetSegIQVideoTriggerx86(sm200_num, TriggerLevel, TriggerEdge);
                sm_api.smSetSegIQFMTParamsx86(sm200_num, fftSize, ref frequencies, ref ampls, count);
                sm_api.smSetSegIQSegmentCountx86(sm200_num, segmentcount);
                SMLink_Status = sm_api.smSetSegIQSegmentx86(sm200_num, segment, triggerType, preTrigger, capturesize, ToP);
            }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }


    /// <summary>
    /// 仅限SM200B中，设置设备进行250mbps进行2秒内的IQ采集的建议配置
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="Attenuator"></param>
    /// <param name="RefLevel"></param>
    /// <param name="DataType">指定返回32位复杂float还是16位复杂short</param>
    /// <param name="CenterF">分段I/Q捕获的中心频率</param>
    /// <param name="triggerType">指定触发器段的触发器类型</param>
    /// <param name="preTrigger">在触发事件之前捕获的样本数量.这是对捕获大小的补充.对于即时触发器,将预触发器添加到捕获大小,然后将其设置为零</param>
    /// <param name="capturesize">在触发事件之后要捕获的样本数量.对于即时触发器,将预触发器添加到此值,并将预触发器设置为零。</param>
    /// <param name="ToP">等待触发器返回之前的时间量.如果发生超时,捕获仍然在超时的时刻发生</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_CD_SegIQ_RC(bool ISx64, int sm200_num, int Attenuator, double RefLevel, SmDataType DataType
, double CenterF
        , SmTriggerType triggerType
, int preTrigger, int capturesize, double ToP)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            if (ISx64)
            {
                M_CD_AttenuatorAndRefLevel(ISx64, sm200_num, Attenuator, RefLevel);
                sm_api.smSetSegIQDataType(sm200_num, DataType);
                sm_api.smSetSegIQCenterFreq(sm200_num, CenterF);
                sm_api.smSetSegIQSegmentCount(sm200_num, 1);
                SMLink_Status = sm_api.smSetSegIQSegment(sm200_num, 0, triggerType, preTrigger, capturesize, ToP);
                sm_api.smConfigure(sm200_num, SmMode.smModeIQSegmentedCapture);
            }
            else
            {
                M_CD_AttenuatorAndRefLevel(ISx64, sm200_num, Attenuator, RefLevel);
                sm_api.smSetSegIQDataTypex86(sm200_num, DataType);
                sm_api.smSetSegIQCenterFreqx86(sm200_num, CenterF);
                sm_api.smSetSegIQSegmentCountx86(sm200_num, 1);
                SMLink_Status = sm_api.smSetSegIQSegmentx86(sm200_num, 0, triggerType, preTrigger, capturesize, ToP);
                sm_api.smConfigurex86(sm200_num, SmMode.smModeIQSegmentedCapture);
            }

            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }
    #endregion

    #region Vrt设置
    /// <summary>
    /// 配置IQ采集时的Vrt包内相关信息
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="VrtPN">Vrt包中数目:在每个VRT数据包中采样I/Q样本的数目</param>
    /// <param name="VrtID">Vrt信息标识:用于VRT信息流的sid新流标识符</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_CD_IQStream_VRT(bool ISx64, int sm200_num, short VrtPN, int VrtID)
    {
        SmStatus temp_s = SMLink_Status;
        try
        {
            if (ISx64)
            {
                sm_api.smSetVrtPacketSize(sm200_num, VrtPN);
                SMLink_Status = sm_api.smSetVrtStreamID(sm200_num, VrtID);
            }
            else
            {
                sm_api.smSetVrtPacketSizex86(sm200_num, VrtPN);
                SMLink_Status = sm_api.smSetVrtStreamIDx86(sm200_num, VrtID);
            }
            //获取设备状态字符串
            SMLink_StatusString = sm_api.smGetStatusString(SMLink_Status, ISx64);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return temp_s;
    }
    #endregion
    #endregion

    #region 音频配置
    /// <summary>
    /// (音频处理_统一配置)调用此方法进行音频获取的参数统一配置配置
    /// </summary>  
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="centerFreq">中心频率</param>
    /// <param name="bandwidth">带宽</param>
    /// <param name="audioHighPassFreq">高通</param>
    /// <param name="audioLowPassFreq">低通</param>
    /// <param name="fmDeemphasis">去加重</param>
    /// <param name="type">类型</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_CD_Audio_UC(bool ISx64, int sm200_num, double centerFreq
        , SmAudioType type = SmAudioType.smAudioTypeFM
        , double bandwidth = 12000, double audioLowPassFreq = 8000, double audioHighPassFreq = 20, double fmDeemphasis = 75)
    {
        try
        {
            if (ISx64)
            {
                sm_api.smSetAudioCenterFreq(sm200_num, centerFreq);
                sm_api.smSetAudioType(sm200_num, type);
                sm_api.smSetAudioFilters(sm200_num, bandwidth, audioLowPassFreq, audioHighPassFreq);
                sm_api.smSetAudioFMDeemphasis(sm200_num, fmDeemphasis);
                return sm_api.smConfigure(sm200_num, SmMode.smModeAudio);
            }
            else
            {
                sm_api.smSetAudioCenterFreqx86(sm200_num, centerFreq);
                sm_api.smSetAudioTypex86(sm200_num, type);
                sm_api.smSetAudioFiltersx86(sm200_num, bandwidth, audioLowPassFreq, audioHighPassFreq);
                sm_api.smSetAudioFMDeemphasisx86(sm200_num, fmDeemphasis);
                return sm_api.smConfigurex86(sm200_num, SmMode.smModeAudio);
            }
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }

    }
    #endregion
    #endregion

    #region 获取结果
    #region 扫频结果
    /// <summary>
    /// 获取扫频0(0)的结果
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="RBW">当前帧rbw</param>
    /// <param name="VBW">当前帧的vbw</param>
    /// <param name="actualStartFreq">当前帧的起始频率</param>
    /// <param name="binSize">当前帧的宽度</param>
    /// <param name="Time">当前帧的时刻</param>
    /// <returns>返回ArrayList,必定只包含两个float数组,否则结果为错误的;第一个为最小,第二个为最大,若"检波器"选择平均,则两个数组相同,反之为最小最大</returns>
    public static ArrayList M_GR_Sweep(bool ISx64, int sm200_num, ref double RBW, ref double VBW,
        ref double actualStartFreq, ref double binSize, ref long Time)
    {
        ArrayList sweepResult = new ArrayList();
        try
        {
            DateTime dt = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            int sweepLength = 0;
            if (ISx64)
            {

                SMLink_Status = sm_api.smGetSweepParameters(sm200_num, ref RBW, ref VBW, ref actualStartFreq,
                    ref binSize, ref sweepLength);
                if (SMLink_Status >= 0)
                {
                    float[] sweepMin = new float[sweepLength];
                    float[] sweepMax = new float[sweepLength];
                    IntPtr si = Marshal.AllocCoTaskMem(sweepMin.Length);
                    IntPtr sim = Marshal.AllocCoTaskMem(sweepMin.Length);
                    sm_api.smGetSweep(sm200_num, sweepMin, sweepMax, ref Time);
                    //sm_api.smGetSweep(sm200_num, si, sim, ref 时刻);
                    sweepResult.Add(sweepMin);
                    sweepResult.Add(sweepMax);
                }
            }
            else
            {
                SMLink_Status = sm_api.smGetSweepParametersx86(sm200_num, ref RBW, ref VBW, ref actualStartFreq,
                    ref binSize, ref sweepLength);
                if (SMLink_Status >= 0)
                {
                    float[] sweepMin = new float[sweepLength];
                    float[] sweepMax = new float[sweepLength];
                    IntPtr si = Marshal.AllocCoTaskMem(sweepMin.Length);
                    IntPtr sim = Marshal.AllocCoTaskMem(sweepMin.Length);
                    sm_api.smGetSweepx86(sm200_num, sweepMin, sweepMax, ref Time);
                    //sm_api.smGetSweep(sm200_num, si, sim, ref 时刻);
                    sweepResult.Add(sweepMin);
                    sweepResult.Add(sweepMax);
                }
            }
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return sweepResult;
    }




    public static ArrayList M_GR_RealTime()
    {
        ArrayList sweepResult = new ArrayList();
        try
        {























        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return sweepResult;
    }
    #endregion
    #region IQ结果
    #region IQ流
    /// <summary>
    /// (IQ采集_结果_默认结果)获取IQ采集50MS/s的结果
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="IQStream_CaptureSize">捕获大小,单位s(例:捕获1ms数据,IQStream_CaptureSize = 1e-3)</param>
    /// <param name="iqSampleRate">当前实际采样率</param>
    /// <param name="iqBandwidth">当前实际带宽</param>
    /// <param name="iqTimeStamp">当前时刻</param>
    /// <param name="sampleLoss">采集结果丢失的点数</param>
    /// <param name="samplesRemaining">对于设备内部采集的固定间隔采,集结果剩余的点数.可在计算后的到每个采样率对应的固定采集间隔.该参数结果不影响采集结果</param>
    /// <returns>返回ArrayLIst仅包含一个float数组,数组内部分别为负数的实部、虚部依次排列,所以采集的IQ的数据点数为数组长度的一般</returns>
    public static ArrayList M_GR_IQStream_DR(bool ISx64, int sm200_num
        , double IQStream_CaptureSize, ref double iqSampleRate, ref double iqBandwidth
        , ref long iqTimeStamp, ref int sampleLoss, ref int samplesRemaining)
    {
        ArrayList IQResult = new ArrayList();
        try
        {
            if (ISx64)
            {
                sm_api.smGetIQParameters(0, ref iqSampleRate, ref iqBandwidth);
                int iqLen = (int)(iqSampleRate * IQStream_CaptureSize);
                float[] iq = new float[iqLen * 2];
                int triggerCount = 10;
                double[] triggers = new double[triggerCount];
                sm_api.smGetIQ(0, iq, iqLen, triggers, triggerCount, ref iqTimeStamp,
                SmBool.smFalse, ref sampleLoss, ref samplesRemaining);
                IQResult.Add(iq);
            }
            else
            {
                sm_api.smGetIQParametersx86(0, ref iqSampleRate, ref iqBandwidth);
                int iqLen = (int)(iqSampleRate * IQStream_CaptureSize);
                float[] iq = new float[iqLen * 2];
                int triggerCount = 10;
                double[] triggers = new double[triggerCount];
                sm_api.smGetIQx86(0, iq, iqLen, triggers, triggerCount, ref iqTimeStamp,
                SmBool.smFalse, ref sampleLoss, ref samplesRemaining);
                IQResult.Add(iq);
            }
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return IQResult;
    }
    /// <summary>
    /// 获取IQ采集50MS/s的结果
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="IQStream_CaptureSize">捕获大小,单位s(例:捕获1ms数据,IQStream_CaptureSize = 1e-3)</param>
    /// <param name="iqSampleRate">当前实际采样率</param>
    /// <param name="iqBandwidth">当前实际带宽</param>
    /// <param name="iqTimeStamp">当前时刻</param>
    /// <param name="sampleLoss">采集结果丢失的点数</param>
    /// <param name="samplesRemaining">对于设备内部采集的固定间隔采,集结果剩余的点数.可在计算后的到每个采样率对应的固定采集间隔.该参数结果不影响采集结果</param>
    /// <param name="avg_p">Avg_Pow</param>
    /// <returns>返回ArrayLIst仅包含一个float数组,数组为按照API文档方式计算复数的结果,单个结果 = 10*log10(实部*实部+虚部*虚部)</returns>
    public static ArrayList M_GR_IQStream_dBm(bool ISx64, int sm200_num
, double IQStream_CaptureSize, ref double iqSampleRate, ref double iqBandwidth
, ref long iqTimeStamp, ref int sampleLoss, ref int samplesRemaining, ref double avg_p)
    {
        ArrayList IQResult = new ArrayList(); int temp_i = 0;
        float iqTotal = 0;
        try
        {
            if (ISx64) { sm_api.smGetIQParameters(0, ref iqSampleRate, ref iqBandwidth); } else { sm_api.smGetIQParametersx86(0, ref iqSampleRate, ref iqBandwidth); }
            int iqLen = (int)(iqSampleRate * IQStream_CaptureSize);
            float[] iq = new float[iqLen * 2];
            int triggerCount = 10;
            double[] triggers = new double[triggerCount];
            if (ISx64)
            {
                sm_api.smGetIQ(0, iq, iqLen, triggers, triggerCount, ref iqTimeStamp,
  SmBool.smTrue, ref sampleLoss, ref samplesRemaining);
            }
            else
            {
                sm_api.smGetIQx86(0, iq, iqLen, triggers, triggerCount, ref iqTimeStamp,
        SmBool.smTrue, ref sampleLoss, ref samplesRemaining);
            }
            float[] tempF = new float[iqLen];


            for (int i = 0; i < iqLen; i++)
            {
                tempF[i] = 10 * (float)Math.Log10(Math.Pow(iq[i], 2) + Math.Pow(iq[i + 1], 2));
                if (float.IsInfinity(tempF[i]))
                {
                    tempF[i] = float.NaN;
                }
                else
                {
                    iqTotal += tempF[i];
                    temp_i++;
                }

            }
            avg_p = iqTotal / temp_i;
            IQResult.Add(tempF);
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return IQResult;
    }

    #endregion

    #region SegIQ结果

    //注意修改采集大小对10取余的其他值
    public static ArrayList M_GR_SegIQ(bool ISx64, int sm200_num, int captureSize)
    {
        ArrayList segIQResult = new ArrayList();
        int samplesRead = 0;
        int bufSize = (int)captureSize / 10;
        float[] buf = new float[2 * bufSize];
        int samplesToRead = 0;
        try
        {
            if (ISx64)
            {
                sm_api.smSegIQCaptureStart(sm200_num, 0);
                // Block until complete
                sm_api.smSegIQCaptureWait(sm200_num, 0);
            }
            else
            {
                sm_api.smSegIQCaptureStartx86(sm200_num, 0);
                // Block until complete
                sm_api.smSegIQCaptureWaitx86(sm200_num, 0);
            }
            while (samplesRead < captureSize)
            {
                int samplesLeftToRead = (captureSize - samplesRead);
                if (samplesLeftToRead > bufSize) { samplesToRead = bufSize; }
                else { samplesToRead = samplesLeftToRead; }
                if (ISx64) { sm_api.smSegIQCaptureRead(sm200_num, 0, 0, buf, samplesRead, samplesToRead); } else { sm_api.smSegIQCaptureReadx86(sm200_num, 0, 0, buf, samplesRead, samplesToRead); }
                samplesRead += samplesToRead;
                segIQResult.Add(buf);
            }
            if (ISx64) { sm_api.smSegIQCaptureFinish(sm200_num, 0); }
            else { sm_api.smSegIQCaptureFinishx86(sm200_num, 0); }
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
        return segIQResult;
    }
    #endregion
    #endregion

    #region 音频结果
    /// <summary>
    /// 调用此方法获取音频处理结果(使用api中:smGetAudio)
    /// </summary>
    /// <param name="ISx64">是否使用x64api.true:x64;false:x86</param>
    /// <param name="sm200_num">当前设备号</param>
    /// <param name="audio">需要获取的音频结果,长度固定位1000</param>
    /// <returns>返回设备对应状态</returns>
    public static SmStatus M_GR_Audio(bool ISx64, int sm200_num, float[] audio)
    {
        try
        {
            if (ISx64)
            {
                return sm_api.smGetAudio(sm200_num, audio);
            }
            else
            {
                return sm_api.smGetAudiox86(sm200_num, audio);
            }
        }
        catch { throw new Exception("sm_api错误,注意程序位数."); }
    }
    #endregion

    #endregion


}
#region sm_api
class sm_api
{
    public static int SM_INVALID_HANDLE = -1;

    public static int SM_TRUE = 1;
    public static int SM_FALSE = 0;

    public static int SM_MAX_DEVICES = 9;

    public static int SM200A_AUTO_ATTEN = -1;
    // Valid atten values [0,6] or -1 for auto
    public static int SM200A_MAX_ATTEN = 6;

    // Maximum number of sweeps that can be queued up
    // Sweep indices [0,15]
    public static int SM200A_MAX_SWEEP_QUEUE_SZ = 16;

    // Device is only calibrated to 100 kHz
    public static double SM200A_MIN_FREQ = 100.0e3;
    // Device is only calibrated to 20 GHz
    public static double SM200A_MAX_FREQ = 20.6e9;
    public static double SM200A_MAX_REF_LEVEL = 20.0;
    public static int SM200A_MAX_IQStream_DECIMATION = 4096;

    // The frequency at which the manually controlled preselector filters end.
    // Past this frequency, the preselector filters are always enabled.
    public static double SM200A_PRESELECTOR_MAX_FREQ = 645.0e6;

    // Minimum RBW for fast sweep with Nuttall window
    public static double SM200A_FAST_SWEEP_MIN_RBW = 30.0e3;

    // Min/max span for device configured in RTSA measurement mode
    public static double SM200A_RTSA_MIN_SPAN = 200.0e3;
    public static double SM200A_RTSA_MAX_SPAN = 160.0e6;

    // Sweep time range [1us, 100s]
    public static double SM200A_MIN_SWEEP_TIME = 1.0e-6;
    public static double SM200A_MAX_SWEEP_TIME = 100.0;

    // Max number of bytes per SPI transfer
    public static int SM200A_SPI_MAX_BYTES = 4;

    // For GPIO sweeps
    public static int SM200A_GPIO_SWEEP_MAX_STEPS = 64;

    // For IQ GPIO switching
    public static int SM200A_GPIO_SWITCH_MAX_STEPS = 64;
    public static int SM200A_GPIO_SWITCH_MIN_COUNT = 2;
    public static int SM200A_GPIO_SWITCH_MAX_COUNT = 4194303 - 1; // 2^22 - 1

    // FPGA internal temperature (Celsius)
    // Returned from smGetDeviceDiagnostics()
    public static double SM200A_TEMP_WARNING = 95.0;
    public static double SM200A_TEMP_MAX = 102.0;

    #region x64
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetDeviceList(int[] serials, ref int deviceCount);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smOpenDevice(ref int device);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smOpenDeviceBySerial(ref int device, int serialNumber);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smCloseDevice(int device);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smPreset(int device);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smPresetSerial(int serialNumber);

    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetDeviceInfo(int device,
        ref SmDeviceType deviceType, ref int serialNumber);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetFirmwareVersion(int device,
        ref int major, ref int minor, ref int revision);

    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetDeviceDiagnostics(int device,
        ref float voltage, ref float current, ref float temperature);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetFullDeviceDiagnostics(int device, ref SmDeviceDiagnostics diagnostics);

    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetPowerState(int device, SmPowerState powerState);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetPowerState(int device, ref SmPowerState powerState);

    // Overrides reference level when set to non-auto values
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetAttenuator(int device, int atten);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetAttenuator(int device, ref int atten);

    // Uses this when attenuation is automatic
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetRefLevel(int device, double refLevel);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetRefLevel(int device, ref double refLevel);

    // Set preselector state for all measurement modes
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetPreselector(int device, SmBool enabled);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetPreselector(int device, ref SmBool enabled);

    // Configure IO routines
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetGPIOState(int device,
        SmGPIOState lowerState, SmGPIOState upperState);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetGPIOState(int device,
        ref SmGPIOState lowerState, ref SmGPIOState upperState);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smWriteGPIOImm(int device, byte data);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smReadGPIOImm(int device, ref byte data);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smWriteSPI(int device, uint data, int byteCount);
    // For standard sweeps only
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetGPIOSweepDisabled(int device);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetGPIOSweep(int device, [In, Out]SmGPIOStep[] steps, int stepCount);
    // For IQ streaming only
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetGPIOSwitchingDisabled(int device);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetGPIOSwitching(int device, byte[] gpio,
        uint[] counts, int gpioSteps);

    // Enable the external reference out port
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetExternalReference(int device, SmBool enabled);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetExternalReference(int device, ref SmBool enabled);
    // Specify whether to use the internal reference or reference on the ref in port
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetReference(int device, SmReference reference);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetReference(int device, ref SmReference reference);

    // Enable whether or not the API auto updates the timebase calibration
    // value when a valid GPS lock is acquired.
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetGPSTimebaseUpdate(int device, SmBool enabled);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetGPSTimebaseUpdate(int device, ref SmBool enabled);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetGPSHoldoverInfo(int device,
        ref SmBool usingGPSHoldover, ref ulong lastHoldoverTime);

    // Returns whether the GPS is locked, can be called anytime
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetGPSState(int device, ref SmGPSState GPSState);

    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetSweepSpeed(int device, SmSweepSpeed sweepSpeed);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetSweepCenterSpan(int device, double centerFreqHz,
        double spanHz);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetSweepStartStop(int device, double startFreqHz,
        double stopFreqHz);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetSweepCoupling(int device, double rbw, double vbw,
        double sweepTime);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetSweepDetector(int device, SmDetector detector,
        SmVideoUnits videoUnits);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetSweepScale(int device, SmScale scale);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetSweepWindow(int device, SmWindowType window);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetSweepSpurReject(int device, SmBool spurRejectEnabled);

    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetRealTimeCenterSpan(int device, double centerFreqHz,
        double spanHz);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetRealTimeRBW(int device, double rbw);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetRealTimeDetector(int device, SmDetector detector);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetRealTimeScale(int device, SmScale scale,
        double frameRef, double frameScale);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetRealTimeWindow(int device, SmWindowType window);

    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetIQBaseSampleRate(int device, SmIQStreamSampleRate sampleRate);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetIQDataType(int device, SmDataType dataType);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetIQCenterFreq(int device, double centerFreqHz);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetIQCenterFreq(int device, ref double centerFreqHz);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetIQSampleRate(int device, int decimation);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetIQBandwidth(int device, SmBool enableSoftwareFilter,
        double bandwidth);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetIQExtTriggerEdge(int device, SmTriggerEdge edge);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetIQExtTriggerEdge(int device, ref SmTriggerEdge edge);
    // Experimental
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetIQUSBQueueSize(int device, float ms);

    // Begin Segmented I/Q configuration, SM200B only
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetSegIQDataType(int device, SmDataType dataType);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetSegIQCenterFreq(int device, double centerFreqHz);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetSegIQVideoTrigger(int device, double triggerLevel, SmTriggerEdge triggerEdge);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetSegIQExtTrigger(int device, SmTriggerEdge extTriggerEdge);
    //[DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    //public static extern SmStatus smSetSegIQFMTParams(int device, int fftSize, const ref double frequencies,
    //const ref double ampls, int count);       
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetSegIQFMTParams(int device, int fftSize, ref double frequencies,
ref double ampls, int count);


    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetSegIQSegmentCount(int device, int segmentCount);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetSegIQSegment(int device, int segment, SmTriggerType triggerType,
        int preTrigger, int captureSize, double timeoutSeconds);

    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetAudioCenterFreq(int device, double centerFreqHz);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetAudioType(int device, SmAudioType audioType);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetAudioFilters(int device, double ifBandwidth,
        double audioLpf, double audioHpf);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetAudioFMDeemphasis(int device, double deemphasis);

    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smConfigure(int device, SmMode mode);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetCurrentMode(int device, ref SmMode mode);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smAbort(int device);

    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetSweepParameters(int device, ref double actualRBW,
        ref double actualVBW, ref double actualStartFreq, ref double binSize, ref int sweepSize);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetRealTimeParameters(int device, ref double actualRBW,
        ref int sweepSize, ref double actualStartFreq, ref double binSize, ref int frameWidth,
        ref int frameHeight, ref double poi);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetIQParameters(int device, ref double sampleRate,
        ref double bandwidth);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetIQCorrection(int device, ref float scale);

    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSegIQGetMaxCaptures(int device, ref int maxCaptures);

    // Performs a single sweep, blocking function
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetSweep(int device, float[] sweepMin,
        float[] sweepMax, ref long nsSinceEpoch);



    // Queued sweep mechanisms
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smStartSweep(int device, int pos);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smFinishSweep(int device, int pos,
        float[] sweepMin, float[] sweepMax, ref long nsSinceEpoch);

    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetRealTimeFrame(int device, float[] colorFrame,
        float[] alphaFrame, float[] sweepMin, float[] sweepMax, ref int frameCount,
        ref long nsSinceEpoch);

    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetIQ(int device, float[] iqBuf, int iqBufSize,
        double[] triggers, int triggerBufSize, ref long nsSinceEpoch, SmBool purge,
        ref int sampleLoss, ref int samplesRemaining);

    #region I/Q采集功能
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSegIQCaptureStart(int device, int capture);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSegIQCaptureWait(int device, int capture);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSegIQCaptureWaitAsync(int device, int capture, ref SmBool completed);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSegIQCaptureTimeout(int device, int capture, int segment, ref SmBool timedOut);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSegIQCaptureTime(int device, int capture, int segment, ref Int64 nsSinceEpoch);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSegIQCaptureRead(int device, int capture, int segment, float[] iq, int offset, int len);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSegIQCaptureFinish(int device, int capture);
    // Convenience function for a single segment capture.
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSegIQCaptureFull(int device, int capture, float[] iq, int offset, int len,
    ref Int64 nsSinceEpoch, ref SmBool timedOut);
    #endregion

    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetAudio(int device, float[] audio);

    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetGPSInfo(int device, SmBool refresh,
        ref SmBool updated, ref long secSinceEpoch, ref double latitude,
        ref double longitude, ref double altitude, char[] nmea, ref int nmeaLen);

    // Accepts values between [10-90] as the temp threshold for when the fan turns on
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetFanThreshold(int device, int temp);
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetFanThreshold(int device, ref int temp);

    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetCalDate(int device, ref ulong lastCalDate);

    public static string smGetAPIString(bool Isx64)
    {
        if (Isx64)
        {
            IntPtr strPtr = smGetAPIVersion(); return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(strPtr);
        }
        else { IntPtr strPtr = smGetAPIVersionx86(); return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(strPtr); }

    }

    public static string smGetStatusString(SmStatus status, bool isx64)
    {
        if (isx64)
        {
            IntPtr strPtr = smGetErrorString(status);
            return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(strPtr);
        }
        else
        {
            IntPtr strPtr = smGetErrorStringx86(status);
            return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(strPtr);
        }
    }

    public static string smGetProductString(bool Isx64)
    {
        if (Isx64)
        {
            IntPtr strPtr = smGetProductID();
            return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(strPtr);
        }
        else
        {
            IntPtr strPtr = smGetProductIDx86();
            return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(strPtr);
        }
    }

    // Call string variants above instead
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr smGetAPIVersion();
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr smGetProductID();
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr smGetErrorString(SmStatus status);



    //mine
    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetVrtPacketSize(int device, short samplesPerPkt);

    [DllImport("sm_api_x64.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetVrtStreamID(int device, int sid);
    #endregion
    #region x86
    //=======================================x86==============================
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetDeviceListx86(int[] serials, ref int deviceCount);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smOpenDevicex86(ref int device);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smOpenDeviceBySerialx86(ref int device, int serialNumber);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smCloseDevicex86(int device);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smPresetx86(int device);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smPresetSerialx86(int serialNumber);

    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetDeviceInfox86(int device,
        ref SmDeviceType deviceType, ref int serialNumber);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetFirmwareVersionx86(int device,
        ref int major, ref int minor, ref int revision);

    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetDeviceDiagnosticsx86(int device,
        ref float voltage, ref float current, ref float temperature);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetFullDeviceDiagnosticsx86(int device, ref SmDeviceDiagnostics diagnostics);

    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetPowerStatex86(int device, SmPowerState powerState);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetPowerStatex86(int device, ref SmPowerState powerState);

    // Overrides reference level when set to non-auto values
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetAttenuatorx86(int device, int atten);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetAttenuatorx86(int device, ref int atten);

    // Uses this when attenuation is automatic
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetRefLevelx86(int device, double refLevel);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetRefLevelx86(int device, ref double refLevel);

    // Set preselector state for all measurement modes
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetPreselectorx86(int device, SmBool enabled);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetPreselectorx86(int device, ref SmBool enabled);

    // Configure IO routines
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetGPIOStatex86(int device,
        SmGPIOState lowerState, SmGPIOState upperState);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetGPIOStatex86(int device,
        ref SmGPIOState lowerState, ref SmGPIOState upperState);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smWriteGPIOImmx86(int device, byte data);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smReadGPIOImmx86(int device, ref byte data);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smWriteSPIx86(int device, uint data, int byteCount);
    // For standard sweeps only
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetGPIOSweepDisabledx86(int device);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetGPIOSweepx86(int device, [In, Out]SmGPIOStep[] steps, int stepCount);
    // For IQ streaming only
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetGPIOSwitchingDisabledx86(int device);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetGPIOSwitchingx86(int device, byte[] gpio,
        uint[] counts, int gpioSteps);

    // Enable the external reference out port
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetExternalReferencex86(int device, SmBool enabled);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetExternalReferencex86(int device, ref SmBool enabled);
    // Specify whether to use the internal reference or reference on the ref in port
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetReferencex86(int device, SmReference reference);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetReferencex86(int device, ref SmReference reference);

    // Enable whether or not the API auto updates the timebase calibration
    // value when a valid GPS lock is acquired.
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetGPSTimebaseUpdatex86(int device, SmBool enabled);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetGPSTimebaseUpdatex86(int device, ref SmBool enabled);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetGPSHoldoverInfox86(int device,
        ref SmBool usingGPSHoldover, ref ulong lastHoldoverTime);

    // Returns whether the GPS is locked, can be called anytime
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetGPSStatex86(int device, ref SmGPSState GPSState);

    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetSweepSpeedx86(int device, SmSweepSpeed sweepSpeed);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetSweepCenterSpanx86(int device, double centerFreqHz,
        double spanHz);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetSweepStartStopx86(int device, double startFreqHz,
        double stopFreqHz);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetSweepCouplingx86(int device, double rbw, double vbw,
        double sweepTime);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetSweepDetectorx86(int device, SmDetector detector,
        SmVideoUnits videoUnits);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetSweepScalex86(int device, SmScale scale);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetSweepWindowx86(int device, SmWindowType window);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetSweepSpurRejectx86(int device, SmBool spurRejectEnabled);

    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetRealTimeCenterSpanx86(int device, double centerFreqHz,
        double spanHz);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetRealTimeRBWx86(int device, double rbw);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetRealTimeDetectorx86(int device, SmDetector detector);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetRealTimeScalex86(int device, SmScale scale,
        double frameRef, double frameScale);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetRealTimeWindowx86(int device, SmWindowType window);

    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetIQBaseSampleRatex86(int device, SmIQStreamSampleRate sampleRate);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetIQDataTypex86(int device, SmDataType dataType);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetIQCenterFreqx86(int device, double centerFreqHz);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetIQCenterFreqx86(int device, ref double centerFreqHz);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetIQSampleRatex86(int device, int decimation);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetIQBandwidthx86(int device, SmBool enableSoftwareFilter,
        double bandwidth);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetIQExtTriggerEdgex86(int device, SmTriggerEdge edge);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetIQExtTriggerEdgex86(int device, ref SmTriggerEdge edge);
    // Experimental
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetIQUSBQueueSizex86(int device, float ms);

    // Begin Segmented I/Q configuration, SM200B only
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetSegIQDataTypex86(int device, SmDataType dataType);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetSegIQCenterFreqx86(int device, double centerFreqHz);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetSegIQVideoTriggerx86(int device, double triggerLevel, SmTriggerEdge triggerEdge);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetSegIQExtTriggerx86(int device, SmTriggerEdge extTriggerEdge);
    //[DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    //public static extern SmStatus smSetSegIQFMTParams(int device, int fftSize, const ref double frequencies,
    //const ref double ampls, int count);       
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetSegIQFMTParamsx86(int device, int fftSize, ref double frequencies,
ref double ampls, int count);


    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetSegIQSegmentCountx86(int device, int segmentCount);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetSegIQSegmentx86(int device, int segment, SmTriggerType triggerType,
        int preTrigger, int captureSize, double timeoutSeconds);

    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetAudioCenterFreqx86(int device, double centerFreqHz);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetAudioTypex86(int device, SmAudioType audioType);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetAudioFiltersx86(int device, double ifBandwidth,
        double audioLpf, double audioHpf);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetAudioFMDeemphasisx86(int device, double deemphasis);

    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smConfigurex86(int device, SmMode mode);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetCurrentModex86(int device, ref SmMode mode);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smAbortx86(int device);

    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetSweepParametersx86(int device, ref double actualRBW,
        ref double actualVBW, ref double actualStartFreq, ref double binSize, ref int sweepSize);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetRealTimeParametersx86(int device, ref double actualRBW,
        ref int sweepSize, ref double actualStartFreq, ref double binSize, ref int frameWidth,
        ref int frameHeight, ref double poi);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetIQParametersx86(int device, ref double sampleRate,
        ref double bandwidth);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetIQCorrectionx86(int device, ref float scale);

    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSegIQGetMaxCapturesx86(int device, ref int maxCaptures);

    // Performs a single sweep, blocking function
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetSweepx86(int device, float[] sweepMin,
        float[] sweepMax, ref long nsSinceEpoch);



    // Queued sweep mechanisms
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smStartSweepx86(int device, int pos);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smFinishSweepx86(int device, int pos,
        float[] sweepMin, float[] sweepMax, ref long nsSinceEpoch);

    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetRealTimeFramex86(int device, float[] colorFrame,
        float[] alphaFrame, float[] sweepMin, float[] sweepMax, ref int frameCount,
        ref long nsSinceEpoch);

    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetIQx86(int device, float[] iqBuf, int iqBufSize,
        double[] triggers, int triggerBufSize, ref long nsSinceEpoch, SmBool purge,
        ref int sampleLoss, ref int samplesRemaining);

    #region I/Q采集功能
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSegIQCaptureStartx86(int device, int capture);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSegIQCaptureWaitx86(int device, int capture);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSegIQCaptureWaitAsyncx86(int device, int capture, ref SmBool completed);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSegIQCaptureTimeoutx86(int device, int capture, int segment, ref SmBool timedOut);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSegIQCaptureTimex86(int device, int capture, int segment, ref Int64 nsSinceEpoch);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSegIQCaptureReadx86(int device, int capture, int segment, float[] iq, int offset, int len);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSegIQCaptureFinishx86(int device, int capture);
    // Convenience function for a single segment capture.
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSegIQCaptureFullx86(int device, int capture, float[] iq, int offset, int len,
    ref Int64 nsSinceEpoch, ref SmBool timedOut);
    #endregion

    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetAudiox86(int device, float[] audio);

    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetGPSInfox86(int device, SmBool refresh,
        ref SmBool updated, ref long secSinceEpoch, ref double latitude,
        ref double longitude, ref double altitude, char[] nmea, ref int nmeaLen);

    // Accepts values between [10-90] as the temp threshold for when the fan turns on
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetFanThresholdx86(int device, int temp);
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetFanThresholdx86(int device, ref int temp);

    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smGetCalDatex86(int device, ref ulong lastCalDate);

    // Call string variants above instead
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr smGetAPIVersionx86();
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr smGetProductIDx86();
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr smGetErrorStringx86(SmStatus status);

    //mine
    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetVrtPacketSizex86(int device, short samplesPerPkt);

    [DllImport("sm_api_x86.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern SmStatus smSetVrtStreamIDx86(int device, int sid);
    //========================================================================
    #endregion
}

public enum SmStatus
{
    smCalErr = -1003, // Internal use
    smMeasErr = -1002, // Internal use
    smErrorIOErr = -1001, // Internal use

    // Calibration file unable to be used with the API
    smInvalidCalibrationFileErr = -200,
    // Invalid center frequency specified
    smInvalidCenterFreqErr = -101,
    // IQ decimation value provided not a valid value
    smInvalidIQDecimationErr = -100,

    // If the core FX3 program fails to run
    smFx3RunErr = -52,
    // Only can connect up to SM_MAX_DEVICES receivers
    smMaxDevicesConnectedErr = -51,
    // FPGA boot error
    smFPGABootErr = -50,
    // Boot error
    smBootErr = -49,

    // Requesting GPS information when the GPS is not locked
    smGpsNotLockedErr = -16,
    // Invalid API version for target device, TBD
    smVersionMismatchErr = -14,
    // Unable to allocate resources needed to configure the measurement mode
    smAllocationErr = -13,

    // Invalid or already active sweep position
    smInvalidSweepPosition = -10,
    // Attempting to perform an operation that cannot currently be performed.
    // Often the result of trying to do something while the device is currently
    //   making measurements or not in an idle state.
    smInvalidConfigurationErr = -8,
    // Device disconnected, likely USB error detected
    smConnectionLostErr = -6,
    // Required parameter found to have invalid value
    smInvalidParameterErr = -5,
    // One or more required pointer parameters were null
    smNullPtrErr = -4,
    // User specified invalid device index
    smInvalidDeviceErr = -3,
    // Unable to open device
    smDeviceNotFoundErr = -2,

    // Function returned successfully
    smNoError = 0,

    // One or more of the provided settings were adjusted
    smSettingClamped = 1,
    // Measurement includes data which caused an ADC overload (clipping/compression)
    smAdcOverflow = 2,
    // Measurement is uncalibrated, overrides ADC overflow
    smUncalData = 3,
    // Temperature drift occured, measurements uncalibrated, reconfigure the device
    smTempDriftWarning = 4,
    // Warning when the preselector span is smaller than the user selected span
    smSpanExceedsPreselector = 5,
    // Warning when the internal temperature gets too hot. The device is close to shutting down
    smTempHighWarning = 6,
    // Returned when the API was unable to keep up with the necessary processing
    smCpuLimited = 7,
    // Returned when the API detects a device with newer features than what was available
    //   when this version of the API was released. Suggested fix, update the API.
    smUpdateAPI = 8
};

public enum SmDataType
{
    smDataType32fc = 0,
    smDataType16sc = 1
};

public enum SmMode
{
    smModeIdle = 0,
    smModeSweeping = 1,
    smModeRealTime = 2,
    smModeIQ = 3,
    smModeIQStreaming = 3,
    smModeIQSegmentedCapture = 5,
    smModeAudio = 4
};

public enum SmSweepSpeed
{
    smSweepSpeedAuto = 0,
    smSweepSpeedNormal = 1,
    smSweepSpeedFast = 2
};

public enum SmIQStreamSampleRate
{
    smIQStreamSampleRate50M = 0,
    smIQStreamSampleRate61_44M = 1,
};

public enum SmPowerState
{
    smPowerStateOn = 0,
    smPowerStateStandby = 1
};

public enum SmDetector
{
    smDetectorAverage = 0,
    smDetectorMinMax = 1
};

public enum SmScale
{
    smScaleLog = 0, // Sweep in dBm
    smScaleLin = 1, // Sweep in mV
    smScaleFullScale = 2 // N/A
};

public enum SmVideoUnits
{
    smVideoLog = 0,
    smVideoVoltage = 1,
    smVideoPower = 2,
    smVideoSample = 3
};

public enum SmWindowType
{
    smWindowFlatTop = 0,
    // 1 (N/A)
    smWindowNutall = 2,
    smWindowBlackman = 3,
    smWindowHamming = 4,
    smWindowGaussian6dB = 5,
    smWindowRect = 6
};

public enum SmTriggerType
{
    smTriggerTypeImmediate = 0,
    smTriggerTypeVideo = 1,
    smTriggerTypeExternal = 2,
    smTriggerTypeFrequencyMask = 3
};

public enum SmTriggerEdge
{
    smTriggerEdgeRising = 0,
    smTriggerEdgeFalling = 1
};

public enum SmBool
{
    smFalse = 0,
    smTrue = 1
};

public enum SmGPIOState
{
    smGPIOStateOutput = 0,
    smGPIOStateInput = 1
};

public enum SmReference
{
    smReferenceUseInternal = 0,
    smReferenceUseExternal = 1
};

public enum SmDeviceType
{
    smDeviceTypeSM200A = 0,
    smDeviceTypeSM200B = 1
};

public enum SmAudioType
{
    smAudioTypeAM = 0,
    smAudioTypeFM = 1,
    smAudioTypeUSB = 2,
    smAudioTypeLSB = 3,
    smAudioTypeCW = 4
};

public enum SmGPSState
{
    smGPSStateNotPresent = 0,
    smGPSStateLocked = 1,
    smGPSStateDisciplined = 2
};

[StructLayout(LayoutKind.Sequential)]
public struct SmGPIOStep
{
    public double freq;
    public Byte mask; // gpio bits

    public SmGPIOStep(double f, Byte m)
    {
        this.freq = f;
        this.mask = m;
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct SmDeviceDiagnostics
{
    float voltage;
    float currentInput;
    float currentOCXO;
    float current58;
    float tempFPGAInternal;
    float tempFPGANear;
    float tempOCXO;
    float tempVCO;
    float tempRFBoardLO;
    float tempPowerSupply;
}

#endregion
