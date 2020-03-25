using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SessionTask.DataAccess.Entities;
using SessionTask.Models;
using SessionTask.Models.Helpers;

namespace SessionTask.DataAccess.Services
{
    public class SessionTaskRepository : ISessionTaskRepository
    {
        private readonly SessionTaskContext _dbContext;
        public SessionTaskRepository(SessionTaskContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<FeaturePermissionDto> GetUserFeatues(int userId)
        {
            //Get the list of all feature permissions assigned to the 
            //user
            var userFeatures = _dbContext.FeatureUserPermissionXref
                        .Where(x => x.UserId == userId)
                        .Select(y => new FeaturePermissionDto
                        {
                            FeatureName = y.Feature.FeatureName,
                            Permission = y.Permission.Name
                        }).ToList();

            var roleIds = _dbContext.UserRoleXref.Where(x => x.UserId == userId).Select(x => x.RoleId);

            //get the list of all feature permissions assigned to the 
            //user role
            var roleFeatures = _dbContext.FeatureRolePermissionXref.Where(x => roleIds.Contains(x.RoleId))
            .Select(y => new FeaturePermissionDto
            {
                FeatureName = y.Feature.FeatureName,
                Permission = y.Permission.Name
            }).ToList();

            var features = new List<FeaturePermissionDto>();
            features.AddRange(userFeatures);
            features.AddRange(roleFeatures);

            return features;
        }

        /// <summary>
        /// Authenticate user and once the user is authenticated 
        /// then get the list of all feature permissions 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public UserDto AuthenticateUser(string username, string password)
        {
            //(Models.Permission)Enum.Parse(typeof(Models.Permission), y.Permission.Name)
            var user = _dbContext.User
                .Where(x => x.UserName == username && x.Password == password)
                .Select(x => new UserDto
                {
                    UserId = x.UserId,
                    Username = x.UserName,
                    Features = x.FeatureUserPermissionXref.Select(y => new FeaturePermissionDto
                    {
                        FeatureName = y.Feature.FeatureName,
                        Permission = y.Permission.Name
                    }).ToList()
                })
                .SingleOrDefault();

            // return null if user not found
            if (user == null)
                return null;

            var roleIds = _dbContext.UserRoleXref.Where(x => x.UserId == user.UserId).Select(x => x.RoleId);

            var roleFeatures = _dbContext.FeatureRolePermissionXref.Where(x => roleIds.Contains(x.RoleId))
            .Select(y => new FeaturePermissionDto
            {
                FeatureName = y.Feature.FeatureName,
                Permission = y.Permission.Name
            }).ToList();

            var features = new List<FeaturePermissionDto>();
            features.AddRange(user.Features);
            features.AddRange(roleFeatures);
            user.Features = features;

            return user;
        }

        public async Task<int> SaveEvent(EventDto eventDetails)
        {
            Event eventEntity = new Event();

            if (eventDetails.EventId > 0)
            {
                eventEntity = _dbContext.Event.Find(eventDetails.EventId);
            }
            eventEntity.EventName = eventDetails.EventName;
            eventEntity.Description = eventDetails.EventDescription;
            eventEntity.EndTime = eventDetails.EndTime;
            eventEntity.StartTime = eventDetails.StartTime;
            eventEntity.EventDate = eventDetails.EventDate;
            eventEntity.MaxCount = eventDetails.MaxCount.ToInt();
            if (eventDetails.EventId == 0)
            {
                _dbContext.Event.Add(eventEntity);
            }
            await _dbContext.SaveChangesAsync();
            return eventEntity.EventId;
        }

        public async Task<int> SaveSession(SessionDto session)
        {
            Session sessionEntity = new Session();
            if (session.SessionId > 0)
            {
                sessionEntity = _dbContext.Session.Find(session.SessionId);
            }
            sessionEntity.EventId = session.EventId;
            sessionEntity.SessionName = session.SessionName;
            sessionEntity.Description = session.Description;
            sessionEntity.HostName = session.HostName;
            sessionEntity.StartTime = session.StartTime;
            sessionEntity.MaxCount = session.MaxCount?.ToInt();
            sessionEntity.EndTime = session.EndTime;
            if (session.SessionId == 0)
            {
                _dbContext.Session.Add(sessionEntity);
            }
            await _dbContext.SaveChangesAsync();
            return sessionEntity.SessionId;
        }

        public async Task<IEnumerable<EventDto>> GetEvents()
        {
            return await _dbContext.Event.Select(x => new EventDto
            {
                EventId = x.EventId,
                EventName = x.EventName,
                EventDescription = x.Description,
                EventDate = x.EventDate,
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                MaxCount = x.MaxCount.HasValue ? x.MaxCount.Value.ToString() : null
            }).ToListAsync();
        }

        public async Task<IEnumerable<SessionDto>> GetSessions(int eventId, int userId)
        {
            return await _dbContext.Session.Where(x => x.EventId == eventId)
                 .Select(x => new SessionDto
                 {
                     SessionId = x.SessionId,
                     EventId = x.EventId,
                     SessionName = x.SessionName,
                     Description = x.Description,
                     HostName = x.HostName,
                     StartTime = x.StartTime,
                     EndTime = x.EndTime,
                     MaxCount = x.MaxCount.HasValue ? x.MaxCount.Value.ToString() : null,
                     IsEnrolled = x.UserSessionXref.FirstOrDefault(x => x.UserId == userId) != null,
                     IsApproved = x.UserSessionXref.FirstOrDefault(x => x.UserId == userId) != null ?
                        x.UserSessionXref.FirstOrDefault(x => x.UserId == userId).IsApproved : false
                 })
                 .ToListAsync();
        }

        public async Task<bool> ApproveAttendees(ApproveAttendeesDto approveAttendees)
        {
            foreach (var userId in approveAttendees.UserIds)
            {
                var userSession = await _dbContext.UserSessionXref
                    .FirstOrDefaultAsync(x => x.SessionId == approveAttendees.SessionId && x.UserId == userId);
                if (userSession != null)
                {
                    userSession.IsApproved = true;
                }
            }
            var noOfRecords = await _dbContext.SaveChangesAsync();
            return noOfRecords == approveAttendees.UserIds.Count();
        }

        public async Task<int> EnrollToSession(EnrollSessionDto enrollToSession)
        {
            var userSession = await _dbContext.UserSessionXref
                .FirstOrDefaultAsync(x => x.UserId == enrollToSession.UserId && x.SessionId == enrollToSession.SessionId);
            if (userSession == null)
            {
                _dbContext.UserSessionXref.Add(new UserSessionXref
                {
                    UserId = enrollToSession.UserId,
                    SessionId = enrollToSession.SessionId,
                    IsApproved = false
                });
                return await _dbContext.SaveChangesAsync();
            }
            return -1;
        }

        public async Task<IEnumerable<SessionAttendeeDto>> GetSessionAttendees(int sessionId, int userId)
        {
            return await _dbContext.UserSessionXref.Where(x => x.SessionId == sessionId)
                 .Select(x => new SessionAttendeeDto
                 {
                     UserId = x.UserId,
                     AttendeeName = x.User.UserName,
                     EventName = _dbContext.Event.FirstOrDefault(y => y.EventId == x.Session.EventId).EventName,
                     SessionName = x.Session.SessionName,
                     HostName = x.Session.HostName,
                     IsApproved = x.IsApproved
                 }).ToListAsync();
        }
    }
}
