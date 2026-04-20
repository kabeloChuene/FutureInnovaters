using System;
using System.Collections.Generic;
using SmartSeatAllocation.Core.Models;

namespace SmartSeatAllocation.Core.DTOs
{
    public class CreateParticipantDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public DepartmentType Department { get; set; }
    }

    public class UpdateParticipantDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class ParticipantDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        public int? AssignedSessionId { get; set; }
        public string AssignedSessionType { get; set; }
        public bool IsAssigned { get; set; }

        public static ParticipantDto FromEntity(Participant participant)
        {
            return new ParticipantDto
            {
                Id = participant.Id,
                Name = participant.Name,
                Email = participant.Email,
                Department = participant.Department.ToString(),
                AssignedSessionId = participant.AssignedSessionId,
                AssignedSessionType = participant.AssignedSessionType?.ToString(),
                IsAssigned = participant.IsAssigned()
            };
        }
    }

    public class ParticipantSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public int? AssignedSessionId { get; set; }

        public static ParticipantSummaryDto FromEntity(Participant participant)
        {
            return new ParticipantSummaryDto
            {
                Id = participant.Id,
                Name = participant.Name,
                Department = participant.Department.ToString(),
                AssignedSessionId = participant.AssignedSessionId
            };
        }
    }
}
                AssignedSessionId = participant.AssignedSessionId
            };
        }
    }
}
