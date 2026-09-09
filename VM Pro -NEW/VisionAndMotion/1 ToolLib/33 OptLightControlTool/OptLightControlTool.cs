using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using HalconDotNet;
using ViewWindow.Model;
using System.Reflection;
using System.ComponentModel;
using System.Threading;

namespace VMPro
{
    [Serializable]
    internal class OptLightControlTool : ToolBase
    {
        internal OptLightControlTool()
        {
            //HOperatorSet.GenEmptyRegion(out _searchRegion);
        }

        internal bool controlMode = true;
        private object obj = new object();
        /// <summary>
        /// 运行工具
        /// </summary>
        /// <param name="updateImage">是否刷新图像</param>
        public override void Run(bool updateImage, bool b, string toolName)
        {
            try
            {
                lock (obj)
                {
                    toolRunStatu = Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Succeed : ToolRunStatu.未知原因;

                    if (Light_OPTTool.isFirstRun)
                    {
                        Light_OPTTool.isFirstRun = false;
                        foreach (KeyValuePair<string, string> item in ((Light_OPTTool)(Job.FindJobByName("其它设备").FindToolByName("奥普特光源控制"))).L_controls)
                        {
                            //////Frm_OPTLightTool.Instance.ctl_opt._optControl  = new OPTControl(ControlMode.ID, item.Key, item.Value);
                        }
                        
                    }
                    //////if (controlMode)
                    //////    Frm_OPTLightTool.Instance.ctl_opt._optControl.TurnOnChannel(4);
                    //////else
                    //////    Frm_OPTLightTool.Instance.ctl_opt._optControl.TurnOffChannel(4);
                    Thread.Sleep(100);
                    toolRunStatu = Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

    }
}
