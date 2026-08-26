using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HalconDotNet;
using System.Windows.Forms;
using System.Diagnostics;
using System.Data;
using System.Text.RegularExpressions;
using System.Drawing;

namespace VMPro
{
    /// <summary>
    /// 二维平面手眼标定工具。
    ///
    /// 本工具并非六轴机器人常见的 3D Hand-Eye（AX=XB）标定，而是建立图像平面
    /// 像素坐标与设备机械平面坐标之间的二维仿射关系：
    ///     (pixelRow, pixelColumn) -> (mechanicalX, mechanicalY)
    ///
    /// 标定阶段由多组同名点调用 HALCON VectorToHomMat2d() 求出 homMat2D；运行阶段
    /// 使用同一矩阵转换图像、XY 点或 XYU 位姿。项目历史结构中 XY.X 通常表示 Row，
    /// XY.Y 通常表示 Column，不能仅根据字段名把它理解成普通笛卡尔 X/Y。
    /// </summary>
    [Serializable]
    internal class EyeHandCalibTool : ToolBase
    {
        /// <summary>
        /// 初始化第 0 个拍照位和一个空的位姿输出，保证新建工具在尚未标定时结构完整。
        /// 第 0 个拍照位是后续多拍照位补偿的基准。
        /// </summary>
        internal EyeHandCalibTool()
        {
            XY photoPos = new XY();
            photoPos.X = 0;
            photoPos.Y = 0;
            D_photoPos.Add(0, photoPos);

            XYU xyu = new XYU();
            toolPar.ResultPar.位置.Add(xyu);
        }
        internal ToolPar toolPar = new ToolPar();
        /// <summary>是否启用同一次标定对应多个相机拍照位置。</summary>
        internal bool multPhoto = false;
        /// <summary>
        /// 标定用的流程名称
        /// </summary>
        internal string calibJobName = string.Empty;
        /// <summary>供界面显示和工程保存使用的标定状态描述。</summary>
        internal string calibResult = "未标定";
        /// <summary>
        /// 标定用的输出项的名称
        /// </summary>
        internal string calibItemName = string.Empty;
        /// <summary>是否同时执行旋转中心标定。</summary>
        internal bool calibRotateCenter = false;
        /// <summary>机械旋转轴 U 的正方向说明，用于统一现场旋转方向约定。</summary>
        internal string dirctionOfU = "俯视时，当U增加时旋转轴顺时针旋转";
        /// <summary>
        /// 当前拍照位编号
        /// </summary>
        internal int curPhotoPosIndex = 0;
        /// <summary>
        /// 工具锁
        /// </summary>
        private object obj = new object();
        /// <summary>标定检查或最终输出使用的人工补偿量。</summary>
        internal XY calibOffset = new XY();

        /// <summary>
        /// 多拍照位机械坐标表。键为拍照位编号，值为相对同一机械坐标系的 X/Y 位置。
        /// </summary>
        internal Dictionary<int, XY> D_photoPos = new Dictionary<int, XY>();
        /// <summary>
        /// 标定旋转中心时的机械位置
        /// </summary>
        internal XY rotateCenter = new XY();
        /// <summary>用于标定精度检查的参考机械点。</summary>
        internal XY checkPoint = new XY();

        /// <summary>
        /// 像素平面到机械平面的二维仿射变换矩阵。
        /// 注意这里是“仿射”，不是注释旧称中的“放射”；矩阵可包含平移、旋转、
        /// X/Y 不同比例缩放和斜切。
        /// </summary>
        internal HTuple homMat2D = (HTuple)(new HHomMat2D());
        /// <summary>
        /// 主标定数据的持久化副本。界面表格每行通常表示组号、像素X、像素Y、机械X、机械Y。
        /// </summary>
        internal List<List<double>> L_calibData = new List<List<double>>();

        /// <summary>
        /// 旋转中心标定数据的持久化副本。
        /// </summary>
        internal List<List<double>> L_calibRotateCenterData = new List<List<double>>();
        /// <summary>标定检查数据的持久化副本。</summary>
        internal List<List<double>> L_calibCheckData = new List<List<double>>();
        /// <summary>
        /// 标定类型 四点标定|九点标定
        /// </summary>
        internal CalibType calibrationType = CalibType.Four_Point;
        /// <summary>
        /// 相机安装类型 眼在手外|眼在手上
        /// </summary>
        internal FixedType fixedType = FixedType.OutsideHand;
        /// <summary>
        /// 仿射矩阵分解得到的第一个输出轴平移量，仅用于诊断和界面展示。
        /// </summary>
        private HTuple _translateX = 0;
        internal HTuple TranslateX
        {
            get
            {
                _translateX = Math.Round((double)_translateX, 5);
                return _translateX;
            }
            set
            {
                value = Math.Round((double)value, 5);
                _translateX = value;
            }
        }
        /// <summary>
        /// 仿射矩阵分解得到的第二个输出轴平移量，仅用于诊断和界面展示。
        /// </summary>
        private HTuple _translateY = 0;
        internal HTuple TranslateY
        {
            get
            {
                _translateY = Math.Round((double)_translateY, 5);
                return _translateY;
            }
            set
            {
                value = Math.Round((double)value, 5);
                _translateY = value;
            }
        }
        /// <summary>
        /// 仿射矩阵分解得到的第一轴缩放。与 1 偏差较大时应检查点位或坐标顺序。
        /// </summary>
        private HTuple _scanX = 1;
        internal HTuple ScanX
        {
            get
            {
                _scanX = Math.Round((double)_scanX, 5);
                return _scanX;
            }
            set
            {
                value = Math.Round((double)value, 5);
                _scanX = value;
            }
        }
        /// <summary>
        /// 仿射矩阵分解得到的第二轴缩放。与 ScanX 差异较大意味着存在非等比映射。
        /// </summary>
        private HTuple _scanY = 1;
        internal HTuple ScanY
        {
            get
            {
                _scanY = Math.Round((double)_scanY, 5);
                return _scanY;
            }
            set
            {
                value = Math.Round((double)value, 5);
                _scanY = value;
            }
        }
        /// <summary>
        /// 仿射矩阵分解得到的整体旋转角，HALCON 中使用弧度。
        /// </summary>
        private HTuple _rotation = 0;
        internal HTuple Rotation
        {
            get
            {
                _rotation = Math.Round((double)_rotation, 5);
                return _rotation;
            }
            set
            {
                value = Math.Round((double)value, 5);
                _rotation = value;
            }
        }
        /// <summary>
        /// 仿射矩阵分解得到的轴夹角/斜切参数，可用于判断标定数据是否引入明显错切。
        /// </summary>
        private HTuple _theta = 0;
        internal HTuple Theta
        {
            get
            {
                _theta = Math.Round((double)_theta, 5);
                return _theta;
            }
            set
            {
                value = Math.Round((double)value, 5);
                _theta = value;
            }
        }


        internal void ClearLastInput()
        {
            try
            {
                // 流程每轮重新解析输入连接前清空旧引用，避免断开连接后继续使用上一轮数据。
                toolPar.InputPar.点 = null;
                toolPar.InputPar.图像 = null;
                toolPar.InputPar.位置 = null;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 复位界面显示的仿射分解参数和标定数据表。
        /// 此方法恢复单位缩放、零旋转、零平移的显示状态，不执行重新标定。
        /// </summary>
        internal void ResetTool()
        {
            try
            {
                TranslateX = 0;
                TranslateY = 0;
                ScanX = 1;
                ScanY = 1;
                Rotation = 0;
                Theta = 0;
                Frm_EyeHandCalibTool.Instance.dgv_calibrateData.Rows.Clear();
                Frm_EyeHandCalibTool.Instance.tbx_translateX.Text = "0";
                Frm_EyeHandCalibTool.Instance.tbx_translateY.Text = "0";
                Frm_EyeHandCalibTool.Instance.tbx_scaleX.Text = "1";
                Frm_EyeHandCalibTool.Instance.tbx_scaleY.Text = "1";
                Frm_EyeHandCalibTool.Instance.tbx_rotation.Text = "0";
                Frm_EyeHandCalibTool.Instance.tbx_theta.Text = "0";
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 从界面采集的像素/机械对应点计算二维仿射矩阵，并保存标定数据。
        ///
        /// 眼在手外：相机固定、工件或机械末端移动，直接建立像素点到机械点的对应关系。
        /// 眼在手上：相机随轴移动；相机向一个方向运动，等价于固定相机下工件向反方向运动，
        /// 因而以第一组机械位置为基准对机械位移取反。
        /// </summary>
        internal void Calibrate()
        {
            try
            {
                // 四个等长数组按索引构成同名点：图像 Row/Column 对应机械 X/Y。
                List<double> L_pixelRow = new List<double>();
                List<double> L_pixelCol = new List<double>();
                List<double> L_MechanicalX = new List<double>();
                List<double> L_MechanicalY = new List<double>();
                try
                {
                    if (fixedType == FixedType.OutsideHand)                //眼在手外
                    {
                        // 固定相机模式直接读取对应点。维护表格列时必须同时核对 Designer 中的列顺序，
                        // 特别注意界面可能包含“组号”列，不能把组号误当 pixelRow。
                        for (int i = 0; i < Frm_EyeHandCalibTool.Instance.dgv_calibrateData.Rows.Count; i++)
                        {
                            L_pixelRow.Add(Convert.ToDouble(Frm_EyeHandCalibTool.Instance.dgv_calibrateData.Rows[i].Cells[0].Value));
                            L_pixelCol.Add(Convert.ToDouble(Frm_EyeHandCalibTool.Instance.dgv_calibrateData.Rows[i].Cells[1].Value));
                            L_MechanicalX.Add(Convert.ToDouble(Frm_EyeHandCalibTool.Instance.dgv_calibrateData.Rows[i].Cells[2].Value));
                            L_MechanicalY.Add(Convert.ToDouble(Frm_EyeHandCalibTool.Instance.dgv_calibrateData.Rows[i].Cells[3].Value));
                        }
                    }
                    else                   //眼在手上
                    {
                        // 当相机安装在运动轴上时，相机向左移动 d，等价于固定相机时工件向右移动 d。
                        // 因此以第一组机械位置为原点，将各组相对位移反向后再参与标定。
                        double baseX = Convert.ToDouble(Frm_EyeHandCalibTool.Instance.dgv_calibrateData.Rows[0].Cells[3].Value);
                        double baseY = Convert.ToDouble(Frm_EyeHandCalibTool.Instance.dgv_calibrateData.Rows[0].Cells[4].Value);
                        for (int i = 0; i < Frm_EyeHandCalibTool.Instance.dgv_calibrateData.Rows.Count - 1; i++)
                        {
                            L_pixelRow.Add(Convert.ToDouble(Frm_EyeHandCalibTool.Instance.dgv_calibrateData.Rows[i].Cells[1].Value));
                            L_pixelCol.Add(Convert.ToDouble(Frm_EyeHandCalibTool.Instance.dgv_calibrateData.Rows[i].Cells[2].Value));
                            L_MechanicalX.Add(baseX - (Convert.ToDouble(Frm_EyeHandCalibTool.Instance.dgv_calibrateData.Rows[i].Cells[3].Value) - baseX));
                            L_MechanicalY.Add(baseY - (Convert.ToDouble(Frm_EyeHandCalibTool.Instance.dgv_calibrateData.Rows[i].Cells[4].Value) - baseY));
                        }
                    }
                }
                catch
                {
                    Frm_Main.Instance.OutputMsg("标定失败，标定数据不合法（错误代码：10301）", Color.Red);
                    return;
                }

                try
                {
                    // VectorToHomMat2d 使用最小二乘方式求解一般二维仿射矩阵。
                    // 输入必须有足够且不共线的对应点，否则无法唯一确定变换关系。
                    HTuple pixelRow = new HTuple(L_pixelRow.ToArray());
                    HTuple pixelCol = new HTuple(L_pixelCol.ToArray());
                    HTuple mechanicalX = new HTuple(L_MechanicalX.ToArray());
                    HTuple mechanicalY = new HTuple(L_MechanicalY.ToArray());
                    HOperatorSet.VectorToHomMat2d(pixelRow, pixelCol, mechanicalX, mechanicalY, out homMat2D);
                }
                catch
                {
                    Frm_Main.Instance.OutputMsg("标定失败，当前标定数据无法确定仿射变换关系（错误代码：10302）", Color.Red);
                }

                // 将矩阵分解成便于现场诊断的缩放、旋转、斜切和平移参数；
                // 实际运行仍以 homMat2D 为准，并不是用这些显示值重新拼装矩阵。
                HOperatorSet.HomMat2dToAffinePar(homMat2D, out _scanX, out _scanY, out _rotation, out _theta, out _translateX, out _translateY);
                Frm_EyeHandCalibTool.Instance.tbx_translateX.Text = (Convert.ToDouble(TranslateX.ToString())).ToString("0.00000");
                Frm_EyeHandCalibTool.Instance.tbx_translateY.Text = (Convert.ToDouble(TranslateY.ToString())).ToString("0.00000");
                Frm_EyeHandCalibTool.Instance.tbx_scaleX.Text = (Convert.ToDouble(ScanX.ToString())).ToString("0.00000");
                Frm_EyeHandCalibTool.Instance.tbx_scaleY.Text = (Convert.ToDouble(ScanY.ToString())).ToString("0.00000");
                Frm_EyeHandCalibTool.Instance.tbx_rotation.Text = (Convert.ToDouble(Rotation.ToString())).ToString("0.00000");
                Frm_EyeHandCalibTool.Instance.tbx_theta.Text = (Convert.ToDouble(Theta.ToString())).ToString("0.00000");

                // 保存主标定表。Rows.Count-1 用于排除 DataGridView 自动追加的空白新行。
                L_calibData.Clear();
                for (int i = 0; i < Frm_EyeHandCalibTool.Instance.dgv_calibrateData.Rows.Count - 1; i++)
                {
                    List<double> list = new List<double>();
                    for (int j = 0; j < 5; j++)
                    {
                        list.Add(Math.Round(Convert.ToDouble(Frm_EyeHandCalibTool.Instance.dgv_calibrateData.Rows[i].Cells[j].Value), 3));
                    }
                    L_calibData.Add(list);
                }

                // 保存旋转中心采样表，同样排除最后的空白新行。
                L_calibRotateCenterData.Clear();
                for (int i = 0; i < Frm_EyeHandCalibTool.Instance.dataGridView2.Rows.Count - 1; i++)
                {
                    List<double> list = new List<double>();
                    for (int j = 0; j < 3; j++)
                    {
                        list.Add(Math.Round(Convert.ToDouble(Frm_EyeHandCalibTool.Instance.dataGridView2.Rows[i].Cells[j].Value), 3));
                    }
                    L_calibRotateCenterData.Add(list);
                }

                // 保存最终计算/人工确认的旋转中心机械坐标。
                rotateCenter.X = Convert.ToDouble(Frm_EyeHandCalibTool.Instance.textBox6.Value);
                rotateCenter.Y = Convert.ToDouble(Frm_EyeHandCalibTool.Instance.textBox5.Value);

                Frm_Output.Instance.OutputMsg("标定成功", Color.Green);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 使用已标定的 homMat2D 转换本轮输入。
        /// </summary>
        /// <param name="updateImage">为 true 时将变换后的图像刷新到当前流程图像窗口。</param>
        /// <param name="temp">流程框架兼容参数，本实现当前未使用。</param>
        /// <param name="toolName">流程框架传入的当前工具名称。</param>
        public override void Run(bool updateImage, bool temp, string toolName)
        {
            try
            {
                lock (obj)
                {
                    toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Succeed : ToolRunStatu.未知原因);

                    // 图像变换使用同一“像素 -> 机械”矩阵。由于机械单位通常与像素单位不同，
                    // 输出图像可能发生缩放/旋转/错切；下游若只需要机械坐标，应优先连接“点/位置”输出。
                    // nearest_neighbor 保留离散灰度且速度较快，false 表示不自动扩展输出图像尺寸。
                    if (toolPar.InputPar.图像 != null)
                    {
                        HObject temp111;
                        HOperatorSet.AffineTransImage(toolPar.InputPar.图像, out temp111, homMat2D, (HTuple)"nearest_neighbor", (HTuple)"false");
                        toolPar.ResultPar.图像 = temp111;
                        if (updateImage)
                            GetImageWindowControl().hwc_imageWindow.HobjectToHimage(toolPar.ResultPar.图像);
                    }

                    // 转换带角度的位置列表。Point.X/Y 按 Row/Column 输入，变换后写成机械 X/Y。
                    // 当前代码只变换位置点，U 原样透传；仿射矩阵中的旋转量不会自动叠加到 U。
                    if (toolPar.InputPar.位置 != null)
                    {
                        toolPar.ResultPar.位置.Clear();
                        for (int i = 0; i < toolPar.InputPar.位置.Count; i++)
                        {
                            HTuple rowAfterTrans;
                            HTuple colAfterTrans;
                            HOperatorSet.AffineTransPoint2d(homMat2D, (HTuple)toolPar.InputPar.位置[i].Point.X, (HTuple)toolPar.InputPar.位置[i].Point.Y, out rowAfterTrans, out colAfterTrans);

                            XYU xyu = new XYU();
                            xyu.Point.X = rowAfterTrans;
                            xyu.Point.Y = colAfterTrans;
                            xyu.U = toolPar.InputPar.位置[i].U;

                            toolPar.ResultPar.位置.Add(xyu);
                        }



                    }

                    // 转换普通点列表。这里同样是“仿射变换”，不是放射/投影变换。
                    if (toolPar.InputPar.点 != null)
                    {
                        toolPar.ResultPar.点.Clear();
                        for (int i = 0; i < toolPar.InputPar.点.Count; i++)
                        {
                            HTuple rowAfterTrans;
                            HTuple colAfterTrans;
                            HOperatorSet.AffineTransPoint2d(homMat2D, (HTuple)toolPar.InputPar.点[i].X, (HTuple)toolPar.InputPar.点[i].Y, out rowAfterTrans, out colAfterTrans);


                            // 一套标定可能服务多个拍照位：先得到第 0 拍照位下的机械坐标，
                            // 再加上当前拍照位相对第 0 位的机械平移差。此补偿只处理平移，不处理拍照位旋转。
                            double spanX = D_photoPos[curPhotoPosIndex].X - D_photoPos[0].X;
                            double spanY = D_photoPos[curPhotoPosIndex].Y - D_photoPos[0].Y;
                            rowAfterTrans = rowAfterTrans + spanX;
                            colAfterTrans = colAfterTrans + spanY;

                            XY temp1 = new XY();
                            temp1.X = rowAfterTrans;
                            temp1.Y = colAfterTrans;

                            toolPar.ResultPar.点.Add(temp1);
                        }

                    }
                    toolRunStatu = (Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功);
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }


        [Serializable]
        public class ToolPar : ToolParBase
        {
            private InputPar _inputPar = new InputPar();

            public InputPar InputPar
            {
                get { return _inputPar; }
                set { _inputPar = value; }
            }
            private RunPar _runPar = new RunPar();

            public RunPar RunPar
            {
                get { return _runPar; }
                set { _runPar = value; }
            }
            private ResultPar _resultPar = new ResultPar();

            public ResultPar ResultPar
            {
                get { return _resultPar; }
                set { _resultPar = value; }
            }
        }
        [Serializable]
        public class InputPar
        {
            /// <summary>
            /// 可选输入图像。若连接此项，运行时会把整幅图像按像素到机械仿射矩阵重采样。
            /// </summary>
            private HObject _图像;

            public HObject 图像
            {
                get { return _图像; }
                set { _图像 = value; }
            }

            /// <summary>待转换普通点列表；X/Y 按项目历史约定表示 Row/Column。</summary>
            private List<XY> _点 = new List<XY>();

            public List<XY> 点
            {
                get { return _点; }
                set { _点 = value; }
            }

            /// <summary>待转换位置列表；仅 Point 被仿射转换，U 当前保持原值。</summary>
            private List<XYU> _位置 = new List<XYU>();

            public List<XYU> 位置
            {
                get { return _位置; }
                set { _位置 = value; }
            }
        }
        [Serializable]
        public class RunPar
        {

        }
        [Serializable]
        internal class ResultPar
        {
            /// <summary>仿射重采样后的图像；仅在输入图像非空时更新。</summary>
            private HObject _图像;
            public HObject 图像
            {
                get { return _图像; }
                set { _图像 = value; }
            }

            /// <summary>转换到机械坐标系并叠加多拍照位平移补偿后的普通点。</summary>
            private List<XY> _点 = new List<XY>();

            public List<XY> 点
            {
                get { return _点; }
                set { _点 = value; }
            }

            /// <summary>Point 已转换到机械坐标系、U 保持输入值的位置列表。</summary>
            private List<XYU> _位置 = new List<XYU>();

            public List<XYU> 位置
            {
                get { return _位置; }
                set { _位置 = value; }
            }
        }



    }
}
