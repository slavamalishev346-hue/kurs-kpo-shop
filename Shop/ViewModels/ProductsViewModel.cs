using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Shop.Core;
using Shop.Interfaces.DTO;
using Shop.Interfaces.Services;

namespace Shop.ViewModels
{
    public class ProductsViewModel : BaseViewModel
    {
        private readonly IProductService _productService;


        private ObservableCollection<ProductDto> _products;
        public ObservableCollection<ProductDto> Products
        {
            get { return _products; }
            set
            {
                _products = value;
                OnPropertyChanged(nameof(Products));
            }
        }

        private ProductDto _selectedProduct;
        public ProductDto SelectedProduct
        {
            get { return _selectedProduct; }
            set
            {
                _selectedProduct = value;
                OnPropertyChanged(nameof(SelectedProduct));
            }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
            }
        }

        private bool _isLowStockFilter;
        public bool IsLowStockFilter
        {
            get { return _isLowStockFilter; }
            set
            {
                _isLowStockFilter = value;
                OnPropertyChanged(nameof(IsLowStockFilter));
            }
        }

        private bool _isExpiringSoonFilter;
        public bool IsExpiringSoonFilter
        {
            get { return _isExpiringSoonFilter; }
            set
            {
                _isExpiringSoonFilter = value;
                OnPropertyChanged(nameof(IsExpiringSoonFilter));
            }
        }

        // --- Команды ---
        public RelayCommand LoadProductsCommand { get; set; }
        public RelayCommand SearchCommand { get; set; }
        public RelayCommand ApplyFiltersCommand { get; set; }
        public RelayCommand AddProductCommand { get; set; }
        public RelayCommand EditProductCommand { get; set; }
        public RelayCommand DeleteProductCommand { get; set; }
        public RelayCommand ApplyDiscountCommand { get; set; }

        public ProductsViewModel(IProductService productService)
        {
            _productService = productService;

            LoadProductsCommand = new RelayCommand(_ => LoadProducts());
            SearchCommand = new RelayCommand(_ => ApplySearch());
            ApplyFiltersCommand = new RelayCommand(_ => ApplyFilters());
            AddProductCommand = new RelayCommand(_ => OpenAddProductWindow());
            EditProductCommand = new RelayCommand(_ => OpenEditProductWindow(), _ => SelectedProduct != null);
            DeleteProductCommand = new RelayCommand(_ => DeleteProduct(), _ => SelectedProduct != null);
            ApplyDiscountCommand = new RelayCommand(_ => ApplyDiscount());

            // Автомат загрузка при создании
            LoadProducts();
        }

   
        private void LoadProducts()
        {
            try
            {
                var products = _productService.GetAllProducts();
                Products = new ObservableCollection<ProductDto>(products);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки товаров: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplySearch()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                LoadProducts();
                return;
            }

            var filtered = Products
                .Where(p => p.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                            p.Article.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                .ToList();

            Products = new ObservableCollection<ProductDto>(filtered);
        }

        private void ApplyFilters()
        {
            try
            {
                List<ProductDto> filtered;

                if (IsLowStockFilter && IsExpiringSoonFilter)
                {
                    var lowStock = _productService.GetLowStockProducts();
                    var expiringSoon = _productService.GetExpiringSoonProducts();
                    filtered = lowStock.Intersect(expiringSoon).ToList();
                }
                else if (IsLowStockFilter)
                {
                    filtered = _productService.GetLowStockProducts();
                }
                else if (IsExpiringSoonFilter)
                {
                    filtered = _productService.GetExpiringSoonProducts();
                }
                else
                {
                    LoadProducts();
                    return;
                }

                Products = new ObservableCollection<ProductDto>(filtered);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка применения фильтров: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenAddProductWindow()
        {
            var editWindow = new Views.ProductEditWindow(null);
            editWindow.Owner = Application.Current.MainWindow;

            if (editWindow.ShowDialog() == true)
            {
                LoadProducts();
            }
        }

        private void OpenEditProductWindow()
        {
            if (SelectedProduct == null) return;

            var editWindow = new Views.ProductEditWindow(SelectedProduct);
            editWindow.Owner = Application.Current.MainWindow;

            if (editWindow.ShowDialog() == true)
            {
                LoadProducts();
            }
        }

        private void DeleteProduct()
        {
            if (SelectedProduct == null) return;

            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить товар '{SelectedProduct.Name}'?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _productService.DeleteProduct(SelectedProduct.Id);
                    LoadProducts();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ApplyDiscount()
        {
            try
            {
                _productService.UpdatePricesByExpiration();
                MessageBox.Show("Уценка применена успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadProducts();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка применения уценки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}