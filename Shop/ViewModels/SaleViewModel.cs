using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Shop.Core;
using Shop.Interfaces.DTO;
using Shop.Interfaces.Services;

namespace Shop.ViewModels
{
    public class SaleViewModel : BaseViewModel
    {
        private readonly ISaleService _saleService;
        private readonly IProductService _productService;
        private readonly int _currentCashierId = 3;

        private ObservableCollection<ProductDto> _availableProducts;
        public ObservableCollection<ProductDto> AvailableProducts
        {
            get { return _availableProducts; }
            set
            {
                _availableProducts = value;
                OnPropertyChanged(nameof(AvailableProducts));
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

        private ObservableCollection<SaleItemDto> _cart;
        public ObservableCollection<SaleItemDto> Cart
        {
            get { return _cart; }
            set
            {
                _cart = value;
                OnPropertyChanged(nameof(Cart));
                RecalculateTotals();
            }
        }

        private string _loyaltyCardNumber;
        public string LoyaltyCardNumber
        {
            get { return _loyaltyCardNumber; }
            set
            {
                _loyaltyCardNumber = value;
                OnPropertyChanged(nameof(LoyaltyCardNumber));
            }
        }

        private decimal _totalAmount;
        public decimal TotalAmount
        {
            get { return _totalAmount; }
            set
            {
                _totalAmount = value;
                OnPropertyChanged(nameof(TotalAmount));
            }
        }

        private decimal _discountAmount;
        public decimal DiscountAmount
        {
            get { return _discountAmount; }
            set
            {
                _discountAmount = value;
                OnPropertyChanged(nameof(DiscountAmount));
            }
        }

        private decimal _finalAmount;
        public decimal FinalAmount
        {
            get { return _finalAmount; }
            set
            {
                _finalAmount = value;
                OnPropertyChanged(nameof(FinalAmount));
            }
        }

        public RelayCommand AddToCartCommand { get; }
        public RelayCommand RemoveFromCartCommand { get; }
        public RelayCommand CheckoutCommand { get; }
        public RelayCommand ClearCartCommand { get; }

        public SaleViewModel(ISaleService saleService, IProductService productService)
        {
            _saleService = saleService;
            _productService = productService;
            Cart = new ObservableCollection<SaleItemDto>();

            AddToCartCommand = new RelayCommand(param => AddToCart(param as ProductDto));
            RemoveFromCartCommand = new RelayCommand(param => RemoveFromCart(param as SaleItemDto), _ => Cart.Any());
            CheckoutCommand = new RelayCommand(_ => ProcessCheckout(), _ => Cart.Any());
            ClearCartCommand = new RelayCommand(_ => Cart.Clear());

            LoadAvailableProducts();
        }

        private void LoadAvailableProducts()
        {
            var products = _productService.GetAllProducts().Where(p => p.TotalQuantity > 0).ToList();
            AvailableProducts = new ObservableCollection<ProductDto>(products);
        }

        private void AddToCart(ProductDto product)
        {
            if (product == null) return;

            var supplyItem = product.SupplyItems.FirstOrDefault(si => si.QuantityRemain > 0);
            if (supplyItem == null)
            {
                MessageBox.Show("Товара нет в наличии!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

         
            var existingItem = Cart.FirstOrDefault(ci => ci.SupplyItemId == supplyItem.Id);
            if (existingItem != null)
            {
                if (existingItem.Quantity + 1 > supplyItem.QuantityRemain)
                {
                    MessageBox.Show("Недостаточно остатка на складе!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                
                Cart.Remove(existingItem);
                Cart.Add(new SaleItemDto
                {
                    SupplyItemId = supplyItem.Id,
                    ProductName = existingItem.ProductName,
                    Article = existingItem.Article,
                    Quantity = existingItem.Quantity + 1,
                    PriceAtSale = existingItem.PriceAtSale
                });
            }
            else
            {
                Cart.Add(new SaleItemDto
                {
                    SupplyItemId = supplyItem.Id,
                    ProductName = product.Name,
                    Article = product.Article,
                    Quantity = 1,
                    PriceAtSale = supplyItem.DiscountedPrice.HasValue ? supplyItem.DiscountedPrice.Value : product.Price
                });
            }

            RecalculateTotals();
        }

        private void RemoveFromCart(SaleItemDto item)
        {
            if (item == null) return;
            Cart.Remove(item);
            RecalculateTotals();
        }

        private void RecalculateTotals()
        {
            TotalAmount = Cart.Sum(item => item.PriceAtSale * item.Quantity);
            FinalAmount = TotalAmount;
        }

        private void ProcessCheckout()
        {
            try
            {
                var saleDto = _saleService.CreateSale(Cart.ToList(), LoyaltyCardNumber, _currentCashierId);

                MessageBox.Show($"Продажа успешно оформлена!\nИтого: {saleDto.FinalAmount:0.00} ₽", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                Cart.Clear();
                LoyaltyCardNumber = "";
                LoadAvailableProducts();
            }
            catch (Exception ex)
            {
                string fullError = ex.Message;
                if (ex.InnerException != null)
                    fullError += "\n\nInner: " + ex.InnerException.Message;
                if (ex.InnerException?.InnerException != null)
                    fullError += "\n\nInner2: " + ex.InnerException.InnerException.Message;

                MessageBox.Show(fullError, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}