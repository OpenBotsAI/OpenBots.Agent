using System;
using System.Collections.Generic;

namespace OpenBots.Server.SDK.Model
{
    public class ServerInfo
    {
        public Guid PersonId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }
        public bool ForcedPasswordChange { get; set; }
        public bool IsUserConsentRequired { get; set; }
        public bool IsJoinOrgRequestPending { get; set; }
        public List<OrganizationListing> MyOrganizations { get; set; }
    }
}
