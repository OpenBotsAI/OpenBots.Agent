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
    /// EmailAccountLookup
    /// </summary>
    [DataContract]
        public partial class EmailAccountLookup :  IEquatable<EmailAccountLookup>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailAccountLookup" /> class.
        /// </summary>
        /// <param name="emailAccountId">emailAccountId.</param>
        /// <param name="emailAccountName">emailAccountName.</param>
        public EmailAccountLookup(Guid? emailAccountId = default(Guid?), string emailAccountName = default(string))
        {
            this.EmailAccountId = emailAccountId;
            this.EmailAccountName = emailAccountName;
        }
        
        /// <summary>
        /// Gets or Sets EmailAccountId
        /// </summary>
        [DataMember(Name="emailAccountId", EmitDefaultValue=false)]
        public Guid? EmailAccountId { get; set; }

        /// <summary>
        /// Gets or Sets EmailAccountName
        /// </summary>
        [DataMember(Name="emailAccountName", EmitDefaultValue=false)]
        public string EmailAccountName { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class EmailAccountLookup {\n");
            sb.Append("  EmailAccountId: ").Append(EmailAccountId).Append("\n");
            sb.Append("  EmailAccountName: ").Append(EmailAccountName).Append("\n");
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
            return this.Equals(input as EmailAccountLookup);
        }

        /// <summary>
        /// Returns true if EmailAccountLookup instances are equal
        /// </summary>
        /// <param name="input">Instance of EmailAccountLookup to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(EmailAccountLookup input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.EmailAccountId == input.EmailAccountId ||
                    (this.EmailAccountId != null &&
                    this.EmailAccountId.Equals(input.EmailAccountId))
                ) && 
                (
                    this.EmailAccountName == input.EmailAccountName ||
                    (this.EmailAccountName != null &&
                    this.EmailAccountName.Equals(input.EmailAccountName))
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
                if (this.EmailAccountId != null)
                    hashCode = hashCode * 59 + this.EmailAccountId.GetHashCode();
                if (this.EmailAccountName != null)
                    hashCode = hashCode * 59 + this.EmailAccountName.GetHashCode();
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
