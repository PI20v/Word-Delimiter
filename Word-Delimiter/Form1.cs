using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using NPOI.XWPF.UserModel;
using static NPOI.HSSF.Util.HSSFColor;

namespace Word_Delimiter
{
    public partial class Form1 : Form
    {
        public static char[] punctuation = new char[] { ' ', '\n', '.', ',', '!', '?', '"', '(', ')', ';', ':', '\'', '{', '}' };
        int limit;
        public Color selectionColor = Color.Red;

        public bool isProcessed = false;

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            try
            {
                using (var reader = new StreamReader("config.xml"))
                {
                    var serializer = new XmlSerializer(typeof(int));
                    try
                    {
                        selectionColor = Color.FromArgb((int)serializer.Deserialize(reader));
                    }
                    catch (Exception)
                    {
                        return;
                    }
                }
            }
            catch (FileNotFoundException)
            {
                return;
            }
            catch (Exception exc)
            {
                MessageBox.Show("Непредвиденная ошибка при открытии файла настроек\n\n" + exc.Message);
            }
        }

        private void ProcessText(String str)
        {
            if (limit == 0)
            {
                if (textBox1.Text.Length != 0)
                {
                    if (textBox1.Text == "0")
                    {
                        MessageBox.Show("Максимальная длина не может быть равна 0");
                    }
                    else
                    {
                        MessageBox.Show("Введенное число невозможно считать");
                    }
                }
                else
                {
                    MessageBox.Show("Введите максимальную длину слова");
                }
                return;
            }
            else if (limit < 0)
            {
                MessageBox.Show("Максимальная длина не может быть отрицательной");
                return;
            }
            int words = 0, short_words = 0, letter_counter = 0, word_counter = 0;
            String resulting_text = string.Empty;

            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (punctuation.Contains(c))
                {
                    if (letter_counter != 0)
                    {
                        words++; short_words++;
                    }
                    resulting_text += c;
                    letter_counter = 0;
                }
                else
                {
                    if (letter_counter < limit)
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

                        if (word_counter == 15000)
                        {
                            Invoke(new Action(() => richTextBox2.Text += resulting_text));
                            word_counter = 0;
                            resulting_text = string.Empty;
                        }
                    }
                }
            }
            if (letter_counter != 0)
            {
                short_words++;
                words++;
            }
                
            Invoke(new Action(() => richTextBox2.Text += resulting_text));
            MessageBox.Show($"Обработано слов: {words}\nСлов укорочено: {words - short_words}\nСлов без изменений: {short_words}");
        }

        String temp;
        //Обработка текста
        private async void button1_Click(object sender, EventArgs e)
        {
            richTextBox2.Text = String.Empty;
            String inputText = richTextBox1.Text;
            richTextBox1.SelectAll();
            richTextBox1.SelectionColor = Color.Black;
            await Task.Run(() => ProcessText(inputText));

            if (MessageBox.Show("Сохранить результат?", "Сохранить результат?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                saveFileDialog.RestoreDirectory = true;
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var writer = new StreamWriter(saveFileDialog.FileName))
                        {
                            await writer.WriteAsync(richTextBox2.Text);
                        }
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show("Непредвиденная ошибка при сохранении текста в файл\n" + exc.Message);
                    }
                }
            }
            temp = richTextBox1.Text;
            isProcessed = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (Int32.TryParse(textBox1.Text, out limit))
            {
                textBox1.ForeColor = Color.Black;
            }
            else
            {
                textBox1.ForeColor = Color.Red;
            }
        }

        public bool isClosed_FindWords = true;
        private void button2_Click(object sender, EventArgs e)
        {
            if (isClosed_FindWords)
            {
                FindWords findWords = new FindWords();
                findWords.Show(this);
                isClosed_FindWords = false;
            }
        }

        private void richTextBox2_SelectionChanged(object sender, EventArgs e)
        {
            if (!isProcessed)
            {
                return;
            }
            if (richTextBox2.SelectionStart == richTextBox2.Text.Length)
                return;
            char c;
            String subStr;
            int words = 0;
            String found = string.Empty;
            richTextBox1.SelectionColor = Color.Black;
            int pos = richTextBox2.SelectionStart;

            if (pos != 0)
            {
                c = richTextBox2.Text[pos - 1];
                while (!(punctuation.Contains(c) | pos == 0))
                {
                    c = richTextBox2.Text[pos - 1];
                    pos--;
                }

                subStr = richTextBox2.Text.Substring(0, pos + 1).Replace("...", String.Empty);

                for (int i = 0; i < subStr.Length; i++)
                {
                    c = subStr[i];
                    if (punctuation.Contains(c))
                        words++;
                }

                pos = 0;
                for (int words_counter = 0; words_counter != words; pos++)
                {
                    c = richTextBox1.Text[pos];
                    if (punctuation.Contains(c))
                        words_counter++;
                }
            }

            for (int i = pos; i < richTextBox1.Text.Length; i++)
            {
                c = richTextBox1.Text[i];
                if (punctuation.Contains(c))
                    break;
                else
                    found += c;
            }

            richTextBox1.SelectionStart = pos;
            richTextBox1.SelectionLength = found.Length;
            richTextBox1.SelectionColor = selectionColor;
            richTextBox1.ScrollToCaret();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ColorDialog dialog = new ColorDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                selectionColor = dialog.Color;
                try
                {
                    using (var writer = new StreamWriter("config.xml"))
                    {
                        var serializer = new XmlSerializer(typeof(int));
                        try
                        {
                            serializer.Serialize(writer, selectionColor.ToArgb());
                        }
                        catch (Exception)
                        {
                            return;
                        }
                    }
                }
                catch (FileNotFoundException)
                {
                    return;
                }
                catch (Exception exc)
                {
                    MessageBox.Show("Непредвиденная ошибка при сохранении файла настроек\n\n" + exc.Message);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "txt files (*.txt)|*.txt|docx files (.docx)|*.docx|All files (*.*)|*.*";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                

                if (openFileDialog.FileName.EndsWith(".docx") || openFileDialog.FileName.EndsWith(".DOCX"))
                {
                    XWPFDocument document = null;
                    try
                    {
                        using (FileStream file = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                        {
                            document = new XWPFDocument(file);
                        }
                        richTextBox1.Clear();
                        richTextBox2.Clear();
                        foreach (var paragraph in document.Paragraphs)
                        {
                            richTextBox1.Text += paragraph.Text + Environment.NewLine;
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
                else
                {
                    using (var reader = new StreamReader(openFileDialog.FileName))
                    {
                        richTextBox1.Clear();
                        richTextBox2.Clear();
                        richTextBox1.Text = reader.ReadToEnd();
                    }
                }
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (isProcessed && richTextBox1.Text != temp) 
                isProcessed = false;
        }
    }
}
