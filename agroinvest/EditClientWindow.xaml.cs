using System;
using System.Data.SqlClient;
using System.Windows;

namespace AgroInvestApp
{
    public partial class EditClientWindow : Window
    {
        private int? clientId;

        public EditClientWindow(int? id = null)
        {
            InitializeComponent();
            clientId = id;
            if (id.HasValue)
                LoadClientData();
        }

        private void LoadClientData()
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Клиенты WHERE ID_клиента = @id", conn);
                cmd.Parameters.AddWithValue("@id", clientId.Value);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    txtName.Text = reader["Название"].ToString();
                    txtINN.Text = reader["ИНН"].ToString();
                    txtPhone.Text = reader["Телефон"].ToString();
                    txtEmail.Text = reader["Email"].ToString();
                    txtCity.Text = reader["Город"].ToString();
                    txtAddress.Text = reader["Адрес"].ToString();
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название клиента");
                return;
            }
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                SqlCommand cmd;
                if (clientId.HasValue)
                {
                    cmd = new SqlCommand(@"UPDATE Клиенты SET Название=@name, ИНН=@inn, Телефон=@phone,
                                           Email=@email, Город=@city, Адрес=@address WHERE ID_клиента=@id", conn);
                    cmd.Parameters.AddWithValue("@id", clientId.Value);
                }
                else
                {
                    cmd = new SqlCommand(@"INSERT INTO Клиенты (Название,ИНН,Телефон,Email,Город,Адрес)
                                           VALUES (@name,@inn,@phone,@email,@city,@address)", conn);
                }
                cmd.Parameters.AddWithValue("@name", txtName.Text);
                cmd.Parameters.AddWithValue("@inn", string.IsNullOrEmpty(txtINN.Text) ? (object)DBNull.Value : txtINN.Text);
                cmd.Parameters.AddWithValue("@phone", txtPhone.Text);
                cmd.Parameters.AddWithValue("@email", string.IsNullOrEmpty(txtEmail.Text) ? (object)DBNull.Value : txtEmail.Text);
                cmd.Parameters.AddWithValue("@city", txtCity.Text);
                cmd.Parameters.AddWithValue("@address", string.IsNullOrEmpty(txtAddress.Text) ? (object)DBNull.Value : txtAddress.Text);
                cmd.ExecuteNonQuery();
            }
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}