using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Paint
{
    public partial class StarParametrsForm : Form
    {
        public StarParametrsForm()
        {
            InitializeComponent();
        }

        public string GetNumber()
        {
            return NumberTextBox.Text;
        }

        public string GetRatio()
        {
            return RatioTextBox.Text;
        }

        private void StarParametrsForm_Load(object sender, EventArgs e)
        {
            NumberTextBox.Text = (MainForm.n).ToString();
            RatioTextBox.Text = (MainForm.RadiusRatio).ToString();
        }
    }
}
