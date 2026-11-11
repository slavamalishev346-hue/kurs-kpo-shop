using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using Shop.Interfaces.DTO;

namespace Shop.Views
{
    public partial class ProductEditWindow : Window
    {
        // Сюда запишем данные для передачи в ViewModel
        public ProductDto ResultDto { get; private set; }

        // Флаг: редактируем существующий или создаём новый
        private readonly bool _isEditMode;
        private readonly ProductDto _existingProduct;

        // ✅ Конструктор теперь принимает опциональный DTO
        public ProductEditWindow(ProductDto productToEdit = null)
        {
            InitializeComponent();

            _existingProduct = productToEdit;
            _isEditMode = productToEdit != null;

            if (_isEditMode)
            {
                // Режим РЕДАКТИРОВАНИЯ: предзаполняем поля
                Title = "Редактирование товара";
                txtName.Text = _existingProduct.Name;
                txtArticle.Text = _existingProduct.Article;
                txtPrice.Text = _existingProduct.Price.ToString();
                txtCost.Text = _existingProduct.Cost.ToString();

                // Показываем путь к текущей картинке (если есть)
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string currentImagePath = Path.Combine(basePath, "Images", "Products", $"{_existingProduct.Article}.jpg");
                if (File.Exists(currentImagePath))
                {
                    txtImagePath.Text = currentImagePath;
                }

                // Артикул при редактировании лучше не давать менять (чтобы не сломать связь с картинками)
                txtArticle.IsReadOnly = true;
                txtArticle.Background = System.Windows.Media.Brushes.LightGray;
            }
            else
            {
                // Режим ДОБАВЛЕНИЯ
                Title = "Добавление нового товара";
            }
        }

        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.png",
                Title = "Выберите изображение товара"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                txtImagePath.Text = openFileDialog.FileName;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // 1. Базовая валидация
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название товара!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtArticle.Text))
            {
                MessageBox.Show("Введите артикул!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Введите корректную цену продажи!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!decimal.TryParse(txtCost.Text, out decimal cost) || cost <= 0)
            {
                MessageBox.Show("Введите корректную себестоимость!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. Формируем DTO
            ResultDto = new ProductDto
            {
                Id = _isEditMode ? _existingProduct.Id : 0, // При редактировании сохраняем Id
                Name = txtName.Text.Trim(),
                Article = txtArticle.Text.Trim().ToUpper(),
                Price = price,
                Cost = cost
            };

            if (!string.IsNullOrWhiteSpace(txtImagePath.Text) && File.Exists(txtImagePath.Text))
            {
                try
                {
                    string basePath = AppDomain.CurrentDomain.BaseDirectory;
                    string targetFolder = Path.Combine(basePath, "Images", "Products");

                    if (!Directory.Exists(targetFolder))
                        Directory.CreateDirectory(targetFolder);

                    string extension = Path.GetExtension(txtImagePath.Text);
                    if (extension.ToLower() != ".jpg")
                        extension = ".jpg";

                    string targetPath = Path.Combine(targetFolder, $"{ResultDto.Article}{extension}");
                    File.Copy(txtImagePath.Text, targetPath, true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не удалось сохранить изображение: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

          
            this.DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}