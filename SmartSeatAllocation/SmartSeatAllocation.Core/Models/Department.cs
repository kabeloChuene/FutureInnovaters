namespace SmartSeatAllocation.Core.Models
{
    public enum DepartmentType
    {
        DivisionA,
        DivisionB,
        DivisionC
    }

    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DepartmentType Type { get; set; }
        public int TotalParticipants { get; set; }
        public int SeatsAllocated { get; set; }
        public int MaxPerSession { get; set; }

        public Department()
        {
        }

        public Department(DepartmentType type)
        {
            Type = type;
            SetupDepartmentDefaults();
        }

        private void SetupDepartmentDefaults()
        {
            switch (Type)
            {
                case DepartmentType.DivisionA:
                    Name = "Division A";
                    TotalParticipants = 24;
                    SeatsAllocated = 24;
                    MaxPerSession = 8;
                    break;
                case DepartmentType.DivisionB:
                    Name = "Division B";
                    TotalParticipants = 18;
                    SeatsAllocated = 18;
                    MaxPerSession = 8;
                    break;
                case DepartmentType.DivisionC:
                    Name = "Division C";
                    TotalParticipants = 18;
                    SeatsAllocated = 18;
                    MaxPerSession = 6;
                    break;
            }
        }
    }
}
