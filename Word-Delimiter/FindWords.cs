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
    public partial class FindWords : Form
    {
        Form1? owner;

        public FindWords()
        {
            InitializeComponent();
        }

        //найти следующее совпадение
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0)
                return;
            FindNext(owner.richTextBox1.SelectionStart);
        }

        public void FindNext(int pos)
        {
            owner.richTextBox1.SelectionColor = Color.Black;
            while (pos > -1)
            {
                if (Form1.punctuation.Contains(owner.richTextBox1.Text[pos]))
                {
                    pos++;
                    break;
                }
                pos--;
            }
            pos = pos == -1 ? 0 : pos;

            int found_pos = owner.richTextBox1.Text.Substring(pos).IndexOf(textBox1.Text) + pos;
            if (found_pos < pos | found_pos == -1)
            {
                MessageBox.Show("Далее нет совпадений");
            }

            while (found_pos > -1)
            {
                if (Form1.punctuation.Contains(owner.richTextBox1.Text[found_pos]))
                {
                    found_pos++;
                    break;
                }
                found_pos--;
            }

            int len = 0;
            for (int i = found_pos; i < owner.richTextBox1.Text.Length; i++)
            {
                if (Form1.punctuation.Contains(owner.richTextBox1.Text[i]))
                {
                    len = i - found_pos;
                    break;
                }
            }

            int word_count = 0;
            for (int i = 0; i < found_pos; i++)
            {
                char c = owner.richTextBox1.Text[i];
                if (Form1.punctuation.Contains(c))
                {
                    word_count++;
                    break;
                }
            }

            //Закраска в первом
            owner.richTextBox1.SelectionStart = found_pos;
            owner.richTextBox1.SelectionLength = len;
            owner.richTextBox1.SelectionColor = Color.Red;
            owner.richTextBox1.ScrollToCaret();
            owner.richTextBox1.SelectionStart = found_pos + len;
        }

        private void FindWords_FormClosing(object sender, FormClosingEventArgs e)
        {
            owner.isClosed_FindWords = true;
        }

        private void FindWords_Load(object sender, EventArgs e)
        {
            owner = this.Owner as Form1;
        }
    }
}
