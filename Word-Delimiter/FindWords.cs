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
        private char[] punctuation;
        private String text;

        public FindWords()
        {
            InitializeComponent();
        }
        int cur_pos = 0; 

        //найти следующее совпадение
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0)
                return;
            FindNext(cur_pos);
        }

        struct result
        {
            public int found_pos;
            public int len;
        }

        public async void FindNext(int pos)
        {
            owner.richTextBox1.SelectionStart = 0;
            owner.richTextBox1.SelectionLength = pos;
            owner.richTextBox1.ForeColor = Color.Black;
            owner.richTextBox1.SelectionColor = Color.Black;

            Task<result> task = Task<result>.Factory.StartNew((object pos) =>
            {
                int found_pos, len;
                result res = new result();
                res.len = res.found_pos = -1;
                String str = text;
                found_pos = text.Substring((int)pos).IndexOf(textBox1.Text) + (int)pos;
                if (found_pos < (int)pos | found_pos == -1)
                {
                    MessageBox.Show("Далее нет совпадений");
                    return res;
                }

                while (found_pos > -1)
                {
                    if (Form1.punctuation.Contains(text[found_pos]))
                    {
                        found_pos++;
                        break;
                    }
                    found_pos--;
                }

                if (found_pos == -1)
                    found_pos = 0;

                len = 0;
                for (int i = found_pos; i < text.Length; i++)
                {
                    if (Form1.punctuation.Contains(text[i]))
                    {
                        len = i - found_pos;
                        break;
                    }
                }
                if (len == 0)
                    len = text.Length - found_pos;
               
                res.len = len;
                res.found_pos = found_pos;
                return res;
            }, pos);
            int found_pos, len;
            result res = await task;
            found_pos = res.found_pos;
            len = res.len;

            //Закраска в первом
            if (found_pos == -1)
                return;
            owner.richTextBox1.SelectionStart = found_pos;
            owner.richTextBox1.SelectionLength = len;
            owner.richTextBox1.SelectionColor = Color.Red;
            owner.richTextBox1.ScrollToCaret();
            owner.richTextBox1.SelectionStart = found_pos + len;
            cur_pos = found_pos + len;
        }

        private void FindWords_FormClosing(object sender, FormClosingEventArgs e)
        {
            owner.isClosed_FindWords = true;
        }

        private void FindWords_Load(object sender, EventArgs e)
        {
            owner = this.Owner as Form1;
            punctuation = Form1.punctuation;
            text = owner.richTextBox1.Text;
        }
    }
}
