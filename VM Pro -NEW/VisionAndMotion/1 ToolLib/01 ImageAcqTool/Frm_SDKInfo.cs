using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VMPro
{
    internal partial class Frm_SDKInfo : Frm_FormBase
    {
        internal Frm_SDKInfo()
        {
            InitializeComponent();
            load();
        }

        /// <summary>
        /// 相机品牌列表
        /// </summary>
        static string[] CameraType = new string[8] { "巴斯勒相机", "海康威视相机", "迈德威视相机", "大恒相机", "大华相机", "灰点相机", "映美精相机", "SVS相机" };
        /// <summary>
        /// 各品牌相机SDK版本列表
        /// </summary>
        static string[] SDKVersion = new string[8] { "5.2.0.13457 64-Bit", "V2.4.0 build20180326(SDK:V2.4.0.3)", "", "", "", "", "", "" };
        /// <summary>
        /// 各品牌相机SDK加载状态
        /// </summary>
        internal static bool[] LoadState = new bool[8] { false, false, false, false, false, false, false, false  };

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Frm_SDKInfo _instance;
        internal static Frm_SDKInfo Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_SDKInfo();
                return _instance;
            }
        }


        /// <summary>
        /// 加载SDK信息
        /// </summary>
        private void load()
        {
            try
            {
                dgv_sdkInfo.Rows.Add(8);
                for (int i = 0; i < dgv_sdkInfo.Rows.Count; i++)
                {
                    dgv_sdkInfo.Rows[i].Cells[0].Value = i + 1;
                    dgv_sdkInfo.Rows[i].Cells[1].Value = CameraType[i];
                    dgv_sdkInfo.Rows[i].Cells[2].Value = SDKVersion[i];
                    dgv_sdkInfo.Rows[i].Cells[3].Value = (LoadState[i] ? " 成功    √" : " 失败    ×");
                    if (i % 2 != 0)
                        dgv_sdkInfo.Rows[i].DefaultCellStyle.BackColor = Color.LightGray;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }


        private void Frm_SDKInfo_Shown(object sender, EventArgs e)
        {
            dgv_sdkInfo.Rows[0].Cells[0].Selected = false;
        }

    }
}
