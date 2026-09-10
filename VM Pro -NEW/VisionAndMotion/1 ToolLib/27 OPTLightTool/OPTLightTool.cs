using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HalconDotNet;
using ViewWindow.Model;
using HalconTool;
using System.Threading;

namespace VMPro
{
    [Serializable]
    internal class Light_OPTTool : ToolBase
    {

        internal  static bool isFirstRun = true;
        public   Dictionary<string, string> L_controls = new Dictionary<string, string>();

        //////private void CloseLight(object o)
        //////{
        //////    try
        //////    {
        //////        Thread.Sleep(2000);
        //////        Frm_OPTLightTool.Instance.ctl_opt._optControl.TurnOffChannel(1);
        //////    }
        //////    catch (Exception ex)
        //////    {
        //////        LogHelper.SaveErrorInfo(ex);
        //////    }
        //////}
        /// <summary>
        /// 运行工具
        /// </summary>
        public override void Run(bool updateImage1, bool b, string toolName)
        {
            try
            {
                toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Succeed : ToolRunStatu.未知原因 );

                //////if (!Frm_OPTLightTool.Instance.ctl_opt._optControl.IsConnnected)
                //////    Frm_OPTLightTool.Instance.ctl_opt._optControl.Open();

                //////if (isStrobscopic)
                //////{
                //////    Frm_OPTLightTool.Instance.ctl_opt._optControl.TurnOnChannel(1);
                //////    ThreadPool.QueueUserWorkItem(new WaitCallback(CloseLight));
                //////}
                //////else
                //////{
                //////    if (!Frm_OPTLightTool.Instance.ctl_opt._optControl.ChannelsInf[0].IsTurnOn)
                //////        Frm_OPTLightTool.Instance.ctl_opt._optControl.TurnOnChannel(0);
                //////}


                toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

    }
}
