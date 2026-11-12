using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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
        private readonly int _currentCashierId; 

        // === Списки ===
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
        private List<ProductDto> _allProducts; 

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                FilterProducts(); 
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

        
        private int _quantityToAdd = 1;
        public int QuantityToAdd
        {
            get { return _quantityToAdd; }
            set
            {
                _quantityToAdd = value;
                OnPropertyChanged(nameof(QuantityToAdd));
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

        private decimal _cardDiscountPercent;
        public decimal CardDiscountPercent
        {
            get { return _cardDiscountPercent; }
            set
            {
                _cardDiscountPercent = value;
                OnPropertyChanged(nameof(CardDiscountPercent));
            }
        }

        private string _cardCustomerName;
        public string CardCustomerName
        {
            get { return _cardCustomerName; }
            set
            {
                _cardCustomerName = value;
                OnPropertyChanged(nameof(CardCustomerName));
            }
        }

        // Итоги 
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
        public RelayCommand ApplyCardCommand { get; }   
        public RelayCommand ResetCardCommand { get; }   

        public SaleViewModel(ISaleService saleService, IProductService productService, int cashierId)
        {
            _saleService = saleService;
            _productService = productService;
            _currentCashierId = cashierId; 
            Cart = new ObservableCollection<SaleItemDto>();

            Cart.CollectionChanged += Cart_CollectionChanged;
            AddToCartCommand = new RelayCommand(param => AddToCart(SelectedProduct));
            RemoveFromCartCommand = new RelayCommand(param => RemoveFromCart(param as SaleItemDto));
            CheckoutCommand = new RelayCommand(_ => ProcessCheckout());
            ClearCartCommand = new RelayCommand(_ => { Cart.Clear(); RecalculateTotals(); });
            ApplyCardCommand = new RelayCommand(_ => ApplyLoyaltyCard());
            ResetCardCommand = new RelayCommand(_ => ResetLoyaltyCard());

            LoadAvailableProducts();
        }

        private void LoadAvailableProducts()
        {
         
            _allProducts = _productService.GetAllProducts()
                .Where(p => p.TotalQuantity > 0)
                .ToList();

            
            FilterProducts();
        }

        private void AddToCart(ProductDto product)
        {
            if (product == null)
            {
                MessageBox.Show("Выберите товар из списка!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (QuantityToAdd <= 0)
            {
                MessageBox.Show("Количество должно быть больше нуля!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var supplyItem = product.SupplyItems.FirstOrDefault(si => si.QuantityRemain > 0);
            if (supplyItem == null)
            {
                MessageBox.Show("Товара нет в наличии!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

           
            var existingItem = Cart.FirstOrDefault(ci => ci.SupplyItemId == supplyItem.Id);
            int currentQty = existingItem != null ? existingItem.Quantity : 0;

            if (currentQty + QuantityToAdd > supplyItem.QuantityRemain)
            {
                MessageBox.Show(
                    $"Недостаточно остатка! Доступно: {supplyItem.QuantityRemain - currentQty} шт.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal priceAtSale = supplyItem.DiscountedPrice.HasValue
                       ? supplyItem.DiscountedPrice.Value
                       : product.Price;

            if (existingItem != null)
            {
                Cart.Remove(existingItem);
                Cart.Add(new SaleItemDto
                {
                    SupplyItemId = supplyItem.Id,
                    ProductName = existingItem.ProductName,
                    Article = existingItem.Article,
                    Quantity = existingItem.Quantity + QuantityToAdd,
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
                    Quantity = QuantityToAdd,
                    PriceAtSale = priceAtSale
                });
            }

            QuantityToAdd = 1; 
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
            DiscountAmount = TotalAmount * CardDiscountPercent / 100m;
            FinalAmount = TotalAmount - DiscountAmount;
        }

        private void ApplyLoyaltyCard()
        {
            if (string.IsNullOrWhiteSpace(LoyaltyCardNumber))
            {
                MessageBox.Show("Введите номер карты!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                CardDiscountPercent = _saleService.GetLoyaltyDiscountPercent(LoyaltyCardNumber);
                CardCustomerName = $"Скидка {CardDiscountPercent:0}% применена";
                RecalculateTotals();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                CardDiscountPercent = 0;
                CardCustomerName = "";
                RecalculateTotals();
            }
        }

        private void ResetLoyaltyCard()
        {
            LoyaltyCardNumber = "";
            CardDiscountPercent = 0;
            CardCustomerName = "";
            RecalculateTotals();
        }

        private void ProcessCheckout()
        {
            if (!Cart.Any())
            {
                MessageBox.Show("Чек пуст!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var saleDto = _saleService.CreateSale(
                    Cart.ToList(), LoyaltyCardNumber, _currentCashierId);

                MessageBox.Show(
                    $"Продажа №{saleDto.Id} оформлена!\n" +
                    $"Сумма: {saleDto.TotalAmount:0.00} ₽\n" +
                    $"Скидка: {saleDto.DiscountAmount:0.00} ₽\n" +
                    $"К оплате: {saleDto.FinalAmount:0.00} ₽",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                Cart.Clear();
                ResetLoyaltyCard();
                SearchText = "";
                LoadAvailableProducts();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
      
        private void FilterProducts()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                AvailableProducts = new ObservableCollection<ProductDto>(_allProducts);
            }
            else
            {
                var filtered = _allProducts
                    .Where(p => p.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                                p.Article.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                AvailableProducts = new ObservableCollection<ProductDto>(filtered);
            }
        }

       
        private void Cart_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SaleItemDto item in e.NewItems)
                {
                    item.PropertyChanged += Item_PropertyChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (SaleItemDto item in e.OldItems)
                {
                    item.PropertyChanged -= Item_PropertyChanged;
                }
            }
        }

        
        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SaleItemDto.Quantity))
            {
                RecalculateTotals();
            }
        }
    }
}