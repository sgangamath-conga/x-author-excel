using System.Collections.Generic;

namespace Apttus.XAuthor.Core
{
    public class AppAssignmentModel
    {
        public string ApplicationId { get; set; }
        public List<Assignments> Assignments { get; set; }

        public AppAssignmentModel()
        {
            Assignments = new List<Assignments>();
        }
    }

    public class Assignments
    {
        public string AssignmentId { get; set; }
        public string AssignmentName { get; set; }
        public User User { get; set; }
        public Profile Profile { get; set; }
    }
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool Checked { get; set; }
    }

    public class Profile
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Checked { get; set; }
    }
}
