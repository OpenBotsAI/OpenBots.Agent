/* 
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
    /// JobAgentsLookup
    /// </summary>
    [DataContract]
        public partial class JobAgentsLookup :  IEquatable<JobAgentsLookup>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JobAgentsLookup" /> class.
        /// </summary>
        /// <param name="agentId">agentId.</param>
        /// <param name="agentName">agentName.</param>
        public JobAgentsLookup(Guid? agentId = default(Guid?), string agentName = default(string))
        {
            this.AgentId = agentId;
            this.AgentName = agentName;
        }
        
        /// <summary>
        /// Gets or Sets AgentId
        /// </summary>
        [DataMember(Name="agentId", EmitDefaultValue=false)]
        public Guid? AgentId { get; set; }

        /// <summary>
        /// Gets or Sets AgentName
        /// </summary>
        [DataMember(Name="agentName", EmitDefaultValue=false)]
        public string AgentName { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class JobAgentsLookup {\n");
            sb.Append("  AgentId: ").Append(AgentId).Append("\n");
            sb.Append("  AgentName: ").Append(AgentName).Append("\n");
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
            return this.Equals(input as JobAgentsLookup);
        }

        /// <summary>
        /// Returns true if JobAgentsLookup instances are equal
        /// </summary>
        /// <param name="input">Instance of JobAgentsLookup to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(JobAgentsLookup input)
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
