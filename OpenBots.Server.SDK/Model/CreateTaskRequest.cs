using System;

namespace OpenBots.Server.SDK.Model
{
    public class CreateTaskRequest
    {
		public string Name { get; set; }

		public string Description { get; set; }

		public string CaseNumber { get; set; }

		public string CaseType { get; set; }

		public string DueOn { get; set; }

		public string Status { get; set; }

		public string AssignedTo { get; set; }

		public Guid? TaskQueueId { get; set; }

		public long? OrganizationUnitId { get; set; }

		public long? UserId { get; set; }
	}
}
