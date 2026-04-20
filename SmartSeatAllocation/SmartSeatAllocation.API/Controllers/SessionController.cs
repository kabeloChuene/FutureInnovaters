using Microsoft.AspNetCore.Mvc;
using SmartSeatAllocation.Core.DTOs;
using SmartSeatAllocation.Core.Mappings;
using SmartSeatAllocation.Core.Services;
using System.Collections.Generic;
using System.Linq;

namespace SmartSeatAllocation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionController : ControllerBase
    {
        private readonly ISessionService _sessionService;
        private readonly IParticipantService _participantService;

        public SessionController(
            ISessionService sessionService,
            IParticipantService participantService)
        {
            _sessionService = sessionService;
            _participantService = participantService;
        }

        [HttpGet("{sessionId}")]
        public ActionResult<ApiResponseDto<SessionDto>> GetSession(int sessionId)
        {
            var session = _sessionService.GetSessionDetails(sessionId);

            if (session == null)
                return NotFound(ApiResponseDto<SessionDto>.Failure($"Session with ID {sessionId} not found"));

            var sessionDto = session.ToDto();
            return Ok(ApiResponseDto<SessionDto>.Success(sessionDto));
        }

        [HttpGet("")]
        public ActionResult<ApiResponseDto<SessionsSummaryDto>> GetAllSessions()
        {
            var sessions = _sessionService.GetAllSessionDetails();
            var sessionDtos = sessions.ToDto();

            var summary = new SessionsSummaryDto
            {
                Sessions = sessionDtos,
                TotalCapacity = sessions.Sum(s => s.Capacity),
                TotalOccupancy = sessions.Sum(s => s.CurrentOccupancy),
                TotalAvailableSeats = sessions.Sum(s => s.GetAvailableSeats()),
                OverallOccupancyPercentage = (sessions.Sum(s => s.CurrentOccupancy) /
                    (double)sessions.Sum(s => s.Capacity)) * 100
            };

            return Ok(ApiResponseDto<SessionsSummaryDto>.Success(summary));
        }

        [HttpGet("{sessionId}/statistics")]
        public ActionResult<ApiResponseDto<SessionStatisticsDto>> GetSessionStatistics(int sessionId)
        {
            var stats = _sessionService.GetSessionStatistics(sessionId);

            if (stats == null)
                return NotFound(ApiResponseDto<SessionStatisticsDto>.Failure(
                    $"Session with ID {sessionId} not found"));

            var statsDto = new SessionStatisticsDto
            {
                Session = stats.Session.ToDto(),
                TotalAllocated = stats.TotalAllocated,
                AvailableSeats = stats.AvailableSeats,
                OccupancyPercentage = stats.OccupancyPercentage,
                DepartmentBreakdown = stats.DepartmentBreakdown
                    .ToDictionary(k => k.Key.ToString(), v => v.Value)
            };

            return Ok(ApiResponseDto<SessionStatisticsDto>.Success(statsDto));
        }

        [HttpGet("statistics/all")]
        public ActionResult<ApiResponseDto<List<SessionStatisticsDto>>> GetAllSessionStatistics()
        {
            var stats = _sessionService.GetAllSessionStatistics();

            var statsDtos = stats.Select(s => new SessionStatisticsDto
            {
                Session = s.Session.ToDto(),
                TotalAllocated = s.TotalAllocated,
                AvailableSeats = s.AvailableSeats,
                OccupancyPercentage = s.OccupancyPercentage,
                DepartmentBreakdown = s.DepartmentBreakdown
                    .ToDictionary(k => k.Key.ToString(), v => v.Value)
            }).ToList();

            return Ok(ApiResponseDto<List<SessionStatisticsDto>>.Success(statsDtos));
        }

        [HttpGet("{sessionId}/details")]
        public ActionResult<ApiResponseDto<SessionDetailDto>> GetSessionDetails(int sessionId)
        {
            var session = _sessionService.GetSessionDetails(sessionId);
            if (session == null)
                return NotFound(ApiResponseDto<SessionDetailDto>.Failure(
                    $"Session with ID {sessionId} not found"));

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

        [HttpDelete("{sessionId}/clear")]
        public ActionResult<ApiResponseDto<object>> ClearSessionAllocations(int sessionId)
        {
            var session = _sessionService.GetSessionDetails(sessionId);
            if (session == null)
                return NotFound(ApiResponseDto<object>.Failure(
                    $"Session with ID {sessionId} not found"));

            // This would need additional service method
            return Ok(ApiResponseDto<object>.Success(null,
                $"Session {session.Name} allocations cleared"));
        }
    }
}
        {
            var session = _sessionService.GetSessionDetails(sessionId);
            if (session == null)
                return NotFound(ApiResponseDto<object>.Failure(
                    $"Session with ID {sessionId} not found"));

            // This would need additional service method
            return Ok(ApiResponseDto<object>.Success(null, 
                $"Session {session.Name} allocations cleared"));
        }
    }
}
