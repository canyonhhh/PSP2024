using PSPOS.ApiService.Repositories.Interfaces;
using PSPOS.ApiService.Services.Interfaces;
using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IDiscountRepository _discountRepository;
        private readonly IOrderRepository _orderRepository;

        public DiscountService(IDiscountRepository discountRepository, IOrderRepository orderRepository)
        {
            _discountRepository = discountRepository;
            _orderRepository = orderRepository;
        }

        public async Task<IEnumerable<Discount>> GetAllDiscountsAsync(DateTime? from, DateTime? to, int page, int pageSize)
        {
            return await _discountRepository.GetAllDiscountsAsync(from, to, page, pageSize);
        }

        public async Task<Discount?> GetDiscountByIdAsync(Guid discountId)
        {
            return await _discountRepository.GetDiscountByIdAsync(discountId);
        }

        public async Task AddDiscountAsync(Discount discount)
        {
            await _discountRepository.AddDiscountAsync(discount);
        }
        public async Task AddAppliedDiscountAsync(AppliedDiscount discount)
        {
            if (discount == null)
                throw new ArgumentNullException(nameof(discount), "Applied discount cannot be null.");

            await _discountRepository.AddAppliedDiscountAsync(discount);
        }
        public async Task UpdateDiscountAsync(Guid discountId, Discount updatedDiscount)
        {
            var existingDiscount = await _discountRepository.GetDiscountByIdAsync(discountId);
            if (existingDiscount == null)
                throw new KeyNotFoundException("Discount not found");


            existingDiscount.Name = updatedDiscount.Name;
            existingDiscount.Description = updatedDiscount.Description;
            existingDiscount.Method = updatedDiscount.Method;
            existingDiscount.Active = updatedDiscount.Active;
            existingDiscount.Amount = updatedDiscount.Amount;
            existingDiscount.Percentage = updatedDiscount.Percentage;
            existingDiscount.EndDate = updatedDiscount.EndDate;
            existingDiscount.BusinessId = updatedDiscount.BusinessId;
            existingDiscount.ProductOrServiceGroupId = updatedDiscount.ProductOrServiceGroupId;

            await _discountRepository.UpdateDiscountAsync(existingDiscount);
        }

        public async Task DeleteDiscountAsync(Guid discountId)
        {
            await _discountRepository.DeleteDiscountAsync(discountId);
        }
        public async Task ApplyDiscountToOrderItemAsync(Guid orderId, Guid orderItemId, Guid discountId)
        {
            var discount = await _discountRepository.GetDiscountByIdAsync(discountId);
            if (discount == null)
                throw new KeyNotFoundException("Discount not found.");

            if (!discount.Active || discount.EndDate < DateTime.Now)
                throw new InvalidOperationException("The discount is either inactive or expired.");

            var orderItems = await _orderRepository.GetAllItemsOfOrderAsyncO(orderId);
            var orderItem = orderItems.FirstOrDefault(oi => oi.Id == orderItemId);

            if (orderItem == null)
                throw new KeyNotFoundException("Order item not found in the specified order.");

            decimal originalTotalPrice = orderItem.Price * orderItem.Quantity;
            decimal discountedTotalPrice = originalTotalPrice;

            switch ((discount.Method ?? "").ToUpper())
            {
                case "FIXED":
                    discountedTotalPrice -= discount.Amount;
                    break;

                case "PERCENTAGE":
                    discountedTotalPrice -= (originalTotalPrice * (discount.Percentage / 100));
                    break;

                default:
                    throw new InvalidOperationException("Invalid discount method.");
            }

            if (discountedTotalPrice < 0)
                discountedTotalPrice = 0;

            if (orderItem.Quantity != 0)
            {
                discountedTotalPrice = discountedTotalPrice / orderItem.Quantity;
            }

            orderItem.Price = discountedTotalPrice;
            orderItem.UpdatedAt = DateTime.UtcNow;

            await _orderRepository.UpdateOrderItemAsync(orderItem);
        }

    }
}