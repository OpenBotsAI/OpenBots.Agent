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
using SwaggerDateConverter = OpenBots.Service.API.Client.SwaggerDateConverter;

namespace OpenBots.Service.API.Model
{
    /// <summary>
    /// AllAgentsViewModel
    /// </summary>
    [DataContract]
    public partial class AllAgentsViewModel : IEquatable<AllAgentsViewModel>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AllAgentsViewModel" /> class.
        /// </summary>
        /// <param name="id">id.</param>
        /// <param name="name">name.</param>
        /// <param name="machineName">machineName.</param>
        /// <param name="macAddresses">macAddresses.</param>
        /// <param name="ipAddresses">ipAddresses.</param>
        /// <param name="isEnabled">isEnabled.</param>
        /// <param name="lastReportedOn">lastReportedOn.</param>
        /// <param name="lastReportedStatus">lastReportedStatus.</param>
        /// <param name="lastReportedWork">lastReportedWork.</param>
        /// <param name="lastReportedMessage">lastReportedMessage.</param>
        /// <param name="isHealthy">isHealthy.</param>
        /// <param name="status">status.</param>
        /// <param name="credentialId">credentialId.</param>
        /// <param name="createdOn">createdOn.</param>
        /// <param name="ipOption">ipOption.</param>
        /// <param name="isEnhancedSecurity">isEnhancedSecurity.</param>
        public AllAgentsViewModel(Guid? id = default(Guid?), string name = default(string), string machineName = default(string), string macAddresses = default(string), string ipAddresses = default(string), bool? isEnabled = default(bool?), DateTime? lastReportedOn = default(DateTime?), string lastReportedStatus = default(string), string lastReportedWork = default(string), string lastReportedMessage = default(string), bool? isHealthy = default(bool?), string status = default(string), Guid? credentialId = default(Guid?), DateTime? createdOn = default(DateTime?), string ipOption = default(string), bool? isEnhancedSecurity = default(bool?))
        {
            this.Id = id;
            this.Name = name;
            this.MachineName = machineName;
            this.MacAddresses = macAddresses;
            this.IpAddresses = ipAddresses;
            this.IsEnabled = isEnabled;
            this.LastReportedOn = lastReportedOn;
            this.LastReportedStatus = lastReportedStatus;
            this.LastReportedWork = lastReportedWork;
            this.LastReportedMessage = lastReportedMessage;
            this.IsHealthy = isHealthy;
            this.Status = status;
            this.CredentialId = credentialId;
            this.CreatedOn = createdOn;
            this.IpOption = ipOption;
            this.IsEnhancedSecurity = isEnhancedSecurity;
        }

        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public Guid? Id { get; set; }

        /// <summary>
        /// Gets or Sets Name
        /// </summary>
        [DataMember(Name = "name", EmitDefaultValue = false)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or Sets MachineName
        /// </summary>
        [DataMember(Name = "machineName", EmitDefaultValue = false)]
        public string MachineName { get; set; }

        /// <summary>
        /// Gets or Sets MacAddresses
        /// </summary>
        [DataMember(Name = "macAddresses", EmitDefaultValue = false)]
        public string MacAddresses { get; set; }

        /// <summary>
        /// Gets or Sets IpAddresses
        /// </summary>
        [DataMember(Name = "ipAddresses", EmitDefaultValue = false)]
        public string IpAddresses { get; set; }

        /// <summary>
        /// Gets or Sets IsEnabled
        /// </summary>
        [DataMember(Name = "isEnabled", EmitDefaultValue = false)]
        public bool? IsEnabled { get; set; }

        /// <summary>
        /// Gets or Sets LastReportedOn
        /// </summary>
        [DataMember(Name = "lastReportedOn", EmitDefaultValue = false)]
        public DateTime? LastReportedOn { get; set; }

        /// <summary>
        /// Gets or Sets LastReportedStatus
        /// </summary>
        [DataMember(Name = "lastReportedStatus", EmitDefaultValue = false)]
        public string LastReportedStatus { get; set; }

        /// <summary>
        /// Gets or Sets LastReportedWork
        /// </summary>
        [DataMember(Name = "lastReportedWork", EmitDefaultValue = false)]
        public string LastReportedWork { get; set; }

        /// <summary>
        /// Gets or Sets LastReportedMessage
        /// </summary>
        [DataMember(Name = "lastReportedMessage", EmitDefaultValue = false)]
        public string LastReportedMessage { get; set; }

        /// <summary>
        /// Gets or Sets IsHealthy
        /// </summary>
        [DataMember(Name = "isHealthy", EmitDefaultValue = false)]
        public bool? IsHealthy { get; set; }

        /// <summary>
        /// Gets or Sets Status
        /// </summary>
        [DataMember(Name = "status", EmitDefaultValue = false)]
        public string Status { get; set; }

        /// <summary>
        /// Gets or Sets CredentialId
        /// </summary>
        [DataMember(Name = "credentialId", EmitDefaultValue = false)]
        public Guid? CredentialId { get; set; }

        /// <summary>
        /// Gets or Sets CreatedOn
        /// </summary>
        [DataMember(Name = "createdOn", EmitDefaultValue = false)]
        public DateTime? CreatedOn { get; set; }

        /// <summary>
        /// Gets or Sets IpOption
        /// </summary>
        [DataMember(Name = "ipOption", EmitDefaultValue = false)]
        public string IpOption { get; set; }

        /// <summary>
        /// Gets or Sets IsEnhancedSecurity
        /// </summary>
        [DataMember(Name = "isEnhancedSecurity", EmitDefaultValue = false)]
        public bool? IsEnhancedSecurity { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class AllAgentsViewModel {\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  MachineName: ").Append(MachineName).Append("\n");
            sb.Append("  MacAddresses: ").Append(MacAddresses).Append("\n");
            sb.Append("  IpAddresses: ").Append(IpAddresses).Append("\n");
            sb.Append("  IsEnabled: ").Append(IsEnabled).Append("\n");
            sb.Append("  LastReportedOn: ").Append(LastReportedOn).Append("\n");
            sb.Append("  LastReportedStatus: ").Append(LastReportedStatus).Append("\n");
            sb.Append("  LastReportedWork: ").Append(LastReportedWork).Append("\n");
            sb.Append("  LastReportedMessage: ").Append(LastReportedMessage).Append("\n");
            sb.Append("  IsHealthy: ").Append(IsHealthy).Append("\n");
            sb.Append("  Status: ").Append(Status).Append("\n");
            sb.Append("  CredentialId: ").Append(CredentialId).Append("\n");
            sb.Append("  CreatedOn: ").Append(CreatedOn).Append("\n");
            sb.Append("  IpOption: ").Append(IpOption).Append("\n");
            sb.Append("  IsEnhancedSecurity: ").Append(IsEnhancedSecurity).Append("\n");
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
            return this.Equals(input as AllAgentsViewModel);
        }

        /// <summary>
        /// Returns true if AllAgentsViewModel instances are equal
        /// </summary>
        /// <param name="input">Instance of AllAgentsViewModel to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(AllAgentsViewModel input)
        {
            if (input == null)
                return false;

            return
                (
                    this.Id == input.Id ||
                    (this.Id != null &&
                    this.Id.Equals(input.Id))
                ) &&
                (
                    this.Name == input.Name ||
                    (this.Name != null &&
                    this.Name.Equals(input.Name))
                ) &&
                (
                    this.MachineName == input.MachineName ||
                    (this.MachineName != null &&
                    this.MachineName.Equals(input.MachineName))
                ) &&
                (
                    this.MacAddresses == input.MacAddresses ||
                    (this.MacAddresses != null &&
                    this.MacAddresses.Equals(input.MacAddresses))
                ) &&
                (
                    this.IpAddresses == input.IpAddresses ||
                    (this.IpAddresses != null &&
                    this.IpAddresses.Equals(input.IpAddresses))
                ) &&
                (
                    this.IsEnabled == input.IsEnabled ||
                    (this.IsEnabled != null &&
                    this.IsEnabled.Equals(input.IsEnabled))
                ) &&
                (
                    this.LastReportedOn == input.LastReportedOn ||
                    (this.LastReportedOn != null &&
                    this.LastReportedOn.Equals(input.LastReportedOn))
                ) &&
                (
                    this.LastReportedStatus == input.LastReportedStatus ||
                    (this.LastReportedStatus != null &&
                    this.LastReportedStatus.Equals(input.LastReportedStatus))
                ) &&
                (
                    this.LastReportedWork == input.LastReportedWork ||
                    (this.LastReportedWork != null &&
                    this.LastReportedWork.Equals(input.LastReportedWork))
                ) &&
                (
                    this.LastReportedMessage == input.LastReportedMessage ||
                    (this.LastReportedMessage != null &&
                    this.LastReportedMessage.Equals(input.LastReportedMessage))
                ) &&
                (
                    this.IsHealthy == input.IsHealthy ||
                    (this.IsHealthy != null &&
                    this.IsHealthy.Equals(input.IsHealthy))
                ) &&
                (
                    this.Status == input.Status ||
                    (this.Status != null &&
                    this.Status.Equals(input.Status))
                ) &&
                (
                    this.CredentialId == input.CredentialId ||
                    (this.CredentialId != null &&
                    this.CredentialId.Equals(input.CredentialId))
                ) &&
                (
                    this.CreatedOn == input.CreatedOn ||
                    (this.CreatedOn != null &&
                    this.CreatedOn.Equals(input.CreatedOn))
                ) &&
                (
                    this.IpOption == input.IpOption ||
                    (this.IpOption != null &&
                    this.IpOption.Equals(input.IpOption))
                ) &&
                (
                    this.IsEnhancedSecurity == input.IsEnhancedSecurity ||
                    (this.IsEnhancedSecurity != null &&
                    this.IsEnhancedSecurity.Equals(input.IsEnhancedSecurity))
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
                if (this.Id != null)
                    hashCode = hashCode * 59 + this.Id.GetHashCode();
                if (this.Name != null)
                    hashCode = hashCode * 59 + this.Name.GetHashCode();
                if (this.MachineName != null)
                    hashCode = hashCode * 59 + this.MachineName.GetHashCode();
                if (this.MacAddresses != null)
                    hashCode = hashCode * 59 + this.MacAddresses.GetHashCode();
                if (this.IpAddresses != null)
                    hashCode = hashCode * 59 + this.IpAddresses.GetHashCode();
                if (this.IsEnabled != null)
                    hashCode = hashCode * 59 + this.IsEnabled.GetHashCode();
                if (this.LastReportedOn != null)
                    hashCode = hashCode * 59 + this.LastReportedOn.GetHashCode();
                if (this.LastReportedStatus != null)
                    hashCode = hashCode * 59 + this.LastReportedStatus.GetHashCode();
                if (this.LastReportedWork != null)
                    hashCode = hashCode * 59 + this.LastReportedWork.GetHashCode();
                if (this.LastReportedMessage != null)
                    hashCode = hashCode * 59 + this.LastReportedMessage.GetHashCode();
                if (this.IsHealthy != null)
                    hashCode = hashCode * 59 + this.IsHealthy.GetHashCode();
                if (this.Status != null)
                    hashCode = hashCode * 59 + this.Status.GetHashCode();
                if (this.CredentialId != null)
                    hashCode = hashCode * 59 + this.CredentialId.GetHashCode();
                if (this.CreatedOn != null)
                    hashCode = hashCode * 59 + this.CreatedOn.GetHashCode();
                if (this.IpOption != null)
                    hashCode = hashCode * 59 + this.IpOption.GetHashCode();
                if (this.IsEnhancedSecurity != null)
                    hashCode = hashCode * 59 + this.IsEnhancedSecurity.GetHashCode();
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