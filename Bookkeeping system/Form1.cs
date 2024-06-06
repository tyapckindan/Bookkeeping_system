using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Bookkeeping_system
{
    enum RowState
    {
        Existed,
        New,
        Modifed,
        ModifedNew,
        Deleted
    }

    public partial class Form1 : Form
    {
        DataBase database = new DataBase();

        int selectedRow;
        string useDB = "books_db";

        public int selectedTlsMenuStrip = SelectedTls.selTls;
        public Form1()
        {
            InitializeComponent();

            textBoxStatus.Visible = false;
            label8.Visible = false;
            label9.Visible = false;
            label10.Visible = false;
            label11.Visible = false;
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void CreateColumns()
        {
            if (Convert.ToInt32(selectedTlsMenuStrip) == 0)
            {
                dataGridView1.Columns.Add("id", "id");
                dataGridView1.Columns.Add("book_name", "Название книги");
                dataGridView1.Columns.Add("author", "Автор");
                dataGridView1.Columns.Add("count_of", "Количество");
                dataGridView1.Columns.Add("IsNew", String.Empty);
            }
            if (Convert.ToInt32(selectedTlsMenuStrip) == 1)
            {
                dataGridView1.Columns.Add("id", "id");
                dataGridView1.Columns.Add("client_name", "Имя посетителя");
                dataGridView1.Columns.Add("book_name", "Название книги");
                dataGridView1.Columns.Add("date_time", "Дата выдачи");
                dataGridView1.Columns.Add("status_book", "Статус книги");
                dataGridView1.Columns.Add("IsNew", String.Empty);
            }
        }

        private void Change()
        {
            var selectedRowIndex = dataGridView1.CurrentCell.RowIndex;

            int selectedTls = 4;

            if (selectedTlsMenuStrip == 1) { selectedTls = 5; }

            var id = textBoxId.Text;
            var book_name = textBoxName.Text;
            var author = textBoxAuthor.Text;
            var count_of = textBoxCount_of.Text;
            var status = textBoxStatus.Text;

            if (dataGridView1.Rows[selectedRowIndex].Cells[0].Value.ToString() != string.Empty)
            {

                if (selectedTls == 4) { dataGridView1.Rows[selectedRowIndex].Cells[selectedTls].Value = RowState.Modifed; }

                dataGridView1.Rows[selectedRowIndex].SetValues(id, book_name, author, count_of);

                if (int.TryParse(textBoxCount_of.Text, out int count) && selectedTls == 5)
                {
                    if (selectedTlsMenuStrip != 0) { dataGridView1.Rows[selectedRowIndex].SetValues(id, book_name, author, count_of, count, status); }
                    dataGridView1.Rows[selectedRowIndex].Cells[selectedTls].Value = RowState.Modifed;
                }
                if (int.TryParse(textBoxCount_of.Text, out count) == false && selectedTls == 4)
                {
                    MessageBox.Show("Количество должно иметь числовой формат!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void ReadingleRow(DataGridView dgw, IDataRecord record)
        {
            if (Convert.ToInt32(selectedTlsMenuStrip) == 0)
            {
                dgw.Rows.Add(record.GetInt32(0), record.GetString(1), record.GetString(2), record.GetInt32(3), RowState.ModifedNew);
            }
            if (Convert.ToInt32(selectedTlsMenuStrip) == 1)
            {
                dgw.Rows.Add(record.GetInt32(0), record.GetString(1), record.GetString(2), record.GetString(3), record.GetString(4), RowState.ModifedNew);
            }
        }

        private void CreateBtn_Click(object sender, EventArgs e)
        {
            Add_Form addfrm = new Add_Form();
            addfrm.Show();
        }

        private void RefreshDataGrid(DataGridView dgw)
        {
            dgw.Rows.Clear();

            string querystring = $"select * from " + useDB;

            SqlCommand com = new SqlCommand(querystring, database.GetConnection());

            database.OpenConnection();

            SqlDataReader reader = com.ExecuteReader();

            while (reader.Read())
            {
                ReadingleRow(dgw, reader);
            }
            reader.Close();
        }

        private void ClearFields()
        {
            textBoxId.Text = "";
            textBoxName.Text = "";
            textBoxAuthor.Text = "";
            textBoxCount_of.Text = "";
            textBoxStatus.Text = "";
        }

        private void btnRefresh_Click(DataGridView dgw)
        {
            RefreshDataGrid(dataGridView1);
            ClearFields();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            CreateColumns();
            btnRefresh_Click(dataGridView1);
        }

        private void btnDelStrClick_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void ChangeBookBtn_Click(object sender, EventArgs e)
        {
            Change();
            ClearFields();
        }

        private void DeleteRow()
        {
            int index = dataGridView1.CurrentCell.RowIndex;

            int selectedTls = 4;
            if (selectedTlsMenuStrip == 1) { selectedTls = 5; }

            dataGridView1.Rows[index].Visible = false;

            if (dataGridView1.Rows[index].Cells[0].Value.ToString() == string.Empty)
            {
                dataGridView1.Rows[index].Cells[selectedTls].Value = RowState.Deleted;
                return;
            }
            dataGridView1.Rows[index].Cells[selectedTls].Value = RowState.Deleted;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRow = e.RowIndex;
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[selectedRow];

                textBoxId.Text = row.Cells[0].Value.ToString();
                textBoxName.Text = row.Cells[1].Value.ToString();
                textBoxAuthor.Text = row.Cells[2].Value.ToString();
                textBoxCount_of.Text = row.Cells[3].Value.ToString();
                if (selectedTlsMenuStrip != 0)
                {
                    textBoxStatus.Text = row.Cells[4].Value.ToString();
                }
            }
        }

        private void Update()
        {
            database.OpenConnection();

            int selectedTls = 4;

            if (selectedTlsMenuStrip == 1) { selectedTls = 5; }

            for (int index = 0; index < dataGridView1.Rows.Count; index++)
            {
                var rowState = (RowState)dataGridView1.Rows[index].Cells[selectedTls].Value;

                if (rowState == RowState.Existed)
                    continue;

                if (rowState == RowState.Deleted)
                {
                    var id = Convert.ToInt32(dataGridView1.Rows[index].Cells[0].Value);

                    var deleteQuery = $"delete from " + useDB + $" where id = '{id}'";

                    var com = new SqlCommand(deleteQuery, database.GetConnection());
                    com.ExecuteNonQuery();
                }

                if (rowState == RowState.Modifed)
                {
                    var id = dataGridView1.Rows[index].Cells[0].Value.ToString();
                    var str2 = dataGridView1.Rows[index].Cells[1].Value.ToString();
                    var str3 = dataGridView1.Rows[index].Cells[2].Value.ToString();
                    var str4 = dataGridView1.Rows[index].Cells[3].Value.ToString();

                    var changeQuery = $"update books_db set book_name = '{str2}', author = '{str3}' count_of = '{id}'";

                    if (selectedTlsMenuStrip != 0)
                    {
                        var str5 = dataGridView1.Rows[index].Cells[4].Value.ToString();
                        if (selectedTlsMenuStrip == 0)
                            changeQuery = $"update client_db set client_name = '{str2}', book_name = '{str3}', date_time = '{str4}', status_book = '{str5}' where id = '{id}'";
                    }

                    var com = new SqlCommand(changeQuery, database.GetConnection());
                    com.ExecuteNonQuery();
                }
            }

            database.CloseConnection();
        }

        private void DelBookBtn_Click(object sender, EventArgs e)
        {
            DeleteRow();
            ClearFields();
        }

        private void SaveBookBtn_Click(object sender, EventArgs e)
        {
            Update();
            ClearFields();
        }

        private void книгиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.Columns.Clear();

            SelectedTls.selTls = 0;
            selectedTlsMenuStrip = 0;

            label3.Visible = true;
            label6.Visible = true;
            label5.Visible = true;
            label7.Visible = true;
            textBoxStatus.Visible = false;
            label8.Visible = false;
            label9.Visible = false;
            label10.Visible = false;
            label11.Visible = false;
            label3.Text = "Книги";
            label2.Text = "Книга:";

            useDB = "books_db";

            ClearFields();
            CreateColumns();
            RefreshDataGrid(dataGridView1);
        }

        private void посетителиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.Columns.Clear();

            SelectedTls.selTls = 1;
            selectedTlsMenuStrip = 1;

            label6.Visible = false;
            label5.Visible = false;
            label7.Visible = false;
            textBoxStatus.Visible = true;
            label8.Visible = true;
            label9.Visible = true;
            label10.Visible = true;
            label11.Visible = true;
            label3.Text = "Посетители";
            label2.Text = "Посетитель:";

            useDB = "client_db";

            ClearFields();
            CreateColumns();
            RefreshDataGrid(dataGridView1);
        }
        private void Search(DataGridView dgw)
        {
            dgw.Rows.Clear();

            string searcString = $"";

            if (Convert.ToInt32(selectedTlsMenuStrip) == 0)
            {
                searcString = $"select * from books_db where concat (book_name, author, count_of) like '%" + textBoxSearch.Text + "%'";
            }
            if (Convert.ToInt32(selectedTlsMenuStrip) == 1)
            {
                searcString = $"select * from client_db where concat (client_name, book_name, date_time, status_book) like '%" + textBoxSearch.Text + "%'";
            }

            SqlCommand com = new SqlCommand(searcString, database.GetConnection());

            database.OpenConnection();

            SqlDataReader read = com.ExecuteReader();

            while (read.Read())
            {
                ReadingleRow(dgw, read);
            }
            read.Close();
        }

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            Search(dataGridView1);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshDataGrid(dataGridView1);
            ClearFields();
        }
    }
}