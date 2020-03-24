using System.Collections.Generic;
using System.Threading.Tasks;
using SessionTask.DataAccess.Entities;
using SessionTask.Models;

namespace SessionTask.DataAccess.Services
{
    public interface ISessionTaskRepository
    {
        UserDto AuthenticateUser(string userName, string password);
        Task<int> SaveEvent(EventDto eventDetails);
        Task<IEnumerable<EventDto>> GetEvents();
        Task<IEnumerable<SessionDto>> GetSessions(int eventId, int userId);
        Task<int> SaveSession(SessionDto session);
        Task<bool> ApproveAttendees(ApproveAttendeesDto approveAttendees);
        Task<int> EnrollToSession(EnrollSessionDto enrollToSession);
        Task<IEnumerable<SessionAttendeeDto>> GetSessionAttendees(int sessionId, int userId);
    }
}
