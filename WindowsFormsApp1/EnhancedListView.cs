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
        private List<T> _items;

        private T _selectedItem;

        public T SelectedItem { 
            get {
                return _selectedItem;
            }
            set {
                _selectedItem = value;
                Draw();
            }
        }

        public int VerticalPadding { get; set; } = 10;
        
        public int LeftMargin { get; set; } = 5;

        public Color HoverColor { get; set; } = Color.Blue;

        public Color SelectColor { get; set; } = Color.Blue;
        
        
        private string _filterText = "";
        public string FilterText {
            get {
                return _filterText;
            } 
            set {
                _filterText = value;
                Draw();
            } 
        }

        public EnhancedListView () {
            this.AutoScroll = true;
            _items = new List<T>();
            AllowDrop = true;
        }         



        public void Add(T item) {
            _items.Add(item);
            Draw();
        }


        public void Draw() {            
            this.Width = this.Parent.Width;
            this.Height = this.Parent.Height;
            this.Location = new Point(0, 0);

            List<Control> newControls = new List<Control>();            
            int cursorY = VerticalPadding;
            foreach (T item in _items) {
                if (!item.ToString().ToLower().Contains(_filterText.ToLower())) {
                    continue;
                }
                Label l = new Label() {
                    Text = item.ToString(),
                    Location = new Point(LeftMargin, cursorY),
                    Font = Font,
                    ForeColor = ForeColor,                    
                };
                if (item.Equals(_selectedItem))
                {
                    l.ForeColor = SelectColor;
                }
                else
                {
                    l.MouseEnter += (sender, args) =>
                    {
                        l.ForeColor = HoverColor;
                    };
                    l.MouseLeave += (sender, args) =>
                    {
                        l.ForeColor = ForeColor;
                    };
                }
                l.Click += (sender, args) =>
                {
                    //_selectedItem = item;
                    //Draw();
                    //l.DoDragDrop(item, DragDropEffects.Move | DragDropEffects.Copy);
                };
                l.MouseDown += (sender, args) => {
                    l.DoDragDrop(item, DragDropEffects.Move | DragDropEffects.Copy);
                };
                newControls.Add(l);
                cursorY += l.Font.Height + VerticalPadding;
            }
            this.Controls.Clear();
            this.Controls.AddRange(newControls.ToArray());
        }

        protected override void OnDragOver(DragEventArgs drgevent)
        {
            base.OnDragOver(drgevent);
            //MessageBox.Show("Hello?");
        }

    }
}
