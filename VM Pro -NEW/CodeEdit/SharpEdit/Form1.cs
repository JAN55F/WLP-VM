using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FastColoredTextBoxNS;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;

namespace SharpEdit
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CommConfig.AppLocationPath = Application.StartupPath;
            UpStyle();

            InitStylesPriority();
        }

        /// <summary>
        /// 窗体对象实例
        /// </summary>
        private static Form1 _instance;
        public static Form1 Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Form1();
                return _instance;
            }
        }

        private void UpStyle()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            UpdateStyles();
        }
        private void CreatNew()
        {
            #region 初始化数据
            fctb.Text = @"/*
日志记录
时间：" + DateTime.Now.ToLocalTime() + @"
用户：" + Environment.UserName + @"
记录：
*/
using System;
using System.Text;
class Program
{
    static void Main()
    {
       //代码
       
    }
}";

            #endregion
        }
        private void InitStylesPriority()
        {

            fctb.ClearUndo();
            fctb.IsChanged = false;

            CreatNew();

            KeyWordsAuto.CreatMenu(fctb, imageList1);
            KeysWordCMDAuto.CreatMenu(consoleTextBox1, imageList1);
        }
        #region 系统UI
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (Directory.Exists(CommConfig.BinPath))
                    Directory.Delete(CommConfig.BinPath, true);
                this.Hide();
                e.Cancel = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) != 0 && e.KeyCode == Keys.Up)
            { fctb.Focus(); }
            else if ((Control.ModifierKeys & Keys.Control) != 0 && e.KeyCode == Keys.Down)
            { consoleTextBox1.Focus(); }
        }

        private void fctb_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void fctb_KeyDown(object sender, KeyEventArgs e)
        {
            if (CompliReslut.RowErrorNumber != -1)
            {
                fctb[CompliReslut.RowErrorNumber - 1].BackgroundBrush = null;
                fctb.Invalidate();
                CompliReslut.RowErrorNumber = -1;
            }
        }
        #endregion

        #region 右击菜单
        private void 粘贴ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fctb.Paste();
        }

        private void 复制ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fctb.Copy();
        }

        private void 剪切ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fctb.Cut();

        }

        private void 全选ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fctb.SelectAll();
        }

        private void 清除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fctb.Clear();
        }
        #endregion

        #region 关闭按钮
        private void picClose_MouseEnter(object sender, EventArgs e)
        {
            picClose.Image = Properties.Resources.close2;
        }

        private void picClose_MouseLeave(object sender, EventArgs e)
        {
            picClose.Image = Properties.Resources.close1;
        }

        private void picClose_MouseDown(object sender, MouseEventArgs e)
        {
            picClose.Image = Properties.Resources.close3;
        }

        private void picClose_MouseUp(object sender, MouseEventArgs e)
        {
            this.Close();
        }
        #endregion

        #region 最小化按钮
        private void picMin_MouseEnter(object sender, EventArgs e)
        {
            picMin.Image = Properties.Resources.min2;
        }
        private void picMin_MouseLeave(object sender, EventArgs e)
        {
            picMin.Image = Properties.Resources.min1;
        }
        private void picMin_MouseDown(object sender, MouseEventArgs e)
        {
            picMin.Image = Properties.Resources.min3;
        }
        private void picMin_MouseUp(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        #endregion

        #region 全屏按钮
        private void picMax_MouseEnter(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                picMax.Image = Properties.Resources.forMax2;
            }
            else
            {
                picMax.Image = Properties.Resources.forNor2;
            }
        }

        private void picMax_MouseLeave(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                picMax.Image = Properties.Resources.forMax1;
            }
            else
            {
                picMax.Image = Properties.Resources.forNor1;
            }
        }

        private void picMax_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                picMax.Image = Properties.Resources.forMax3;
            }
            else
            {
                picMax.Image = Properties.Resources.forNor3;
            }
        }

        private void picMax_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
            }
        }
        #endregion

        #region 移动
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void splitContainer1_Panel1_MouseMove(object sender, MouseEventArgs e)
        {
            //移动
            if (e.Button == MouseButtons.Left)
            {
                if (e.X < this.Width)
                {
                    ReleaseCapture();
                    SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            }
        }
        private void label1_MouseMove(object sender, MouseEventArgs e)
        {
            //移动
            if (e.Button == MouseButtons.Left)
            {
                if (e.X < this.Width)
                {
                    ReleaseCapture();
                    SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            }
        }
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                picMax.Image = Properties.Resources.forMax1;
            }
            else
            {
                picMax.Image = Properties.Resources.forNor1;
            }
        }

        #endregion

        #region 动画
        public bool isShow = true;
        int loadSpeed = 10;
        public void StartMeun()
        {
            if (isShow)
            {
                timerBack.Enabled = true;
            }
            else
            {
                timerCell.Enabled = true;
            }
        }
        private void CellOutputHide()
        {
            //////panel1.Height -= loadSpeed;
            loadSpeed += loadSpeed;
            //////if (panel1.Height <= 30)
            {
                timerCell.Enabled = false;
                //////panel1.Height = 30;
                loadSpeed = 10;

                //back
                timerBack.Enabled = true;
            }
        }
        private void CellOutputShow()
        {
            //////panel1.Height += loadSpeed;
            loadSpeed += loadSpeed;
            //////if (panel1.Height >= 310)
            {
                timerCell.Enabled = false;
                //////panel1.Height = 310;
                loadSpeed = 10;

                //OK
                // picDic.Image = Properties.Resources.rdir1;
                isShow = false;//显示过程完成
            }
        }

        private void backOutputHide()
        {
            //////panel1.Left += loadSpeed;
            loadSpeed += loadSpeed;
            //////if (panel1.Left >= this.Width)
            {
                timerBack.Enabled = false;
                //////panel1.Left = this.Width;
                loadSpeed = 10;

                //OK
                //picDic.Image = Properties.Resources.dir1;
                isShow = true;//显示过程结束
            }
        }
        private void backOutputShow()
        {
            //////panel1.Left -= loadSpeed;
            loadSpeed += loadSpeed;
            //////if (panel1.Left <= this.Width - panel1.Width - 18)
            {
                timerBack.Enabled = false;
                //////panel1.Left = this.Width - panel1.Width - 18;
                loadSpeed = 10;

                //celll
                timerCell.Enabled = true;
            }
        }
        private void timerCell_Tick(object sender, EventArgs e)
        {
            if (isShow)
            {
                CellOutputShow();
            }
            else
            {
                CellOutputHide();
            }
        }

        private void timerBack_Tick(object sender, EventArgs e)
        {
            if (isShow)
            {
                backOutputShow();
            }
            else
            {
                backOutputHide();
            }
        }

        #endregion


        public void FileSave()
        {
            if (!Directory.Exists(CommConfig.ClassLibPath))
            {
                Directory.CreateDirectory(CommConfig.ClassLibPath);
            }
            File.WriteAllText(CommConfig.fileName, fctb.Text);
        }

        #region 控制台
        bool stop;
        string comHead = ">: ";
        private void StartConsole()
        {
            //consoleTextBox1.WriteLine("C# Editer v2.0  墨云软件  [帮助请输入：\"?\"]\n");
            string text = "";
            stop = false;
            do
            {
                consoleTextBox1.WriteLine(comHead);
                text = consoleTextBox1.ReadLine();
                consoleTextBox1.WriteLine(RunCommand(text));
            } while (!stop);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            Stop();
            base.OnClosing(e);
        }

        void Stop()
        {
            stop = true;
            consoleTextBox1.IsReadLineMode = false;
        }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            StartConsole();
        }

        private string RunCommand(string c)
        {
            if (c == "csc" || c.StartsWith("csc "))//进入CSC命令模式
            {
                //return "";
                CommandRun m = new CommandRun(this);
                return m.RunCsc(c);
            }

            #region 基础命令
            StringBuilder sbr = new StringBuilder();
            switch (c)
            {
                case "exit":
                    this.Close();
                    break;
                case "min":
                    this.WindowState = FormWindowState.Minimized;
                    break;
                case "max":
                    this.WindowState = FormWindowState.Maximized;
                    break;
                case "nor":
                    this.WindowState = FormWindowState.Normal;
                    break;
                case "clear":
                    fctb.Clear();
                    break;
                case "save":
                    FileSave();
                    break;
                case "code":
                    AddClassFileList();
                    if (isShow)
                        StartMeun();
                    break;
                case "?":
                case "help":
                    AddText(Properties.Resources.help);
                    if (isShow)
                        StartMeun();
                    break;
                case "test":
                    AddText(Properties.Resources.TestCode);
                    if (isShow)
                        StartMeun();
                    break;
                case "hide":
                    if (!isShow)
                        StartMeun();
                    break;
                case "new":
                    CreatNew();
                    break;
                default:
                    sbr.AppendLine("unknown command！");
                    break;
            }

            return sbr.ToString();
            #endregion
        }
        #endregion

        /// <summary>
        /// 当前工具所属的流程
        /// </summary>
        internal static string jobName = string.Empty;
        /// <summary>
        /// 当前工具名
        /// </summary>
        internal static string toolName = string.Empty;


        #region 添加UI
        public void AddCsc()
        {
            //////panControlView.Controls.Clear();
            ComplieUI cp = new ComplieUI();
            cp.BackColor = Color.Transparent;
            cp.fctb = this.fctb;
            cp.Dock = DockStyle.Fill;
            //////panControlView.Controls.Add(cp);
        }
        public void AddClassFileList()
        {
            //////panControlView.Controls.Clear();
            ClassFileList cp = new ClassFileList();
            cp.BackColor = Color.Transparent;
            cp.fctb = this.fctb;
            cp.Dock = DockStyle.Fill;
            //////panControlView.Controls.Add(cp);
        }
        public void AddText(string txt)
        {
            //////panControlView.Controls.Clear();
            TextBox tx = new TextBox();
            tx.Text = txt;
            tx.Dock = DockStyle.Fill;
            tx.BorderStyle = BorderStyle.None;
            tx.BackColor = Color.FromArgb(30, 17, 18);
            tx.ForeColor = Color.Green;
            tx.Multiline = true;
            tx.ScrollBars = ScrollBars.Both;
            tx.Font = new System.Drawing.Font("微软雅黑", 9);
            //////panControlView.Controls.Add(tx);
        }
        #endregion

        private void consoleTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up )
                e.Handled = true;
        }

        private void splitContainer3_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void picClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            this.label1.Text = this.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string program = fctb.Text.Trim();
            string output = "";
            new Runner().CompileAndRun(program, out output);
            this.consoleTextBox1.Text = output;
           // ((CodeEditTool)Job.GetToolByToolName(jobName, toolName)).input = consoleTextBox1.Text.Trim();
        }

    }
    public class Runner
    {
        private static readonly string[] ConsoleAssemblyNames = new[]
                                                                    {
                                                                        "mscorlib.dll", "Microsoft.CSharp.dll",
                                                                        "System.dll", "System.Core.dll",
                                                                        "System.Data.dll", "System.Data.DataSetExtensions.dll",
                                                                        "System.Xml.dll", "System.Xml.Linq.dll",
                                                                    };

        private static readonly Dictionary<string, string> CSharpCodeProviderOptions = new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } };

        public bool CompileAndRun(string code, out string output, string input1)
        {
            code = code.Replace("Input1", input1);

            var compiled = Compile(code);

            if (compiled.Errors.HasErrors)
            {
                output = ReturnErrors(compiled);
                return false;
            }
            output = ReturnOutput(compiled);
            return true;
        }

        public bool CompileAndRun(string code, out string output)
        {

            var compiled = Compile(code);

            if (compiled.Errors.HasErrors)
            {
                output = ReturnErrors(compiled);
                return false;
            }
            output = ReturnOutput(compiled);
            return true;
        }

        private static string ReturnErrors(CompilerResults compiled)
        {
            var builder = new StringBuilder();
            builder.AppendLine("Compilation errors:");
            compiled.Errors.Cast<CompilerError>()
                .Select(ce => ce.ErrorText)
                .ToList()
                .ForEach(error => builder.AppendLine(error));
            return builder.ToString();
        }

        private static CompilerResults Compile(string code)
        {
            using (var csc = new CSharpCodeProvider(CSharpCodeProviderOptions))
            {
                var parameters = new CompilerParameters(ConsoleAssemblyNames) { IncludeDebugInformation = true, GenerateInMemory = true };
                var results = csc.CompileAssemblyFromSource(parameters, code);
                return results;
            }
        }

        private static string ReturnOutput(CompilerResults compiled)
        {

            using (var writer = new StringWriter())
            {
                Console.SetOut(writer);
                //Console.SetIn(new StringReader(""));
                var assembly = compiled.CompiledAssembly;

                Type mainType = assembly.GetTypes().First(x => x.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).Count(m => m.Name.ToLower() == "main") > 0);

                var main = mainType.GetMethod("Main", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                var postedParams = new object[] { new string[] { } };
                main.Invoke(null, postedParams);
                return writer.ToString();
            }
        }
    }

}
