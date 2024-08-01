using System.Security.Principal;

namespace ImpersonationDemo.Models
{
    public class AuthenticationData
    { 
        public IIdentity? WindowsIdentity { get; set; }
    }
}
