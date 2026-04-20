using System;
using System.Collections.Generic;

namespace SmartSeatAllocation.Core.Models
{
    public enum SessionType
    {
        Morning,
        Midday,
        Afternoon,
        Custom
    }

    public enum SessionStage
    {
        Upcoming,
        InProgress,
        Completed
    }

    public class Session
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public SessionType Type { get; set; }
        public SessionStage Stage { get; set; }
        public string TimeSlot { get; set; }
        public int Capacity { get; set; }
        public int CurrentOccupancy { get; set; }
        public int TotalAttendees { get; set; }
        public int OwnerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Dictionary<DepartmentType, int> DepartmentAllocations { get; set; }

        public Session()
        {
            DepartmentAllocations = new Dictionary<DepartmentType, int>();
            Name = string.Empty;
            Description = string.Empty;
            TimeSlot = string.Empty;
            Type = SessionType.Custom;
            Stage = SessionStage.Upcoming;
            Capacity = 20;
            CurrentOccupancy = 0;
            TotalAttendees = Capacity;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            InitializeDepartmentAllocations();
        }

        public Session(SessionType type) : this()
        {
            Type = type;
            SetupSessionDefaults();
        }

        private void SetupSessionDefaults()
        {
            switch (Type)
            {
                case SessionType.Morning:
                    Name = "Morning";
                    TimeSlot = "09:00 - 10:30";
                    Description = "Morning training session";
                    break;
                case SessionType.Midday:
                    Name = "Midday";
                    TimeSlot = "11:00 - 12:30";
                    Description = "Midday training session";
                    break;
                case SessionType.Afternoon:
                    Name = "Afternoon";
                    TimeSlot = "13:00 - 14:30";
                    Description = "Afternoon training session";
                    break;
                case SessionType.Custom:
                    Name = "Custom Session";
                    TimeSlot = "TBD";
                    Description = "Custom training session";
                    break;
            }
        }

        private void InitializeDepartmentAllocations()
        {
            DepartmentAllocations[DepartmentType.DivisionA] = 0;
            DepartmentAllocations[DepartmentType.DivisionB] = 0;
            DepartmentAllocations[DepartmentType.DivisionC] = 0;
        }

        public int SeatsLeft => Capacity - CurrentOccupancy;

        public int GetAvailableSeats()
        {
            return SeatsLeft;
        }

        public bool HasAvailableSeats()
        {
            return GetAvailableSeats() > 0;
        }

        public bool IsOwner(int userId)
        {
            return OwnerId == userId;
        }

        public void Touch()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
