using Microsoft.AspNetCore.Mvc;
using Massora.Business.Services;
using Massora.Domain.Entities;
using Massora.Business.DTOs;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Massora.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;
        private readonly IMapper _mapper;

        public VehicleController(IVehicleService vehicleService, IMapper mapper)
        {
            _vehicleService = vehicleService;
            _mapper = mapper;
        }
        
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<VehicleDto>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string searchTerm = null
           )
        {
            var loggedInUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // 2. Service metodunu 'await' ile çağırıyoruz.
            var paginatedResult = await _vehicleService.GetVehiclesPaginatedAsync(loggedInUserId,pageNumber, pageSize, searchTerm);

            // 3. Gelen sonucu doğrudan Ok() ile dönüyoruz.
            return Ok(paginatedResult);
        }  
        

        [HttpGet("{id}")]
        public async Task<ActionResult<VehicleDto>> GetById(int id)
        {
            var vehicle = await _vehicleService.GetByIdAsync(id);
            if (vehicle == null)
                return NotFound();

            var vehicleDto = _mapper.Map<VehicleDto>(vehicle);
            return Ok(vehicleDto);
        }

        [HttpGet("company/{companyId}")]
        public async Task<ActionResult<IEnumerable<VehicleDto>>> GetByCompanyId(int companyId)
        {
            var vehicles = await _vehicleService.GetByCompanyIdAsync(companyId);
            var vehicleDtos = _mapper.Map<IEnumerable<VehicleDto>>(vehicles);
            return Ok(vehicleDtos);
        }

        [HttpGet("license-plate/{licensePlate}")]
        public async Task<ActionResult<VehicleDto>> GetByLicensePlate(string licensePlate)
        {
            var vehicle = await _vehicleService.GetByLicensePlateAsync(licensePlate);
            if (vehicle == null)
                return NotFound();

            var vehicleDto = _mapper.Map<VehicleDto>(vehicle);
            return Ok(vehicleDto);
        }

        [HttpPost]
        public async Task<ActionResult<CreateVehicleDto>> Create(CreateVehicleDto createVehicleDto)
        {
            var vehicle = _mapper.Map<Vehicle>(createVehicleDto);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var newVehicle = await _vehicleService.AddAsync(vehicle, userId);
            var vehicleDto = _mapper.Map<CreateVehicleDto>(newVehicle);
            return CreatedAtAction(nameof(GetById), new { id = newVehicle.Id }, vehicleDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateVehicleDto updateVehicleDto)
        {
            var vehicle = _mapper.Map<Vehicle>(updateVehicleDto);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            // Parametre sırası: id, userId, vehicle
            await _vehicleService.UpdateAsync(id, userId, vehicle);

            return NoContent(); // Başarılı update işlemlerinde 204 No Content dönmek standarttır.
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _vehicleService.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("for-dropdown")]
        public async Task<IActionResult> GetForDropdown()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _vehicleService.GetVehiclesForDropdownAsync(userId);
            return Ok(result);
        }
    }
} 