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
    public partial class Form1 : Form
    {
        static char[] punctuation = new char[] { ' ', '\n', '.', ',', '!', '?', '"', '(', ')', ';', ':', '\'', '{', '}' };

        public Form1()
        {
            InitializeComponent();
        }

        private void ProcessText(String str)
        {
            int words = 0, short_words = 0, letter_counter = 0, word_counter = 0;
            String resulting_text = string.Empty;

            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (punctuation.Contains(c))
                {
                    if (letter_counter < 3)
                        words++; short_words++;
                    resulting_text += c;
                    letter_counter = 0;
                }
                else
                {
                    if (letter_counter < 3)
                    {
                        resulting_text += c;
                        letter_counter++;
                    }
                    else
                    {
                        while (!punctuation.Contains(c))
                        {
                            try
                            {
                                c = str[++i];
                            }
                            catch (Exception)
                            {
                                resulting_text += "...";

                                Invoke(new Action(() => richTextBox2.Text += resulting_text));
                                resulting_text = string.Empty;
                                word_counter = 0;
                                words++;

                                MessageBox.Show($"Обработано слов: {words}\nСлов укорочено: {words - short_words}\nСлов без изменений: {short_words}");
                                return;
                            }
                        }
                        resulting_text += $"...{c}";
                        letter_counter = 0;
                        word_counter++;
                        words++;

                        if (word_counter == 10000)
                        {
                            Invoke(new Action(() => richTextBox2.Text += resulting_text));
                            word_counter = 0;
                            resulting_text = string.Empty;
                        }
                    }
                }
            }
            if (letter_counter <= 3)
            {
                short_words++;
                words++;
            }
                
            Invoke(new Action(() => richTextBox2.Text += resulting_text));
            MessageBox.Show($"Обработано слов: {words}\nСлов укорочено: {words - short_words}\nСлов без изменений: {short_words}");
        }

        //Обработка текста
        private async void button1_Click(object sender, EventArgs e)
        {
            richTextBox2.Text = String.Empty;
            String inputText = richTextBox1.Text;
            await Task.Run(() => ProcessText(inputText));
        }
    }
}
