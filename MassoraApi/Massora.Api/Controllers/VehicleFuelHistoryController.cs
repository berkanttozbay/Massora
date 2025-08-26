using AutoMapper;
using Massora.Business.DTOs;
using Massora.Business.Services;
using Massora.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Massora.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VehicleFuelHistoryController : ControllerBase
    {
        private readonly IVehicleFuelHistoryService _fuelHistoryService;
        private readonly IMapper _mapper;

        public VehicleFuelHistoryController(IVehicleFuelHistoryService fuelHistoryService, IMapper mapper)
        {
            _fuelHistoryService = fuelHistoryService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResultModel<VehicleFuelHistoryDto>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string searchTerm = null)
        {
            var loggedInUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // 2. Service metodunu 'await' ile çağırıyoruz.
            var paginatedResult = await _fuelHistoryService.GetVehicleFuelHistoriesPaginatedAsync(loggedInUserId,pageNumber, pageSize, searchTerm);

            // 3. Gelen sonucu doğrudan Ok() ile dönüyoruz.
            return Ok(paginatedResult);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VehicleFuelHistoryDto>> GetById(int id)
        {
            var fuelHistory = await _fuelHistoryService.GetByIdAsync(id);
            if (fuelHistory == null)
                return NotFound();

            var fuelHistoryDto = _mapper.Map<VehicleFuelHistoryDto>(fuelHistory);
            return Ok(fuelHistoryDto);
        }

        [HttpGet("vehicle/{vehicleId}")]
        public async Task<ActionResult<IEnumerable<VehicleFuelHistoryDto>>> GetByVehicleId(int vehicleId)
        {
            var fuelHistories = await _fuelHistoryService.GetByVehicleIdAsync(vehicleId);
            var fuelHistoryDtos = _mapper.Map<IEnumerable<VehicleFuelHistoryDto>>(fuelHistories);
            return Ok(fuelHistoryDtos);
        }

        [HttpGet("driver/{driverId}")]
        public async Task<ActionResult<IEnumerable<VehicleFuelHistoryDto>>> GetByDriverId(int driverId)
        {
            var fuelHistories = await _fuelHistoryService.GetByDriverIdAsync(driverId);
            var fuelHistoryDtos = _mapper.Map<IEnumerable<VehicleFuelHistoryDto>>(fuelHistories);
            return Ok(fuelHistoryDtos);
        }

        [HttpGet("date-range")]
        public async Task<ActionResult<IEnumerable<VehicleFuelHistoryDto>>> GetByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var fuelHistories = await _fuelHistoryService.GetByDateRangeAsync(startDate, endDate);
            var fuelHistoryDtos = _mapper.Map<IEnumerable<VehicleFuelHistoryDto>>(fuelHistories);
            return Ok(fuelHistoryDtos);
        }

        [HttpPost]
        public async Task<ActionResult<VehicleFuelHistoryDto>> Create(CreateVehicleFuelHistoryDto createFuelHistoryDto)
        {
            var fuelHistory = _mapper.Map<VehicleFuelHistory>(createFuelHistoryDto);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var newFuelHistory = await _fuelHistoryService.AddAsync(fuelHistory, userId);
            return CreatedAtAction(nameof(GetById), new { id = newFuelHistory.Id }, fuelHistory);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateVehicleFuelHistoryDto updateFuelHistoryDto)
        {
            var fuelHistory = _mapper.Map<VehicleFuelHistory>(updateFuelHistoryDto);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            // Parametre sırası: id, userId, vehicle
            await _fuelHistoryService.UpdateAsync(id, userId, fuelHistory);

            return NoContent(); // Başarılı update işlemlerinde 204 No Content dönmek standarttır.
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _fuelHistoryService.DeleteAsync(id);
            return NoContent();
        }
        



    }
} 