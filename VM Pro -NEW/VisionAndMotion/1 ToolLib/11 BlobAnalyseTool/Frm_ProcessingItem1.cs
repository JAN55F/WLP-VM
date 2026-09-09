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
    public partial class Frm_ProcessingItem1 : Form
    {
        public Frm_ProcessingItem1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体实例对象
        /// </summary>
        private static Frm_ProcessingItem1 _instance;
        public static Frm_ProcessingItem1 Instance
        {
            get
            {
                _instance = new Frm_ProcessingItem1();
                return _instance;
            }
        }
        /// <summary>
        /// 工具对象
        /// </summary>
        internal static BlobAnalyseTool blobAnalyseTool = new BlobAnalyseTool();


        private void Frm_ProcessingItemConfig1_Load(object sender, EventArgs e)
        {
            try
            {
                this.TopMost = true;
                cbx_elementType.SelectedIndex = 0;
                cbx_elementType.Text = blobAnalyseTool.L_prePorcessing[Frm_BlobAnalyseTool.Instance.dgv_processingItem.SelectedRows[0].Index].ElementType;
                tbx_elementSize.Text = blobAnalyseTool.L_prePorcessing[Frm_BlobAnalyseTool.Instance.dgv_processingItem.SelectedRows[0].Index].ElementSize.ToString();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }
        private void btn_saveAndExit_Click(object sender, EventArgs e)
        {
            try
            {
                blobAnalyseTool.L_prePorcessing[Frm_BlobAnalyseTool.Instance.dgv_processingItem.SelectedRows[0].Index].ElementType = cbx_elementType.Text.Trim();
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
