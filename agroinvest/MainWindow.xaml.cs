using System;
using System.Windows;

namespace AgroInvestApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // БД отключена — данные не загружаем
            // LoadClients();
            // LoadTransport();
            // LoadOrders();

            // Можно вывести сообщение в заголовок окна
            this.Title = "Агроинвест (БД отключена)";
        }

        // ========== КЛИЕНТЫ ==========
        private void LoadClients()
        {
            // Закомментирован код работы с БД
            // try { ... } catch ...
            MessageBox.Show("База данных отключена. Функция недоступна.");
        }

        private void BtnAddClient_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("База данных отключена. Добавление невозможно.");
        }

        private void BtnEditClient_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("База данных отключена. Редактирование невозможно.");
        }

        private void BtnDeleteClient_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("База данных отключена. Удаление невозможно.");
        }

        private void BtnRefreshClients_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("База данных отключена. Обновление невозможно.");
        }

        // ========== ТРАНСПОРТ ==========
        private void LoadTransport()
        {
            MessageBox.Show("База данных отключена. Загрузка транспорта недоступна.");
        }

        private void BtnAddTransport_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("База данных отключена. Добавление невозможно.");
        }

        private void BtnEditTransport_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("База данных отключена. Редактирование невозможно.");
        }

        private void BtnDeleteTransport_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("База данных отключена. Удаление невозможно.");
        }

        private void BtnRefreshTransport_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("База данных отключена. Обновление невозможно.");
        }

        // ========== ЗАКАЗЫ ==========
        private void LoadOrders()
        {
            MessageBox.Show("База данных отключена. Загрузка заказов недоступна.");
        }

        private void BtnAddOrder_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("База данных отключена. Добавление невозможно.");
        }

        private void BtnEditOrder_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("База данных отключена. Редактирование невозможно.");
        }

        private void BtnDeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("База данных отключена. Удаление невозможно.");
        }

        private void BtnRefreshOrders_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("База данных отключена. Обновление невозможно.");
        }
    }
}