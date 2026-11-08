using System.Windows.Input;
using Shop.Core;
using Shop.Interfaces.DTO;
using Shop.Interfaces.Services;

namespace Shop.ViewModels
{
    public class LoyaltyViewModel : BaseViewModel
    {
        private readonly ILoyaltyService _loyaltyService;

        private string _searchQuery;
        private LoyaltyCardDto _currentCard;
        private string _message;
        private string _newCardPhone;
        private string _newCardName;

        public LoyaltyViewModel(ILoyaltyService loyaltyService)
        {
            _loyaltyService = loyaltyService;
            SearchCommand = new RelayCommand(ExecuteSearch);
            IssueCardCommand = new RelayCommand(ExecuteIssueCard);
        }

        public string SearchQuery
        {
            get { return _searchQuery; }
            set
            {
                _searchQuery = value;
                OnPropertyChanged();
            }
        }

        public LoyaltyCardDto CurrentCard
        {
            get { return _currentCard; }
            set
            {
                _currentCard = value;
                OnPropertyChanged();
            }
        }

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public string NewCardPhone
        {
            get { return _newCardPhone; }
            set
            {
                _newCardPhone = value;
                OnPropertyChanged();
            }
        }

        public string NewCardName
        {
            get { return _newCardName; }
            set
            {
                _newCardName = value;
                OnPropertyChanged();
            }
        }

        public ICommand SearchCommand { get; private set; }
        public ICommand IssueCardCommand { get; private set; }

        private void ExecuteSearch(object parameter)
        {
            Message = "";

            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                return;
            }

            CurrentCard = _loyaltyService.GetCardByNumber(SearchQuery);

            if (CurrentCard == null)
            {
                Message = "Карта не найдена. Вы можете оформить новую ниже.";
            }
        }

        private void ExecuteIssueCard(object parameter)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NewCardPhone) || string.IsNullOrWhiteSpace(NewCardName))
                {
                    Message = "Заполните все поля!";
                    return;
                }

                _loyaltyService.IssueNewCard(NewCardPhone, NewCardName);
                Message = "Карта успешно выдана! Номер: " + NewCardPhone;

                NewCardPhone = "";
                NewCardName = "";
                SearchQuery = "";
                CurrentCard = null;
            }
            catch (System.Exception ex)
            {
                Message = "Ошибка: " + ex.Message;
            }
        }
    }
}