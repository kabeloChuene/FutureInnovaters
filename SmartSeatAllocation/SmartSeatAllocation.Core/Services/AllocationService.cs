using System;
using System.Collections.Generic;
using System.Linq;
using SmartSeatAllocation.Core.Models;

namespace SmartSeatAllocation.Core.Services
{
    public interface IAllocationService
    {
        AllocationResult AllocateParticipant(int participantId, int sessionId);
        AllocationResult DeallocateParticipant(int participantId);
        AllocationResult GetAllocationStatus(int participantId);
        List<AllocationResult> GetSessionAllocationStatus(int sessionId);
        bool CanAllocate(int participantId, int sessionId);
        string GetValidationMessage(int participantId, int sessionId);
    }

    public class AllocationService : IAllocationService
    {
        private readonly List<Participant> _participants;
        private readonly List<Session> _sessions;
        private readonly List<Department> _departments;

        public AllocationService()
        {
            _participants = new List<Participant>();
            _sessions = new List<Session>();
            _departments = new List<Department>();
            InitializeDefaultData();
        }

        private void InitializeDefaultData()
        {
            // Initialize sessions
            _sessions.Add(new Session(SessionType.Morning) { Id = 1 });
            _sessions.Add(new Session(SessionType.Midday) { Id = 2 });
            _sessions.Add(new Session(SessionType.Afternoon) { Id = 3 });

            // Initialize departments
            _departments.Add(new Department(DepartmentType.DivisionA) { Id = 1 });
            _departments.Add(new Department(DepartmentType.DivisionB) { Id = 2 });
            _departments.Add(new Department(DepartmentType.DivisionC) { Id = 3 });
        }

        // User-facing API Methods
        public AllocationResult AllocateParticipant(int participantId, int sessionId)
        {
            var participant = _participants.FirstOrDefault(p => p.Id == participantId);
            if (participant == null)
                return AllocationResult.Failure($"Participant with ID {participantId} not found");

            var session = _sessions.FirstOrDefault(s => s.Id == sessionId);
            if (session == null)
                return AllocationResult.Failure($"Session with ID {sessionId} not found");

            // Validate allocation
            if (!CanAllocate(participantId, sessionId))
                return AllocationResult.Failure(GetValidationMessage(participantId, sessionId));

            // Perform allocation
            participant.AssignedSessionId = sessionId;
            participant.AssignedSessionType = session.Type;
            session.CurrentOccupancy++;
            session.DepartmentAllocations[participant.Department]++;

            return AllocationResult.Success(participant, session);
        }

        public AllocationResult DeallocateParticipant(int participantId)
        {
            var participant = _participants.FirstOrDefault(p => p.Id == participantId);
            if (participant == null)
                return AllocationResult.Failure($"Participant with ID {participantId} not found");

            if (!participant.IsAssigned())
                return AllocationResult.Failure($"Participant {participant.Name} is not assigned to any session");

            var session = _sessions.FirstOrDefault(s => s.Id == participant.AssignedSessionId);
            if (session != null)
            {
                session.CurrentOccupancy--;
                session.DepartmentAllocations[participant.Department]--;
            }

            participant.AssignedSessionId = null;
            participant.AssignedSessionType = null;

            return new AllocationResult(true, $"Successfully deallocated {participant.Name}");
        }

        public AllocationResult GetAllocationStatus(int participantId)
        {
            var participant = _participants.FirstOrDefault(p => p.Id == participantId);
            if (participant == null)
                return AllocationResult.Failure($"Participant with ID {participantId} not found");

            if (!participant.IsAssigned())
                return AllocationResult.Failure($"{participant.Name} is not yet allocated to any session");

            var session = _sessions.FirstOrDefault(s => s.Id == participant.AssignedSessionId);
            return new AllocationResult(true, $"{participant.Name} is allocated to {session?.Name} session", participant, session);
        }

        public List<AllocationResult> GetSessionAllocationStatus(int sessionId)
        {
            var session = _sessions.FirstOrDefault(s => s.Id == sessionId);
            if (session == null)
                return new List<AllocationResult> { AllocationResult.Failure($"Session with ID {sessionId} not found") };

            var results = new List<AllocationResult>();
            results.Add(new AllocationResult(true, 
                $"Session: {session.Name} ({session.TimeSlot}) - Occupancy: {session.CurrentOccupancy}/{session.Capacity}"));

            var assignedParticipants = _participants.Where(p => p.AssignedSessionId == sessionId).ToList();
            foreach (var participant in assignedParticipants)
            {
                results.Add(new AllocationResult(true, 
                    $"  - {participant.Name} ({participant.Department})", participant, session));
            }

            return results;
        }

        public bool CanAllocate(int participantId, int sessionId)
        {
            var participant = _participants.FirstOrDefault(p => p.Id == participantId);
            if (participant == null)
                return false;

            var session = _sessions.FirstOrDefault(s => s.Id == sessionId);
            if (session == null)
                return false;

            // Rule 1: Check if participant is already allocated
            if (participant.IsAssigned())
                return false;

            // Rule 2: Check session capacity
            if (!session.HasAvailableSeats())
                return false;

            // Rule 3: Check department limit for this session
            if (session.DepartmentAllocations[participant.Department] >= GetDepartmentMaxPerSession(participant.Department))
                return false;

            // Rule 4: Check department total allocation
            var departmentAllocated = _participants
                .Where(p => p.Department == participant.Department && p.IsAssigned())
                .Count();

            var department = _departments.FirstOrDefault(d => d.Type == participant.Department);
            if (departmentAllocated >= department.SeatsAllocated)
                return false;

            return true;
        }

        public string GetValidationMessage(int participantId, int sessionId)
        {
            var participant = _participants.FirstOrDefault(p => p.Id == participantId);
            if (participant == null)
                return $"Participant with ID {participantId} not found";

            var session = _sessions.FirstOrDefault(s => s.Id == sessionId);
            if (session == null)
                return $"Session with ID {sessionId} not found";

            // Check each validation rule in order
            if (participant.IsAssigned())
                return $"{participant.Name} is already assigned to {participant.AssignedSessionType} session";

            if (!session.HasAvailableSeats())
                return $"{session.Name} session is full ({session.CurrentOccupancy}/{session.Capacity})";

            if (session.DepartmentAllocations[participant.Department] >= GetDepartmentMaxPerSession(participant.Department))
                return $"{participant.Department} has reached its limit of {GetDepartmentMaxPerSession(participant.Department)} seats in {session.Name} session";

            var departmentAllocated = _participants
                .Where(p => p.Department == participant.Department && p.IsAssigned())
                .Count();

            var department = _departments.FirstOrDefault(d => d.Type == participant.Department);
            if (departmentAllocated >= department.SeatsAllocated)
                return $"{participant.Department} has reached its total allocation limit ({department.SeatsAllocated} seats)";

            return "Unknown validation error";
        }

        // Helper methods
        public int GetDepartmentMaxPerSession(DepartmentType departmentType)
        {
            var department = _departments.FirstOrDefault(d => d.Type == departmentType);
            return department?.MaxPerSession ?? 0;
        }

        public Session GetSession(int sessionId)
        {
            return _sessions.FirstOrDefault(s => s.Id == sessionId);
        }

        public Participant GetParticipant(int participantId)
        {
            return _participants.FirstOrDefault(p => p.Id == participantId);
        }

        public List<Session> GetAllSessions()
        {
            return _sessions;
        }

        public List<Participant> GetAllParticipants()
        {
            return _participants;
        }

        public Participant AddParticipant(string name, string email, DepartmentType department)
        {
            var participant = new Participant(name, email, department)
            {
                Id = _participants.Count + 1
            };
            _participants.Add(participant);
            return participant;
        }

        public void ClearAllAllocations()
        {
            foreach (var participant in _participants)
            {
                participant.AssignedSessionId = null;
                participant.AssignedSessionType = null;
            }

            foreach (var session in _sessions)
            {
                session.CurrentOccupancy = 0;
                foreach (var dept in session.DepartmentAllocations.Keys.ToList())
                {
                    session.DepartmentAllocations[dept] = 0;
                }
            }
        }
    }
}
