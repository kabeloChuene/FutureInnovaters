using System.Collections.Generic;
using System.Linq;
using SmartSeatAllocation.Core.Models;

namespace SmartSeatAllocation.Core.Services
{
    public interface ISessionService
    {
        Session GetSessionDetails(int sessionId);
        List<Session> GetAllSessionDetails();
        SessionStatistics GetSessionStatistics(int sessionId);
        List<SessionStatistics> GetAllSessionStatistics();
    }

    public class SessionStatistics
    {
        public Session Session { get; set; }
        public int TotalAllocated { get; set; }
        public int AvailableSeats { get; set; }
        public double OccupancyPercentage { get; set; }
        public Dictionary<DepartmentType, int> DepartmentBreakdown { get; set; }
    }

    public class SessionService : ISessionService
    {
        private readonly AllocationService _allocationService;

        public SessionService(AllocationService allocationService)
        {
            _allocationService = allocationService;
        }

        public Session GetSessionDetails(int sessionId)
        {
            return _allocationService.GetSession(sessionId);
        }

        public List<Session> GetAllSessionDetails()
        {
            return _allocationService.GetAllSessions();
        }

        public SessionStatistics GetSessionStatistics(int sessionId)
        {
            var session = _allocationService.GetSession(sessionId);
            if (session == null)
                return null;

            return new SessionStatistics
            {
                Session = session,
                TotalAllocated = session.CurrentOccupancy,
                AvailableSeats = session.GetAvailableSeats(),
                OccupancyPercentage = (session.CurrentOccupancy / (double)session.Capacity) * 100,
                DepartmentBreakdown = new Dictionary<DepartmentType, int>(session.DepartmentAllocations)
            };
        }

        public List<SessionStatistics> GetAllSessionStatistics()
        {
            var sessions = _allocationService.GetAllSessions();
            return sessions.Select(s => GetSessionStatistics(s.Id)).ToList();
        }
    }
}
