using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bookkeeping_system
{
    public partial class Add_Form : Form
    {
        DataBase database = new DataBase();
        public int selectedTlsMenuStrip = SelectedTls.selTls;
        public Add_Form()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            if (selectedTlsMenuStrip == 0)
            {
                label2.Text = "Книги";
                label7.Text = "Название книги:";
                label8.Text = "               Автор:";
                label6.Text = "  Количество:";
                label3.Visible = false;
                textBox4.Visible = false;
            }
            if (selectedTlsMenuStrip == 1)
            {
                label2.Text = "Посетители";
                label7.Text = "                 ФИО:";
                label8.Text = "Название книги:";
                label6.Text = "Дата выдачи:";
                label3.Visible = true;
                textBox4.Visible = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            database.OpenConnection();

            if (selectedTlsMenuStrip == 0)
            {
                var book_name = textBox1.Text;
                var author = textBox2.Text;

                if (int.TryParse(textBox3.Text, out int count_of))
                {
                    var addQuery = $"insert into books_db (book_name, author, count_of) values ('{book_name}', '{author}','{count_of}')";

                    var com = new SqlCommand(addQuery, database.GetConnection());
                    com.ExecuteNonQuery();

                    MessageBox.Show("Запись была создана!", "Успешно.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("Количество должно иметь числовой формат!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            if (selectedTlsMenuStrip == 1)
            {
                var client_name = textBox1.Text;
                var book_name = textBox2.Text;
                var date_time = textBox3.Text;
                var status_book = textBox4.Text;
                var addQuery = $"insert into client_db (client_name, book_name, date_time, status_book) values ('{client_name}', '{book_name}', '{date_time}', '{status_book}')";

                    var com = new SqlCommand(addQuery, database.GetConnection());
                    com.ExecuteNonQuery();

                    MessageBox.Show("Запись успешно создана!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            database.CloseConnection();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
        }
    }
}
