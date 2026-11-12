using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Shop.Interfaces.DTO
{
    public class SaleItemDto : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public int SupplyItemId { get; set; }
        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged();
                }
            }
        }
        public decimal PriceAtSale { get; set; } 

        public string ProductName { get; set; }
        public string Article { get; set; }
  
        public decimal Subtotal
        {
            get { return PriceAtSale * Quantity; }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}