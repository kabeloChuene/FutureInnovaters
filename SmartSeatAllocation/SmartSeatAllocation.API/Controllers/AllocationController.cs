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
    public class AllocationController : ControllerBase
    {
        private readonly IAllocationService _allocationService;
        private readonly ISessionService _sessionService;
        private readonly IParticipantService _participantService;

        public AllocationController(
            IAllocationService allocationService,
            ISessionService sessionService,
            IParticipantService participantService)
        {
            _allocationService = allocationService;
            _sessionService = sessionService;
            _participantService = participantService;
        }

    
        [HttpPost("allocate")]
        public ActionResult<ApiResponseDto<AllocationResponseDto>> AllocateParticipant([FromBody] AllocationRequestDto request)
        {
            if (request?.ParticipantId <= 0 || request?.SessionId <= 0)
                return BadRequest(ApiResponseDto<AllocationResponseDto>.Failure(
                    "Invalid participant or session ID",
                    "INVALID_INPUT"));

            var result = _allocationService.AllocateParticipant(request.ParticipantId, request.SessionId);
            var responseDto = result.ToResponseDto();

            if (!result.IsSuccess)
                return BadRequest(ApiResponseDto<AllocationResponseDto>.Failure(responseDto.Message));

            return Ok(ApiResponseDto<AllocationResponseDto>.Success(responseDto, "Participant allocated successfully"));
        }

        [HttpPost("deallocate/{participantId}")]
        public ActionResult<ApiResponseDto<AllocationResponseDto>> DeallocateParticipant(int participantId)
        {
            var result = _allocationService.DeallocateParticipant(participantId);
            var responseDto = result.ToResponseDto();

            if (!result.IsSuccess)
                return BadRequest(ApiResponseDto<AllocationResponseDto>.Failure(responseDto.Message));

            return Ok(ApiResponseDto<AllocationResponseDto>.Success(responseDto, "Participant deallocated successfully"));
        }

        [HttpGet("participant/{participantId}/status")]
        public ActionResult<ApiResponseDto<AllocationResponseDto>> GetParticipantStatus(int participantId)
        {
            var result = _allocationService.GetAllocationStatus(participantId);
            var responseDto = result.ToResponseDto();

            if (!result.IsSuccess)
                return NotFound(ApiResponseDto<AllocationResponseDto>.Failure(responseDto.Message));

            return Ok(ApiResponseDto<AllocationResponseDto>.Success(responseDto));
        }

        [HttpGet("session/{sessionId}/allocations")]
        public ActionResult<ApiResponseDto<SessionDetailDto>> GetSessionAllocations(int sessionId)
        {
            var session = _sessionService.GetSessionDetails(sessionId);
            if (session == null)
                return NotFound(ApiResponseDto<SessionDetailDto>.Failure($"Session with ID {sessionId} not found"));

            var participants = _participantService.GetAllParticipants()
                .Where(p => p.AssignedSessionId == sessionId)
                .ToList();

            var sessionDetail = new SessionDetailDto
            {
                Id = session.Id,
                Name = session.Name,
                Type = session.Type.ToString(),
                TimeSlot = session.TimeSlot,
                Capacity = session.Capacity,
                CurrentOccupancy = session.CurrentOccupancy,
                AvailableSeats = session.GetAvailableSeats(),
                OccupancyPercentage = (session.CurrentOccupancy / (double)session.Capacity) * 100,
                DepartmentBreakdown = session.DepartmentAllocations
                    .ToDictionary(k => k.Key.ToString(), v => v.Value),
                AllocatedParticipants = participants.ToSummaryDto()
            };

            return Ok(ApiResponseDto<SessionDetailDto>.Success(sessionDetail));
        }

        [HttpGet("validate")]
        public ActionResult<ApiResponseDto<AllocationValidationDto>> ValidateAllocation(
            [FromQuery] int participantId, 
            [FromQuery] int sessionId)
        {
            if (participantId <= 0 || sessionId <= 0)
                return BadRequest(ApiResponseDto<AllocationValidationDto>.Failure(
                    "Invalid participant or session ID"));

            var canAllocate = _allocationService.CanAllocate(participantId, sessionId);
            var message = _allocationService.GetValidationMessage(participantId, sessionId);

            var validationDto = canAllocate
                ? AllocationValidationDto.Valid(participantId, sessionId)
                : AllocationValidationDto.Invalid(participantId, sessionId, message);

            return Ok(ApiResponseDto<AllocationValidationDto>.Success(validationDto));
        }

        [HttpPost("batch-allocate")]
        public ActionResult<ApiResponseDto<BatchAllocationResultDto>> BatchAllocate(
            [FromBody] BatchAllocationDto batchRequest)
        {
            if (batchRequest?.Allocations == null || batchRequest.Allocations.Count == 0)
                return BadRequest(ApiResponseDto<BatchAllocationResultDto>.Failure(
                    "No allocations provided"));

            var result = new BatchAllocationResultDto();

            foreach (var allocation in batchRequest.Allocations)
            {
                var allocationResult = _allocationService.AllocateParticipant(
                    allocation.ParticipantId,
                    allocation.SessionId);

                result.Results.Add(allocationResult.ToResponseDto());

                if (allocationResult.IsSuccess)
                    result.SuccessCount++;
                else
                    result.FailureCount++;
            }

            result.AllSuccessful = result.FailureCount == 0;

            return Ok(ApiResponseDto<BatchAllocationResultDto>.Success(
                result,
                $"Batch allocation completed: {result.SuccessCount} successful, {result.FailureCount} failed"));
        }

        [HttpGet("report")]
        public ActionResult<ApiResponseDto<object>> GetAllocationReport()
        {
            var allParticipants = _participantService.GetAllParticipants();
            var allocatedCount = allParticipants.Count(p => p.IsAssigned());
            var unallocatedCount = allParticipants.Count(p => !p.IsAssigned());

            var report = new
            {
                TotalParticipants = allParticipants.Count,
                AllocatedParticipants = allocatedCount,
                UnallocatedParticipants = unallocatedCount,
                AllocationRate = (allocatedCount / (double)allParticipants.Count) * 100,
                ByDepartment = allParticipants
                    .GroupBy(p => p.Department)
                    .Select(g => new
                    {
                        Department = g.Key.ToString(),
                        Total = g.Count(),
                        Allocated = g.Count(p => p.IsAssigned()),
                        Unallocated = g.Count(p => !p.IsAssigned())
                    })
                    .ToList()
            };

            return Ok(ApiResponseDto<object>.Success(report));
        }
    }
}
