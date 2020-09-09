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
    /// JobProcessLookup
    /// </summary>
    [DataContract]
        public partial class JobProcessLookup :  IEquatable<JobProcessLookup>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JobProcessLookup" /> class.
        /// </summary>
        /// <param name="processId">processId.</param>
        /// <param name="processName">processName.</param>
        public JobProcessLookup(Guid? processId = default(Guid?), string processName = default(string))
        {
            this.ProcessId = processId;
            this.ProcessName = processName;
        }
        
        /// <summary>
        /// Gets or Sets ProcessId
        /// </summary>
        [DataMember(Name="processId", EmitDefaultValue=false)]
        public Guid? ProcessId { get; set; }

        /// <summary>
        /// Gets or Sets ProcessName
        /// </summary>
        [DataMember(Name="processName", EmitDefaultValue=false)]
        public string ProcessName { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class JobProcessLookup {\n");
            sb.Append("  ProcessId: ").Append(ProcessId).Append("\n");
            sb.Append("  ProcessName: ").Append(ProcessName).Append("\n");
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
            return this.Equals(input as JobProcessLookup);
        }

        /// <summary>
        /// Returns true if JobProcessLookup instances are equal
        /// </summary>
        /// <param name="input">Instance of JobProcessLookup to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(JobProcessLookup input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.ProcessId == input.ProcessId ||
                    (this.ProcessId != null &&
                    this.ProcessId.Equals(input.ProcessId))
                ) && 
                (
                    this.ProcessName == input.ProcessName ||
                    (this.ProcessName != null &&
                    this.ProcessName.Equals(input.ProcessName))
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
                if (this.ProcessId != null)
                    hashCode = hashCode * 59 + this.ProcessId.GetHashCode();
                if (this.ProcessName != null)
                    hashCode = hashCode * 59 + this.ProcessName.GetHashCode();
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
