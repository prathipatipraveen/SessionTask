using System.Collections.Generic;

namespace SessionTask.Models
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<FeaturePermissionDto> Features { get; set; }
        public string Token { get; set; }
    }
}
