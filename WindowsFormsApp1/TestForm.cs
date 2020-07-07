using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class TestForm : Form
    {
        EnhancedListView<string> testList;
        public TestForm()
        {
            InitializeComponent();
            testList = new EnhancedListView<string>();
            testList.Font = new Font(FontFamily.GenericSansSerif, 13, FontStyle.Regular);
            testList.VerticalPadding = 5;
            testList.ForeColor = Color.DarkBlue;
            testList.HoverColor = Color.Cyan;
            testList.SelectColor = Color.Magenta;
            testList.BorderStyle = BorderStyle.FixedSingle;
            panel1.Controls.Add(testList);
            testList.Add("Cucumber");
            testList.Add("tomato");
            testList.Add("Banana");
            testList.Add("Cantalope");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            testList.FilterText = textBox1.Text;
        }
    }
}
