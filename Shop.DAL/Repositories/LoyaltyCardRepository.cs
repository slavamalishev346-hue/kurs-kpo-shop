using System.Collections.Generic;
using System.Linq;
using Shop.DAL;
using Shop.Domain.Entities;
using Shop.Interfaces.Repositories;

namespace Shop.DAL.Repositories
{
    public class LoyaltyCardRepository : ILoyaltyCardRepository
    {
        private readonly MagazContext _context;

        public LoyaltyCardRepository(MagazContext context)
        {
            _context = context;
        }

        public Loyaltycard GetByCardNumber(string cardNumber)
        {
            
            var card = _context.Loyaltycards.FirstOrDefault(c => c.CardNumber == cardNumber);


            if (card != null)
            {
                card.Level = _context.Loyaltylevels.FirstOrDefault(l => l.Id == card.LevelId);
            }

            return card;
        }

        public List<Loyaltycard> GetList()
        {
            return _context.Loyaltycards.ToList();
        }

        public Loyaltycard GetById(int id)
        {
            var card = _context.Loyaltycards.Find(id);

            if (card != null)
            {
                card.Level = _context.Loyaltylevels.FirstOrDefault(l => l.Id == card.LevelId);
            }

            return card;
        }

        public void Add(Loyaltycard entity)
        {
            _context.Loyaltycards.Add(entity);
        }

        public void Update(Loyaltycard entity)
        {
            _context.Loyaltycards.Update(entity);
        }

        public void Delete(int id)
        {
            var card = _context.Loyaltycards.Find(id);
        }
    }
}