using JumperApp.Model;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Net.Mail;

namespace JumperApp
{
    /// <summary>
    /// Логика взаимодействия для AgentsPage.xaml
    /// </summary>
    public partial class AgentsList : Page
    {
        public string Path;
        public List<Agent> Agents;
        public List<string> Pages;
        public const int CountItemToPage = 10;

        public AgentsList()
        {
            InitializeComponent();
            Path = Directory.GetCurrentDirectory().Replace("\\bin\\Debug", ""); ;

            try
            {
                using (var context = new JumperEntities())
                {
                    Agents = context.Agent.Include(nameof(AgentType)).ToList();
                    AgentDataGrid.ItemsSource = Agents;
                    var agentType = context.AgentType.ToList();
                    agentType.Insert(0, new AgentType { Title = "Все типы" });
                    TypeAgentComboBox.ItemsSource = agentType;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            var sort = new[]
            {
                "Нет сортировки",
                "Возрастание по наименованию",
                "Убывание по наименованию",
                "Возрастание по размеру скидки",
                "Убывание по размеру скидки",
                "Возрастание по приоритету агента",
                "Убывание по приоритету агента",
            };
            SortComboBox.ItemsSource = sort;
            RefreshPage();
            ShowAgents();
        }

        private void RefreshListView()
        {
            try
            {
                using (var context = new JumperEntities())
                {
                    Agents = context.Agent.Include(nameof(AgentType)).ToList();

                    if (SearchTextBox.Text != "")
                    {
                        Agents = Agents.Where(x => x.Title.Contains(SearchTextBox.Text) || x.Email.Contains(SearchTextBox.Text) || x.Phone.Contains(SearchTextBox.Text)).ToList();
                    }

                    if (TypeAgentComboBox.SelectedIndex != 0)
                    {
                        Agents = Agents.Where(x => x.AgentType.Title == ((AgentType)TypeAgentComboBox.SelectedItem).Title).ToList();
                    }

                    switch (SortComboBox.SelectedIndex)
                    {
                        case 1:
                            Agents = Agents.OrderBy(x => x.Title).ToList();
                            break;
                        case 2:
                            Agents = Agents.OrderByDescending(x => x.Title).ToList();
                            break;
                        case 3:
                            Agents = Agents.OrderBy(x => x.CountSales).ToList();
                            break;
                        case 4:
                            Agents = Agents.OrderByDescending(x => x.CountSales).ToList();
                            break;
                        case 5:
                            Agents = Agents.OrderBy(x => x.Priority).ToList();
                            break;
                        case 6:
                            Agents = Agents.OrderByDescending(x => x.Priority).ToList();
                            break;
                    }

                    AgentDataGrid.ItemsSource = Agents;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            RefreshPage();
        }

        private void SortComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshListView();
        }

        private void TypeAgentComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshListView();
        }


        //https://learn.microsoft.com/en-us/dotnet/desktop/wpf/controls/how-to-add-a-watermark-to-a-textbox?view=netframeworkdesktop-4.8
        private void SearchTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchTextBox.Text == "")
            {
                var textImageBrush = new ImageBrush();
                textImageBrush.ImageSource =
                    new BitmapImage(
                        new Uri(Path + "//Image//Placeholder.png")
                    );
                textImageBrush.Stretch = Stretch.Uniform;
                textImageBrush.AlignmentX = AlignmentX.Left;
                SearchTextBox.Background = textImageBrush;
            }
            else
            {
                SearchTextBox.Background = null;
            }

            RefreshListView();
            ShowAgents();
        }

        private void PageListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowAgents();
        }

        private void ShowAgents()
        {
            if (PageListBox.SelectedIndex == 0)
            {
                AgentDataGrid.ItemsSource = Agents.Take(CountItemToPage).ToList();
            }
            else if (PageListBox.SelectedIndex == PageListBox.Items.Count - 1)
            {
                AgentDataGrid.ItemsSource = Agents.Skip((PageListBox.SelectedIndex - 2) * CountItemToPage).Take(CountItemToPage).ToList();
            }
            else
            {
                AgentDataGrid.ItemsSource = Agents.Skip((PageListBox.SelectedIndex - 1) * CountItemToPage).Take(CountItemToPage).ToList();
            }

            AgentScrollViewer.ScrollToHome();
        }

        private void RefreshPage()
        {
            Pages = new List<string>();

            if (Agents.Count > CountItemToPage)
            {
                Pages.Add("<");
                var countPage = (Agents.Count + CountItemToPage - 1) / CountItemToPage;
                for (var i = 1; i <= countPage; i++)
                {
                    Pages.Add(i.ToString());
                }

                Pages.Add(">");
            }

            PageListBox.ItemsSource = Pages;
            PageListBox.SelectedIndex = 1;
        }

        private void AgentDataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AgentDataGrid.SelectedItems.Count > 1)
            {
                PriorityButton.Visibility = Visibility.Visible;
            }
            if (AgentDataGrid.SelectedItems.Count <= 1)
            {
                PriorityButton.Visibility = Visibility.Hidden;
            }
        }

        private void PriorityButton_OnClick(object sender, RoutedEventArgs e)
        {
            var page = PageListBox.SelectedIndex;
            var maxPriority = AgentDataGrid.SelectedItems.Cast<Agent>().Max(x => x.Priority);
            var priorityChangeWindow = new Window.PriorityChangeWindow(maxPriority);
            priorityChangeWindow.ShowDialog();
            if (priorityChangeWindow.DialogResult == true) { }
            {
                int.TryParse(priorityChangeWindow.PriorityTextBox.Text, out var newPriority);

                try
                {
                    using (var context = new JumperEntities())
                    {
                        foreach (var agent in AgentDataGrid.SelectedItems.Cast<Agent>())
                        {
                            var tempAgent = context.Agent.First(x => x.ID == agent.ID);
                            tempAgent.Priority = newPriority;
                        }

                        context.SaveChanges();
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine(e);
                    throw;
                }

                RefreshListView();
                PageListBox.SelectedIndex = page;
            }
        }

        private void AddAgentButton_OnClick(object sender, RoutedEventArgs e)
        {
            var addAgentWindow = new Window.AddAgentWindow();
            addAgentWindow.ShowDialog();
            if (addAgentWindow.DialogResult == true)
            {

            }
        }

        private void AgentDataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}