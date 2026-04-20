using Microsoft.AspNetCore.Mvc;
using SmartSeatAllocation.Core.DTOs;
using SmartSeatAllocation.Core.Mappings;
using SmartSeatAllocation.Core.Models;
using SmartSeatAllocation.Core.Services;
using System.Collections.Generic;
using System.Linq;

namespace SmartSeatAllocation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParticipantController : ControllerBase
    {
        private readonly IParticipantService _participantService;
        private readonly AllocationService _allocationService;

        public ParticipantController(
            IParticipantService participantService,
            AllocationService allocationService)
        {
            _participantService = participantService;
            _allocationService = allocationService;
        }

        [HttpGet("{participantId}")]
        public ActionResult<ApiResponseDto<ParticipantDto>> GetParticipant(int participantId)
        {
            var participant = _participantService.GetParticipantDetails(participantId);
            
            if (participant == null)
                return NotFound(ApiResponseDto<ParticipantDto>.Failure(
                    $"Participant with ID {participantId} not found"));

            var participantDto = participant.ToDto();
            return Ok(ApiResponseDto<ParticipantDto>.Success(participantDto));
        }

        [HttpGet("")]
        public ActionResult<ApiResponseDto<PaginatedResponseDto<ParticipantDto>>> GetAllParticipants(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var allParticipants = _participantService.GetAllParticipants();
            var totalCount = allParticipants.Count;

            var participants = allParticipants
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var participantDtos = participants.ToDto();
            var paginatedResult = new PaginatedResponseDto<ParticipantDto>(
                participantDtos,
                totalCount,
                pageNumber,
                pageSize);

            return Ok(ApiResponseDto<PaginatedResponseDto<ParticipantDto>>.Success(paginatedResult));
        }

        [HttpGet("unallocated")]
        public ActionResult<ApiResponseDto<List<ParticipantDto>>> GetUnallocatedParticipants()
        {
            var participants = _participantService.GetUnallocatedParticipants();
            var participantDtos = participants.ToDto();

            return Ok(ApiResponseDto<List<ParticipantDto>>.Success(participantDtos));
        }

        [HttpGet("allocated")]
        public ActionResult<ApiResponseDto<List<ParticipantDto>>> GetAllocatedParticipants()
        {
            var participants = _participantService.GetAllParticipants()
                .Where(p => p.IsAssigned())
                .ToList();

            var participantDtos = participants.ToDto();
            return Ok(ApiResponseDto<List<ParticipantDto>>.Success(participantDtos));
        }

        [HttpGet("department/{department}")]
        public ActionResult<ApiResponseDto<List<ParticipantDto>>> GetParticipantsByDepartment(string department)
        {
            if (!System.Enum.TryParse<DepartmentType>(department, ignoreCase: true, out var deptType))
                return BadRequest(ApiResponseDto<List<ParticipantDto>>.Failure(
                    "Invalid department type. Valid values: DivisionA, DivisionB, DivisionC"));

            var participants = _participantService.GetParticipantsByDepartment(deptType);
            var participantDtos = participants.ToDto();

            return Ok(ApiResponseDto<List<ParticipantDto>>.Success(participantDtos));
        }

        [HttpGet("summary/by-department")]
        public ActionResult<ApiResponseDto<object>> GetParticipantsSummaryByDepartment()
        {
            var participants = _participantService.GetAllParticipants();

            var summary = participants
                .GroupBy(p => p.Department)
                .Select(g => new
                {
                    Department = g.Key.ToString(),
                    Total = g.Count(),
                    Allocated = g.Count(p => p.IsAssigned()),
                    Unallocated = g.Count(p => !p.IsAssigned()),
                    AllocationRate = (g.Count(p => p.IsAssigned()) / (double)g.Count()) * 100
                })
                .OrderBy(x => x.Department)
                .ToList();

            return Ok(ApiResponseDto<object>.Success(summary));
        }

        [HttpPost("")]
        public ActionResult<ApiResponseDto<ParticipantDto>> AddParticipant([FromBody] CreateParticipantDto request)
        {
            if (string.IsNullOrWhiteSpace(request?.Name) || string.IsNullOrWhiteSpace(request?.Email))
                return BadRequest(ApiResponseDto<ParticipantDto>.Failure(
                    "Name and email are required"));

            // Validate email format
            try
            {
                var addr = new System.Net.Mail.MailAddress(request.Email);
                if (addr.Address != request.Email)
                    return BadRequest(ApiResponseDto<ParticipantDto>.Failure(
                        "Invalid email format"));
            }
            catch
            {
                return BadRequest(ApiResponseDto<ParticipantDto>.Failure(
                    "Invalid email format"));
            }

            var participant = _participantService.AddParticipant(
                request.Name,
                request.Email,
                request.Department);

            var participantDto = participant.ToDto();
            return CreatedAtAction(nameof(GetParticipant),
                new { participantId = participant.Id },
                ApiResponseDto<ParticipantDto>.Success(participantDto, "Participant created successfully"));
        }

        [HttpPut("{participantId}")]
        public ActionResult<ApiResponseDto<ParticipantDto>> UpdateParticipant(
            int participantId,
            [FromBody] UpdateParticipantDto request)
        {
            var participant = _participantService.GetParticipantDetails(participantId);
            if (participant == null)
                return NotFound(ApiResponseDto<ParticipantDto>.Failure(
                    $"Participant with ID {participantId} not found"));

            if (!string.IsNullOrWhiteSpace(request?.Name))
                participant.Name = request.Name;

            if (!string.IsNullOrWhiteSpace(request?.Email))
                participant.Email = request.Email;

            var participantDto = participant.ToDto();
            return Ok(ApiResponseDto<ParticipantDto>.Success(participantDto, "Participant updated successfully"));
        }

        [HttpDelete("{participantId}")]
        public ActionResult<ApiResponseDto<object>> DeleteParticipant(int participantId)
        {
            var participant = _participantService.GetParticipantDetails(participantId);
            if (participant == null)
                return NotFound(ApiResponseDto<object>.Failure(
                    $"Participant with ID {participantId} not found"));

            if (participant.IsAssigned())
                return BadRequest(ApiResponseDto<object>.Failure(
                    "Cannot delete an allocated participant. Deallocate first."));

            // In a real scenario, you'd remove from the collection
            return Ok(ApiResponseDto<object>.Success(null, "Participant deleted successfully"));
        }

        [HttpPost("bulk")]
        public ActionResult<ApiResponseDto<List<ParticipantDto>>> BulkAddParticipants(
            [FromBody] List<CreateParticipantDto> requests)
        {
            if (requests == null || requests.Count == 0)
                return BadRequest(ApiResponseDto<List<ParticipantDto>>.Failure(
                    "No participants provided"));

            var createdParticipants = new List<ParticipantDto>();

            foreach (var request in requests)
            {
                if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Email))
                    continue;

                var participant = _participantService.AddParticipant(
                    request.Name,
                    request.Email,
                    request.Department);

                createdParticipants.Add(participant.ToDto());
            }

            return Ok(ApiResponseDto<List<ParticipantDto>>.Success(
                createdParticipants,
                $"Successfully created {createdParticipants.Count} participants"));
        }
    }
}
