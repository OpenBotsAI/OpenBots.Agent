﻿/* 
 * OpenBots Server API
 *
 * No description provided (generated by Swagger Codegen https://github.com/swagger-api/swagger-codegen)
 *
 * OpenAPI spec version: v1
 * 
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */
using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using SwaggerDateConverter = OpenBots.Server.SDK.Client.SwaggerDateConverter;

namespace OpenBots.Server.SDK.Model
{
    /// <summary>
    /// ResolvedAgentResponseViewModel
    /// </summary>
    [DataContract]
    public partial class ResolvedAgentResponseViewModel : IEquatable<ResolvedAgentResponseViewModel>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResolvedAgentResponseViewModel" /> class.
        /// </summary>
        /// <param name="agentId">agentId.</param>
        /// <param name="agentName">agentName.</param>
        /// <param name="agentGroupsCS">agentGroupsCS.</param>
        /// <param name="heartbeatInterval">heartbeatInterval.</param>
        /// <param name="jobLoggingInterval">jobLoggingInterval.</param>
        /// <param name="verifySslCertificate">verifySslCertificate.</param>
        public ResolvedAgentResponseViewModel(Guid? agentId = default(Guid?), string agentName = default(string), string agentGroupsCS = default(string), int? heartbeatInterval = default(int?), int? jobLoggingInterval = default(int?), bool? verifySslCertificate = default(bool?))
        {
            this.AgentId = agentId;
            this.AgentName = agentName;
            this.AgentGroupsCS = agentGroupsCS;
            this.HeartbeatInterval = heartbeatInterval;
            this.JobLoggingInterval = jobLoggingInterval;
            this.VerifySslCertificate = verifySslCertificate;
        }

        /// <summary>
        /// Gets or Sets AgentId
        /// </summary>
        [DataMember(Name = "agentId", EmitDefaultValue = false)]
        public Guid? AgentId { get; set; }

        /// <summary>
        /// Gets or Sets AgentName
        /// </summary>
        [DataMember(Name = "agentName", EmitDefaultValue = false)]
        public string AgentName { get; set; }

        /// <summary>
        /// Gets or Sets AgentGroupsCS
        /// </summary>
        [DataMember(Name = "agentGroupsCS", EmitDefaultValue = false)]
        public string AgentGroupsCS { get; set; }

        /// <summary>
        /// Gets or Sets HeartbeatInterval
        /// </summary>
        [DataMember(Name = "heartbeatInterval", EmitDefaultValue = false)]
        public int? HeartbeatInterval { get; set; }

        /// <summary>
        /// Gets or Sets JobLoggingInterval
        /// </summary>
        [DataMember(Name = "jobLoggingInterval", EmitDefaultValue = false)]
        public int? JobLoggingInterval { get; set; }

        /// <summary>
        /// Gets or Sets VerifySslCertificate
        /// </summary>
        [DataMember(Name = "verifySslCertificate", EmitDefaultValue = false)]
        public bool? VerifySslCertificate { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ResolvedAgentResponseViewModel {\n");
            sb.Append("  AgentId: ").Append(AgentId).Append("\n");
            sb.Append("  AgentName: ").Append(AgentName).Append("\n");
            sb.Append("  AgentGroupsCS: ").Append(AgentGroupsCS).Append("\n");
            sb.Append("  HeartbeatInterval: ").Append(HeartbeatInterval).Append("\n");
            sb.Append("  JobLoggingInterval: ").Append(JobLoggingInterval).Append("\n");
            sb.Append("  VerifySslCertificate: ").Append(VerifySslCertificate).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="input">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object input)
        {
            return this.Equals(input as ResolvedAgentResponseViewModel);
        }

        /// <summary>
        /// Returns true if ResolvedAgentResponseViewModel instances are equal
        /// </summary>
        /// <param name="input">Instance of ResolvedAgentResponseViewModel to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ResolvedAgentResponseViewModel input)
        {
            if (input == null)
                return false;

            return
                (
                    this.AgentId == input.AgentId ||
                    (this.AgentId != null &&
                    this.AgentId.Equals(input.AgentId))
                ) &&
                (
                    this.AgentName == input.AgentName ||
                    (this.AgentName != null &&
                    this.AgentName.Equals(input.AgentName))
                ) &&
                (
                    this.AgentGroupsCS == input.AgentGroupsCS ||
                    (this.AgentGroupsCS != null &&
                    this.AgentGroupsCS.Equals(input.AgentGroupsCS))
                ) &&
                (
                    this.HeartbeatInterval == input.HeartbeatInterval ||
                    (this.HeartbeatInterval != null &&
                    this.HeartbeatInterval.Equals(input.HeartbeatInterval))
                ) &&
                (
                    this.JobLoggingInterval == input.JobLoggingInterval ||
                    (this.JobLoggingInterval != null &&
                    this.JobLoggingInterval.Equals(input.JobLoggingInterval))
                ) &&
                (
                    this.VerifySslCertificate == input.VerifySslCertificate ||
                    (this.VerifySslCertificate != null &&
                    this.VerifySslCertificate.Equals(input.VerifySslCertificate))
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hashCode = 41;
                if (this.AgentId != null)
                    hashCode = hashCode * 59 + this.AgentId.GetHashCode();
                if (this.AgentName != null)
                    hashCode = hashCode * 59 + this.AgentName.GetHashCode();
                if (this.AgentGroupsCS != null)
                    hashCode = hashCode * 59 + this.AgentGroupsCS.GetHashCode();
                if (this.HeartbeatInterval != null)
                    hashCode = hashCode * 59 + this.HeartbeatInterval.GetHashCode();
                if (this.JobLoggingInterval != null)
                    hashCode = hashCode * 59 + this.JobLoggingInterval.GetHashCode();
                if (this.VerifySslCertificate != null)
                    hashCode = hashCode * 59 + this.VerifySslCertificate.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// To validate all properties of the instance
        /// </summary>
        /// <param name="validationContext">Validation context</param>
        /// <returns>Validation Result</returns>
        IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}