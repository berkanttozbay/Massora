using AutoMapper;
using Massora.Business.DTOs;
using Massora.Business.Services;
using Massora.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Massora.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Sadece Admin veya Şirket Kullanıcıları erişebilir.
    public class PartnerCompanyController : ControllerBase
    {
        private readonly IPartnerCompanyService _partnerCompanyService;
        private readonly IMapper _mapper;
        private readonly ILogger<PartnerCompanyController> _logger;


        public PartnerCompanyController(IPartnerCompanyService partnerCompanyService, IMapper mapper, ILogger<PartnerCompanyController> logger)
        {
            _partnerCompanyService = partnerCompanyService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        // Sadece Admin veya Şirket Kullanıcıları erişebilir.
        public async Task<ActionResult<IEnumerable<PartnerCompanyDto>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string searchTerm = null)
        {
            var loggedInUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // 2. Service metodunu 'await' ile çağırıyoruz.
            var paginatedResult = await _partnerCompanyService.GetPartnerCompaniesPaginatedAsync(loggedInUserId,pageNumber, pageSize, searchTerm);

            // 3. Gelen sonucu doğrudan Ok() ile dönüyoruz.
            return Ok(paginatedResult);
        }
        

        [HttpGet("{id}")]
        public async Task<ActionResult<PartnerCompanyDto>> GetById(int id)
        {
            var partnerCompany = await _partnerCompanyService.GetByIdAsync(id);
            if (partnerCompany == null)
                return NotFound();

            var partnerCompanyDto = _mapper.Map<PartnerCompanyDto>(partnerCompany);
            return Ok(partnerCompanyDto);
        }

        [HttpGet("company/{companyId}")]
        public async Task<ActionResult<IEnumerable<PartnerCompanyDto>>> GetByCompanyId(int companyId)
        {
            var partnerCompanies = await _partnerCompanyService.GetByCompanyIdAsync(companyId);
            var partnerCompanyDtos = _mapper.Map<IEnumerable<PartnerCompanyDto>>(partnerCompanies);
            return Ok(partnerCompanyDtos);
        }

        [HttpPost]
        public async Task<ActionResult<PartnerCompanyDto>> Create(CreatePartnerCompanyDto createPartnerCompanyDto)
        {
            var partnerCompany = _mapper.Map<PartnerCompany>(createPartnerCompanyDto);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var newPartnerCompany = await _partnerCompanyService.AddAsync(partnerCompany, userId);
            var vehicleDto = _mapper.Map<PartnerCompanyDto>(partnerCompany);
            return CreatedAtAction(nameof(GetById), new { id = partnerCompany.Id }, vehicleDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdatePartnerCompanyDto updatePartnerCompanyDto)
        {
            var vehicle = _mapper.Map<PartnerCompany>(updatePartnerCompanyDto);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            // Parametre sırası: id, userId, vehicle
            await _partnerCompanyService.UpdateAsync(id, userId, vehicle);

            return NoContent(); // Başarılı update işlemlerinde 204 No Content dönmek standarttır.
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _partnerCompanyService.DeleteAsync(id);
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

            var result = await _partnerCompanyService.GetPartnerCompaniesForDropdownAsync(userId);
            return Ok(result);
        }
    }
} 