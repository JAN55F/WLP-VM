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

namespace VMPro
{
    [Serializable]
    internal class MarkTool : ToolBase
    {
        internal MarkTool()
        {
            //HOperatorSet.GenEmptyRegion(out _searchRegion);
        }

        internal XY inputPoint = new XY();

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
                    HOperatorSet.DispCross(GetImageWindowControl().hwc_imageWindow.HWindowHalconID, inputPoint .X  , inputPoint .Y  , 50, 0);
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
