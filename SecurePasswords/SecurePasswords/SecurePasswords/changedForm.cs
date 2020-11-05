using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace SecurePasswords
{
    public partial class changedForm : Form
    {
        public changedForm()
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

        SqlConnection sqlConnection;
        string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Tr3be\source\repos\SecurePasswords\SecurePasswords\Database.mdf;Integrated Security=True";


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

        private void changedForm_Deactivate(object sender, EventArgs e)
        {
            Animation("EXIT", 5);
        }

        private void changedForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics graph = e.Graphics;
            graph.FillRectangle(new SolidBrush(Color.MediumSlateBlue), new Rectangle(0, 0, Width, 30));
            graph.DrawString(Text, Font, new SolidBrush(Color.White), new Rectangle(30, 5, 200, 20));
            graph.DrawRectangle(new Pen(Color.MediumSlateBlue, 2), new Rectangle(1, 1, Width - 2, Height - 2));
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Animation("EXIT", 5);
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            pictureBox1.BackColor = Color.Crimson;
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            pictureBox1.BackColor = Color.MediumSlateBlue;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawLine(new Pen(Color.White, 1.5f), new Point(9, 9), new Point(21, 21));
            e.Graphics.DrawLine(new Pen(Color.White, 1.5f), new Point(21, 9), new Point(9, 21));
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            Animation("HIDE", 5);
        }

        private void pictureBox3_MouseEnter(object sender, EventArgs e)
        {
            pictureBox3.BackColor = Color.RoyalBlue;
        }

        private void pictureBox3_MouseLeave(object sender, EventArgs e)
        {
            pictureBox3.BackColor = Color.MediumSlateBlue;
        }

        private void pictureBox3_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawLine(new Pen(Color.White, 1.5f), new Point(9, 21), new Point(21, 21));
        }



        private async void Create_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrWhiteSpace(textBox1.Text))
            {


                sqlConnection = new SqlConnection(connectionString);

                await sqlConnection.OpenAsync();


                SqlCommand command = new SqlCommand("UPDATE [Resource] SET [Ресурс]=@Ресурс WHERE [Идентификатор_ресурса]=@Идентификатор_ресурса", sqlConnection);

                command.Parameters.AddWithValue("Идентификатор_ресурса", Properties.Settings.Default.id_resource);
                command.Parameters.AddWithValue("Ресурс", textBox1.Text);

                await command.ExecuteNonQueryAsync();



                



                label1.Text = "Successfully";

                Timer T = new Timer() { Interval = 300 };

                int count = 0;
                T.Tick += delegate (Object st, EventArgs x)
                {
                    count++;
                    if (count == 1)
                    {
                        T.Stop();
                        Animation("EXIT", 5);
                    }
                };
                T.Start();
            }
            else { label1.Text = "Enter the title"; }
            

        }

    }
}
