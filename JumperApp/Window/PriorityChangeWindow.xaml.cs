using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using JumperApp.Model;

namespace JumperApp.Window
{
    /// <summary>
    /// Логика взаимодействия для PriorityChangeWindow.xaml
    /// </summary>
    public partial class PriorityChangeWindow : System.Windows.Window
    {
        public PriorityChangeWindow(int maxPriority)
        {
            InitializeComponent();
            PriorityTextBox.Text = maxPriority.ToString();
        }

        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(PriorityTextBox.Text, out var newPriority) || newPriority < 0)
            {
                MessageBox.Show("Текстовое поле может содержать только целые числа равные и больше 0", "Предупреждение от Попрыжёнка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                DialogResult = true;
            }


        }
    }
}
