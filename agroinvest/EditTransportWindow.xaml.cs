using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace AgroInvestApp
{
    public partial class EditTransportWindow : Window
    {
        private int? transportId;
        private DataTable clientsTable;

        public EditTransportWindow(int? id = null)
        {
            InitializeComponent();
            transportId = id;
            LoadClients();
            if (id.HasValue)
                LoadTransportData();
        }

        private void LoadClients()
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter("SELECT ID_клиента, Название FROM Клиенты", conn);
                clientsTable = new DataTable();
                da.Fill(clientsTable);
                cmbClient.ItemsSource = clientsTable.DefaultView;
                cmbClient.DisplayMemberPath = "Название";
                cmbClient.SelectedValuePath = "ID_клиента";
            }
        }

        private void LoadTransportData()
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Транспорт WHERE ID_транспорта = @id", conn);
                cmd.Parameters.AddWithValue("@id", transportId.Value);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    cmbClient.SelectedValue = reader["ID_клиента"];
                    txtRegNumber.Text = reader["Госномер"].ToString();
                    txtBrand.Text = reader["Марка"].ToString();
                    txtModel.Text = reader["Модель"].ToString();

                    string type = reader["Тип"].ToString();
                    foreach (ComboBoxItem item in cmbType.Items)
                    {
                        if (item.Content.ToString() == type)
                        {
                            cmbType.SelectedItem = item;
                            break;
                        }
                    }
                    if (cmbType.SelectedItem == null && !string.IsNullOrEmpty(type))
                        cmbType.Text = type;

                    txtYear.Text = reader["Год_выпуска"].ToString();
                    txtVin.Text = reader["VIN"].ToString();
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cmbClient.SelectedValue == null)
            {
                MessageBox.Show("Выберите клиента");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtRegNumber.Text))
            {
                MessageBox.Show("Введите госномер");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtBrand.Text))
            {
                MessageBox.Show("Введите марку");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtModel.Text))
            {
                MessageBox.Show("Введите модель");
                return;
            }

            string typeValue = (cmbType.SelectedItem as ComboBoxItem)?.Content.ToString() ?? cmbType.Text;
            if (string.IsNullOrWhiteSpace(typeValue))
            {
                MessageBox.Show("Выберите или введите тип");
                return;
            }

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                SqlCommand cmd;
                if (transportId.HasValue)
                {
                    cmd = new SqlCommand(@"UPDATE Транспорт 
                                           SET ID_клиента = @clientId, Госномер = @reg, Марка = @brand, 
                                               Модель = @model, Тип = @type, Год_выпуска = @year, VIN = @vin
                                           WHERE ID_транспорта = @id", conn);
                    cmd.Parameters.AddWithValue("@id", transportId.Value);
                }
                else
                {
                    cmd = new SqlCommand(@"INSERT INTO Транспорт (ID_клиента, Госномер, Марка, Модель, Тип, Год_выпуска, VIN)
                                           VALUES (@clientId, @reg, @brand, @model, @type, @year, @vin)", conn);
                }
                cmd.Parameters.AddWithValue("@clientId", cmbClient.SelectedValue);
                cmd.Parameters.AddWithValue("@reg", txtRegNumber.Text);
                cmd.Parameters.AddWithValue("@brand", txtBrand.Text);
                cmd.Parameters.AddWithValue("@model", txtModel.Text);
                cmd.Parameters.AddWithValue("@type", typeValue);
                cmd.Parameters.AddWithValue("@year", string.IsNullOrEmpty(txtYear.Text) ? (object)DBNull.Value : Convert.ToInt32(txtYear.Text));
                cmd.Parameters.AddWithValue("@vin", string.IsNullOrEmpty(txtVin.Text) ? (object)DBNull.Value : txtVin.Text);
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