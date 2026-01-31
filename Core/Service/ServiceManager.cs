using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceAbstraction;

namespace Service
{
    public class ServiceManager(
        Func<ISeatReservationService> _seatReservationService,
        Func<ISeatService> _seatService
        ) : IServiceManager
    {
        public ISeatReservationService SeatReservationService => _seatReservationService.Invoke();
        public ISeatService SeatService => _seatService.Invoke();
    }
}
