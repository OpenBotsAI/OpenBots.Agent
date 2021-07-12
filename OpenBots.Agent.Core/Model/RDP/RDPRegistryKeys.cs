using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenBots.Agent.Core.Model.RDP
{
    public class RDPRegistryKeys
    {
        public string SubKey { get; } = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System";
        public string LegalNoticeCaptionKey { get; } = "legalnoticecaption";
        public string LegalNoticeTextKey { get; } = "legalnoticetext";
        public string LegalNoticeCaptionValue { get; set; }
        public string LegalNoticeTextValue { get; set; }
    }
}
