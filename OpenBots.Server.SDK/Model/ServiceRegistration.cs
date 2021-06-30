using System;

namespace OpenBots.Server.SDK.Model
{
    public class ServiceRegistration
    {
        public string Name { get; set; }

        public Guid? Id { get; set; }
        public bool? IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeleteOn { get; set; }
        public byte[] Timestamp { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }

        public string Environment { get; set; }

        public int Version { get; set; }

        public ServiceProtocol Protocol { get; set; }

        public Uri ServiceBaseUri { get; set; }

        public Uri HealthCheckUri { get; set; }

        public Uri OpenAPIUri { get; set; }

        public Uri SwaggerUri { get; set; }

        public string Description { get; set; }

        public string ClientSDKNugetPackageID { get; set; }

        public bool IsCurrentlyUnderMaintenance { get; set; }

        public DateTime? MaintenanceEndsOnUTC { get; set; }

        public string ServiceTag { get; set; }

        public enum ServiceProtocol : int
        {
            Unknown = 0,
            HTTPS = 1,
            GRPC = 2
        }
    }
}
