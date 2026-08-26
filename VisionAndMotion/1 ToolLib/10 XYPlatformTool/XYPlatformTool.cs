using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using HalconDotNet;

namespace VMPro
{
    [Serializable]
    internal class XYPlatformTool : ToolBase
    {

        /// <summary>
        /// 取料位置
        /// </summary>
        internal XYU pickPos = new XYU();
        /// <summary>
        /// 取料位置补偿量
        /// </summary>
        internal XYU pickPosOffset = new XYU();
        /// <summary>
        /// 特征点坐标
        /// </summary>
        internal XYU featurePos = new XYU();
        /// <summary>
        /// 输出位置
        /// </summary>
        internal XYU outputPos = new XYU();
        /// <summary>
        /// 输入位置
        /// </summary>
        internal XYU inputPos = new XYU();
        /// <summary>
        /// 工具锁
        /// </summary>
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
                    outputPos = ((pickPos + pickPosOffset) + (inputPos - featurePos));
                    outputPos.U = + (pickPos + pickPosOffset).U +(featurePos .U  -inputPos.U);
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
