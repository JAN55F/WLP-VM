using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Controls.Properties;

namespace Controls
{
    public delegate void DSelectedIndexChanged();
    public partial class CComboBox : UserControl
    {
        public CComboBox()
        {
            InitializeComponent();
            cbx_item.Items.AddRange(Items);
            UpdateDropDownWidth();
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
                _selectedIndex = value;
                if (_syncing)
                    return;
                try
                {
                    if (cbx_item.SelectedIndex != value)
                    {
                        _syncing = true;
                        try
                        {
                            cbx_item.SelectedIndex = value;
                        }
                        finally
                        {
                            _syncing = false;
                        }
                    }
                }
                catch
                {
                    //设置索引失败不影响主流程，避免异常冒泡导致未处理崩溃
                }
                finally
                {
                    //无论程序设置还是用户选择，都让缓存与内部下拉框的真实显示保持一致；
                    //否则程序化选中后 TextStr 仍是旧值，调用方（如发送目标）会拿到过期内容
                    _selectedIndex = cbx_item.SelectedIndex;
                    _text = cbx_item.Text;
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
                _text = value;
                if (_syncing)
                    return;
                _syncing = true;
                try
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        if (cbx_item.SelectedIndex != -1)
                            cbx_item.SelectedIndex = -1;
                        cbx_item.Text = string.Empty;
                        return;
                    }

                    bool matched = false;
                    for (int i = 0; i < cbx_item.Items.Count; i++)
                    {
                        if (string.Equals(cbx_item.Items[i].ToString(), value, StringComparison.Ordinal))
                        {
                            if (cbx_item.SelectedIndex != i)
                                cbx_item.SelectedIndex = i;
                            matched = true;
                            break;
                        }
                    }

                    if (!matched)
                    {
                        if (cbx_item.SelectedIndex != -1)
                            cbx_item.SelectedIndex = -1;
                        cbx_item.Text = value;
                    }
                }
                catch
                {
                    //设置文本失败不影响主流程，避免异常冒泡导致未处理崩溃
                }
                finally
                {
                    _syncing = false;
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
                if (value)
                    cbx_item.DropDownStyle = ComboBoxStyle.DropDown;
                else
                    cbx_item.DropDownStyle = ComboBoxStyle.DropDownList;
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
                    //重建项后内部选中会被重置，同步缓存，保证 TextStr 与显示一致
                    _selectedIndex = cbx_item.SelectedIndex;
                    _text = cbx_item.Text;
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

        /// <summary>
        /// 移除指定项；若移除的是当前选中项则清空选择，否则保持原选中
        /// </summary>
        /// <param name="item">项</param>
        public void Remove(string item)
        {
            try
            {
                string oldText = TextStr;
                bool removed = false;
                for (int i = 0; i < Items.Length; i++)
                {
                    if (string.Equals(Items[i], item, StringComparison.Ordinal))
                    {
                        string[] items = new string[Items.Length - 1];
                        for (int j = 0, k = 0; j < Items.Length; j++)
                        {
                            if (j == i) continue;
                            items[k++] = Items[j];
                        }
                        Items = items;
                        removed = true;
                        break;
                    }
                }
                if (removed)
                {
                    if (item == oldText)
                        TextStr = string.Empty;      //移除的正是选中项，清空选择
                    else
                        TextStr = oldText;           //恢复原选中项
                }
            }
            catch
            {
            }
        }

        private void UpdateDropDownWidth()
        {
            int width = cbx_item.Width;
            using (Graphics graphics = cbx_item.CreateGraphics())
            {
                foreach (object item in cbx_item.Items)
                {
                    int itemWidth = TextRenderer.MeasureText(graphics, item.ToString(), cbx_item.Font).Width + 28;
                    if (itemWidth > width)
                        width = itemWidth;
                }
            }
            cbx_item.DropDownWidth = Math.Max(width, cbx_item.Width);
        }


        private void cbx_item_SelectedIndexChanged(object sender, EventArgs e)
        {
            //代码设置 SelectedIndex/TextStr 时(_syncing=true)内部事件已由属性 setter 处理，直接返回防止重入
            if (_syncing)
                return;
            _selectedIndex = cbx_item.SelectedIndex;
            _text = cbx_item.Text;
            if (SelectedIndexChanged != null)
                SelectedIndexChanged();
        }
        private void ComboBox_Enter(object sender, EventArgs e)
        {
            lbl_line.Height = 2;
            lbl_line.BackColor = Color.FromArgb(18, 150, 219);
            btn_showItem.Image = Resources.BlueImage;
        }
        private void ComboBox_Leave(object sender, EventArgs e)
        {
            lbl_line.Height = 1;
            lbl_line.BackColor = Color.Gray;
            btn_showItem.Image = Resources.GrayImage;
        }
        private void btn_showItem_Click(object sender, EventArgs e)
        {
            cbx_item.DroppedDown = true;
        }

        private void cbx_item_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;

            e.DrawBackground();
            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            Color backColor = selected ? Color.FromArgb(18, 150, 219) : Color.White;
            Color foreColor = selected ? Color.White : Color.FromArgb(50, 50, 50);

            using (SolidBrush backBrush = new SolidBrush(backColor))
                e.Graphics.FillRectangle(backBrush, e.Bounds);

            Rectangle textRect = new Rectangle(e.Bounds.Left + 8, e.Bounds.Top + 3, e.Bounds.Width - 12, e.Bounds.Height - 6);
            TextRenderer.DrawText(e.Graphics, cbx_item.Items[e.Index].ToString(), cbx_item.Font, textRect, foreColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            e.DrawFocusRectangle();
        }

    }
}
