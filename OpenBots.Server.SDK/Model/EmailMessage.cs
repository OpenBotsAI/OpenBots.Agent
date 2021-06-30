using System.Collections.Generic;

namespace OpenBots.Server.SDK.Model
{
    public class EmailMessage
    {
        public List<EmailAddress> To { get; set; }
        public List<EmailAddress> CC { get; set; }
        public List<EmailAddress> BCC { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsBodyHtml { get; } = true;
    }
}
