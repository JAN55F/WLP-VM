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
    internal class RegionFeatureTool : ToolBase
    {
        internal RegionFeatureTool()
        {
            HOperatorSet.GenEmptyObj(out inputRegion);
            HOperatorSet.GenEmptyObj(out outRectangle2);
        }
        /// <summary>
        /// 流程名
        /// </summary>
        internal string jobName = string.Empty;
        internal HObject inputRegion = new HObject();
        private HTuple _roundness = 1;
        internal HTuple Roundness
        {
            get
            {
                _roundness = Math.Round((double)_roundness, 3);
                return _roundness;
            }
            set
            {
                _roundness = Math.Round((double)value, 3);
                _roundness = value;
            }
        }
        /// <summary>
        /// 区域中心点
        /// </summary>
        private XY _centerPoint = new XY();
        internal XY CenterPoint
        {
            get
            {
                _centerPoint.X = Math.Round(_centerPoint.X, 3);
                _centerPoint.Y = Math.Round(_centerPoint.Y, 3);
                return _centerPoint;
            }
            set { _centerPoint = value; }
        }
        /// <summary>
        /// 外接仿射矩形
        /// </summary>
        internal HObject outRectangle2 = new HObject();
        /// <summary>
        /// 输入的第一条线段
        /// </summary>
        internal Line line1 = new Line();
        /// <summary>
        /// 输入的第二条线段
        /// </summary>
        internal Line line2 = new Line();
        internal int leftTopRow;
        internal int leftTopCol;
        internal int rightDownRow;
        internal int rightDownCol;
        internal bool LeftTopRowUseConst = true;
        internal bool LeftTopColUseConst = true;
        internal bool RightDownRowUseConst = true;
        internal bool RightDownColUseConst = true;
        internal int leftTopRowConstValue;
        internal int leftTopColConstValue;
        internal int rightDownRowConstValue;
        internal int rightDownColConstValue;
        /// <summary>
        /// 计算出来的线与线之间的距离值
        /// </summary>
        private XY _resultDistance;
        internal XY ResultDistance
        {
            get
            {
                if (_resultDistance == null)
                    _resultDistance = new XY();

                _resultDistance.X  = Math.Round(_resultDistance.X , 3);
                _resultDistance.Y  = Math.Round(_resultDistance.Y , 3);



                return _resultDistance;
            }
            set { _resultDistance = value; }
        }


        /// <summary>
        /// 工具恢复到初始状态
        /// </summary>
        internal void ResetTool()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 清空上次运行的所有输入
        /// </summary>
        internal void ClearLastInput()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
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
                    HOperatorSet.RegionFeatures(inputRegion, new HTuple("roundness"), out _roundness);
                    HTuple row, col;
                    HOperatorSet.RegionFeatures(inputRegion, new HTuple("row"), out row);
                    HOperatorSet.RegionFeatures(inputRegion, new HTuple("column"), out col);
                    HObject cross;
                    HOperatorSet.GenCrossContourXld(out cross, row, col, new HTuple(20), new HTuple(0));
                    GetImageWindowControl().hwc_imageWindow.DispObj(cross);
                    CenterPoint = new XY(row.ToDArr()[0], col.ToDArr()[0]);
                    HTuple row1, col1, angle, length1, length2;
                    HOperatorSet.SmallestRectangle2(inputRegion, out row1, out col1, out angle, out length1, out length2);
                    HOperatorSet.GenRectangle2(out outRectangle2, row1, col1, angle, length1, length2);
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
