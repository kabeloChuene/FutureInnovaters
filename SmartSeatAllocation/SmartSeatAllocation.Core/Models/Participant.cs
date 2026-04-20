namespace SmartSeatAllocation.Core.Models
{
    public class Participant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DepartmentType Department { get; set; }
        public int? AssignedSessionId { get; set; }
        public SessionType? AssignedSessionType { get; set; }

        public Participant()
        {
        }

        public Participant(string name, string email, DepartmentType department)
        {
            Name = name;
            Email = email;
            Department = department;
            AssignedSessionId = null;
            AssignedSessionType = null;
        }

        public bool IsAssigned()
        {
            return AssignedSessionId.HasValue;
        }
    }
}
