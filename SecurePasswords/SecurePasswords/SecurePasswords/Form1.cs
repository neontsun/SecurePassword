using System;
using System.Drawing;
using System.Data.SqlClient;
using System.Windows.Forms;
using Placeholder;

namespace SecurePasswords
{
    public partial class Form1 : Form
    {

        #region Блок объявления переменных


        //пароль для входа
        private string VerificationPassword = Properties.Settings.Default.password;

        //Кол-во попыток входа
        private int charCount;

        //Массив картинок на главном экране
        private Image[] slideshow =
        {
            Properties.Resources.slide_picture_one,
            Properties.Resources.slide_picture_two,
            Properties.Resources.slide_picture_three,
            Properties.Resources.slide_picture_four
        };

        int i;

        Random rand = new Random();

        SqlConnection sqlConnection;
        string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Tr3be\source\repos\SecurePasswords\SecurePasswords\Database.mdf;Integrated Security=True";

        

        #endregion

        public Form1()
        {

            InitializeComponent();

            //Перемещение формы
            Boolean isDragging = false;
            Point lastCursor = new Point(0, 0), lastForm = new Point(0, 0);

            MouseDown += delegate (Object sender, MouseEventArgs e)
            {
                isDragging = true;
                lastCursor = Cursor.Position;
                lastForm = Location;
            };
            MouseMove += delegate (Object sender, MouseEventArgs e)
            {
                if (isDragging)
                {
                    Location = Point.Add(lastForm, new Size(Point.Subtract(Cursor.Position, new Size(lastCursor))));
                }
            };
            MouseUp += delegate (Object sender, MouseEventArgs e)
            {
                isDragging = false;
            };
        }


        // Загрузка формы
        private async void Form1_Load(object sender, EventArgs e)
        {

            if (VerificationPassword == "")
            {
                checkPassword.Visible = false;
                checkPassword.Location = new Point(279, 394);

                login.Location = new Point(279, 341);
                checkPassword.Text = "";

            }
            else
            {
                checkPassword.Visible = true;
                checkPassword.Location = new Point(279, 341);

                login.Location = new Point(279, 394);

                checkPassword.addPlaceholder("Pass");
            }

            welcomePanel.Location = new Point(2, 30);

            pictureBox2.Location = new Point(0, 0);

            pictureBox2.Image = slideshow[i = rand.Next(0, 4)];


            #region База данных



            sqlConnection = new SqlConnection(connectionString);

            await sqlConnection.OpenAsync();

            SqlDataReader sqlReader = null;

            SqlCommand command = new SqlCommand("SELECT * FROM [Resource]", sqlConnection);


            try
            {
                sqlReader = await command.ExecuteReaderAsync();

                Properties.Settings.Default.date_count = 0;
                Properties.Settings.Default.Save();

                while (await sqlReader.ReadAsync())
                {
                    listBox1.Items.Add(Convert.ToString(Convert.ToString(sqlReader["Ресурс"])));

                    Properties.Settings.Default.date_count += 1;
                    Properties.Settings.Default.Save();
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("Message", ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (sqlReader != null) { sqlReader.Close(); }
            }



            #endregion
        }


        //До закрытия формы
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (sqlConnection != null && sqlConnection.State != System.Data.ConnectionState.Closed)
            {
                sqlConnection.Close();
            }
        }


        //До закрытия формы
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (sqlConnection != null && sqlConnection.State != System.Data.ConnectionState.Closed)
            {
                sqlConnection.Close();
            }
        }


        //Логин клик
        private void login_Click(object sender, EventArgs e)
        {
            if (checkPassword.Text == VerificationPassword)
            {
                pictureBox2.Visible = false;
                welcomePanel.Visible = false;
            }
            else
            {
                MessageBox.Show("Неверный пароль");
                charCount++;
                checkPassword.Text = "";

                if (charCount == 4)
                {
                    MessageBox.Show("Вы израсходовали попытки");
                    Application.Exit();
                }
            }



            
        }


        //Настройки меню ToolStrip - Настройки
        private void настройкиToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (VerificationPassword == "")
            {
                oldPassword.Visible = false;
                oldPassword.Text = "";
            }
            else
            {
                oldPassword.addPlaceholder("Old pass");
            }

            settingPanel.Visible = true;
            settingPanel.Location = new Point(9, 35);

            newPassword.addPlaceholder("New pass");
            confirm_newPassword.addPlaceholder("Confirm pass");
        }


        //Изменение пароля
        private void changedPassword_Click(object sender, EventArgs e)
        {
            if (oldPassword.Text != VerificationPassword)
            {
                hint.Text = "Неверный пароль"; hint.Visible = true;
            }
            else
            {

                if (newPassword.Text == "")
                {
                    hint.Text = "Введите новый пароль"; hint.Visible = true;
                }
                else
                {
                    if (confirm_newPassword.Text == "")
                    {
                        hint.Text = "Подтвердите пароль";
                        hint.Visible = true;
                    }
                    else
                    {
                        Properties.Settings.Default.password = newPassword.Text;
                        Properties.Settings.Default.Save();
                        hint.Text = "Пароль успешно сохранен";
                        hint.Visible = true;
                    }
                }
            }
        }


        //Кнопка назад
        private void settingPanel_back_Click(object sender, EventArgs e)
        {
            settingPanel.Visible = false;
            hint.Text = "";
        }


        //Сброс пароля
        private void resetPassword_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.password == null)
            {
                hint.Text = "Пароль уже сброшен";
                hint.Visible = true;
            }
            else
            {
                Properties.Settings.Default.password = null;
                Properties.Settings.Default.Save();
                hint.Text = "Пароль успешно сброшен";
                hint.Visible = true;
            }
        }


        //Настройки меню ToolStrip - О программе
        private void оПрограммеToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Программа для удобного и безопасного хранения паролей.");
        }


        //Настройки меню ToolStrip - Версия
        private void версияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Текущая версия: 1.1");
        }


        //Обнуление подсказки на панели настроек
        private void newPassword_Enter(object sender, EventArgs e)
        {
            hint.Text = "";
        }


        //Обнуление подсказки на панели настроек
        private void confirm_newPassword_Enter(object sender, EventArgs e)
        {
            hint.Text = "";
        }


        //Анимация
        private void Animation(String Mode, Int32 Duration)
        {
            Timer T = new Timer { Interval = Duration };
            T.Tick += delegate (Object sender, EventArgs e)
            {
                switch (Mode)
                {
                    case "SHOW":
                        if (Opacity != 1) { Opacity += 0.1; } else { T.Stop(); }
                        break;

                    case "HIDE":
                        if (Opacity > 0) { Opacity -= 0.1; } else { T.Stop(); WindowState = FormWindowState.Minimized; }
                        break;

                    case "EXIT":
                        if (Opacity > 0) { Opacity -= 0.1; } else { Close(); }
                        break;

                    default: break;
                }
            };

            T.Start();
        }


        //Анимация при активации формы
        private void Form1_Activated(object sender, EventArgs e)
        {
            Animation("SHOW", 5);
        }


        //Перерисовка формы
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics graph = e.Graphics;
            graph.FillRectangle(new SolidBrush(Color.MediumSlateBlue), new Rectangle(0, 0, Width, 30));
            graph.DrawString(Text, Font, new SolidBrush(Color.White), new Rectangle(30, 5, 200, 20));
            graph.DrawRectangle(new Pen(Color.MediumSlateBlue, 2), new Rectangle(1, 1, Width - 2, Height - 2));
        }


        #region Кнопки ЗАКРЫТЬ и СКРЫТЬ




        //Кнопка - Закрыть - при клике
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Animation("EXIT", 5);


            if (sqlConnection != null && sqlConnection.State != System.Data.ConnectionState.Closed)
            {
                sqlConnection.Close();
            }
        }

        //Кнопка - Закрыть - при наведение
        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            pictureBox1.BackColor = Color.Crimson;
        }


        //Кнопка - Закрыть - при снятии наведения 
        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            pictureBox1.BackColor = Color.MediumSlateBlue;
        }


        //Кнопка - Закрыть - рисовка
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawLine(new Pen(Color.White, 1.5f), new Point(9, 9), new Point(21, 21));
            e.Graphics.DrawLine(new Pen(Color.White, 1.5f), new Point(21, 9), new Point(9, 21));
        }


        //Кнопка - Скрыть - при клике
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            Animation("HIDE", 5);
        }


        //Кнопка - Скрыть - при наведение
        private void pictureBox3_MouseEnter(object sender, EventArgs e)
        {
            pictureBox3.BackColor = Color.RoyalBlue;
        }


        //Кнопка - Скрыть - при снятии наведения
        private void pictureBox3_MouseLeave(object sender, EventArgs e)
        {
            pictureBox3.BackColor = Color.MediumSlateBlue;
        }


        //Кнопка - Скрыть - рисовка
        private void pictureBox3_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawLine(new Pen(Color.White, 1.5f), new Point(9, 21), new Point(21, 21));
        }



        #endregion



        #region Кнопки при наведении



        //Кнопка - Логин - при наведении
        private void login_MouseEnter(object sender, EventArgs e)
        {
            login.ForeColor = Color.White;
        }

        private void login_MouseLeave(object sender, EventArgs e)
        {
            login.ForeColor = Color.Black;
        }


        //Кнопка - Добавить ресурс - при наведении
        private void addResourse_MouseEnter(object sender, EventArgs e)
        {
            addResourse.ForeColor = Color.White;
        }

        private void addResourse_MouseLeave(object sender, EventArgs e)
        {
            addResourse.ForeColor = Color.Black;
        }


        //Кнопка - Удалить ресурс - при наведении
        private void deleteResourse_MouseEnter(object sender, EventArgs e)
        {
            deleteResourse.ForeColor = Color.White;
        }

        private void deleteResourse_MouseLeave(object sender, EventArgs e)
        {
            deleteResourse.ForeColor = Color.Black;
        }


        //Кнопка - Добавить логин и пароль - при наведении
        private void addLoginOrPassword_MouseEnter(object sender, EventArgs e)
        {
            addLoginOrPassword.ForeColor = Color.White;
        }

        private void addLoginOrPassword_MouseLeave(object sender, EventArgs e)
        {
            addLoginOrPassword.ForeColor = Color.Black;
        }


        //Кнопка - Удалить логин и пароль - при наведении
        private void deleteLoginOrPassword_MouseEnter(object sender, EventArgs e)
        {
            deleteLoginOrPassword.ForeColor = Color.White;
        }

        private void deleteLoginOrPassword_MouseLeave(object sender, EventArgs e)
        {
            deleteLoginOrPassword.ForeColor = Color.Black;
        }


        //Кнопка - Назад - при наведении
        private void settingPanel_back_MouseEnter(object sender, EventArgs e)
        {
            settingPanel_back.ForeColor = Color.White;
        }

        private void settingPanel_back_MouseLeave(object sender, EventArgs e)
        {
            settingPanel_back.ForeColor = Color.Black;
        }


        //Кнопка - Изменить пароль - при наведении
        private void changedPassword_MouseEnter(object sender, EventArgs e)
        {
            changedPassword.ForeColor = Color.White;
        }

        private void changedPassword_MouseLeave(object sender, EventArgs e)
        {
            changedPassword.ForeColor = Color.Black;
        }


        //Кнопка - Сбросить пароль - при наведении
        private void resetPassword_MouseEnter(object sender, EventArgs e)
        {
            resetPassword.ForeColor = Color.White;
        }

        private void resetPassword_MouseLeave(object sender, EventArgs e)
        {
            resetPassword.ForeColor = Color.Black;
        }



        #endregion


        //Кнопка - добавить ресурс - клик
        private void addResourse_Click(object sender, EventArgs e)
        {

            Form aRF = new addResourceForm();
            aRF.Show();
        }

        // Обновление таблицы Resource
        private async void update_resource_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();

            SqlDataReader sqlReader = null;

            SqlCommand command = new SqlCommand("SELECT * FROM [Resource]", sqlConnection);

            Properties.Settings.Default.date_count = 0;
            Properties.Settings.Default.Save();

            try
            {
                sqlReader = await command.ExecuteReaderAsync();

                while (await sqlReader.ReadAsync())
                {
                    listBox1.Items.Add(Convert.ToString(Convert.ToString(sqlReader["Ресурс"])));

                    Properties.Settings.Default.date_count += 1;
                    Properties.Settings.Default.Save();
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("Message", ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (sqlReader != null) { sqlReader.Close(); }
            }
        }



        private async void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                listBox1.SelectedIndex = (e.Y / listBox1.ItemHeight <= listBox1.Items.Count) ? (e.Y / listBox1.ItemHeight) : listBox1.SelectedIndex;

            }
            if (e.Button == MouseButtons.Left)
            {
                Properties.Settings.Default.click = true;
                Properties.Settings.Default.Save();


                SqlDataReader sqlReader = null;


                listBox1.SelectedIndex = (e.Y / listBox1.ItemHeight <= listBox1.Items.Count) ? (e.Y / listBox1.ItemHeight) : listBox1.SelectedIndex;
                Properties.Settings.Default.item_lb_name = listBox1.Items[listBox1.SelectedIndex].ToString();
                Properties.Settings.Default.Save();

                SqlCommand command = new SqlCommand("SELECT Идентификатор_ресурса FROM [Resource] WHERE [Ресурс]=@Ресурс", sqlConnection);

                command.Parameters.AddWithValue("Ресурс", Properties.Settings.Default.item_lb_name);

                sqlReader = await command.ExecuteReaderAsync();

                await sqlReader.ReadAsync();

                Properties.Settings.Default.id_resource = Convert.ToString(sqlReader["Идентификатор_ресурса"]);
                Properties.Settings.Default.Save();

                sqlReader.Close();


            }
            else
            {
                Properties.Settings.Default.click = false;
                Properties.Settings.Default.Save();
            }

        }
 

        private void contextMenuStrip1_Click(object sender, EventArgs e)
        {
            Form changedForm = new changedForm();
            changedForm.Show();


            Properties.Settings.Default.item_lb_index = listBox1.SelectedIndex;
            Properties.Settings.Default.Save();
            Properties.Settings.Default.item_lb_name = listBox1.Items[Properties.Settings.Default.item_lb_index].ToString();
            Properties.Settings.Default.Save();

        }


        //Удалить ресурс
        private async void deleteResourse_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.click)
            {
                SqlCommand command = new SqlCommand("DELETE FROM [Resource] WHERE [Идентификатор_ресурса]=@Идентификатор_ресурса", sqlConnection);

                command.Parameters.AddWithValue("Идентификатор_ресурса", Properties.Settings.Default.id_resource);

                await command.ExecuteNonQueryAsync();

            }
            else
            {
                MessageBox.Show("Select a resource");
            }
        }
    }
}
