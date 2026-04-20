using System;
using System.Collections.Generic;
using SmartSeatAllocation.Core.Models;

namespace SmartSeatAllocation.Core.DTOs
{
    public class SessionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Stage { get; set; }
        public string TimeSlot { get; set; }
        public int Capacity { get; set; }
        public int CurrentOccupancy { get; set; }
        public int TotalAttendees { get; set; }
        public int SeatsLeft { get; set; }
        public int AvailableSeats { get; set; }
        public double OccupancyPercentage { get; set; }
        public int OwnerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public static SessionDto FromEntity(Session session)
        {
            return new SessionDto
            {
                Id = session.Id,
                Name = session.Name,
                Description = session.Description,
                Type = session.Type.ToString(),
                Stage = session.Stage.ToString(),
                TimeSlot = session.TimeSlot,
                Capacity = session.Capacity,
                CurrentOccupancy = session.CurrentOccupancy,
                TotalAttendees = session.TotalAttendees,
                SeatsLeft = session.SeatsLeft,
                AvailableSeats = session.GetAvailableSeats(),
                OccupancyPercentage = (session.CurrentOccupancy / (double)session.Capacity) * 100,
                OwnerId = session.OwnerId,
                CreatedAt = session.CreatedAt,
                UpdatedAt = session.UpdatedAt
            };
        }
    }

    public class SessionCreateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string TimeSlot { get; set; }
        public int TotalAttendees { get; set; }
        public SessionStage Stage { get; set; }
    }

    public class SessionUpdateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string TimeSlot { get; set; }
        public int TotalAttendees { get; set; }
        public SessionStage Stage { get; set; }
    }

    public class SessionDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Stage { get; set; }
        public string TimeSlot { get; set; }
        public int Capacity { get; set; }
        public int CurrentOccupancy { get; set; }
        public int TotalAttendees { get; set; }
        public int SeatsLeft { get; set; }
        public int AvailableSeats { get; set; }
        public double OccupancyPercentage { get; set; }
        public Dictionary<string, int> DepartmentBreakdown { get; set; }
        public List<ParticipantSummaryDto> AllocatedParticipants { get; set; }
        public int OwnerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public SessionDetailDto()
        {
            DepartmentBreakdown = new Dictionary<string, int>();
            AllocatedParticipants = new List<ParticipantSummaryDto>();
        }
    }

    public class SessionStatisticsDto
    {
        public SessionDto Session { get; set; }
        public int TotalAllocated { get; set; }
        public int AvailableSeats { get; set; }
        public double OccupancyPercentage { get; set; }
        public Dictionary<string, int> DepartmentBreakdown { get; set; }

        public SessionStatisticsDto()
        {
            DepartmentBreakdown = new Dictionary<string, int>();
        }
    }

    public class SessionsSummaryDto
    {
        public List<SessionDto> Sessions { get; set; }
        public int TotalCapacity { get; set; }
        public int TotalOccupancy { get; set; }
        public int TotalAvailableSeats { get; set; }
        public double OverallOccupancyPercentage { get; set; }

        public SessionsSummaryDto()
        {
            Sessions = new List<SessionDto>();
        }
    }

    public class SessionFeedItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Stage { get; set; }
        public string TimeSlot { get; set; }
        public int CurrentOccupancy { get; set; }
        public int SeatsLeft { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
