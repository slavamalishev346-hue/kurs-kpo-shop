using System.Windows;

namespace Shop.Views
{
    public partial class ProductEditWindow : Window
    {
      
        public ProductEditWindow(object productDto)
        {
            InitializeComponent();

           
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
         
            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}