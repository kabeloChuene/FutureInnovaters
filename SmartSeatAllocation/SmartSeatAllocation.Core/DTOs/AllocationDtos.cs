using System;
using System.Collections.Generic;

namespace SmartSeatAllocation.Core.DTOs
{
    public class AllocationRequestDto
    {
        public int ParticipantId { get; set; }
        public int SessionId { get; set; }
    }

    public class AllocationResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int? ParticipantId { get; set; }
        public int? SessionId { get; set; }
        public ParticipantDto? Participant { get; set; }
        public SessionDto? Session { get; set; }
        public string? ErrorCode { get; set; }

        public static AllocationResponseDto Success(ParticipantDto participant, SessionDto session, string message)
        {
            return new AllocationResponseDto
            {
                IsSuccess = true,
                Message = message,
                ParticipantId = participant?.Id,
                SessionId = session?.Id,
                Participant = participant,
                Session = session
            };
        }

        public static AllocationResponseDto Failure(string message, string errorCode = null)
        {
            return new AllocationResponseDto
            {
                IsSuccess = false,
                Message = message,
                ErrorCode = errorCode ?? "ALLOCATION_FAILED"
            };
        }
    }

    public class AllocationValidationDto
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public int ParticipantId { get; set; }
        public int SessionId { get; set; }
        public List<string> ValidationErrors { get; set; }

        public AllocationValidationDto()
        {
            ValidationErrors = new List<string>();
        }

        public static AllocationValidationDto Valid(int participantId, int sessionId)
        {
            return new AllocationValidationDto
            {
                IsValid = true,
                Message = "Allocation is valid",
                ParticipantId = participantId,
                SessionId = sessionId
            };
        }

        public static AllocationValidationDto Invalid(int participantId, int sessionId, params string[] errors)
        {
            return new AllocationValidationDto
            {
                IsValid = false,
                Message = "Allocation validation failed",
                ParticipantId = participantId,
                SessionId = sessionId,
                ValidationErrors = new List<string>(errors)
            };
        }
    }

    public class BatchAllocationDto
    {
        public List<AllocationRequestDto> Allocations { get; set; }

        public BatchAllocationDto()
        {
            Allocations = new List<AllocationRequestDto>();
        }
    }

    public class BatchAllocationResultDto
    {
        public bool AllSuccessful { get; set; }
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public List<AllocationResponseDto> Results { get; set; }

        public BatchAllocationResultDto()
        {
            Results = new List<AllocationResponseDto>();
        }
    }

    public class DeallocationRequestDto
    {
        public int ParticipantId { get; set; }
    }
}
