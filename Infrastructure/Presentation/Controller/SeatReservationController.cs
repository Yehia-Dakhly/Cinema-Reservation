using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using ServiceAbstraction;
using Shared.DataTransferObjects.SeatDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController(ISeatReservationService _seatService, IOutputCacheStore _cacheStore)
        : ControllerBase
    {
        [HttpPost("lock")]
        public async Task<IActionResult> LockSeat([FromBody] LockSeatRequestDTO request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            bool success = await _seatService.LockSeatAsync(request);
            if (!success) return Conflict(new { Message = "This seat is currently unavailable." });
            await _cacheStore.EvictByTagAsync("Seats", CancellationToken.None);
            return Ok(new { Message = "Seat locked successfully. You have 10 minutes to pay." });
        }

        [HttpPost("unlock")]
        public async Task<IActionResult> UnlockSeat([FromBody] UnlockSeatRequestDTO request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            bool success = await _seatService.UnlockSeatAsync(request);
            if (!success) return BadRequest("Could not unlock. Either the lock expired or you do not own it.");
            await _cacheStore.EvictByTagAsync("Seats", CancellationToken.None);
            return Ok(new { Message = "Seat unlocked." });
        }
    }
}
