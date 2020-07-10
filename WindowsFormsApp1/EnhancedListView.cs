using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    class EnhancedListView<T> : Panel
    {
        public List<T> Items { get; }

        private T _selectedItem;

        public T SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                if (_selectable)
                {
                    if (_selectedItem != null)
                    {
                        Controls.Find(_selectedItem.ToString(), false).First().ForeColor = ForeColor;
                    }
                    if (value != null)
                    {
                        _selectedItem = value;
                        Controls.Find(value.ToString(), false).FirstOrDefault().ForeColor = SelectColor;
                    }
                    SelectionChanged?.Invoke(_selectedItem, value);
                }
            }
        }

        public int VerticalPadding { get; set; } = 10;

        public int LeftMargin { get; set; } = 5;

        public Color HoverColor { get; set; } = Color.Blue;

        public Color SelectColor { get; set; } = Color.Blue;

        public delegate void SelectionChangedHandler(T oldItem, T newItem);

        public event SelectionChangedHandler SelectionChanged;

        private int _cursorY;

        private string _filterText = "";
        public string FilterText
        {
            get
            {
                return _filterText;
            }
            set
            {
                _filterText = value;
                int lastLocation = VerticalPadding;
                foreach (Control c in Controls)
                {
                    c.Visible = c.Name.ToLower().Contains(_filterText.ToLower());
                    if (c.Visible)
                    {
                        if (c.Location.Y != lastLocation)
                        {
                            c.Location = new Point(c.Location.X, lastLocation);
                        }
                        lastLocation += Font.Height + VerticalPadding;
                    }
                }
            }
        }

        private bool _selectable = false;
        private bool _draggable = false;

        public EnhancedListView(bool selectable, bool draggable)
        {
            this.AutoScroll = true;
            Items = new List<T>();
            _draggable = draggable;
            _selectable = selectable;
        }



        public void Add(T item)
        {
            Items.Add(item);
            Label l = CreateLabel(item, _cursorY);
            Controls.Add(l);
            _cursorY += l.Font.Height + VerticalPadding;
        }

        public void Clear()
        {
            Items.Clear();
            Controls.Clear();
            SelectedItem = default;
            _cursorY = VerticalPadding;
        }

        public void Remove(T item)
        {
            if (Items.Contains(item))
            {
                Control label = Controls.Find(item.ToString(), false).First();
                int position = label.Location.Y;
                _cursorY -= (label.Font.Height + VerticalPadding);
                Controls.Remove(label);
                foreach (Control c in Controls)
                {
                    if (c.Location.Y > position)
                    {
                        c.Location = new Point(LeftMargin, c.Location.Y - (VerticalPadding + Font.Height));
                    }
                }

                if (_selectedItem.Equals(item))
                {
                    _selectedItem = default;
                }
                Items.Remove(item);
            }
        }

        private Label CreateLabel(T item, int y)
        {
            Label l = new Label()
            {
                Text = item.ToString(),
                Location = new Point(LeftMargin, _cursorY),
                Font = Font,
                ForeColor = ForeColor,
                Width = Width - LeftMargin,
                Name = item.ToString()
            };
            if (item.Equals(_selectedItem))
            {
                l.ForeColor = SelectColor;
            }
            else
            {
                l.MouseEnter += (sender, args) => {
                    l.ForeColor = HoverColor;
                };
                l.MouseLeave += (sender, args) => {
                    if (!item.Equals(_selectedItem))
                    {
                        l.ForeColor = ForeColor;
                    }
                    else
                    {
                        l.ForeColor = SelectColor;
                    }
                };
            }
            l.Click += (sender, args) => {
                SelectedItem = item;
            };
            l.MouseDown += (sender, args) => {
                if (_draggable)
                {
                    l.DoDragDrop(item.ToString(), DragDropEffects.Copy | DragDropEffects.Move);
                }
            };
            return l;
        }

        public void ResizeToParent()
        {
            this.Width = this.Parent.Width;
            this.Height = this.Parent.Height;
            this.Location = new Point(0, 0);
        }

        public void UpdateDisplayText(T item, string newName)
        {
            foreach (Control c in Controls)
            {
                if (c.Text == item.ToString())
                {
                    c.Text = newName;
                    c.Name = newName;
                }
            }
        }


        protected override void OnDragOver(DragEventArgs drgevent)
        {
            base.OnDragOver(drgevent);

        }

    }
}
