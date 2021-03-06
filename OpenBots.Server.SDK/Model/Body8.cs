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
    /// Body8
    /// </summary>
    [DataContract]
        public partial class Body8 :  IEquatable<Body8>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Body8" /> class.
        /// </summary>
        /// <param name="emailMessageJson">emailMessageJson.</param>
        /// <param name="files">files.</param>
        /// <param name="driveName">driveName.</param>
        public Body8(string emailMessageJson = default(string), List<byte[]> files = default(List<byte[]>), string driveName = default(string))
        {
            this.EmailMessageJson = emailMessageJson;
            this.Files = files;
            this.DriveName = driveName;
        }
        
        /// <summary>
        /// Gets or Sets EmailMessageJson
        /// </summary>
        [DataMember(Name="EmailMessageJson", EmitDefaultValue=false)]
        public string EmailMessageJson { get; set; }

        /// <summary>
        /// Gets or Sets Files
        /// </summary>
        [DataMember(Name="Files", EmitDefaultValue=false)]
        public List<byte[]> Files { get; set; }

        /// <summary>
        /// Gets or Sets DriveName
        /// </summary>
        [DataMember(Name="DriveName", EmitDefaultValue=false)]
        public string DriveName { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Body8 {\n");
            sb.Append("  EmailMessageJson: ").Append(EmailMessageJson).Append("\n");
            sb.Append("  Files: ").Append(Files).Append("\n");
            sb.Append("  DriveName: ").Append(DriveName).Append("\n");
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
            return this.Equals(input as Body8);
        }

        /// <summary>
        /// Returns true if Body8 instances are equal
        /// </summary>
        /// <param name="input">Instance of Body8 to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Body8 input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.EmailMessageJson == input.EmailMessageJson ||
                    (this.EmailMessageJson != null &&
                    this.EmailMessageJson.Equals(input.EmailMessageJson))
                ) && 
                (
                    this.Files == input.Files ||
                    this.Files != null &&
                    input.Files != null &&
                    this.Files.SequenceEqual(input.Files)
                ) && 
                (
                    this.DriveName == input.DriveName ||
                    (this.DriveName != null &&
                    this.DriveName.Equals(input.DriveName))
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
                if (this.EmailMessageJson != null)
                    hashCode = hashCode * 59 + this.EmailMessageJson.GetHashCode();
                if (this.Files != null)
                    hashCode = hashCode * 59 + this.Files.GetHashCode();
                if (this.DriveName != null)
                    hashCode = hashCode * 59 + this.DriveName.GetHashCode();
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
