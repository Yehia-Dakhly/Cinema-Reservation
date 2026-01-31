using DomainLayer.Contracts;
using DomainLayer.Models;
using Service.Specifications;
using ServiceAbstraction;
using Shared.DataTransferObjects.SeatDTO;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class SeatReservationService(IUnitOfWorkRepository _unitOfWorkRepository,IConnectionMultiplexer _muexer) : ISeatReservationService
    {
        private readonly IDatabase _redis = _muexer.GetDatabase();
        public async Task<bool> LockSeatAsync(LockSeatRequestDTO request)
        {
            var ticketRepository = _unitOfWorkRepository.GetRepository<Ticket>();
            var spec = new TicketExistenceSpecification(request.EventId, request.SeatId);
            var existingTicketCount = await ticketRepository.CountAsync(spec);
            if (existingTicketCount > 0) return false; 
            string key = $"lock:session:{request.EventId}:seat:{request.SeatId}";
            string value = request.UserId.ToString();
            var expiry = TimeSpan.FromMinutes(10);
            return await _redis.StringSetAsync(key, value, expiry, When.NotExists);
        }
        public async Task<bool> UnlockSeatAsync(UnlockSeatRequestDTO request)
        {
            string key = $"lock:session:{request.EventId}:seat:{request.SeatId}";
            var currentLockValue = await _redis.StringGetAsync(key);
            if (!currentLockValue.HasValue || currentLockValue.ToString() != request.UserId.ToString()) return false;
            bool deleted = await _redis.KeyDeleteAsync(key);
            return deleted;
        }
    }
}
