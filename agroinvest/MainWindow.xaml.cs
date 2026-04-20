using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace AgroInvestApp
{
    public partial class MainWindow : Window
    {
        private DataTable clientsTable;
        private DataTable transportTable;
        private DataTable ordersTable;

        public MainWindow()
        {
            InitializeComponent();
            // При необходимости раскомментировать для работы с БД
            // LoadClients();
            // LoadTransport();
            // LoadOrders();
        }

        // ========== КЛИЕНТЫ ==========
        private void LoadClients()
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Клиенты", conn);
                    clientsTable = new DataTable();
                    da.Fill(clientsTable);
                    dgClients.ItemsSource = clientsTable.DefaultView;
                }
            }
            catch (Exception ex) { MessageBox.Show("Ошибка загрузки клиентов: " + ex.Message); }
        }

        private void BtnAddClient_Click(object sender, RoutedEventArgs e)
        {
            var win = new EditClientWindow();
            if (win.ShowDialog() == true)
                LoadClients();
        }

        private void BtnEditClient_Click(object sender, RoutedEventArgs e)
        {
            if (dgClients.SelectedItem is DataRowView row)
            {
                int id = Convert.ToInt32(row["ID_клиента"]);
                var win = new EditClientWindow(id);
                if (win.ShowDialog() == true)
                    LoadClients();
            }
            else MessageBox.Show("Выберите клиента для редактирования");
        }

        private void BtnDeleteClient_Click(object sender, RoutedEventArgs e)
        {
            if (dgClients.SelectedItem is DataRowView row)
            {
                if (MessageBox.Show("Удалить клиента?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    int id = Convert.ToInt32(row["ID_клиента"]);
                    using (SqlConnection conn = DatabaseHelper.GetConnection())
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand("DELETE FROM Клиенты WHERE ID_клиента = @id", conn);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                    LoadClients();
                    LoadTransport();
                    LoadOrders();
                }
            }
            else MessageBox.Show("Выберите клиента для удаления");
        }

        private void BtnRefreshClients_Click(object sender, RoutedEventArgs e) => LoadClients();

        // ========== ТРАНСПОРТ ==========
        private void LoadTransport()
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT Т.*, К.Название AS Клиент 
                                     FROM Транспорт Т
                                     JOIN Клиенты К ON Т.ID_клиента = К.ID_клиента";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    transportTable = new DataTable();
                    da.Fill(transportTable);
                    dgTransport.ItemsSource = transportTable.DefaultView;
                }
            }
            catch (Exception ex) { MessageBox.Show("Ошибка загрузки транспорта: " + ex.Message); }
        }

        private void BtnAddTransport_Click(object sender, RoutedEventArgs e)
        {
            var win = new EditTransportWindow();
            if (win.ShowDialog() == true)
                LoadTransport();
        }

        private void BtnEditTransport_Click(object sender, RoutedEventArgs e)
        {
            if (dgTransport.SelectedItem is DataRowView row)
            {
                int id = Convert.ToInt32(row["ID_транспорта"]);
                var win = new EditTransportWindow(id);
                if (win.ShowDialog() == true)
                    LoadTransport();
            }
            else MessageBox.Show("Выберите транспортное средство");
        }

        private void BtnDeleteTransport_Click(object sender, RoutedEventArgs e)
        {
            if (dgTransport.SelectedItem is DataRowView row)
            {
                if (MessageBox.Show("Удалить транспорт?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    int id = Convert.ToInt32(row["ID_транспорта"]);
                    using (SqlConnection conn = DatabaseHelper.GetConnection())
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand("DELETE FROM Транспорт WHERE ID_транспорта = @id", conn);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                    LoadTransport();
                    LoadOrders();
                }
            }
            else MessageBox.Show("Выберите транспорт для удаления");
        }

        private void BtnRefreshTransport_Click(object sender, RoutedEventArgs e) => LoadTransport();

        // ========== ЗАКАЗЫ ==========
        private void LoadOrders()
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT З.*, К.Название AS Клиент, 
                                            Т.Марка + ' ' + Т.Модель + ' (' + Т.Госномер + ')' AS Транспорт
                                     FROM Заказы З
                                     JOIN Клиенты К ON З.ID_клиента = К.ID_клиента
                                     JOIN Транспорт Т ON З.ID_транспорта = Т.ID_транспорта
                                     ORDER BY З.Дата_заказа DESC";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    ordersTable = new DataTable();
                    da.Fill(ordersTable);
                    dgOrders.ItemsSource = ordersTable.DefaultView;
                }
            }
            catch (Exception ex) { MessageBox.Show("Ошибка загрузки заказов: " + ex.Message); }
        }

        private void BtnAddOrder_Click(object sender, RoutedEventArgs e)
        {
            var win = new EditOrderWindow();
            if (win.ShowDialog() == true)
                LoadOrders();
        }

        private void BtnEditOrder_Click(object sender, RoutedEventArgs e)
        {
            if (dgOrders.SelectedItem is DataRowView row)
            {
                int id = Convert.ToInt32(row["ID_заказа"]);
                var win = new EditOrderWindow(id);
                if (win.ShowDialog() == true)
                    LoadOrders();
            }
            else MessageBox.Show("Выберите заказ для редактирования");
        }

        private void BtnDeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            if (dgOrders.SelectedItem is DataRowView row)
            {
                if (MessageBox.Show("Удалить заказ?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    int id = Convert.ToInt32(row["ID_заказа"]);
                    using (SqlConnection conn = DatabaseHelper.GetConnection())
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand("DELETE FROM Заказы WHERE ID_заказа = @id", conn);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                    LoadOrders();
                }
            }
            else MessageBox.Show("Выберите заказ для удаления");
        }

        private void BtnRefreshOrders_Click(object sender, RoutedEventArgs e) => LoadOrders();
    }
}
