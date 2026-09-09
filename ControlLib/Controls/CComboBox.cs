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
    public delegate void DSelectedIndexChanged();
    public partial class CComboBox : ModernInputControl
    {
        private readonly ContextMenuStrip modernDropDown = new ContextMenuStrip();
        private TextBox editTextBox;

        public CComboBox()
        {
            InitializeComponent();
            ConfigureModernInput();
            cbx_item.Items.AddRange(Items);
            UpdateDropDownWidth();
        }

        private void ConfigureModernInput()
        {
            BackColor = Color.Transparent;
            lbl_line.Visible = false;
            modernDropDown.Font = Font;
            // 原生 ComboBox 的方形边框无法与 24~30px 的圆角底板平滑融合。
            // 保留它作为稳定的数据模型，显示和下拉菜单由外层控件统一绘制。
            cbx_item.Visible = false;
            btn_showItem.Image = null;
            btn_showItem.BackColor = Color.Transparent;
            btn_showItem.UseVisualStyleBackColor = false;
            btn_showItem.FlatStyle = FlatStyle.Flat;
            btn_showItem.FlatAppearance.BorderSize = 0;
            btn_showItem.Paint += btn_showItem_Paint;
            modernDropDown.ShowImageMargin = false;
            modernDropDown.ShowCheckMargin = false;
            modernDropDown.BackColor = InputSurfaceColor;
            modernDropDown.ForeColor = InputTextColor;
            modernDropDown.Font = Font;
            Disposed += delegate { modernDropDown.Dispose(); };
            TabStop = true;
            LayoutInputChildren();
        }

        protected override void OnModernPaletteChanged()
        {
            if (cbx_item == null)
                return;
            btn_showItem.BackColor = Color.Transparent;
            modernDropDown.BackColor = InputSurfaceColor;
            modernDropDown.ForeColor = InputTextColor;
            if (editTextBox != null)
            {
                editTextBox.BackColor = InputSurfaceColor;
                editTextBox.ForeColor = InputTextColor;
            }
            Invalidate(true);
        }

        protected override void LayoutInputChildren()
        {
            if (cbx_item == null || btn_showItem == null || lbl_line == null)
                return;

            lbl_line.Visible = false;
            int buttonWidth = Math.Max(20, Math.Min(24, Height - 2));
            btn_showItem.SetBounds(Math.Max(3, Width - buttonWidth - 4), 3,
                buttonWidth, Math.Max(1, Height - 6));
            btn_showItem.BringToFront();
            if (editTextBox != null)
            {
                editTextBox.Font = Font;
                int editHeight = Math.Min(editTextBox.PreferredHeight, Math.Max(1, Height - 6));
                editTextBox.SetBounds(8, Math.Max(2, (Height - editHeight) / 2),
                    Math.Max(1, btn_showItem.Left - 12), editHeight);
            }
            UpdateDropDownWidth();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (CanEdit || Width <= 16 || Height <= 4)
                return;

            Rectangle textBounds = new Rectangle(8, 2,
                Math.Max(1, btn_showItem.Left - 12), Math.Max(1, Height - 4));
            string displayText = cbx_item.Text;
            if (string.IsNullOrEmpty(displayText))
                displayText = TextStr;
            TextRenderer.DrawText(e.Graphics, displayText ?? string.Empty, Font, textBounds,
                Enabled ? InputTextColor : SystemColors.GrayText,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left && !CanEdit)
            {
                Focus();
                ShowModernDropDown();
            }
        }

        protected override bool IsInputKey(Keys keyData)
        {
            Keys key = keyData & Keys.KeyCode;
            if (key == Keys.Up || key == Keys.Down || key == Keys.Enter || key == Keys.Space)
                return true;
            return base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (!Enabled)
                return;

            if (e.KeyCode == Keys.Down && e.Alt)
            {
                ShowModernDropDown();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
            {
                ShowModernDropDown();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Down && Items != null && SelectedIndex < Items.Length - 1)
            {
                SelectUserItem(SelectedIndex + 1);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Up && Items != null && SelectedIndex > 0)
            {
                SelectUserItem(SelectedIndex - 1);
                e.Handled = true;
            }
        }

        private void btn_showItem_Paint(object sender, PaintEventArgs e)
        {
            ConfigureQuality(e.Graphics);
            float centerX = btn_showItem.ClientRectangle.Width / 2F;
            float centerY = btn_showItem.ClientRectangle.Height / 2F + 0.5F;
            using (Pen pen = new Pen(Enabled ? InputTextColor : SystemColors.GrayText, 1.6F))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                e.Graphics.DrawLines(pen, new[]
                {
                    new PointF(centerX - 3.5F, centerY - 2F),
                    new PointF(centerX, centerY + 1.5F),
                    new PointF(centerX + 3.5F, centerY - 2F)
                });
            }
        }

        /// <summary>
        /// 选中项改变事件
        /// </summary>
        public event DSelectedIndexChanged SelectedIndexChanged;
        /// <summary>
        /// 选中项索引
        /// </summary>
        private int _selectedIndex = -1;
        /// <summary>
        /// 防止 SelectedIndex/TextStr 与 cbx_item_SelectedIndexChanged 互相触发导致的重入。
        /// 设置内部 ComboBox 前先置位，事件回调中检测到置位直接返回，避免事件链嵌套。
        /// </summary>
        private bool _syncing = false;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (_syncing)
                {
                    _selectedIndex = value;
                    return;
                }

                _syncing = true;
                try
                {
                    if (cbx_item.SelectedIndex != value)
                        cbx_item.SelectedIndex = value;

                    // 程序设置索引不会触发用户事件，但属性、隐藏模型和可见文字
                    // 必须立即保持一致，避免 SelectedIndex 正确而 TextStr 仍是旧值。
                    _selectedIndex = cbx_item.SelectedIndex;
                    _text = cbx_item.Text ?? string.Empty;
                }
                catch
                {
                    //设置索引失败不影响主流程，避免异常冒泡导致未处理崩溃
                    _selectedIndex = cbx_item.SelectedIndex;
                    _text = cbx_item.Text ?? string.Empty;
                }
                finally
                {
                    _syncing = false;
                    if (editTextBox != null && editTextBox.Text != (_text ?? string.Empty))
                        editTextBox.Text = _text ?? string.Empty;
                    Invalidate();
                }
            }
        }
        /// <summary>
        /// 文本
        /// </summary>
        private string _text = string.Empty;
        public string TextStr
        {
            get { return _text; }
            set
            {
                string requestedText = value ?? string.Empty;
                _text = requestedText;
                if (_syncing)
                    return;
                _syncing = true;
                try
                {
                    if (requestedText.Length == 0)
                    {
                        if (cbx_item.SelectedIndex != -1)
                            cbx_item.SelectedIndex = -1;
                        cbx_item.Text = string.Empty;
                        _selectedIndex = -1;
                        _text = string.Empty;
                        return;
                    }

                    bool matched = false;
                    for (int i = 0; i < cbx_item.Items.Count; i++)
                    {
                        if (string.Equals(cbx_item.Items[i].ToString(), requestedText, StringComparison.Ordinal))
                        {
                            if (cbx_item.SelectedIndex != i)
                                cbx_item.SelectedIndex = i;
                            _selectedIndex = i;
                            _text = cbx_item.Text ?? requestedText;
                            matched = true;
                            break;
                        }
                    }

                    if (!matched)
                    {
                        if (cbx_item.SelectedIndex != -1)
                            cbx_item.SelectedIndex = -1;
                        cbx_item.Text = requestedText;
                        _selectedIndex = -1;
                        // DropDownList 会拒绝不在列表中的 Text；外层仍应显示调用方
                        // 设置的文字，因此由 _text 保留该值。
                        _text = requestedText;
                    }
                }
                catch
                {
                    //设置文本失败不影响主流程，避免异常冒泡导致未处理崩溃
                }
                finally
                {
                    _syncing = false;
                    if (editTextBox != null && editTextBox.Text != (_text ?? string.Empty))
                        editTextBox.Text = _text ?? string.Empty;
                    Invalidate();
                }
            }
        }
        /// <summary>
        /// 是否可以编辑
        /// </summary>
        private bool _canEdit = false;
        public bool CanEdit
        {
            get { return _canEdit; }
            set
            {
                _canEdit = value;
                // 隐藏的原生控件仍承担数据模型职责；可编辑模式必须允许任意文本，
                // 否则 IP 等非列表值会被 DropDownList 静默丢弃。
                cbx_item.DropDownStyle = value ? ComboBoxStyle.DropDown : ComboBoxStyle.DropDownList;
                if (value)
                    EnsureEditTextBox();
                if (editTextBox != null)
                {
                    editTextBox.Visible = value;
                    editTextBox.TabStop = value;
                }
                LayoutInputChildren();
                Invalidate();
            }
        }
        /// <summary>
        /// 项
        /// </summary>
        private string[] _items = new string[] { };
        public string[] Items
        {
            get { return _items; }
            set
            {
                _items = value;
                try
                {
                    cbx_item.Items.Clear();
                    cbx_item.Items.AddRange(value);
                    UpdateDropDownWidth();
                }
                catch
                {
                    //清空/添加项失败不影响主流程
                }
            }
        }


        /// <summary>
        /// 删除所有项
        /// </summary>
        public void Clear()
        {
            try
            {
                Items = new string[] { };
                cbx_item.Items.Clear();
                TextStr = string.Empty;
                SelectedIndex = -1;
                UpdateDropDownWidth();
            }
            catch
            {
            }
        }
        /// <summary>
        /// 添加项
        /// </summary>
        /// <param name="item">项</param>
        public void Add(string item)
        {
            if (string.IsNullOrEmpty(item))
                return;

            for (int i = 0; i < Items.Length; i++)
            {
                if (string.Equals(Items[i], item, StringComparison.Ordinal))
                    return;
            }

            string[] items = new string[Items.Length + 1];
            for (int i = 0; i < Items.Length; i++)
            {
                items[i] = Items[i];
            }
            items[Items.Length] = item;
            Items = items;
        }

        private void UpdateDropDownWidth()
        {
            int width = Math.Max(Width, 80);
            foreach (object item in cbx_item.Items)
            {
                int itemWidth = TextRenderer.MeasureText(item.ToString(), Font).Width + 28;
                if (itemWidth > width)
                    width = itemWidth;
            }
            cbx_item.DropDownWidth = width;
        }


        private void cbx_item_SelectedIndexChanged(object sender, EventArgs e)
        {
            //代码设置 SelectedIndex/TextStr 时(_syncing=true)内部事件已由属性 setter 处理，直接返回防止重入
            if (_syncing)
                return;
            _selectedIndex = cbx_item.SelectedIndex;
            _text = cbx_item.Text;
            Invalidate();
            if (SelectedIndexChanged != null)
                SelectedIndexChanged();
        }
        private void ComboBox_Enter(object sender, EventArgs e)
        {
            Invalidate();
            btn_showItem.Invalidate();
        }
        private void ComboBox_Leave(object sender, EventArgs e)
        {
            Invalidate();
            btn_showItem.Invalidate();
        }
        private void btn_showItem_Click(object sender, EventArgs e)
        {
            Focus();
            ShowModernDropDown();
        }

        private void EnsureEditTextBox()
        {
            if (editTextBox != null)
                return;

            editTextBox = new TextBox();
            editTextBox.Name = "modernEditTextBox";
            editTextBox.BorderStyle = BorderStyle.None;
            editTextBox.BackColor = InputSurfaceColor;
            editTextBox.ForeColor = InputTextColor;
            editTextBox.Font = Font;
            editTextBox.Text = _text ?? string.Empty;
            editTextBox.Visible = false;
            editTextBox.TextChanged += delegate
            {
                if (_syncing)
                    return;
                _text = editTextBox.Text;
                cbx_item.Text = _text;
            };
            Controls.Add(editTextBox);
            editTextBox.BringToFront();
        }

        private void ShowModernDropDown()
        {
            if (!Enabled || Items == null || Items.Length == 0)
                return;

            while (modernDropDown.Items.Count > 0)
            {
                ToolStripItem oldItem = modernDropDown.Items[0];
                modernDropDown.Items.RemoveAt(0);
                oldItem.Dispose();
            }
            for (int index = 0; index < Items.Length; index++)
            {
                int itemIndex = index;
                ToolStripMenuItem menuItem = new ToolStripMenuItem(Items[index] ?? string.Empty);
                menuItem.AutoSize = false;
                menuItem.Width = Math.Max(Width, cbx_item.DropDownWidth);
                menuItem.Height = Math.Max(25, Font.Height + 8);
                menuItem.Checked = index == SelectedIndex;
                menuItem.Click += delegate { SelectUserItem(itemIndex); };
                modernDropDown.Items.Add(menuItem);
            }
            modernDropDown.Show(this, new Point(0, Height));
        }

        private void SelectUserItem(int index)
        {
            if (Items == null || index < 0 || index >= Items.Length)
                return;

            _syncing = true;
            try
            {
                _selectedIndex = index;
                _text = Items[index] ?? string.Empty;
                cbx_item.SelectedIndex = index;
                if (editTextBox != null)
                    editTextBox.Text = _text;
            }
            finally
            {
                _syncing = false;
            }
            Invalidate();
            if (SelectedIndexChanged != null)
                SelectedIndexChanged();
        }

        private void cbx_item_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;

            e.DrawBackground();
            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            Color backColor = selected ? Color.FromArgb(226, 240, 252) : InputSurfaceColor;
            Color foreColor = InputTextColor;

            using (SolidBrush backBrush = new SolidBrush(backColor))
                e.Graphics.FillRectangle(backBrush, e.Bounds);

            Rectangle textRect = new Rectangle(e.Bounds.Left + 8, e.Bounds.Top + 3, e.Bounds.Width - 12, e.Bounds.Height - 6);
            TextRenderer.DrawText(e.Graphics, cbx_item.Items[e.Index].ToString(), cbx_item.Font, textRect, foreColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

    }
}
