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
    /// UpdateAgentViewModel
    /// </summary>
    [DataContract]
    public partial class UpdateAgentViewModel : IEquatable<UpdateAgentViewModel>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateAgentViewModel" /> class.
        /// </summary>
        /// <param name="name">name (required).</param>
        /// <param name="machineName">machineName (required).</param>
        /// <param name="macAddresses">macAddresses.</param>
        /// <param name="ipAddresses">ipAddresses.</param>
        /// <param name="isEnabled">isEnabled (required).</param>
        /// <param name="isConnected">isConnected (required).</param>
        /// <param name="credentialId">credentialId.</param>
        /// <param name="ipOption">ipOption.</param>
        /// <param name="isEnhancedSecurity">isEnhancedSecurity.</param>
        /// <param name="agentSetting">agentSetting.</param>
        public UpdateAgentViewModel(string name = default(string), string machineName = default(string), string macAddresses = default(string), string ipAddresses = default(string), bool? isEnabled = default(bool?), bool? isConnected = default(bool?), Guid? credentialId = default(Guid?), string ipOption = default(string), bool? isEnhancedSecurity = default(bool?), AgentSettingViewModel agentSetting = default(AgentSettingViewModel))
        {
            // to ensure "name" is required (not null)
            if (name == null)
            {
                throw new InvalidDataException("name is a required property for UpdateAgentViewModel and cannot be null");
            }
            else
            {
                this.Name = name;
            }
            // to ensure "machineName" is required (not null)
            if (machineName == null)
            {
                throw new InvalidDataException("machineName is a required property for UpdateAgentViewModel and cannot be null");
            }
            else
            {
                this.MachineName = machineName;
            }
            // to ensure "isEnabled" is required (not null)
            if (isEnabled == null)
            {
                throw new InvalidDataException("isEnabled is a required property for UpdateAgentViewModel and cannot be null");
            }
            else
            {
                this.IsEnabled = isEnabled;
            }
            // to ensure "isConnected" is required (not null)
            if (isConnected == null)
            {
                throw new InvalidDataException("isConnected is a required property for UpdateAgentViewModel and cannot be null");
            }
            else
            {
                this.IsConnected = isConnected;
            }
            this.MacAddresses = macAddresses;
            this.IpAddresses = ipAddresses;
            this.CredentialId = credentialId;
            this.IpOption = ipOption;
            this.IsEnhancedSecurity = isEnhancedSecurity;
            this.AgentSetting = agentSetting;
        }

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
        /// Gets or Sets IsConnected
        /// </summary>
        [DataMember(Name = "isConnected", EmitDefaultValue = false)]
        public bool? IsConnected { get; set; }

        /// <summary>
        /// Gets or Sets CredentialId
        /// </summary>
        [DataMember(Name = "credentialId", EmitDefaultValue = false)]
        public Guid? CredentialId { get; set; }

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
        /// Gets or Sets AgentSetting
        /// </summary>
        [DataMember(Name = "agentSetting", EmitDefaultValue = false)]
        public AgentSettingViewModel AgentSetting { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class UpdateAgentViewModel {\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  MachineName: ").Append(MachineName).Append("\n");
            sb.Append("  MacAddresses: ").Append(MacAddresses).Append("\n");
            sb.Append("  IpAddresses: ").Append(IpAddresses).Append("\n");
            sb.Append("  IsEnabled: ").Append(IsEnabled).Append("\n");
            sb.Append("  IsConnected: ").Append(IsConnected).Append("\n");
            sb.Append("  CredentialId: ").Append(CredentialId).Append("\n");
            sb.Append("  IpOption: ").Append(IpOption).Append("\n");
            sb.Append("  IsEnhancedSecurity: ").Append(IsEnhancedSecurity).Append("\n");
            sb.Append("  AgentSetting: ").Append(AgentSetting).Append("\n");
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
            return this.Equals(input as UpdateAgentViewModel);
        }

        /// <summary>
        /// Returns true if UpdateAgentViewModel instances are equal
        /// </summary>
        /// <param name="input">Instance of UpdateAgentViewModel to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(UpdateAgentViewModel input)
        {
            if (input == null)
                return false;

            return
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
                    this.IsConnected == input.IsConnected ||
                    (this.IsConnected != null &&
                    this.IsConnected.Equals(input.IsConnected))
                ) &&
                (
                    this.CredentialId == input.CredentialId ||
                    (this.CredentialId != null &&
                    this.CredentialId.Equals(input.CredentialId))
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
                ) &&
                (
                    this.AgentSetting == input.AgentSetting ||
                    (this.AgentSetting != null &&
                    this.AgentSetting.Equals(input.AgentSetting))
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
                if (this.IsConnected != null)
                    hashCode = hashCode * 59 + this.IsConnected.GetHashCode();
                if (this.CredentialId != null)
                    hashCode = hashCode * 59 + this.CredentialId.GetHashCode();
                if (this.IpOption != null)
                    hashCode = hashCode * 59 + this.IpOption.GetHashCode();
                if (this.IsEnhancedSecurity != null)
                    hashCode = hashCode * 59 + this.IsEnhancedSecurity.GetHashCode();
                if (this.AgentSetting != null)
                    hashCode = hashCode * 59 + this.AgentSetting.GetHashCode();
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
