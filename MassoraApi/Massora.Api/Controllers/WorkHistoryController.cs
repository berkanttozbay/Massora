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
    public class WorkHistoryController : ControllerBase
    {
        private readonly IWorkHistoryService _workHistoryService;
        private readonly IMapper _mapper;

        public WorkHistoryController(IWorkHistoryService workHistoryService, IMapper mapper)
        {
            _workHistoryService = workHistoryService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkHistoryDto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string searchTerm = null)
            
        {
            var loggedInUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // 2. Service metodunu 'await' ile çağırıyoruz.
            var paginatedResult = await _workHistoryService.GetWorkHistoriesPaginatedAsync(loggedInUserId,pageNumber, pageSize, searchTerm);

            // 3. Gelen sonucu doğrudan Ok() ile dönüyoruz.
            return Ok(paginatedResult);
        }
        

        [HttpGet("{id}")]
        public async Task<ActionResult<WorkHistoryDto>> GetById(int id)
        {
            var workHistory = await _workHistoryService.GetByIdAsync(id);
            if (workHistory == null)
                return NotFound();

            var workHistoryDto = _mapper.Map<WorkHistoryDto>(workHistory);
            return Ok(workHistoryDto);
        }

        [HttpGet("driver/{driverId}")]
        public async Task<ActionResult<IEnumerable<WorkHistoryDto>>> GetByDriverId(int driverId)
        {
            var workHistories = await _workHistoryService.GetByDriverIdAsync(driverId);
            var workHistoryDtos = _mapper.Map<IEnumerable<WorkHistoryDto>>(workHistories);
            return Ok(workHistoryDtos);
        }

        [HttpGet("vehicle/{vehicleId}")]
        public async Task<ActionResult<IEnumerable<WorkHistoryDto>>> GetByVehicleId(int vehicleId)
        {
            var workHistories = await _workHistoryService.GetByVehicleIdAsync(vehicleId);
            var workHistoryDtos = _mapper.Map<IEnumerable<WorkHistoryDto>>(workHistories);
            return Ok(workHistoryDtos);
        }

        [HttpGet("company/{companyId}")]
        public async Task<ActionResult<IEnumerable<WorkHistoryDto>>> GetByCompanyId(int companyId)
        {
            var workHistories = await _workHistoryService.GetByCompanyIdAsync(companyId);
            var workHistoryDtos = _mapper.Map<IEnumerable<WorkHistoryDto>>(workHistories);
            return Ok(workHistoryDtos);
        }

        [HttpGet("partner-company/{partnerCompanyId}")]
        public async Task<ActionResult<IEnumerable<WorkHistoryDto>>> GetByPartnerCompanyId(int partnerCompanyId)
        {
            var workHistories = await _workHistoryService.GetByPartnerCompanyIdAsync(partnerCompanyId);
            var workHistoryDtos = _mapper.Map<IEnumerable<WorkHistoryDto>>(workHistories);
            return Ok(workHistoryDtos);
        }

        [HttpGet("date-range")]
        public async Task<ActionResult<IEnumerable<WorkHistoryDto>>> GetByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var workHistories = await _workHistoryService.GetByDateRangeAsync(startDate, endDate);
            var workHistoryDtos = _mapper.Map<IEnumerable<WorkHistoryDto>>(workHistories);
            return Ok(workHistoryDtos);
        }

        [HttpGet("driver/{driverId}/active")]
        public async Task<ActionResult<WorkHistoryDto>> GetActiveWorkByDriverId(int driverId)
        {
            var activeWork = await _workHistoryService.GetActiveWorkByDriverIdAsync(driverId);
            if (activeWork == null)
                return NotFound();

            var workHistoryDto = _mapper.Map<WorkHistoryDto>(activeWork);
            return Ok(workHistoryDto);
        }

        [HttpPost]
        public async Task<ActionResult<WorkHistoryDto>> Create(CreateWorkHistoryDto createWorkHistoryDto)
        {
            var workHistory = _mapper.Map<WorkHistory>(createWorkHistoryDto);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var newFuelHistory = await _workHistoryService.AddAsync(workHistory, userId);
            return CreatedAtAction(nameof(GetById), new { id = newFuelHistory.Id }, workHistory);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateWorkHistoryDto updateWorkHistoryDto)
        {
            var workHistory = _mapper.Map<WorkHistory>(updateWorkHistoryDto);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            // Parametre sırası: id, userId, vehicle
            await _workHistoryService.UpdateAsync(id, userId, workHistory);

            return NoContent(); // Başarılı update işlemlerinde 204 No Content dönmek standarttır.
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _workHistoryService.DeleteAsync(id);
            return NoContent();
        }
    }
} 