using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using VMPro.Properties;

namespace VMPro
{
    /// <summary>
    /// 板卡基类(此类及所有类成员应为非静态，因为一个项目中的板卡数量可能不止一个，所以应为实例类，待后期修改完善)
    /// </summary>
    class CardBase
    {

        /// <summary>
        /// 板卡资源锁
        /// </summary>
        internal static object obj_card = new object();
        /// <summary>
        /// 指示此板卡是否初始化成功
        /// </summary>
        internal static bool initSucceed = false;
        /// <summary>
        /// 正在回零
        /// </summary>
        internal static bool homing = false;
        /// <summary>
        /// 轴集合
        /// </summary>
        internal static List<S_Axis> L_axis = new List<S_Axis>();
        /// <summary>
        /// 输出点集合
        /// </summary>
        protected static List<S_Do> L_do = new List<S_Do>();
        /// <summary>
        /// 输入点集合
        /// </summary>
        protected static List<S_Di> L_di = new List<S_Di>();
        /// <summary>
        /// 表示输入信号的虚拟情况，键表示要虚拟的输入信号，值的第一位为0时表示不虚拟，为1时表示虚拟。值的第二位为0时表示虚拟为高电平，为1时表示虚拟为低电平，为2时表示虚拟为置反
        /// </summary>
        internal static Dictionary<string, string> D_inputSignalVitualStatu = new Dictionary<string, string>();
        /// <summary>
        /// 输出信号的虚拟状态，只在卡为虚拟卡时有意义
        /// </summary>
        internal static Dictionary<string, Level> D_outputSingalVitualStatu = new Dictionary<string, Level>();


        /// <summary>
        /// 绑定轴
        /// </summary>
        /// <param name="actNo">轴映射号</param>
        /// <param name="axisName">轴枚举</param>
        internal static void BindAxis(ushort actNo, object axisName)
        {
            try
            {
                S_Axis axis = new S_Axis();
                axis.actNo = actNo;
                axis.axisName = axisName.ToString();
                axis.homeOK = false  ;
                L_axis.Add(axis);
                //if (!Frm_MotionControl.Instance.cbx_axisName.Items.Contains(axisName.ToString()))
                //    Frm_MotionControl.Instance.cbx_axisName.Items.Add(axisName.ToString());
                //if (Frm_MotionControl.Instance.cbx_axisName.Items.Count > 0)
                //    Frm_MotionControl.Instance.cbx_axisName.SelectedIndex = 0;

                //////if (!Frm_PosTableEdit.Instance.comboBox2.Items.Contains(axisName.ToString()))
                //////    Frm_PosTableEdit.Instance.comboBox2.Items.Add(axisName.ToString());
                if (Frm_PosTableEdit.Instance.comboBox2.Items.Count > 0)
                    Frm_PosTableEdit.Instance.comboBox2.SelectedIndex = 0;

                int row = Frm_MotionControl.Instance.dgv_axisInfo.Rows.Add();
                Frm_MotionControl.Instance.dgv_axisInfo.Rows[row].Cells[0].Value = (Frm_MotionControl.Instance.dgv_axisInfo.Rows.Count - 1).ToString();
                Frm_MotionControl.Instance.dgv_axisInfo.Rows[row].Cells[1].Value = axisName.ToString();
                Frm_MotionControl.Instance.dgv_axisInfo.Rows[row].Cells[5].Value = "已开启";
                Frm_MotionControl.Instance.dgv_axisInfo.Rows[row].Cells[5].Style.BackColor = Color.Green;
                Frm_MotionControl.Instance.dgv_axisInfo.Rows[row].Cells[4].Value = "回零";
                Frm_MotionControl.Instance.dgv_axisInfo.Rows[row].Cells[2].Value = "Jog负";
                Frm_MotionControl.Instance.dgv_axisInfo.Rows[row].Cells[3].Value = "Jog正";
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 绑定通用输入
        /// </summary>
        /// <param name="indexNo">IO编号</param>
        /// <param name="actNo">映射号</param>
        /// <param name="diName">输入枚举</param>
        internal static void BindDi(int indexNo, string actNo, object diName)
        {
            try
            {
                S_Di di = new S_Di();
                di.indexNo = indexNo;
                di.actNo = actNo;
                di.diName = diName.ToString();
                L_di.Add(di);
                int index = Frm_MotionControl.Instance.dgv_diList.Rows.Add();
                Frm_MotionControl.Instance.dgv_diList.Rows[index].Cells[0].Value = indexNo;
                Frm_MotionControl.Instance.dgv_diList.Rows[index].Cells[1].Value = actNo;
                Frm_MotionControl.Instance.dgv_diList.Rows[index].Cells[2].Value = Resources.Off;
                Frm_MotionControl.Instance.dgv_diList.Rows[index].Cells[2].Tag = "Off";
                Frm_MotionControl.Instance.dgv_diList.Rows[index].Cells[3].Value = diName.ToString();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 绑定通用输出
        /// </summary>
        /// <param name="indexNo">IO编号</param>
        /// <param name="actNo">映射号</param>
        /// <param name="doName">输出枚举</param>
        internal static void BindDo(int indexNo, string actNo, object doName)
        {
            try
            {
                S_Do do1 = new S_Do();
                do1.indexNo = indexNo;
                do1.actNo = actNo;
                do1.doName = doName.ToString();
                L_do.Add(do1);
                int index = Frm_MotionControl.Instance.dgv_doList.Rows.Add();
                Frm_MotionControl.Instance.dgv_doList.Rows[index].Cells[0].Value = indexNo;
                Frm_MotionControl.Instance.dgv_doList.Rows[index].Cells[1].Value = actNo;
                Frm_MotionControl.Instance.dgv_doList.Rows[index].Cells[3].Value = Resources.Off;
                Frm_MotionControl.Instance.dgv_doList.Rows[index].Cells[3].Tag = "Off";
                Frm_MotionControl.Instance.dgv_doList.Rows[index].Cells[4].Value = doName.ToString();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        /// <summary>
        /// 通过轴名称获取轴映射号
        /// </summary>
        /// <param name="axisName">轴枚举</param>
        /// <returns>轴映射号</returns>
        internal static S_Axis FindAxisByName(object axisName)
        {
            try
            {
                for (int i = 0; i < L_axis.Count; i++)
                {
                    if (L_axis[i].axisName == axisName.ToString())
                        return L_axis[i];
                }
                return new S_Axis ();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return new S_Axis ();
            }
        }
        /// <summary>
        /// 获取位状态(16位版)
        /// </summary>
        /// <param name="vaule">对象值</param>
        /// <param name="position">第几位</param>
        /// <returns></returns>
        public static bool GetBit16(UInt16 vaule, byte position)
        {
            try
            {
                uint pos = 1;
                pos = pos << position;
                return (vaule & (0xffffffff & pos)) > 1;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return false;
            }
        }
        /// <summary>
        /// 通过输入点枚举获取输入点映射号
        /// </summary>
        /// <param name="diName">输入枚举</param>
        /// <returns>输入映射号</returns>
        protected static string GetDiIndexByName(object diName)
        {
            try
            {
                for (int i = 0; i < L_di.Count; i++)
                {
                    if (L_di[i].diName == diName.ToString())
                        return L_di[i].actNo;
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
        /// 通过输出点枚举名获取输出点映射号
        /// </summary>
        /// <param name="doName">输出枚举</param>
        /// <returns>输出映射号</returns>
        protected static string GetDoIndexByName(object doName)
        {
            try
            {
                for (int i = 0; i < L_di.Count; i++)
                {
                    if (L_do[i].doName == doName.ToString())
                        return L_do[i].actNo;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
                return string.Empty;
            }
        }

    }
    internal class S_Axis
    {
        internal int indexNo;
        internal ushort actNo;
        internal string axisName;
        internal bool homeOK;
    }
    internal struct S_Di
    {
        internal int indexNo;
        internal string actNo;
        internal string diName;
    }
    internal struct S_Do
    {
        internal int indexNo;
        internal string actNo;
        internal string doName;
    }
    internal enum Level
    {
        High,
        Low,
    }
}
