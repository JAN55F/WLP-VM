using ChoiceTech.Halcon.Control;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace VMPro
{
    [Serializable]
    public class ToolBase
    {


        /// <summary>
        /// 生成Point集合的提示信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string FormatShowTip(object value)
        {
            try
            {
                if (value == null)
                    return "空";

                string temp = value.ToString();
                string result = string.Empty;
                switch (temp)
                {
                    case "VisionAndMotionPro.XYU":
                        return string.Empty;

                    case "HObject":
                        return string.Empty;
                    case "VisionAndMotionPro.Point":
                        XY point = value as XY;

                        result = string.Format("{0}  {1}", point.X.ToString("0000.000"), point.Y.ToString("0000.000"));
                        return result;
                    case "Double":
                        break;
                    case "HalconDotNet.HObject":
                        result = "图形变量暂不支持显示";
                        return result;
                    case "System.Collections.Generic.List`1[VisionAndMotionPro.Point]":
                        List<XY> L_point = value as List<XY>;

                        for (int i = 0; i < L_point.Count; i++)
                        {
                            result += string.Format("{0} |  {1}  {2}\r\n", (i + 1), L_point[i].X.ToString("0000.000"), L_point[i].Y.ToString("0000.000"));
                        }
                        return result;
                    case "System.Collections.Generic.List`1[VisionAndMotionPro.XYU]":
                        List<XYU> L_xyu = value as List<XYU>;

                        for (int i = 0; i < L_xyu.Count; i++)
                        {
                            result += string.Format("{0} |  {1}  {2}  {3}\r\n", (i + 1), L_xyu[i].Point.X.ToString("0000.000"), L_xyu[i].Point.Y.ToString("0000.000"), L_xyu[i].U.ToString("0000.000"));
                        }
                        return result;
                    default:
                        result = value.ToString();
                        return result;
                        break;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return string.Empty;
            }
        }
        /// <summary>
        /// 工具锁
        /// </summary>
        internal object obj = new object();
        /// <summary>
        /// 流程名
        /// </summary>
        internal string jobName = string.Empty;
        /// <summary>
        /// 工具运行状态
        /// </summary>
        internal ToolRunStatu toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Run : ToolRunStatu.未运行);
        /// <summary>
        /// 运行工具
        /// </summary>
        public virtual void Run(bool updateImage, bool runTool, string toolName) { }
        public virtual ToolRunResult Execute(ToolRunContext context)
        {
            Stopwatch sw = Stopwatch.StartNew();
            Run(true, false, context == null ? string.Empty : context.ToolName);
            sw.Stop();

            bool success = toolRunStatu == (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功);
            return new ToolRunResult
            {
                Success = success,
                Timeout = toolRunStatu == ToolRunStatu.运行超时,
                Canceled = toolRunStatu == ToolRunStatu.用户取消,
                Status = toolRunStatu,
                Message = toolRunStatu.ToString(),
                ElapsedMs = sw.ElapsedMilliseconds
            };
        }
        /// <summary>
        /// 图像窗体锁
        /// </summary>
        private object obj11 = new object();

        internal object GetValue(object obj, string name)
        {
            PropertyInfo[] dd = obj.GetType().GetProperties();
            foreach (PropertyInfo pi in obj.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
            {
                string temp = Regex.Split(pi.ToString(), " ")[0];
                //if (temp == "HalconDotNet.HObject"
                //    || temp == "VisionAndMotionPro.XYU"
                //    )
                //{
                if (pi.Name == name)
                {


                    return pi.GetValue(obj, null);
                }
            }
            return new object();
        }
        /// <summary>
        /// 设置显示字体
        /// </summary>
        internal void set_display_font(HTuple hv_WindowHandle, HTuple hv_Size, HTuple hv_Font, HTuple hv_Bold, HTuple hv_Slant)
        {
            try
            {
                HTuple hv_OS = null, hv_BufferWindowHandle = new HTuple();
                HTuple hv_Ascent = new HTuple(), hv_Descent = new HTuple();
                HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
                HTuple hv_Scale = new HTuple(), hv_Exception = new HTuple();
                HTuple hv_SubFamily = new HTuple(), hv_Fonts = new HTuple();
                HTuple hv_SystemFonts = new HTuple(), hv_Guess = new HTuple();
                HTuple hv_I = new HTuple(), hv_Index = new HTuple(), hv_AllowedFontSizes = new HTuple();
                HTuple hv_Distances = new HTuple(), hv_Indices = new HTuple();
                HTuple hv_FontSelRegexp = new HTuple(), hv_FontsCourier = new HTuple();
                HTuple hv_Bold_COPY_INP_TMP = hv_Bold.Clone();
                HTuple hv_Font_COPY_INP_TMP = hv_Font.Clone();
                HTuple hv_Size_COPY_INP_TMP = hv_Size.Clone();
                HTuple hv_Slant_COPY_INP_TMP = hv_Slant.Clone();
                HOperatorSet.GetSystem("operating_system", out hv_OS);
                if ((int)((new HTuple(hv_Size_COPY_INP_TMP.TupleEqual(new HTuple()))).TupleOr(
                    new HTuple(hv_Size_COPY_INP_TMP.TupleEqual(-1)))) != 0)
                {
                    hv_Size_COPY_INP_TMP = 16;
                }
                if ((int)(new HTuple(((hv_OS.TupleSubstr(0, 2))).TupleEqual("Win"))) != 0)
                {
                    try
                    {
                        HOperatorSet.OpenWindow(0, 0, 256, 256, 0, "buffer", "", out hv_BufferWindowHandle);
                        HOperatorSet.SetFont(hv_BufferWindowHandle, "-Consolas-16-*-0-*-*-1-");
                        HOperatorSet.GetStringExtents(hv_BufferWindowHandle, "test_string", out hv_Ascent,
                            out hv_Descent, out hv_Width, out hv_Height);
                        hv_Scale = 110.0 / hv_Width;
                        hv_Size_COPY_INP_TMP = ((hv_Size_COPY_INP_TMP * hv_Scale)).TupleInt();
                        HOperatorSet.CloseWindow(hv_BufferWindowHandle);
                    }
                    catch (HalconException HDevExpDefaultException1)
                    {
                        HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    }
                    if ((int)((new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("Courier"))).TupleOr(
                        new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("courier")))) != 0)
                    {
                        hv_Font_COPY_INP_TMP = "Courier New";
                    }
                    else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("sans"))) != 0)
                    {
                        hv_Font_COPY_INP_TMP = "Consolas";
                    }
                    else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("sans"))) != 0)
                    {
                        hv_Font_COPY_INP_TMP = "Arial";
                    }
                    else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("serif"))) != 0)
                    {
                        hv_Font_COPY_INP_TMP = "Times New Roman";
                    }
                    if ((int)(new HTuple(hv_Bold_COPY_INP_TMP.TupleEqual("true"))) != 0)
                    {
                        hv_Bold_COPY_INP_TMP = 1;
                    }
                    else if ((int)(new HTuple(hv_Bold_COPY_INP_TMP.TupleEqual("false"))) != 0)
                    {
                        hv_Bold_COPY_INP_TMP = 0;
                    }
                    else
                    {
                        hv_Exception = "Wrong value of control parameter Bold";
                        throw new HalconException(hv_Exception);
                    }
                    if ((int)(new HTuple(hv_Slant_COPY_INP_TMP.TupleEqual("true"))) != 0)
                    {
                        hv_Slant_COPY_INP_TMP = 1;
                    }
                    else if ((int)(new HTuple(hv_Slant_COPY_INP_TMP.TupleEqual("false"))) != 0)
                    {
                        hv_Slant_COPY_INP_TMP = 0;
                    }
                    else
                    {
                        hv_Exception = "Wrong value of control parameter Slant";
                        throw new HalconException(hv_Exception);
                    }
                    try
                    {
                        HOperatorSet.SetFont(hv_WindowHandle, ((((((("-" + hv_Font_COPY_INP_TMP) + "-") + hv_Size_COPY_INP_TMP) + "-*-") + hv_Slant_COPY_INP_TMP) + "-*-*-") + hv_Bold_COPY_INP_TMP) + "-");
                    }
                    catch (HalconException HDevExpDefaultException1)
                    {
                        HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    }
                }
                else if ((int)(new HTuple(((hv_OS.TupleSubstr(0, 2))).TupleEqual("Dar"))) != 0)
                {
                    hv_SubFamily = 0;
                    if ((int)(new HTuple(hv_Slant_COPY_INP_TMP.TupleEqual("true"))) != 0)
                    {
                        hv_SubFamily = hv_SubFamily.TupleBor(1);
                    }
                    else if ((int)(new HTuple(hv_Slant_COPY_INP_TMP.TupleNotEqual("false"))) != 0)
                    {
                        hv_Exception = "Wrong value of control parameter Slant";
                        throw new HalconException(hv_Exception);
                    }
                    if ((int)(new HTuple(hv_Bold_COPY_INP_TMP.TupleEqual("true"))) != 0)
                    {
                        hv_SubFamily = hv_SubFamily.TupleBor(2);
                    }
                    else if ((int)(new HTuple(hv_Bold_COPY_INP_TMP.TupleNotEqual("false"))) != 0)
                    {
                        hv_Exception = "Wrong value of control parameter Bold";
                        throw new HalconException(hv_Exception);
                    }
                    if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("sans"))) != 0)
                    {
                        hv_Fonts = new HTuple();
                        hv_Fonts[0] = "Menlo-Regular";
                        hv_Fonts[1] = "Menlo-Italic";
                        hv_Fonts[2] = "Menlo-Bold";
                        hv_Fonts[3] = "Menlo-BoldItalic";
                    }
                    else if ((int)((new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("Courier"))).TupleOr(
                        new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("courier")))) != 0)
                    {
                        hv_Fonts = new HTuple();
                        hv_Fonts[0] = "CourierNewPSMT";
                        hv_Fonts[1] = "CourierNewPS-ItalicMT";
                        hv_Fonts[2] = "CourierNewPS-BoldMT";
                        hv_Fonts[3] = "CourierNewPS-BoldItalicMT";
                    }
                    else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("sans"))) != 0)
                    {
                        hv_Fonts = new HTuple();
                        hv_Fonts[0] = "ArialMT";
                        hv_Fonts[1] = "Arial-ItalicMT";
                        hv_Fonts[2] = "Arial-BoldMT";
                        hv_Fonts[3] = "Arial-BoldItalicMT";
                    }
                    else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("serif"))) != 0)
                    {
                        hv_Fonts = new HTuple();
                        hv_Fonts[0] = "TimesNewRomanPSMT";
                        hv_Fonts[1] = "TimesNewRomanPS-ItalicMT";
                        hv_Fonts[2] = "TimesNewRomanPS-BoldMT";
                        hv_Fonts[3] = "TimesNewRomanPS-BoldItalicMT";
                    }
                    else
                    {
                        HOperatorSet.QueryFont(hv_WindowHandle, out hv_SystemFonts);
                        hv_Fonts = new HTuple();
                        hv_Fonts = hv_Fonts.TupleConcat(hv_Font_COPY_INP_TMP);
                        hv_Fonts = hv_Fonts.TupleConcat(hv_Font_COPY_INP_TMP);
                        hv_Fonts = hv_Fonts.TupleConcat(hv_Font_COPY_INP_TMP);
                        hv_Fonts = hv_Fonts.TupleConcat(hv_Font_COPY_INP_TMP);
                        hv_Guess = new HTuple();
                        hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP);
                        hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-Regular");
                        hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "MT");
                        for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_Guess.TupleLength())) - 1); hv_I = (int)hv_I + 1)
                        {
                            HOperatorSet.TupleFind(hv_SystemFonts, hv_Guess.TupleSelect(hv_I), out hv_Index);
                            if ((int)(new HTuple(hv_Index.TupleNotEqual(-1))) != 0)
                            {
                                if (hv_Fonts == null)
                                    hv_Fonts = new HTuple();
                                hv_Fonts[0] = hv_Guess.TupleSelect(hv_I);
                                break;
                            }
                        }
                        hv_Guess = new HTuple();
                        hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-Italic");
                        hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-ItalicMT");
                        hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-Oblique");
                        for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_Guess.TupleLength())) - 1); hv_I = (int)hv_I + 1)
                        {
                            HOperatorSet.TupleFind(hv_SystemFonts, hv_Guess.TupleSelect(hv_I), out hv_Index);
                            if ((int)(new HTuple(hv_Index.TupleNotEqual(-1))) != 0)
                            {
                                if (hv_Fonts == null)
                                    hv_Fonts = new HTuple();
                                hv_Fonts[1] = hv_Guess.TupleSelect(hv_I);
                                break;
                            }
                        }
                        hv_Guess = new HTuple();
                        hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-Bold");
                        hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-BoldMT");
                        for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_Guess.TupleLength())) - 1); hv_I = (int)hv_I + 1)
                        {
                            HOperatorSet.TupleFind(hv_SystemFonts, hv_Guess.TupleSelect(hv_I), out hv_Index);
                            if ((int)(new HTuple(hv_Index.TupleNotEqual(-1))) != 0)
                            {
                                if (hv_Fonts == null)
                                    hv_Fonts = new HTuple();
                                hv_Fonts[2] = hv_Guess.TupleSelect(hv_I);
                                break;
                            }
                        }
                        hv_Guess = new HTuple();
                        hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-BoldItalic");
                        hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-BoldItalicMT");
                        hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-BoldOblique");
                        for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_Guess.TupleLength())) - 1); hv_I = (int)hv_I + 1)
                        {
                            HOperatorSet.TupleFind(hv_SystemFonts, hv_Guess.TupleSelect(hv_I), out hv_Index);
                            if ((int)(new HTuple(hv_Index.TupleNotEqual(-1))) != 0)
                            {
                                if (hv_Fonts == null)
                                    hv_Fonts = new HTuple();
                                hv_Fonts[3] = hv_Guess.TupleSelect(hv_I);
                                break;
                            }
                        }
                    }
                    hv_Font_COPY_INP_TMP = hv_Fonts.TupleSelect(hv_SubFamily);
                    try
                    {
                        HOperatorSet.SetFont(hv_WindowHandle, (hv_Font_COPY_INP_TMP + "-") + hv_Size_COPY_INP_TMP);
                    }
                    catch (HalconException HDevExpDefaultException1)
                    {
                        HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    }
                }
                else
                {
                    hv_Size_COPY_INP_TMP = hv_Size_COPY_INP_TMP * 1.25;
                    hv_AllowedFontSizes = new HTuple();
                    hv_AllowedFontSizes[0] = 11;
                    hv_AllowedFontSizes[1] = 14;
                    hv_AllowedFontSizes[2] = 17;
                    hv_AllowedFontSizes[3] = 20;
                    hv_AllowedFontSizes[4] = 25;
                    hv_AllowedFontSizes[5] = 34;
                    if ((int)(new HTuple(((hv_AllowedFontSizes.TupleFind(hv_Size_COPY_INP_TMP))).TupleEqual(
                        -1))) != 0)
                    {
                        hv_Distances = ((hv_AllowedFontSizes - hv_Size_COPY_INP_TMP)).TupleAbs();
                        HOperatorSet.TupleSortIndex(hv_Distances, out hv_Indices);
                        hv_Size_COPY_INP_TMP = hv_AllowedFontSizes.TupleSelect(hv_Indices.TupleSelect(
                            0));
                    }
                    if ((int)((new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("sans"))).TupleOr(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual(
                        "Courier")))) != 0)
                    {
                        hv_Font_COPY_INP_TMP = "courier";
                    }
                    else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("sans"))) != 0)
                    {
                        hv_Font_COPY_INP_TMP = "helvetica";
                    }
                    else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("serif"))) != 0)
                    {
                        hv_Font_COPY_INP_TMP = "times";
                    }
                    if ((int)(new HTuple(hv_Bold_COPY_INP_TMP.TupleEqual("true"))) != 0)
                    {
                        hv_Bold_COPY_INP_TMP = "bold";
                    }
                    else if ((int)(new HTuple(hv_Bold_COPY_INP_TMP.TupleEqual("false"))) != 0)
                    {
                        hv_Bold_COPY_INP_TMP = "medium";
                    }
                    else
                    {
                        hv_Exception = "Wrong value of control parameter Bold";
                        throw new HalconException(hv_Exception);
                    }
                    if ((int)(new HTuple(hv_Slant_COPY_INP_TMP.TupleEqual("true"))) != 0)
                    {
                        if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("times"))) != 0)
                        {
                            hv_Slant_COPY_INP_TMP = "i";
                        }
                        else
                        {
                            hv_Slant_COPY_INP_TMP = "o";
                        }
                    }
                    else if ((int)(new HTuple(hv_Slant_COPY_INP_TMP.TupleEqual("false"))) != 0)
                    {
                        hv_Slant_COPY_INP_TMP = "r";
                    }
                    else
                    {
                        hv_Exception = "Wrong value of control parameter Slant";
                        throw new HalconException(hv_Exception);
                    }
                    try
                    {
                        HOperatorSet.SetFont(hv_WindowHandle, ((((((("-adobe-" + hv_Font_COPY_INP_TMP) + "-") + hv_Bold_COPY_INP_TMP) + "-") + hv_Slant_COPY_INP_TMP) + "-normal-*-") + hv_Size_COPY_INP_TMP) + "-*-*-*-*-*-*-*");
                    }
                    catch (HalconException HDevExpDefaultException1)
                    {
                        HDevExpDefaultException1.ToHTuple(out hv_Exception);
                        if ((int)((new HTuple(((hv_OS.TupleSubstr(0, 4))).TupleEqual("Linux"))).TupleAnd(
                            new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("courier")))) != 0)
                        {
                            HOperatorSet.QueryFont(hv_WindowHandle, out hv_Fonts);
                            hv_FontSelRegexp = (("^-[^-]*-[^-]*[Cc]ourier[^-]*-" + hv_Bold_COPY_INP_TMP) + "-") + hv_Slant_COPY_INP_TMP;
                            hv_FontsCourier = ((hv_Fonts.TupleRegexpSelect(hv_FontSelRegexp))).TupleRegexpMatch(
                                hv_FontSelRegexp);
                            if ((int)(new HTuple((new HTuple(hv_FontsCourier.TupleLength())).TupleEqual(
                                0))) != 0)
                            {
                                hv_Exception = "Wrong font name";
                            }
                            else
                            {
                                try
                                {
                                    HOperatorSet.SetFont(hv_WindowHandle, (((hv_FontsCourier.TupleSelect(
                                        0)) + "-normal-*-") + hv_Size_COPY_INP_TMP) + "-*-*-*-*-*-*-*");
                                }
                                catch (HalconException HDevExpDefaultException2)
                                {
                                    HDevExpDefaultException2.ToHTuple(out hv_Exception);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 显示文本
        /// </summary>
        /// <param name="jobName">文本信息</param>
        /// <param name="text"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        internal void Show_Text(string text, double row = 20, double col = 20)
        {
            try
            {
                foreach (KeyValuePair<string, Frm_ImageWindow> item in Frm_ImageWindow.D_imageWindow)
                {
                    if (item.Key == Job.FindJobByName(jobName).debugImageWindow)
                    {
                        if ((Machine.machineRunStatu != MachineRunStatu.Running && !Job.FindJobByName(jobName).isRunLoop)
                            || (Machine.machineRunStatu != MachineRunStatu.Running && Job.FindJobByName(jobName).isRunLoop) && Job.FindJobByName(jobName).jobName == Frm_Job.Instance.tbc_jobs.SelectedTab.Text)
                            item.Value.Show();
                        Frm_Main.Instance.disp_message(item.Value.hwc_imageWindow.HWindowHalconID,
                                                            text,
                                                            new HTuple("image"),
                                                            new HTuple(row),
                                                            new HTuple(col),
                                                            new HTuple("green"),
                                                            new HTuple("false"));
                        return;
                    }
                }
                if (!Machine.loading)
                {
                    if (Frm_ImageWindow.D_imageWindow.Count > 0)
                    {
                        Job.FindJobByName(jobName).debugImageWindow = Frm_ImageWindow.D_imageWindow.Values.ToArray()[0].Text;
                        Show_Text(text, row, col);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 显示图像
        /// </summary>
        /// <param name="image"></param>
        internal void Display_Image(HObject image, Frm_ImageWindow frm_imageWindow)
        {
            try
            {
                Application.DoEvents();
                lock (obj11)
                {
                    if (frm_imageWindow.isFullScreenMode)
                        HOperatorSet.DispObj(image, Frm_FullScreen.Instance.windowHandle);
                    else
                        frm_imageWindow.hwc_imageWindow.HobjectToHimage(image);
                    frm_imageWindow.currentImage = image;
                }
                Application.DoEvents();

            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 显示图像
        /// </summary>
        internal void ShowImage(HObject image)
        {
            try
            {
                foreach (KeyValuePair<string, Frm_ImageWindow> item in Frm_ImageWindow.D_imageWindow)
                {
                    if (item.Key == Job.FindJobByName(jobName).debugImageWindow)
                    {
                        //以下两行是防止运行一次流程时流程编辑器老是闪烁，不好看
                        IDockContent temp = Frm_Main.Instance.dockPanel.ActiveDocument;
                        Frm_ImageWindow ff = temp as Frm_ImageWindow;
                        if ((Machine.machineRunStatu != MachineRunStatu.Running && !Job.FindJobByName(jobName).isRunLoop && ff.Text != Job.FindJobByName(jobName).debugImageWindow)
                            || (Machine.machineRunStatu != MachineRunStatu.Running && Job.FindJobByName(jobName).isRunLoop) && Job.FindJobByName(jobName).jobName == Frm_Job.Instance.tbc_jobs.SelectedTab.Text)
                            //if (!Frm_ImageWindow.isMax)         //放大模式不切换窗体
                            item.Value.Show();


                        Display_Image(image, item.Value);



                        //////HTuple w, h;
                        //////HOperatorSet.GetImageSize(image, out w, out h);
                        //////HOperatorSet.SetWindowExtents(Job.FindJobByName(jobName).www, 0, 0, (w.I), (h.I));
                        //////HOperatorSet.SetPart(Job.FindJobByName(jobName).www, 0, 0, (h - 1), (w - 1));
                        //////HOperatorSet.DispObj(image, Job.FindJobByName(jobName).www);


                        Application.DoEvents();

                        return;
                    }
                }
                if (!Machine.loading)
                {
                    if (Frm_ImageWindow.D_imageWindow.Count > 0)
                    {
                        Job.FindJobByName(jobName).debugImageWindow = Frm_ImageWindow.D_imageWindow.Values.ToArray()[0].Text;
                        ShowImage(image);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 显示图像
        /// </summary>
        internal void ShowObj(HObject obj, string color)
        {
            try
            {
                foreach (KeyValuePair<string, Frm_ImageWindow> item in Frm_ImageWindow.D_imageWindow)
                {
                    if (item.Key == Job.FindJobByName(jobName).debugImageWindow)
                    {
                        //以下两行是防止运行一次流程时流程编辑器老是闪烁，不好看
                        IDockContent temp = Frm_Main.Instance.dockPanel.ActiveDocument;
                        Frm_ImageWindow ff = temp as Frm_ImageWindow;
                        if ((Machine.machineRunStatu != MachineRunStatu.Running && !Job.FindJobByName(jobName).isRunLoop && ff.Text != Job.FindJobByName(jobName).debugImageWindow)
                            || (Machine.machineRunStatu != MachineRunStatu.Running && Job.FindJobByName(jobName).isRunLoop) && Job.FindJobByName(jobName).jobName == Frm_Job.Instance.tbc_jobs.SelectedTab.Text)
                            item.Value.Show();
                        item.Value.hwc_imageWindow.DispObj(obj, color);

                        HOperatorSet.SetLineWidth(Job.FindJobByName(jobName).www, 5);
                        HOperatorSet.SetColor(Job.FindJobByName(jobName).www, color);
                        HOperatorSet.DispObj(obj, Job.FindJobByName(jobName).www);
                        return;
                    }
                }
                if (!Machine.loading)
                {
                    if (Frm_ImageWindow.D_imageWindow.Count > 0)
                    {
                        Job.FindJobByName(jobName).debugImageWindow = Frm_ImageWindow.D_imageWindow.Values.ToArray()[0].Text;
                        ShowObj(obj, color);

                    }
                }

                HOperatorSet.SetColor(Job.FindJobByName(jobName).www, color);
                HOperatorSet.DispObj(obj, Job.FindJobByName(jobName).www);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 显示图像
        /// </summary>
        internal void SetDraw(string drawMode)
        {
            try
            {
                foreach (KeyValuePair<string, Frm_ImageWindow> item in Frm_ImageWindow.D_imageWindow)
                {
                    if (item.Key == Job.FindJobByName(jobName).debugImageWindow)
                    {
                        //以下两行是防止运行一次流程时流程编辑器老是闪烁，不好看
                        IDockContent temp = Frm_Main.Instance.dockPanel.ActiveDocument;
                        Frm_ImageWindow ff = temp as Frm_ImageWindow;
                        if ((Machine.machineRunStatu != MachineRunStatu.Running && !Job.FindJobByName(jobName).isRunLoop && ff.Text != Job.FindJobByName(jobName).debugImageWindow)
                            || (Machine.machineRunStatu != MachineRunStatu.Running && Job.FindJobByName(jobName).isRunLoop) && Job.FindJobByName(jobName).jobName == Frm_Job.Instance.tbc_jobs.SelectedTab.Text)
                            item.Value.Show();
                        HOperatorSet.SetDraw(item.Value.hwc_imageWindow.HWindowHalconID, drawMode);

                        HOperatorSet.SetDraw(Job.FindJobByName(jobName).www, drawMode);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 显示图像
        /// </summary>
        internal void SetLineWidth(int width)
        {
            try
            {
                foreach (KeyValuePair<string, Frm_ImageWindow> item in Frm_ImageWindow.D_imageWindow)
                {
                    if (item.Key == Job.FindJobByName(jobName).debugImageWindow)
                    {
                        //以下两行是防止运行一次流程时流程编辑器老是闪烁，不好看
                        IDockContent temp = Frm_Main.Instance.dockPanel.ActiveDocument;
                        Frm_ImageWindow ff = temp as Frm_ImageWindow;
                        if ((Machine.machineRunStatu != MachineRunStatu.Running && !Job.FindJobByName(jobName).isRunLoop && ff.Text != Job.FindJobByName(jobName).debugImageWindow)
                            || (Machine.machineRunStatu != MachineRunStatu.Running && Job.FindJobByName(jobName).isRunLoop) && Job.FindJobByName(jobName).jobName == Frm_Job.Instance.tbc_jobs.SelectedTab.Text)
                            item.Value.Show();
                        HOperatorSet.SetLineWidth(item.Value.hwc_imageWindow.HWindowHalconID, width);

                        HOperatorSet.SetLineWidth(Job.FindJobByName(jobName).www, width);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 通过流程名获取窗体句柄
        /// </summary>
        /// <param name="jobName"></param>
        /// <returns></returns>
        internal Frm_ImageWindow GetImageWindowControl()
        {
            try
            {
                foreach (KeyValuePair<string, Frm_ImageWindow> item in Frm_ImageWindow.D_imageWindow)
                {
                    if (item.Key == Job.FindJobByName(jobName).debugImageWindow)
                    {
                        IDockContent temp = Frm_Main.Instance.dockPanel.ActiveDocument;
                        Frm_ImageWindow ff = temp as Frm_ImageWindow;
                        if ((Machine.machineRunStatu != MachineRunStatu.Running && !Job.FindJobByName(jobName).isRunLoop && ff.Text != Job.FindJobByName(jobName).debugImageWindow)
                      || (Machine.machineRunStatu != MachineRunStatu.Running && Job.FindJobByName(jobName).isRunLoop) && Job.FindJobByName(jobName).jobName == Frm_Job.Instance.tbc_jobs.SelectedTab.Text)
                            item.Value.Show();              //切换到当前图像窗体
                        return item.Value;
                    }
                }
                return Frm_ImageWindow.Instance;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return null;
            }
        }
        /// <summary>
        /// 通过流程名获取窗体句柄，与上一个函数的使用场合不同，此函数用于工具构造函数里面，因为创建工具时还没有流程名，所以只能用这个函数
        /// </summary>
        /// <param name="jobName"></param>
        /// <returns></returns>
        internal Frm_ImageWindow GetImageWindowControl(string jobName)
        {
            try
            {
                foreach (KeyValuePair<string, Frm_ImageWindow> item in Frm_ImageWindow.D_imageWindow)
                {
                    if (item.Key == Job.FindJobByName(jobName).debugImageWindow)
                    {
                        IDockContent temp = Frm_Main.Instance.dockPanel.ActiveDocument;
                        Frm_ImageWindow ff = temp as Frm_ImageWindow;
                        if ((Machine.machineRunStatu != MachineRunStatu.Running && !Job.FindJobByName(jobName).isRunLoop && ff == null)
                            || (Machine.machineRunStatu != MachineRunStatu.Running && !Job.FindJobByName(jobName).isRunLoop && ff.Text != Job.FindJobByName(jobName).debugImageWindow)
                      || (Machine.machineRunStatu != MachineRunStatu.Running && Job.FindJobByName(jobName).isRunLoop) && Job.FindJobByName(jobName).jobName == Frm_Job.Instance.tbc_jobs.SelectedTab.Text)
                            item.Value.Show();              //切换到当前图像窗体
                        return item.Value;
                    }
                }
                return Frm_ImageWindow.Instance;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return null;
            }
        }
        /// <summary>
        /// 设置颜色
        /// </summary>
        internal void SetColor(string jobName, string color)
        {
            try
            {
                foreach (KeyValuePair<string, Frm_ImageWindow> item in Frm_ImageWindow.D_imageWindow)
                {
                    if (item.Key == Job.FindJobByName(jobName).debugImageWindow)
                    {
                        HOperatorSet.SetColor(item.Value.hwc_imageWindow.HWindowHalconID, new HTuple(color));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 在图像中显示字符串
        /// </summary>
        internal void disp_message(HTuple windowHandle, HTuple hv_String, HTuple hv_Row, HTuple hv_Column, HTuple hv_Color, HTuple hv_Box)
        {
            try
            {
                HTuple hv_CoordSystem = "image";
                HTuple hv_M = null, hv_N = null, hv_Red = null;
                HTuple hv_Green = null, hv_Blue = null, hv_RowI1Part = null;
                HTuple hv_ColumnI1Part = null, hv_RowI2Part = null, hv_ColumnI2Part = null;
                HTuple hv_RowIWin = null, hv_ColumnIWin = null, hv_WidthWin = null;
                HTuple hv_HeightWin = null, hv_I = null, hv_RowI = new HTuple();
                HTuple hv_ColumnI = new HTuple(), hv_StringI = new HTuple();
                HTuple hv_MaxAscent = new HTuple(), hv_MaxDescent = new HTuple();
                HTuple hv_MaxWidth = new HTuple(), hv_MaxHeight = new HTuple();
                HTuple hv_R1 = new HTuple(), hv_C1 = new HTuple(), hv_FactorRowI = new HTuple();
                HTuple hv_FactorColumnI = new HTuple(), hv_UseShadow = new HTuple();
                HTuple hv_ShadowColor = new HTuple(), hv_Exception = new HTuple();
                HTuple hv_Width = new HTuple(), hv_Index = new HTuple();
                HTuple hv_Ascent = new HTuple(), hv_Descent = new HTuple();
                HTuple hv_W = new HTuple(), hv_H = new HTuple(), hv_FrameHeight = new HTuple();
                HTuple hv_FrameWidth = new HTuple(), hv_R2 = new HTuple();
                HTuple hv_C2 = new HTuple(), hv_DrawMode = new HTuple();
                HTuple hv_CurrentColor = new HTuple();
                HTuple hv_Box_COPY_INP_TMP = hv_Box.Clone();
                HTuple hv_Color_COPY_INP_TMP = hv_Color.Clone();
                HTuple hv_Column_COPY_INP_TMP = hv_Column.Clone();
                HTuple hv_Row_COPY_INP_TMP = hv_Row.Clone();
                HTuple hv_String_COPY_INP_TMP = hv_String.Clone();
                // return;
                if ((int)(new HTuple(hv_Color_COPY_INP_TMP.TupleEqual(new HTuple()))) != 0)
                {
                    hv_Color_COPY_INP_TMP = "";
                }
                if ((int)(new HTuple(hv_Box_COPY_INP_TMP.TupleEqual(new HTuple()))) != 0)
                {
                    hv_Box_COPY_INP_TMP = "false";
                }
                hv_M = (new HTuple(hv_Row_COPY_INP_TMP.TupleLength())) * (new HTuple(hv_Column_COPY_INP_TMP.TupleLength()
                    ));
                hv_N = new HTuple(hv_Row_COPY_INP_TMP.TupleLength());
                if ((int)((new HTuple(hv_M.TupleEqual(0))).TupleOr(new HTuple(hv_String_COPY_INP_TMP.TupleEqual(
                    new HTuple())))) != 0)
                {
                    return;
                }
                if ((int)(new HTuple(hv_M.TupleNotEqual(1))) != 0)
                {
                    if ((int)(new HTuple((new HTuple(hv_Row_COPY_INP_TMP.TupleLength())).TupleEqual(
                        1))) != 0)
                    {
                        hv_N = new HTuple(hv_Column_COPY_INP_TMP.TupleLength());
                        HOperatorSet.TupleGenConst(hv_N, hv_Row_COPY_INP_TMP, out hv_Row_COPY_INP_TMP);
                    }
                    else if ((int)(new HTuple((new HTuple(hv_Column_COPY_INP_TMP.TupleLength()
                        )).TupleEqual(1))) != 0)
                    {
                        HOperatorSet.TupleGenConst(hv_N, hv_Column_COPY_INP_TMP, out hv_Column_COPY_INP_TMP);
                    }
                    else if ((int)(new HTuple((new HTuple(hv_Column_COPY_INP_TMP.TupleLength()
                        )).TupleNotEqual(new HTuple(hv_Row_COPY_INP_TMP.TupleLength())))) != 0)
                    {
                        throw new HalconException("Number of elements in Row and Column does not match.");
                    }
                    if ((int)(new HTuple((new HTuple(hv_String_COPY_INP_TMP.TupleLength())).TupleEqual(
                        1))) != 0)
                    {
                        HOperatorSet.TupleGenConst(hv_N, hv_String_COPY_INP_TMP, out hv_String_COPY_INP_TMP);
                    }
                    else if ((int)(new HTuple((new HTuple(hv_String_COPY_INP_TMP.TupleLength()
                        )).TupleNotEqual(hv_N))) != 0)
                    {
                        throw new HalconException("Number of elements in Strings does not match number of positions.");
                    }
                }
                // return;
                HOperatorSet.GetRgb(windowHandle, out hv_Red, out hv_Green, out hv_Blue);
                HOperatorSet.GetPart(windowHandle, out hv_RowI1Part, out hv_ColumnI1Part,
                    out hv_RowI2Part, out hv_ColumnI2Part);
                HOperatorSet.GetWindowExtents(windowHandle, out hv_RowIWin, out hv_ColumnIWin,
                    out hv_WidthWin, out hv_HeightWin);
                HOperatorSet.SetPart(windowHandle, 0, 0, hv_HeightWin - 1, hv_WidthWin - 1);
                HTuple end_val89 = hv_N - 1;
                HTuple step_val89 = 1;
                for (hv_I = 0; hv_I.Continue(end_val89, step_val89); hv_I = hv_I.TupleAdd(step_val89))
                {
                    hv_RowI = hv_Row_COPY_INP_TMP.TupleSelect(hv_I);
                    hv_ColumnI = hv_Column_COPY_INP_TMP.TupleSelect(hv_I);
                    if ((int)(new HTuple(hv_N.TupleEqual(1))) != 0)
                    {
                        hv_StringI = hv_String_COPY_INP_TMP.Clone();
                    }
                    else
                    {
                        hv_StringI = hv_String_COPY_INP_TMP.TupleSelect(hv_I);
                    }
                    if ((int)(new HTuple(hv_RowI.TupleEqual(-1))) != 0)
                    {
                        hv_RowI = 12;
                    }
                    if ((int)(new HTuple(hv_ColumnI.TupleEqual(-1))) != 0)
                    {
                        hv_ColumnI = 12;
                    }
                    hv_StringI = ((("" + hv_StringI) + "")).TupleSplit("\n");
                    HOperatorSet.GetFontExtents(windowHandle, out hv_MaxAscent, out hv_MaxDescent,
                        out hv_MaxWidth, out hv_MaxHeight);
                    if ((int)(new HTuple(hv_CoordSystem.TupleEqual("window"))) != 0)
                    {
                        hv_R1 = hv_RowI.Clone();
                        hv_C1 = hv_ColumnI.Clone();
                    }
                    else
                    {
                        hv_FactorRowI = (1.0 * hv_HeightWin) / ((hv_RowI2Part - hv_RowI1Part) + 1);
                        hv_FactorColumnI = (1.0 * hv_WidthWin) / ((hv_ColumnI2Part - hv_ColumnI1Part) + 1);
                        hv_R1 = (((hv_RowI - hv_RowI1Part) + 0.5) * hv_FactorRowI) - 0.5;
                        hv_C1 = (((hv_ColumnI - hv_ColumnI1Part) + 0.5) * hv_FactorColumnI) - 0.5;
                    }
                    hv_UseShadow = 1;
                    hv_ShadowColor = "gray";
                    if ((int)(new HTuple(((hv_Box_COPY_INP_TMP.TupleSelect(0))).TupleEqual("true"))) != 0)
                    {
                        if (hv_Box_COPY_INP_TMP == null)
                            hv_Box_COPY_INP_TMP = new HTuple();
                        hv_Box_COPY_INP_TMP[0] = "#fce9d4";
                        hv_ShadowColor = "#f28d26";
                    }
                    if ((int)(new HTuple((new HTuple(hv_Box_COPY_INP_TMP.TupleLength())).TupleGreater(
                        1))) != 0)
                    {
                        if ((int)(new HTuple(((hv_Box_COPY_INP_TMP.TupleSelect(1))).TupleEqual("true"))) != 0)
                        {
                        }
                        else if ((int)(new HTuple(((hv_Box_COPY_INP_TMP.TupleSelect(1))).TupleEqual(
                            "false"))) != 0)
                        {
                            hv_UseShadow = 0;
                        }
                        else
                        {
                            hv_ShadowColor = hv_Box_COPY_INP_TMP.TupleSelect(1);
                            try
                            {
                                HOperatorSet.SetColor(windowHandle, hv_Box_COPY_INP_TMP.TupleSelect(
                                    1));
                            }
                            catch (HalconException HDevExpDefaultException1)
                            {
                                HDevExpDefaultException1.ToHTuple(out hv_Exception);
                                hv_Exception = new HTuple("Wrong value of control parameter Box[1] (must be a 'true', 'false', or a valid color string)");
                                throw new HalconException(hv_Exception);
                            }
                        }
                    }
                    if ((int)(new HTuple(((hv_Box_COPY_INP_TMP.TupleSelect(0))).TupleNotEqual("false"))) != 0)
                    {
                        try
                        {
                            HOperatorSet.SetColor(windowHandle, hv_Box_COPY_INP_TMP.TupleSelect(
                                0));
                        }
                        catch (HalconException HDevExpDefaultException1)
                        {
                            HDevExpDefaultException1.ToHTuple(out hv_Exception);
                            hv_Exception = new HTuple("Wrong value of control parameter Box[0] (must be a 'true', 'false', or a valid color string)");
                            throw new HalconException(hv_Exception);
                        }
                        hv_StringI = (" " + hv_StringI) + " ";
                        hv_Width = new HTuple();
                        for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_StringI.TupleLength()
                            )) - 1); hv_Index = (int)hv_Index + 1)
                        {
                            HOperatorSet.GetStringExtents(windowHandle, hv_StringI.TupleSelect(hv_Index),
                                out hv_Ascent, out hv_Descent, out hv_W, out hv_H);
                            hv_Width = hv_Width.TupleConcat(hv_W);
                        }
                        hv_FrameHeight = hv_MaxHeight * (new HTuple(hv_StringI.TupleLength()));
                        hv_FrameWidth = (((new HTuple(0)).TupleConcat(hv_Width))).TupleMax();
                        hv_R2 = hv_R1 + hv_FrameHeight;
                        hv_C2 = hv_C1 + hv_FrameWidth;
                        HOperatorSet.GetDraw(windowHandle, out hv_DrawMode);
                        HOperatorSet.SetDraw(windowHandle, "fill");
                        HOperatorSet.SetColor(windowHandle, hv_ShadowColor);
                        if ((int)(hv_UseShadow) != 0)
                        {
                            HOperatorSet.DispRectangle1(windowHandle, hv_R1 + 1, hv_C1 + 1, hv_R2 + 1,
                                hv_C2 + 1);
                        }
                        HOperatorSet.SetColor(windowHandle, hv_Box_COPY_INP_TMP.TupleSelect(0));
                        HOperatorSet.DispRectangle1(windowHandle, hv_R1, hv_C1, hv_R2, hv_C2);
                        HOperatorSet.SetDraw(windowHandle, hv_DrawMode);
                    }
                    //   return;
                    for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_StringI.TupleLength())) - 1); hv_Index = (int)hv_Index + 1)
                    {
                        if ((int)(new HTuple(hv_N.TupleEqual(1))) != 0)
                        {
                            hv_CurrentColor = hv_Color_COPY_INP_TMP.TupleSelect(hv_Index % (new HTuple(hv_Color_COPY_INP_TMP.TupleLength()
                                )));
                        }
                        else
                        {
                            hv_CurrentColor = hv_Color_COPY_INP_TMP.TupleSelect(hv_I % (new HTuple(hv_Color_COPY_INP_TMP.TupleLength()
                                )));
                        }
                        if ((int)((new HTuple(hv_CurrentColor.TupleNotEqual(""))).TupleAnd(new HTuple(hv_CurrentColor.TupleNotEqual(
                            "auto")))) != 0)
                        {
                            try
                            {
                                HOperatorSet.SetColor(windowHandle, hv_CurrentColor);
                            }
                            catch (HalconException HDevExpDefaultException1)
                            {
                                HDevExpDefaultException1.ToHTuple(out hv_Exception);
                                hv_Exception = ((("Wrong value of control parameter Color[" + (hv_Index % (new HTuple(hv_Color_COPY_INP_TMP.TupleLength()
                                    )))) + "] == '") + hv_CurrentColor) + "' (must be a valid color string)";
                                throw new HalconException(hv_Exception);
                            }
                        }
                        else
                        {
                            HOperatorSet.SetRgb(windowHandle, hv_Red, hv_Green, hv_Blue);
                        }
                        hv_RowI = hv_R1 + (hv_MaxHeight * hv_Index);
                        HOperatorSet.SetTposition(windowHandle, hv_RowI, hv_C1);
                        HOperatorSet.WriteString(windowHandle, hv_StringI.TupleSelect(hv_Index));
                    }
                }
                HOperatorSet.SetRgb(windowHandle, hv_Red, hv_Green, hv_Blue);
                HOperatorSet.SetPart(windowHandle, hv_RowI1Part, hv_ColumnI1Part, hv_RowI2Part,
                    hv_ColumnI2Part);
                return;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        internal void DispMessage(HTuple windowHandle, HTuple hv_String, int size, HTuple hv_Row, HTuple hv_Column, HTuple hv_Color, HTuple hv_Box)
        {
            try
            {
                set_display_font(windowHandle, size, "sans", "true", "false");
                set_display_font(windowHandle, size, "sans", "true", "false");
                disp_message(windowHandle, hv_String, hv_Row, hv_Column, hv_Color, hv_Box);
                set_display_font(Job.FindJobByName(jobName).www, 30, "sans", "true", "false");
                //////disp_message(Job.FindJobByName(jobName).www, hv_String, hv_Row, hv_Column, hv_Color, hv_Box);
            }
            catch (Exception ex)
            { }
        }
        /// <summary>
        /// 设置颜色
        /// </summary>
        internal void ClearWindow()
        {
            try
            {
                foreach (KeyValuePair<string, Frm_ImageWindow> item in Frm_ImageWindow.D_imageWindow)
                {
                    if (item.Key == Job.FindJobByName(jobName).debugImageWindow)
                    {
                        HOperatorSet.ClearWindow(item.Value.hwc_imageWindow.HWindowHalconID);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }




    }
    /// <summary>
    /// 工具运行状态
    /// </summary>
    public enum ToolRunStatu
    {
        Not_Run,
        Not_Enabled,
        No_Input_Image,
        Not_Input_Image,
        Character_Untrained,
        Not_Assign_Image_Template,
        Not_Assign_Input_Image,
        Not_Assign_Input_Source,
        Not_Assign_Input_Pos,
        Not_Asign_Input_Source,
        Lack_Of_Input_Image,
        Lack_Of_Input_Search_Region,
        Not_Assign_Path,
        Not_Asign_Input_Image,
        Input_Image_Cannot_Be_Converted,
        Not_Create_Template,
        No_Image_In_Folder,
        File_Error_Or_Path_Invalid,
        Not_Assign_Acq_Device,
        No_Circle_Found,
        Not_Succeed,
        Succeed,
        Unknown,
        No_Input_String,
        未运行,
        未启用,
        缺少输入搜索区域,
        未指定图像路径,
        无输入图像,
        未创建模板,
        未训练字符,
        无输入字符串,
        未指定输入图像,
        未指定图像模板,
        缺少输入图像,
        未指定输入坐标点,
        输入项未链接源,
        输入图像不能被转化,
        文件夹内无图像,
        图像文件异常或路径不合法,
        未指定采集设备,
        成功,
        未知原因,
        定位结果U值超限,
        定位结果X值超限,
        定位结果Y值超限,
        未匹配到模板,
        未找到边,
        运行成功但是所有的特征都被设置不显示,
        未找到电池,
        电池形状异常或视野中有干扰物,
        相机实时状态下不可采集图像,
        未找到线,
        未找到圆,
        未扫描到条码,
        匹配数量不足,
        未指定以太网通讯端,
        未指定以太网触发命令,
        未建立通讯连接,
        相机未连接,
        采集图像时出错,
        未指定被引用标定工具,
        运行超时,
        用户取消,
        未指定保存路径,
    }

    [Serializable]
    public class ToolRunContext
    {
        public string JobName;
        public string ToolName;
        public int TimeoutMs;
        public Func<bool> IsCancellationRequested;
    }

    [Serializable]
    public class ToolRunResult
    {
        public bool Success;
        public bool Canceled;
        public bool Timeout;
        public ToolRunStatu Status;
        public string Message;
        public long ElapsedMs;
    }

}
