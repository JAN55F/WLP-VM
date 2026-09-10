using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VMPro
{
    internal partial class Frm_ProcessingItem : Form
    {
        internal Frm_ProcessingItem()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体实例对象
        /// </summary>
        private static Frm_ProcessingItem _instance;
        internal static Frm_ProcessingItem Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Frm_ProcessingItem();
                return _instance;
            }
        }
        /// <summary>
        /// 工具对象
        /// </summary>
        internal static BlobAnalyseTool blobAnalyseTool = new BlobAnalyseTool();


        private void Frm_ProcessingItemConfig_Load(object sender, EventArgs e)
        {
            try
            {
                this.TopMost = true;
                tbx_minArea.Text = blobAnalyseTool.L_prePorcessing[Frm_BlobAnalyseTool.Instance.dgv_processingItem.SelectedRows[0].Index].MinArea.ToString();
                tbx_maxArea.Text = blobAnalyseTool.L_prePorcessing[Frm_BlobAnalyseTool.Instance.dgv_processingItem.SelectedRows[0].Index].MaxArea.ToString();
                tbx_elementSize.Text = blobAnalyseTool.L_prePorcessing[Frm_BlobAnalyseTool.Instance.dgv_processingItem.SelectedRows[0].Index].ElementSize.ToString();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void btn_applyAndExit_Click(object sender, EventArgs e)
        {
            try
            {
                blobAnalyseTool.L_prePorcessing[Frm_BlobAnalyseTool.Instance.dgv_processingItem.SelectedRows[0].Index].MinArea = Convert.ToInt32(tbx_minArea.Text.Trim());
                blobAnalyseTool.L_prePorcessing[Frm_BlobAnalyseTool.Instance.dgv_processingItem.SelectedRows[0].Index].MaxArea = Convert.ToInt32(tbx_maxArea.Text.Trim());
                blobAnalyseTool.L_prePorcessing[Frm_BlobAnalyseTool.Instance.dgv_processingItem.SelectedRows[0].Index].ElementSize = Convert.ToInt32(tbx_elementSize.Text.Trim());
                this.Close();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

    }
}
