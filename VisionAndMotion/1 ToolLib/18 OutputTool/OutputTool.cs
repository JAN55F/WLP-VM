using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VMPro
{
    [Serializable]
    internal class OutputTool   :ToolBase 
    {

        /// <summary>
        /// 运行工具
        /// </summary>
        public override void Run(bool updateImage, bool temp, string toolName)
        {
            toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Succeed : ToolRunStatu.未知原因);
            //此工具没有要运行的实体
            toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功);
        }

    }
}
