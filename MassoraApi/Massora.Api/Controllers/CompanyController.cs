using Microsoft.AspNetCore.Mvc;
using Massora.Business.Services;
using Massora.Domain.Entities;
using Massora.Business.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace Massora.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        private readonly IMapper _mapper;

        public CompanyController(ICompanyService companyService, IMapper mapper)
        {
            _companyService = companyService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompanyDto>>> GetAll()
        {
            var companies = await _companyService.GetAllAsync();
            var companyDtos = _mapper.Map<IEnumerable<CompanyDto>>(companies);
            return Ok(companyDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CompanyDto>> GetById(int id)
        {
            var company = await _companyService.GetByIdAsync(id);
            if (company == null)
                return NotFound();

            var companyDto = _mapper.Map<CompanyDto>(company);
            return Ok(companyDto);
        }

        [HttpGet("responsible-user/{responsibleUserId}")]
        public async Task<ActionResult<IEnumerable<CompanyDto>>> GetByResponsibleUserId(int responsibleUserId)
        {
            var companies = await _companyService.GetByResponsibleUserIdAsync(responsibleUserId);
            var companyDtos = _mapper.Map<IEnumerable<CompanyDto>>(companies);
            return Ok(companyDtos);
        }

        [HttpPost]
        public async Task<ActionResult<CompanyDto>> Create(CreateCompanyDto createCompanyDto)
        {
            var company = _mapper.Map<Company>(createCompanyDto);
            var createdCompany = await _companyService.AddAsync(company);
            var companyDto = _mapper.Map<CompanyDto>(createdCompany);
            return CreatedAtAction(nameof(GetById), new { id = companyDto.Id }, companyDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateCompanyDto updateCompanyDto)
        {
            var existingCompany = await _companyService.GetByIdAsync(id);
            if (existingCompany == null)
                return NotFound();

            // Update işlemi
            existingCompany.Name = updateCompanyDto.Name;
            existingCompany.ContactPhone = updateCompanyDto.ContactPhone;
            existingCompany.ContactEmail = updateCompanyDto.ContactEmail;
            existingCompany.Address = updateCompanyDto.Address;

            var updatedCompany = await _companyService.UpdateAsync(existingCompany);
            var companyDto = _mapper.Map<CompanyDto>(updatedCompany);
            return Ok(companyDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _companyService.DeleteAsync(id);
            return NoContent();
        }
    }
} 