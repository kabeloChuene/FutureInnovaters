using System.Collections.Generic;
using System.Linq;
using SmartSeatAllocation.Core.Models;

namespace SmartSeatAllocation.Core.Services
{
    public interface IParticipantService
    {
        Participant GetParticipantDetails(int participantId);
        List<Participant> GetAllParticipants();
        List<Participant> GetUnallocatedParticipants();
        List<Participant> GetParticipantsByDepartment(DepartmentType department);
        Participant AddParticipant(string name, string email, DepartmentType department);
    }

    public class ParticipantService : IParticipantService
    {
        private readonly AllocationService _allocationService;

        public ParticipantService(AllocationService allocationService)
        {
            _allocationService = allocationService;
        }

        public Participant GetParticipantDetails(int participantId)
        {
            return _allocationService.GetParticipant(participantId);
        }

        public List<Participant> GetAllParticipants()
        {
            return _allocationService.GetAllParticipants();
        }

        public List<Participant> GetUnallocatedParticipants()
        {
            return _allocationService.GetAllParticipants()
                .Where(p => !p.IsAssigned())
                .ToList();
        }

        public List<Participant> GetParticipantsByDepartment(DepartmentType department)
        {
            return _allocationService.GetAllParticipants()
                .Where(p => p.Department == department)
                .ToList();
        }

        public Participant AddParticipant(string name, string email, DepartmentType department)
        {
            return _allocationService.AddParticipant(name, email, department);
        }
    }
}
