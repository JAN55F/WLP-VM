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
using VMPro.Properties;

namespace VMPro
{
    [Serializable]
    internal class ImageProprecessingTool : ToolBase
    {
        internal Result_ImageProcessing result = new Result_ImageProcessing();
        internal List<ItemType> L_item = new List<ItemType>();
        internal ItemType FindItemByName(string itemName)
        {
            for (int i = 0; i < L_item.Count; i++)
            {
                if (L_item[i].itemName == itemName)
                    return L_item[i];
            }
            return null;
        }
        /// <summary>
        /// 工具锁
        /// </summary>
        private object obj = new object();
        /// <summary>
        /// 输入图像
        /// </summary>
        internal HObject inputImage;
        /// <summary>
        /// 输出图像
        /// </summary>
        internal HObject outputImage;

        /// <summary>
        /// 运行工具
        /// </summary>
        /// <param name="updateImage">是否刷新图像</param>
        public override void Run(bool updateImage, bool runTool, string toolName)
        {
            try
            {
                lock (obj)
                {
                    toolRunStatu = Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Succeed : ToolRunStatu.未知原因;

                    outputImage = inputImage;
                    for (int i = 0; i < L_item.Count; i++)
                    {
                        switch (L_item[i].type)
                        {
                            case "二值化":
                                Binary binary = (Binary)L_item[i].item;
                                HObject temp;
                                HOperatorSet.Threshold(outputImage, out temp, binary.lowThreshold, binary.highThreshold);
                                HObject binImage;
                                HTuple w, h;
                                HOperatorSet.GetImageSize(outputImage, out w, out h);
                                HOperatorSet.RegionToBin(temp, out outputImage, 0, 255, w, h);
                                break;
                            case "填充":
                                HOperatorSet.FillUp(outputImage, out outputImage);
                                break;
                        }
                    }
                    Frm_ImageProprecessingTool.Instance.hWindow_Final1.ClearWindow();
                    Frm_ImageProprecessingTool.Instance.hWindow_Final1.HobjectToHimage(outputImage);
                    result.输出图像 = outputImage;
                    toolRunStatu = Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

    }

    [Serializable]
    internal class ItemType
    {
        internal ItemType(string type, string itemName, itemBase item)
        {
            this.type = type;
            this.itemName = itemName;
            this.item = item;
        }
        internal string type;
        internal string itemName;
        internal itemBase item;
        internal bool enable = true;
    }
    [Serializable]
    internal class itemBase
    {

    }
    [Serializable]
    public class Result_ImageProcessing
    {
        private HObject _输出图像;

        public HObject 输出图像
        {
            get { return _输出图像; }
            set { _输出图像 = value; }
        }
    }
}
