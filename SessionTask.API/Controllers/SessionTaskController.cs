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

        /// <summary>
        /// Save a new event or update an existing event
        /// </summary>
        /// <param name="eventDetails"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizationFilter("Event", "Create,Update")]
        public async Task<int> SaveEvent(EventDto eventDetails)
        {
            return await _sessionTaskRepository.SaveEvent(eventDetails);
        }

        /// <summary>
        /// Save a new session or update an existing session
        /// </summary>
        /// <param name="sessionDetails"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizationFilter("Session", "Create,Update")]
        public async Task<int> SaveSession(SessionDto sessionDetails)
        {
            return await _sessionTaskRepository.SaveSession(sessionDetails);
        }

        /// <summary>
        /// Get the list of all events from the 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<EventDto>> GetEvents()
        {
            return await _sessionTaskRepository.GetEvents();
        }


        /// <summary>
        /// Get the list of sessions from the selected event
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<SessionDto>> GetSessions(int eventId, int userId)
        {
            return await _sessionTaskRepository.GetSessions(eventId, userId);
        }

        /// <summary>
        /// Approve the attendees
        /// </summary>
        /// <param name="approveAttendees"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizationFilter("Attendees", "Update")]
        public async Task<bool> ApproveAttendees(ApproveAttendeesDto approveAttendees)
        {
            return await _sessionTaskRepository.ApproveAttendees(approveAttendees);
        }

        /// <summary>
        /// Enroll to session
        /// </summary>
        /// <param name="enrollToSession"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizationFilter("Enroll", "Create")]
        public async Task<int> EnrollToSession(EnrollSessionDto enrollToSession)
        {
            return await _sessionTaskRepository.EnrollToSession(enrollToSession);
        }

        /// <summary>
        /// Get the list of all attendees based on the selected session
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthorizationFilter("Attendees", "Read")]
        public async Task<IEnumerable<SessionAttendeeDto>> GetSessionAttendees(int sessionId, int userId)
        {
            return await _sessionTaskRepository.GetSessionAttendees(sessionId, userId);
        }
        
    }
}
