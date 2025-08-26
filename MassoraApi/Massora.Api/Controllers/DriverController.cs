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
    public class DriverController : ControllerBase
    {
        private readonly IDriverService _driverService;
        private readonly IMapper _mapper;

        public DriverController(IDriverService driverService, IMapper mapper)
        {
            _driverService = driverService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResultModel<DriverDto>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string searchTerm = null)
        {
            var loggedInUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // 2. Service metodunu 'await' ile çağırıyoruz.
            var paginatedResult = await _driverService.GetDriversPaginatedAsync(loggedInUserId,pageNumber, pageSize, searchTerm);

            // 3. Gelen sonucu doğrudan Ok() ile dönüyoruz.
            return Ok(paginatedResult);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DriverDto>> GetById(int id)
        {
            var driver = await _driverService.GetByIdAsync(id);
            if (driver == null)
                return NotFound();

            var driverDto = _mapper.Map<DriverDto>(driver);
            return Ok(driverDto);
        }

        [HttpGet("company/{companyId}")]
        public async Task<ActionResult<IEnumerable<DriverDto>>> GetByCompanyId(int companyId)
        {
            var drivers = await _driverService.GetByCompanyIdAsync(companyId);
            var driverDtos = _mapper.Map<IEnumerable<DriverDto>>(drivers);
            return Ok(driverDtos);
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<DriverDto>> GetByEmail(string email)
        {
            var driver = await _driverService.GetByEmailAsync(email);
            if (driver == null)
                return NotFound();

            var driverDto = _mapper.Map<DriverDto>(driver);
            return Ok(driverDto);
        }

        [HttpGet("phone/{phone}")]
        public async Task<ActionResult<DriverDto>> GetByPhone(string phone)
        {
            var driver = await _driverService.GetByPhoneAsync(phone);
            if (driver == null)
                return NotFound();

            var driverDto = _mapper.Map<DriverDto>(driver);
            return Ok(driverDto);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<DriverDto>> Create([FromBody] CreateDriverDto createDriverDto)
        {
            var driver = _mapper.Map<Driver>(createDriverDto);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                var newDriver = await _driverService.AddAsync(driver,userId);
                var driverToReturn = _mapper.Map<DriverDto>(newDriver);
                return Ok(driverToReturn);
            }
            catch (Exception ex)
            {
                // Hata durumunu yönet
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateDriverDto updateDriverDto)
        {
            var driver = _mapper.Map<Driver>(updateDriverDto);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            // Parametre sırası: id, userId, vehicle
            await _driverService.UpdateAsync(id, userId, driver);

            return NoContent(); // Başarılı update işlemlerinde 204 No Content dönmek standarttır.
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _driverService.DeleteAsync(id);
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

            var result = await _driverService.GetDriversForDropdownAsync(userId);
            return Ok(result);
        }
    }
} 