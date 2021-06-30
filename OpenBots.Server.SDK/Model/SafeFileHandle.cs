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
using SwaggerDateConverter = OpenBots.Server.SDK.Client.SwaggerDateConverter;

namespace OpenBots.Server.SDK.Model
{
    /// <summary>
    /// SafeFileHandle
    /// </summary>
    [DataContract]
        public partial class SafeFileHandle :  IEquatable<SafeFileHandle>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SafeFileHandle" /> class.
        /// </summary>
        public SafeFileHandle()
        {
        }
        
        /// <summary>
        /// Gets or Sets IsClosed
        /// </summary>
        [DataMember(Name="isClosed", EmitDefaultValue=false)]
        public bool? IsClosed { get; private set; }

        /// <summary>
        /// Gets or Sets IsInvalid
        /// </summary>
        [DataMember(Name="isInvalid", EmitDefaultValue=false)]
        public bool? IsInvalid { get; private set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class SafeFileHandle {\n");
            sb.Append("  IsClosed: ").Append(IsClosed).Append("\n");
            sb.Append("  IsInvalid: ").Append(IsInvalid).Append("\n");
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
            return this.Equals(input as SafeFileHandle);
        }

        /// <summary>
        /// Returns true if SafeFileHandle instances are equal
        /// </summary>
        /// <param name="input">Instance of SafeFileHandle to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(SafeFileHandle input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.IsClosed == input.IsClosed ||
                    (this.IsClosed != null &&
                    this.IsClosed.Equals(input.IsClosed))
                ) && 
                (
                    this.IsInvalid == input.IsInvalid ||
                    (this.IsInvalid != null &&
                    this.IsInvalid.Equals(input.IsInvalid))
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
                if (this.IsClosed != null)
                    hashCode = hashCode * 59 + this.IsClosed.GetHashCode();
                if (this.IsInvalid != null)
                    hashCode = hashCode * 59 + this.IsInvalid.GetHashCode();
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
