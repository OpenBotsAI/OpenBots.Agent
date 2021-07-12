/* 
 * Openbots Documents Connector API
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
    /// AssignToRequest
    /// </summary>
    [DataContract]
        public partial class AssignToRequest :  IEquatable<AssignToRequest>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssignToRequest" /> class.
        /// </summary>
        /// <param name="humanTaskId">humanTaskId.</param>
        /// <param name="assignedToEmail">assignedToEmail.</param>
        public AssignToRequest(Guid? humanTaskId = default(Guid?), string assignedToEmail = default(string))
        {
            this.HumanTaskId = humanTaskId;
            this.AssignedToEmail = assignedToEmail;
        }
        
        /// <summary>
        /// Gets or Sets HumanTaskId
        /// </summary>
        [DataMember(Name="humanTaskId", EmitDefaultValue=false)]
        public Guid? HumanTaskId { get; set; }

        /// <summary>
        /// Gets or Sets AssignedToEmail
        /// </summary>
        [DataMember(Name="assignedToEmail", EmitDefaultValue=false)]
        public string AssignedToEmail { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class AssignToRequest {\n");
            sb.Append("  HumanTaskId: ").Append(HumanTaskId).Append("\n");
            sb.Append("  AssignedToEmail: ").Append(AssignedToEmail).Append("\n");
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
            return this.Equals(input as AssignToRequest);
        }

        /// <summary>
        /// Returns true if AssignToRequest instances are equal
        /// </summary>
        /// <param name="input">Instance of AssignToRequest to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(AssignToRequest input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.HumanTaskId == input.HumanTaskId ||
                    (this.HumanTaskId != null &&
                    this.HumanTaskId.Equals(input.HumanTaskId))
                ) && 
                (
                    this.AssignedToEmail == input.AssignedToEmail ||
                    (this.AssignedToEmail != null &&
                    this.AssignedToEmail.Equals(input.AssignedToEmail))
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
                if (this.HumanTaskId != null)
                    hashCode = hashCode * 59 + this.HumanTaskId.GetHashCode();
                if (this.AssignedToEmail != null)
                    hashCode = hashCode * 59 + this.AssignedToEmail.GetHashCode();
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