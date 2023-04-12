using JumperApp.Model;
using System;
using System.Linq;
using System.Text;
using System.Windows;

namespace JumperApp.Window
{
    /// <summary>
    /// Логика взаимодействия для AddAgentWindow.xaml
    /// </summary>
    public partial class AddAgentWindow : System.Windows.Window
    {
        public AddAgentWindow()
        {
            InitializeComponent();
            try
            {
                using (var context = new JumperEntities())
                {
                    AgentTypeComboBox.ItemsSource = context.AgentType.ToList();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            var errorMessage = new StringBuilder();
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                errorMessage.AppendLine("Укажите наименование");
            }
            if (AgentTypeComboBox.SelectedIndex == -1)
            {
                errorMessage.AppendLine("Выберите тип агента");
            }
            if (string.IsNullOrWhiteSpace(PriorityTextBox.Text))
            {
                errorMessage.AppendLine("Укажите приоритет");
            }
            else if (!int.TryParse(PriorityTextBox.Text, out var priority) || priority < 0)
            {
                errorMessage.AppendLine("Приоритет должен быть положительным целым числом");
            }
            if (string.IsNullOrWhiteSpace(AddressTextBox.Text))
            {
                errorMessage.AppendLine("Укажите адрес");
            }
            if (string.IsNullOrWhiteSpace(INNTextBox.Text))
            {
                errorMessage.AppendLine("Укажите ИНН");
            }
            else if (!int.TryParse(INNTextBox.Text, out var inn) || inn < 0)
            {
                errorMessage.AppendLine("ИНН должен состоять только из цифр больше 0");
            }
            if (string.IsNullOrWhiteSpace(KPPTextBox.Text))
            {
                errorMessage.AppendLine("Укажите наименование");
            }
            else if (!int.TryParse(KPPTextBox.Text, out var kpp) || kpp < 0)
            {
                errorMessage.AppendLine("КПП должен состоять только из цифр больше 0");
            }
            if (string.IsNullOrWhiteSpace(NameDirectorTextBox.Text))
            {
                errorMessage.AppendLine("Укажите имя директора");
            }
            if (string.IsNullOrWhiteSpace(PhoneTextBox.Text))
            {
                errorMessage.AppendLine("Укажите номер телефона");
            }
            if (string.IsNullOrWhiteSpace(NameDirectorTextBox.Text))
            {
                errorMessage.AppendLine("Укажите E-mail компании");
            }
        }
    }
}
