// 版权所有(C) ChoiceTech Corporation。保留所有权利。
// 此代码的发布遵从
// ChoiceTech 公共许可(HY-PL，http://choicetech.cn/hy-pl.html)的条款。
//
//版权所有(C) ChoiceTech Corporation。保留所有权利。

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using HalconDotNet;
using System.Threading;


namespace ChoiceTech.Halcon.Control
{
    public delegate void DoubleClick();
    /// <summary>
    /// halcon鼠标缩放控件
    /// 
    /// 描述:
    ///      1, 必须首先通过this.HobjectToHimage(HObject hobject)传入图片,此图片称为"背景图"
    ///      2, 有了背景图,就可以通过本控件自定义的 this.DispObj(HObject hObj)显示HObject,类似原方法
    ///      3,默认显示红色,DispObj(HObject hObj,string color)可显示其他颜色
    /// </summary>
    public partial class HWindow_Final : UserControl
    {
        #region 私有变量定义.

        private HWindow /**/                 hv_window;                                       //halcon窗体控件的句柄 this.mCtrl_HWindow.HalconWindow;
        // 窗体控件右键菜单内容
        ToolStripMenuItem saveImg_strip;
        ToolStripMenuItem saveWindow_strip;
        ToolStripMenuItem barVisible_strip;
        ToolStripMenuItem histogram_strip;

        private HImage  /**/                 hv_image;                                        //缩放时操作的图片  此处千万不要使用hv_image = new HImage(),不然在生成控件dll的时候,会导致无法序列化,去你妈隔壁,还好老子有版本控制,不然都找不到这种恶心问题
        private int /**/                     hv_imageWidth, hv_imageHeight;                   //图片宽,高
        private string /**/                  str_imgSize;                                     //图片尺寸大小 5120X3840
        private bool    /**/                 drawModel = false;                                //绘制模式下,不允许缩放和鼠标右键菜单

        public ViewWindow.ViewWindow viewWindow;    /**/                                      //ViewWindow
        public HWindowControl hWindowControl;   /**/                                           // 当前halcon窗口

        #endregion


        /// <summary>
        /// 初始化控件
        /// </summary>
        public HWindow_Final()
        {
            InitializeComponent();
            //
            viewWindow = new ViewWindow.ViewWindow(mCtrl_HWindow);
            hWindowControl = this.mCtrl_HWindow;
            hv_window = this.mCtrl_HWindow.HalconWindow;

            //              设定鼠标按下时图标的形状
            //              'arrow'  'default' 'crosshair' 'text I-beam' 'Slashed circle' 'Size All'
            //              'Size NESW' 'Size S' 'Size NWSE' 'Size WE' 'Vertical Arrow' 'Hourglass'
            //
            // hv_window.SetMshape("Hourglass");

            barVisible_strip = new ToolStripMenuItem("显示StatusBar");
            barVisible_strip.CheckOnClick = true;
            barVisible_strip.CheckedChanged += new EventHandler(barVisible_strip_CheckedChanged);
            m_CtrlHStatusLabelCtrl.Visible = false;
            mCtrl_HWindow.Height = this.Height;

            saveImg_strip = new ToolStripMenuItem("保存原始图像");
            saveImg_strip.Click += new EventHandler((s, e) => SaveImage());

            saveWindow_strip = new ToolStripMenuItem("保存窗口缩略图");
            saveWindow_strip.Click += new EventHandler((s, e) => SaveWindowDump());

            histogram_strip = new ToolStripMenuItem("显示直方图(H)");
            histogram_strip.CheckOnClick = true;
            histogram_strip.Checked = false;



            barVisible_strip.Enabled = true;
            histogram_strip.Enabled = false;
            saveImg_strip.Enabled = false;
            saveWindow_strip.Enabled = false;

            mCtrl_HWindow.SizeChanged += new EventHandler((s, e) => DispImageFit());

        }


        /// <summary>
        /// 绘制模式下,不允许缩放和鼠标右键菜单
        /// </summary>
        public bool DrawModel
        {
            get { return drawModel; }
            set
            {
                //缩放控制
                viewWindow.setDrawModel(value);
                drawModel = value;
            }
        }
        private bool _EditModel = true;//绘制的图形是否可以编辑
        public bool EditModel
        {
            get
            {
                return _EditModel;
            }
            set
            {
                viewWindow.setEditModel(value);
                _EditModel = value;
            }
        }

        /// <summary>
        /// 设置image,初始化控件参数
        /// </summary>
        public bool EnableImagePan
        {
            get { return viewWindow._hWndControl.EnableImagePan; }
            set { viewWindow._hWndControl.EnableImagePan = value; }
        }

        public HImage Image
        {
            get
            {
                return this.hv_image;
            }
            set
            {
                if (value != null)
                {
                    if (this.hv_image != null)
                    {
                        this.hv_image.Dispose();
                    }

                    this.hv_image = value;
                    hv_image.GetImageSize(out hv_imageWidth, out hv_imageHeight);
                    str_imgSize = String.Format("{0}X{1}", hv_imageWidth, hv_imageHeight);

                    //DispImageFit(mCtrl_HWindow);
                    try
                    {
                        barVisible_strip.Enabled = true;
                        histogram_strip.Enabled = true;
                        saveImg_strip.Enabled = true;
                        saveWindow_strip.Enabled = true;
                    }
                    catch (Exception)
                    {
                    }



                    ////// int W,H,w,h;
                    //////W =this .Width ;
                    //////H =this.Height ;
                    //////w =   hv_imageWidth;
                    //////h =hv_imageHeight;
                    //////if (w*1.0 / W < h*1.0 / H)
                    //////{
                    //////    HOperatorSet.SetPart(this.HWindowHalconID, 0, -(h * W / H - w) / 2, h, w + (h * W/ H - w) / 2);
                    //////}
                    //////else if (w*1.0 / W > h*1.0 / H)
                    //////{
                    //////    HOperatorSet.SetPart(this.HWindowHalconID, (h - w * H / H) / 2, 0, h - (h - w * H / H) / 2, w);
                    //////}
                    //////else
                    //////{
                    //////    HOperatorSet.SetPart(this.HWindowHalconID, 0,0,H,W);
                    //////}



                    // HOperatorSet.SetPart(this.HWindowHalconID, 0, 0, 3844, 4000);                       


                    viewWindow.displayImage(hv_image);
                }
            }
        }

        /// <summary>
        /// 获得halcon窗体控件的句柄
        /// </summary>
        public IntPtr HWindowHalconID
        {
            get { return this.mCtrl_HWindow.HalconID; }
        }

        public HWindowControl getHWindowControl()
        {
            return this.mCtrl_HWindow;
        }

        /// <summary>
        /// 状态条 显示/隐藏 CheckedChanged事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void barVisible_strip_CheckedChanged(object sender, EventArgs e)
        {
            ToolStripMenuItem strip = sender as ToolStripMenuItem;

            this.SuspendLayout();

            if (strip.Checked)
            {
                m_CtrlHStatusLabelCtrl.Visible = true;
                //mCtrl_HWindow.Height = this.Height - m_CtrlHStatusLabelCtrl.Height - m_CtrlHStatusLabelCtrl.Margin.Top - m_CtrlHStatusLabelCtrl.Margin.Bottom;
                mCtrl_HWindow.HMouseMove += HWindowControl_HMouseMove;
            }
            else
            {
                m_CtrlHStatusLabelCtrl.Visible = false;
                // mCtrl_HWindow.Height = this.Height;
                mCtrl_HWindow.HMouseMove -= HWindowControl_HMouseMove;
            }

            //DispImageFit(mCtrl_HWindow);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        public void showStatusBar()
        {
            barVisible_strip.Checked = true;
        }

        /// <summary>
        /// 保存窗体截图到本地
        /// </summary>
        private void SaveWindowDump()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PNG图像|*.png|所有文件|*.*";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if (String.IsNullOrEmpty(sfd.FileName))
                    return;

                //截取窗口图
                HOperatorSet.DumpWindow(HWindowHalconID, "png best", sfd.FileName);
            }
        }

        /// <summary>
        /// 保存原始图片到本地
        /// </summary>
        private void SaveImage()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "BMP图像|*.bmp|所有文件|*.*";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if (String.IsNullOrEmpty(sfd.FileName))
                {
                    return;
                }

                HOperatorSet.WriteImage(hv_image, "bmp", 0, sfd.FileName);
            }
        }

        /// <summary>
        /// 图片适应大小显示在窗体
        /// </summary>
        /// <param name="hw_Ctrl">halcon窗体控件</param>
        public void DispImageFit()
        {

            try
            {
                this.viewWindow.resetWindowImage();
            }
            catch (Exception)
            {

            }
        }


        /// <summary>
        /// 鼠标在空间窗体里滑动,显示鼠标所在位置的灰度值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HWindowControl_HMouseMove(object sender, HMouseEventArgs e)
        {

            if (hv_image != null)
            {
                try
                {
                    int button_state;
                    double positionX, positionY;
                    string str_value;
                    string str_position;
                    bool _isXOut = true, _isYOut = true;
                    HTuple channel_count;

                    HOperatorSet.CountChannels(hv_image, out channel_count);

                    hv_window.GetMpositionSubPix(out positionY, out positionX, out button_state);
                    str_position = String.Format("RC: {0:0000},{1:0000}", positionY, positionX);

                    _isXOut = (positionX < 0 || positionX >= hv_imageWidth);
                    _isYOut = (positionY < 0 || positionY >= hv_imageHeight);

                    if (!_isXOut && !_isYOut)
                    {
                        if ((int)channel_count == 1)
                        {
                            double grayVal;
                            grayVal = hv_image.GetGrayval((int)positionY, (int)positionX);
                            str_value = String.Format("Val: {0:000}", grayVal);
                        }
                        else if ((int)channel_count == 3)
                        {
                            double grayValRed, grayValGreen, grayValBlue;

                            HImage _RedChannel, _GreenChannel, _BlueChannel;

                            _RedChannel = hv_image.AccessChannel(1);
                            _GreenChannel = hv_image.AccessChannel(2);
                            _BlueChannel = hv_image.AccessChannel(3);

                            grayValRed = _RedChannel.GetGrayval((int)positionY, (int)positionX);
                            grayValGreen = _GreenChannel.GetGrayval((int)positionY, (int)positionX);
                            grayValBlue = _BlueChannel.GetGrayval((int)positionY, (int)positionX);

                            _RedChannel.Dispose();
                            _GreenChannel.Dispose();
                            _BlueChannel.Dispose();

                            str_value = String.Format("Gray: ({0:000}, {1:000}, {2:000})", grayValRed, grayValGreen, grayValBlue);
                        }
                        else
                        {
                            str_value = "";
                        }
                        m_CtrlHStatusLabelCtrl.Text = string.Format("Ch:{0}   ", channel_count) + str_imgSize + "    " + str_position + "    " + str_value;
                    }
                }
                catch (Exception ex)
                {
                    //不处理
                }
            }

        }
        public void ClearWindow()
        {
            try
            {
                this.Invoke(new Action(
                        () =>
                        {
                            //this.hv_image = null;
                            m_CtrlHStatusLabelCtrl.Visible = false;
                            barVisible_strip.Enabled = false;
                            histogram_strip.Enabled = false;
                            saveImg_strip.Enabled = false;
                            saveWindow_strip.Enabled = false;

                            mCtrl_HWindow.HalconWindow.ClearWindow();
                            viewWindow.ClearWindow();

                        }
                    ));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }


        }


        /// <summary>
        /// Hobject转换为的临时Himage,显示背景图
        /// </summary>
        /// <param name="hobject">传递Hobject,必须为图像</param>
        public void HobjectToHimage(HObject hobject)
        {
            if (hobject == null || !hobject.IsInitialized())
            {
                ClearWindow();
                return;
            }

            //记录当前视图：仅当用户已缩放/平移过(当前显示范围≠整幅图)时，显示新图后恢复，
            //避免每次运行工具/刷新图像都跳回自适应窗口，方便放大后查看效果图。
            //首次显示(窗口无图)、未缩放过的窗口、图像尺寸变化的场景仍保持自适应。
            int viewRow1 = 0, viewCol1 = 0, viewRow2 = 0, viewCol2 = 0;
            bool preserveView = false;
            if (this.Image != null && this.Image.IsInitialized())
            {
                GetViewPart(out viewRow1, out viewCol1, out viewRow2, out viewCol2);
                int imgW = viewWindow._hWndControl.imageWidth;
                int imgH = viewWindow._hWndControl.imageHeight;
                if (viewRow2 > viewRow1 && viewCol2 > viewCol1 &&
                    (viewRow1 > 0 || viewCol1 > 0 || viewRow2 < imgH - 1 || viewCol2 < imgW - 1))
                {
                    preserveView = true;
                }
            }

            this.Image = new HImage(hobject);

            //恢复用户之前的缩放/平移视图；新图像尺寸小于旧视图范围(越界)时保持自适应。
            if (preserveView)
            {
                try
                {
                    HTuple imgW, imgH;
                    HOperatorSet.GetImageSize(hobject, out imgW, out imgH);
                    if (viewRow2 <= (int)imgH.I && viewCol2 <= (int)imgW.I)
                    {
                        viewWindow._hWndControl.SetViewPart(viewRow1, viewCol1, viewRow2, viewCol2);
                        HOperatorSet.DispObj(hobject, HWindowHalconID);
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 获取当前窗口显示范围(图像坐标)，用于运行工具后恢复用户缩放/平移视图。
        /// </summary>
        public void GetViewPart(out int row1, out int col1, out int row2, out int col2)
        {
            row1 = 0; col1 = 0; row2 = 0; col2 = 0;
            try
            {
                HTuple r1, c1, r2, c2;
                HOperatorSet.GetPart(HWindowHalconID, out r1, out c1, out r2, out c2);
                row1 = (int)r1.D;
                col1 = (int)c1.D;
                row2 = (int)r2.D;
                col2 = (int)c2.D;
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 恢复窗口显示范围(保留用户缩放/平移视图)，并重新绘制背景图。
        /// </summary>
        public void SetViewPart(HObject backgroundImage, int row1, int col1, int row2, int col2)
        {
            try
            {
                viewWindow._hWndControl.SetViewPart(row1, col1, row2, col2);

                if (backgroundImage != null && backgroundImage.IsInitialized())
                    HOperatorSet.DispObj(backgroundImage, HWindowHalconID);
            }
            catch (Exception)
            {
            }
        }

        #region 缩放后,再次显示传入的HObject


        /// <summary>
        /// 默认红颜色显示
        /// </summary>
        /// <param name="hObj">传入的region.xld,image</param>
        public void DispObj(HObject hObj)
        {

            lock (this)
            {
                viewWindow.displayHobject(hObj, null);
            }


        }

        /// <summary>
        /// 重新开辟内存保存 防止被传入的HObject在其他地方dispose后,不能重现
        /// </summary>
        /// <param name="hObj">传入的region.xld,image</param>
        /// <param name="color">颜色</param>
        public void DispObj(HObject hObj, string color)
        {

            lock (this)
            {
                viewWindow.displayHobject(hObj, color);
            }


        }


        #endregion

        /// <summary>
        /// 鼠标离开事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mCtrl_HWindow_MouseLeave(object sender, EventArgs e)
        {
            //避免鼠标离开窗口,再返回的时候,图表随着鼠标移动
            viewWindow.mouseleave();
        }



        public event DoubleClick doubleClick;

        private void mCtrl_HWindow_Click(object sender, EventArgs e)
        {

        }
        int i = 0;
        private void mCtrl_HWindow_HMouseDown(object sender, HMouseEventArgs e)
        {
            i += 1;

            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

            timer.Interval = 300;

            timer.Tick += (s, e1) => { timer.Enabled = false; i = 0; };

            timer.Enabled = true;

            if (i % 2 == 0)
            {

                timer.Enabled = false;

                i = 0;

                if (doubleClick != null)
                    doubleClick();

                Application.DoEvents();
                Thread.Sleep(5);
                DispImageFit();
                //this.WindowState = (this.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized);

            }
        }


    }


}
