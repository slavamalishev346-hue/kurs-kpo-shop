using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Shop.Core;
using Shop.Interfaces.DTO;
using Shop.Interfaces.Services;
using System.Windows;


namespace Shop.ViewModels
{
    public class ReturnsViewModel : BaseViewModel
    {
        private readonly ISaleService _saleService;
        private readonly int _currentCashierId;

        private List<SaleDto> _allSales;

        public ReturnsViewModel(ISaleService saleService, int cashierId)
        {
            _saleService = saleService;
            _currentCashierId = cashierId;

            PlusReturnCommand = new RelayCommand(p => ChangeReturn(p, +1));
            MinusReturnCommand = new RelayCommand(p => ChangeReturn(p, -1));
            AllReturnCommand = new RelayCommand(p => SetReturnAll(p));
            ProcessReturnCommand = new RelayCommand(_ => ExecuteReturn());
            LoadSales();
        }

        private ObservableCollection<SaleDto> _sales = new ObservableCollection<SaleDto>();
        public ObservableCollection<SaleDto> Sales
        {
            get { return _sales; }
            set { _sales = value; OnPropertyChanged(); }
        }

        private SaleDto _selectedSale;
        public SaleDto SelectedSale
        {
            get { return _selectedSale; }
            set
            {
                _selectedSale = value;
                OnPropertyChanged();
                LoadSaleItems();
            }
        }

        private ObservableCollection<SaleItemDto> _saleItems = new ObservableCollection<SaleItemDto>();
        public ObservableCollection<SaleItemDto> SaleItems
        {
            get { return _saleItems; }
            set
            {
                if (_saleItems != null)
                    foreach (var item in _saleItems)
                        item.PropertyChanged -= SaleItem_PropertyChanged;

                _saleItems = value;

                if (_saleItems != null)
                    foreach (var item in _saleItems)
                        item.PropertyChanged += SaleItem_PropertyChanged;

                OnPropertyChanged();
            }
        }
        public ICommand ProcessReturnCommand { get; }
        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged();
                FilterSales();
            }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set { _message = value; OnPropertyChanged(); }
        }

        private decimal _refundAmount;
        public decimal RefundAmount
        {
            get { return _refundAmount; }
            set { _refundAmount = value; OnPropertyChanged(); }
        }

        public ICommand PlusReturnCommand { get; }
        public ICommand MinusReturnCommand { get; }
        public ICommand AllReturnCommand { get; }

        private void LoadSales()
        {
            _allSales = _saleService.GetAllSales()
                                    .OrderByDescending(s => s.Date)
                                    .ToList();

            FilterSales();
        }

        private void FilterSales()
        {
            var baseList = string.IsNullOrWhiteSpace(SearchText)
                ? _allSales
                : _allSales.Where(s => s.Id.ToString().Contains(SearchText.Trim())).ToList();

            var filtered = baseList.Where(s => !s.IsFullyReturned).ToList();

            Sales = new ObservableCollection<SaleDto>(filtered);

            Message = Sales.Count == 0
                ? "Продаж не найдено."
                : $"Найдено чеков: {Sales.Count}";
        }
        private void LoadSaleItems()
        {
            if (SelectedSale == null)
            {
                SaleItems = new ObservableCollection<SaleItemDto>();
                RecalculateRefund();
                return;
            }

            try
            {
                var sale = _saleService.GetSaleById(SelectedSale.Id);
                SaleItems = new ObservableCollection<SaleItemDto>(sale.Items);
                RecalculateRefund();
            }
            catch (Exception ex)
            {
                SaleItems = new ObservableCollection<SaleItemDto>();
                RecalculateRefund();
                Message = "Ошибка: " + ex.Message;
            }
        }

        private void SaleItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(SaleItemDto.ReturnQuantity))
                return;

            var item = (SaleItemDto)sender;

            if (item.ReturnQuantity < 0)
            {
                item.ReturnQuantity = 0;
            }
            else if (item.ReturnQuantity > item.AvailableToReturn)
            {
                item.ReturnQuantity = item.AvailableToReturn;
                Message = $"По позиции «{item.ProductName}» доступно только {item.AvailableToReturn} шт.";
            }

            RecalculateRefund();
        }

        private void RecalculateRefund()
        {
            RefundAmount = _saleService.CalculateRefund(SaleItems.ToList());
        }

        private void ChangeReturn(object param, int delta)
        {
            if (param is not SaleItemDto item) return;
            item.ReturnQuantity = Math.Clamp(item.ReturnQuantity + delta, 0, item.AvailableToReturn);
        }

        private void SetReturnAll(object param)
        {
            if (param is not SaleItemDto item) return;
            item.ReturnQuantity = item.AvailableToReturn;
        }
        private void ExecuteReturn()
        {
            if (SelectedSale == null) return;

            var itemsToReturn = SaleItems.Where(i => i.ReturnQuantity > 0)
                                         .Select(i => new ReturnDto
                                         {
                                             SaleItemId = i.Id,
                                             Quantity = i.ReturnQuantity
                                         }).ToList();

            if (!itemsToReturn.Any())
            {
                Message = "Укажите количество для возврата хотя бы для одной позиции.";
                return;
            }

            try
            {
                _saleService.ProcessReturn(SelectedSale.Id, itemsToReturn, _currentCashierId);

                MessageBox.Show($"Возврат на сумму {RefundAmount:0.00} ₽ успешно оформлен!", "Успех",
                                MessageBoxButton.OK, MessageBoxImage.Information);

                // Сбрасываем введённые количества
                foreach (var item in SaleItems)
                {
                    item.ReturnQuantity = 0;
                }

                // Перечитываем состав чека
                var saleId = SelectedSale.Id;
                LoadSaleItems();
                LoadSales();
                SelectedSale = Sales.FirstOrDefault(s => s.Id == saleId);
            }
            catch (Exception ex)
            {
                Message = "Ошибка: " + (ex.InnerException?.Message ?? ex.Message);
            }
        }
    }
}