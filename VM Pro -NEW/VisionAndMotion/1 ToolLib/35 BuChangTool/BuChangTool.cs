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
    internal class BuChangTool : ToolBase
    {
        internal XYU templatePos = new XYU();
        internal XYU inputPos = new XYU();
        internal XYU outputPos = new XYU();
        internal XYU buchang = new XYU();
        internal XYU workPos = new XYU();

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
                    toolRunStatu = Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Succeed : ToolRunStatu.未知原因 ;
                   
                    XYU offset = inputPos - (templatePos+buchang );
                    outputPos = workPos + offset; 

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
