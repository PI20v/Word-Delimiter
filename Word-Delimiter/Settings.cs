using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Word_Delimiter
{
    public partial class Settings : Form
    {
        public Color res;
        public String ResText 
        {
            get
            {
                return textBox1.Text;
            }
        }


        public Settings(String punctuation, Color col)
        {
            InitializeComponent();
            res = col;
            textBox1.Text = punctuation;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ColorDialog dialog = new ColorDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                res = dialog.Color;
            }
        }
    }
}
