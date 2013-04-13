using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ClientApplication.Properties;

namespace ClientApplication.View
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            var dpText = TextBox.TextProperty;
            updateBindingSources(this, dpText);

            Settings.Default.Save();

            MessageBox.Show("If no input boxes are surrounded in red, all settings were successfully saved.");
        }

        private static void updateBindingSources(DependencyObject obj, params DependencyProperty[] properties)
        {
            foreach (DependencyProperty depProperty in properties)
            {
                var be = BindingOperations.GetBindingExpression(obj, depProperty);
                if (be != null) be.UpdateSource();
            }

            int count = VisualTreeHelper.GetChildrenCount(obj);
            for (int i = 0; i < count; i++)
            {
                //process child items recursively
                var childObject = VisualTreeHelper.GetChild(obj, i);
                updateBindingSources(childObject, properties);
            }
        }
    }
}
