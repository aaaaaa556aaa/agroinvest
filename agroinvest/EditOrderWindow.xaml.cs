using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace AgroInvestApp
{
    public partial class EditOrderWindow : Window
    {
        private int? orderId;
        private DataTable clientsTable;
        private DataTable transportTable;

        public EditOrderWindow(int? id = null)
        {
            InitializeComponent();
            orderId = id;
            dpOrderDate.SelectedDate = DateTime.Now;
            LoadClients();
            if (id.HasValue)
                LoadOrderData();
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

        private void LoadTransportForClient(int clientId)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT ID_транспорта, Марка + ' ' + Модель + ' (' + Госномер + ')' AS DisplayName FROM Транспорт WHERE ID_клиента = @clientId", conn);
                da.SelectCommand.Parameters.AddWithValue("@clientId", clientId);
                transportTable = new DataTable();
                da.Fill(transportTable);
                cmbTransport.ItemsSource = transportTable.DefaultView;
                cmbTransport.DisplayMemberPath = "DisplayName";
                cmbTransport.SelectedValuePath = "ID_транспорта";
            }
        }

        private void CmbClient_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cmbClient.SelectedValue != null)
            {
                int clientId = (int)cmbClient.SelectedValue;
                LoadTransportForClient(clientId);
            }
            else
            {
                cmbTransport.ItemsSource = null;
            }
        }

        private void LoadOrderData()
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Заказы WHERE ID_заказа = @id", conn);
                cmd.Parameters.AddWithValue("@id", orderId.Value);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    int clientId = Convert.ToInt32(reader["ID_клиента"]);
                    cmbClient.SelectedValue = clientId;
                    LoadTransportForClient(clientId);
                    cmbTransport.SelectedValue = reader["ID_транспорта"];
                    dpOrderDate.SelectedDate = Convert.ToDateTime(reader["Дата_заказа"]);

                    string workType = reader["Тип_работы"].ToString();
                    foreach (ComboBoxItem item in cmbWorkType.Items)
                    {
                        if (item.Content.ToString() == workType)
                        {
                            cmbWorkType.SelectedItem = item;
                            break;
                        }
                    }
                    if (cmbWorkType.SelectedItem == null && !string.IsNullOrEmpty(workType))
                        cmbWorkType.Text = workType;

                    string status = reader["Статус"].ToString();
                    foreach (ComboBoxItem item in cmbStatus.Items)
                    {
                        if (item.Content.ToString() == status)
                        {
                            cmbStatus.SelectedItem = item;
                            break;
                        }
                    }
                    txtCost.Text = reader["Стоимость"].ToString();
                    bool installment = Convert.ToBoolean(reader["Рассрочка"]);
                    chkInstallment.IsChecked = installment;
                    txtInstallmentMonths.IsEnabled = installment;
                    if (installment && reader["Срок_рассрочки_мес"] != DBNull.Value)
                        txtInstallmentMonths.Text = reader["Срок_рассрочки_мес"].ToString();
                    txtComment.Text = reader["Комментарий"].ToString();
                }
            }
        }

        private void ChkInstallment_CheckedChanged(object sender, RoutedEventArgs e)
        {
            bool isChecked = chkInstallment.IsChecked == true;
            txtInstallmentMonths.IsEnabled = isChecked;
            if (!isChecked)
                txtInstallmentMonths.Text = "";
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cmbClient.SelectedValue == null)
            {
                MessageBox.Show("Выберите клиента");
                return;
            }
            if (cmbTransport.SelectedValue == null)
            {
                MessageBox.Show("Выберите транспорт");
                return;
            }
            if (dpOrderDate.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату заказа");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtCost.Text))
            {
                MessageBox.Show("Введите стоимость");
                return;
            }
            if (!decimal.TryParse(txtCost.Text, out decimal cost))
            {
                MessageBox.Show("Стоимость должна быть числом");
                return;
            }

            bool installment = chkInstallment.IsChecked == true;
            int? installmentMonths = null;
            if (installment)
            {
                if (string.IsNullOrWhiteSpace(txtInstallmentMonths.Text))
                {
                    MessageBox.Show("Укажите срок рассрочки в месяцах");
                    return;
                }
                if (!int.TryParse(txtInstallmentMonths.Text, out int months) || months <= 0)
                {
                    MessageBox.Show("Срок рассрочки должен быть положительным целым числом");
                    return;
                }
                installmentMonths = months;
            }

            string workTypeValue = (cmbWorkType.SelectedItem as ComboBoxItem)?.Content.ToString() ?? cmbWorkType.Text;
            string statusValue = (cmbStatus.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Новый";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                SqlCommand cmd;
                if (orderId.HasValue)
                {
                    cmd = new SqlCommand(@"UPDATE Заказы 
                                           SET ID_клиента = @clientId, ID_транспорта = @transportId, Дата_заказа = @date,
                                               Тип_работы = @workType, Статус = @status, Стоимость = @cost,
                                               Рассрочка = @installment, Срок_рассрочки_мес = @installmentMonths, Комментарий = @comment
                                           WHERE ID_заказа = @id", conn);
                    cmd.Parameters.AddWithValue("@id", orderId.Value);
                }
                else
                {
                    cmd = new SqlCommand(@"INSERT INTO Заказы (ID_клиента, ID_транспорта, Дата_заказа, Тип_работы, Статус, Стоимость, Рассрочка, Срок_рассрочки_мес, Комментарий)
                                           VALUES (@clientId, @transportId, @date, @workType, @status, @cost, @installment, @installmentMonths, @comment)", conn);
                }
                cmd.Parameters.AddWithValue("@clientId", cmbClient.SelectedValue);
                cmd.Parameters.AddWithValue("@transportId", cmbTransport.SelectedValue);
                cmd.Parameters.AddWithValue("@date", dpOrderDate.SelectedDate.Value);
                cmd.Parameters.AddWithValue("@workType", workTypeValue);
                cmd.Parameters.AddWithValue("@status", statusValue);
                cmd.Parameters.AddWithValue("@cost", cost);
                cmd.Parameters.AddWithValue("@installment", installment);
                cmd.Parameters.AddWithValue("@installmentMonths", installmentMonths.HasValue ? (object)installmentMonths.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@comment", string.IsNullOrEmpty(txtComment.Text) ? (object)DBNull.Value : txtComment.Text);
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