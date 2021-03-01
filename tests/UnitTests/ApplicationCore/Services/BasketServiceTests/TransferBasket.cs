using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Services;
using ApplicationCore.Specifications;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.ApplicationCore.Services.BasketServiceTests
{
    public class TransferBasket
    {
        private readonly Mock<IAsyncRepository<Basket>> _mockBasketRepo;
        private readonly Mock<IAsyncRepository<BasketItem>> _mockBasketItemRepo;

        public TransferBasket()
        {
            _mockBasketRepo = new Mock<IAsyncRepository<Basket>>();
            _mockBasketItemRepo = new Mock<IAsyncRepository<BasketItem>>();
        }

        [Fact]
        public async Task InvokesFirstOrDefaultAsyncOnceIfAnonymousBasketNotExists()
        {
            // test için mock nesnesini hazırlıyoruz
            // mock basket repo üzerinde first or default kullanılırsa 
            // ilk kullanımda anonBasket'i
            // ikinci kullanımda userBasket'i döndür
            Basket anonBasket = null;
            Basket userBasket = new Basket() { BuyerId = "existent-basket-user-id", Items = new List<BasketItem>() };
            _mockBasketRepo.SetupSequence(x => x.FirstOrDefaultAsync(It.IsAny<BasketWithItemsSpecification>()))
                .ReturnsAsync(anonBasket)
                .ReturnsAsync(userBasket);

            var basketService = new BasketService(_mockBasketRepo.Object, _mockBasketItemRepo.Object);

            await basketService.TransferBasketAsync("nonexistent-basket-anon-id", "existent-basket-user-id");

            // basketRepo üzerinde FirstOrDefaultAsync bir kez çağrıldığını doğrula
            _mockBasketRepo.Verify(x => x.FirstOrDefaultAsync(It.IsAny<BasketWithItemsSpecification>()), Times.Once);
        }
    }
}
