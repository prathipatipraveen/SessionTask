using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SessionTask.API.Security;
using SessionTask.DataAccess.Services;
using SessionTask.Models;

namespace SessionTask.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[Action]")]
    public class SessionTaskController : ControllerBase
    {
        private readonly ISessionTaskRepository _sessionTaskRepository;
        public SessionTaskController(ISessionTaskRepository sessionTaskRepository)
        {
            _sessionTaskRepository = sessionTaskRepository;
        }

        [HttpPost]
        [AuthorizationFilter("Event", "Create,Update")]
        public async Task<int> SaveEvent(EventDto eventDetails)
        {
            return await _sessionTaskRepository.SaveEvent(eventDetails);
        }

        [HttpPost]
        [AuthorizationFilter("Session", "Create,Update")]
        public async Task<int> SaveSession(SessionDto sessionDetails)
        {
            return await _sessionTaskRepository.SaveSession(sessionDetails);
        }

        [HttpGet]
        public async Task<IEnumerable<EventDto>> GetEvents()
        {
            return await _sessionTaskRepository.GetEvents();
        }

        [HttpGet]
        public async Task<IEnumerable<SessionDto>> GetSessions(int eventId, int userId)
        {
            return await _sessionTaskRepository.GetSessions(eventId, userId);
        }

        [HttpPost]
        [AuthorizationFilter("Attendees", "Update")]
        public async Task<bool> ApproveAttendees(ApproveAttendeesDto approveAttendees)
        {
            return await _sessionTaskRepository.ApproveAttendees(approveAttendees);
        }

        [HttpPost]
        [AuthorizationFilter("Enroll", "Create")]
        public async Task<int> EnrollToSession(EnrollSessionDto enrollToSession)
        {
            return await _sessionTaskRepository.EnrollToSession(enrollToSession);
        }

        [HttpGet]
        [AuthorizationFilter("Attendees", "Read")]
        public async Task<IEnumerable<SessionAttendeeDto>> GetSessionAttendees(int sessionId, int userId)
        {
            return await _sessionTaskRepository.GetSessionAttendees(sessionId, userId);
        }
        
    }
}
