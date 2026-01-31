using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using ServiceAbstraction;
using Shared;
using Shared.DataTransferObjects.SeatDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeatController(ISeatService _seatService) : ControllerBase
    {
        [HttpGet]
        [OutputCache(Duration = 10, VaryByQueryKeys = ["EventId", "DesiredPageIndx", "VenueId"], Tags = ["Seats"])]
        public async Task<IActionResult> GetAllSeatsAsync([FromQuery] SeatQueryStruct query)
        {
            if (query.EventId <= 0)
                return BadRequest("You must provide an EventId (SessionId) to check seat availability.");
            var result = await _seatService.GetAllSeatsAsync(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [OutputCache(Duration = 2, Tags = ["Seats"])]
        public async Task<IActionResult> GetSeatByIdAsync(int id)
        {
            var result = await _seatService.GetSeatByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("row")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> CreateSeatRow([FromBody] CreateSeatRowRequestDTO request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            bool success = await _seatService.CreateSeatRowAsync(request);
            if (!success)
                return BadRequest("Failed. This row might already exist.");
            return StatusCode(201, new { Message = $"Created {request.SeatCount} seats for Row {request.Row}." });
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> DeleteSeat([FromBody] DeleteSeatRequestDTO request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            bool success = await _seatService.DeleteSeatAsync(request);
            if (!success) return NotFound("Seat not found.");
            return NoContent(); 
        }
    }
}
