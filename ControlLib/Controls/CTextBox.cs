using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Controls.Properties;

namespace Controls
{
    public delegate void DTextStrChanged(string textStr);
    public partial class CTextBox : ModernInputControl
    {
        private bool syncingText;

        public CTextBox()
        {
            InitializeComponent();
            ConfigureModernInput();
            SyncDisplayFromValue();
        }

        private void ConfigureModernInput()
        {
            BackColor = Color.Transparent;
            lbl_line.Visible = false;
            tbx_text.BorderStyle = BorderStyle.None;
            tbx_text.BackColor = InputSurfaceColor;
            tbx_text.ForeColor = InputTextColor;
            btn_eye.BackgroundImage = null;
            btn_eye.BackColor = Color.Transparent;
            btn_eye.UseVisualStyleBackColor = false;
            btn_eye.FlatStyle = FlatStyle.Flat;
            btn_eye.FlatAppearance.BorderSize = 0;
            btn_eye.Paint += btn_eye_Paint;
            LayoutInputChildren();
        }

        protected override void OnModernPaletteChanged()
        {
            if (tbx_text == null)
                return;
            tbx_text.BackColor = InputSurfaceColor;
            btn_eye.BackColor = Color.Transparent;
            ApplyTextPresentation();
            Invalidate(true);
        }

        protected override void LayoutInputChildren()
        {
            if (tbx_text == null || btn_eye == null || lbl_line == null)
                return;

            lbl_line.Visible = false;
            int eyeWidth = btn_eye.Visible ? Math.Max(18, Math.Min(22, Height - 4)) : 0;
            int preferredHeight = Math.Min(tbx_text.PreferredHeight, Math.Max(1, Height - 6));
            tbx_text.SetBounds(8, Math.Max(2, (Height - preferredHeight) / 2),
                Math.Max(1, Width - 16 - eyeWidth), preferredHeight);
            if (eyeWidth > 0)
                btn_eye.SetBounds(Width - eyeWidth - 4, 3, eyeWidth, Math.Max(1, Height - 6));
        }

        private void btn_eye_Paint(object sender, PaintEventArgs e)
        {
            ConfigureQuality(e.Graphics);
            RectangleF eye = new RectangleF(4.5F, (btn_eye.Height / 2F) - 4F,
                Math.Max(7F, btn_eye.Width - 9F), 8F);
            using (Pen pen = new Pen(Enabled ? InputTextColor : SystemColors.GrayText, 1.35F))
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddBezier(eye.Left, eye.Top + eye.Height / 2F,
                    eye.Left + eye.Width * 0.25F, eye.Top - 1F,
                    eye.Right - eye.Width * 0.25F, eye.Top - 1F,
                    eye.Right, eye.Top + eye.Height / 2F);
                path.AddBezier(eye.Right, eye.Top + eye.Height / 2F,
                    eye.Right - eye.Width * 0.25F, eye.Bottom + 1F,
                    eye.Left + eye.Width * 0.25F, eye.Bottom + 1F,
                    eye.Left, eye.Top + eye.Height / 2F);
                e.Graphics.DrawPath(pen, path);
                float pupil = Math.Max(2F, Math.Min(4F, eye.Height / 2F));
                e.Graphics.DrawEllipse(pen, eye.Left + (eye.Width - pupil) / 2F,
                    eye.Top + (eye.Height - pupil) / 2F, pupil, pupil);
            }
        }

        /// <summary>
        /// 文本改变事件
        /// </summary>
        public event DTextStrChanged TextStrChanged;

        private void SetEditorText(string text)
        {
            string displayText = text ?? string.Empty;
            if (tbx_text.Text == displayText)
                return;

            syncingText = true;
            try
            {
                tbx_text.Text = displayText;
            }
            finally
            {
                syncingText = false;
            }
        }

        private void SyncDisplayFromValue()
        {
            SetEditorText(string.IsNullOrEmpty(_text) ? DefaultText : _text);
            ApplyTextPresentation();
        }

        private void ApplyTextPresentation()
        {
            if (tbx_text == null || btn_eye == null)
                return;

            bool emptyValue = string.IsNullOrEmpty(_text);
            tbx_text.ForeColor = emptyValue ? Color.DarkGray : InputTextColor;
            tbx_text.PasswordChar = PasswordChar && !emptyValue ? '*' : '\0';
            btn_eye.Visible = PasswordChar;
            btn_eye.Invalidate();
            LayoutInputChildren();
        }

        private void RaiseTextStrChanged()
        {
            if (TextStrChanged != null)
                TextStrChanged(_text);
        }

        /// <summary>
        /// 控件文本
        /// </summary>
        private string _text = string.Empty;
        public string TextStr
        {
            get
            {
                return _text;
            }
            set
            {
                string requestedText = value ?? string.Empty;
                bool changed = !string.Equals(_text, requestedText, StringComparison.Ordinal);
                // 先提交逻辑值，再同步占位文字；内部 TextChanged 被抑制，确保
                // 清空时不会先发送旧值、随后再发送空值。
                _text = requestedText;
                SyncDisplayFromValue();
                if (changed)
                    RaiseTextStrChanged();
            }
        }
        /// <summary>
        /// 默认文本
        /// </summary>
        private string _defaultText = string.Empty;
        public string DefaultText
        {
            get { return _defaultText; }
            set
            {
                string requestedText = value ?? string.Empty;
                if (string.Equals(_defaultText, requestedText, StringComparison.Ordinal))
                    return;
                _defaultText = requestedText;
                // DefaultText 只改变空值时的视觉占位，不是一次逻辑文本变更。
                if (string.IsNullOrEmpty(_text))
                    SyncDisplayFromValue();
            }
        }
        /// <summary>
        /// 是否以密码形式显示
        /// </summary>
        private bool _passwordChar = false;
        public bool PasswordChar
        {
            get { return _passwordChar; }
            set
            {
                _passwordChar = value;
                ApplyTextPresentation();
            }
        }


        private void TextBox_Enter(object sender, EventArgs e)
        {
            Invalidate();
            if (tbx_text.Text == DefaultText)
            {
                tbx_text.SelectionStart = 0;
                tbx_text.SelectionLength = 0;
            }
        }
        private void TextBox_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_text))
                SyncDisplayFromValue();
            Invalidate();
        }
        private void tbx_text_TextChanged(object sender, EventArgs e)
        {
            if (syncingText)
                return;

            string enteredText = tbx_text.Text ?? string.Empty;
            if (string.IsNullOrEmpty(_text) && enteredText == DefaultText)
            {
                ApplyTextPresentation();
                return;
            }

            bool changed = !string.Equals(_text, enteredText, StringComparison.Ordinal);
            _text = enteredText;
            if (enteredText.Length == 0)
                SyncDisplayFromValue();
            else
                ApplyTextPresentation();
            if (changed)
                RaiseTextStrChanged();
        }
        private void tbx_text_KeyDown(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(_text) && tbx_text.Text == DefaultText && e.KeyCode != Keys.Back)
            {
                // 清除占位文字只是为接收即将到来的按键，不应先发送一次空值事件。
                SetEditorText(string.Empty);
                tbx_text.ForeColor = InputTextColor;
            }
        }
        private void tbx_text_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (tbx_text.Text == DefaultText)
                {
                    tbx_text.SelectionStart = 0;
                    tbx_text.SelectionLength = 0;
                }
            }
        }
        private void btn_eye_MouseDown(object sender, MouseEventArgs e)
        {
            tbx_text.PasswordChar = '\0';
            btn_eye.Invalidate();
        }
        private void btn_eye_MouseUp(object sender, MouseEventArgs e)
        {
            if (PasswordChar && TextStr != string.Empty)
                tbx_text.PasswordChar = '*';
            else
                tbx_text.PasswordChar = '\0';
            btn_eye.Invalidate();
            tbx_text.Focus();
            tbx_text.SelectionStart = 0;
            tbx_text.SelectionLength = 0;
        }
        private void TextBox_Load(object sender, EventArgs e)
        {
            SyncDisplayFromValue();
        }

    }
}
