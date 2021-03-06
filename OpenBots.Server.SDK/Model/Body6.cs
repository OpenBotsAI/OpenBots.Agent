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
    /// Body6
    /// </summary>
    [DataContract]
        public partial class Body6 :  IEquatable<Body6>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Body6" /> class.
        /// </summary>
        /// <param name="id">id.</param>
        /// <param name="name">name.</param>
        /// <param name="contentType">contentType.</param>
        /// <param name="sizeInBytes">sizeInBytes.</param>
        /// <param name="fileId">fileId.</param>
        /// <param name="_file">_file.</param>
        /// <param name="driveName">driveName.</param>
        public Body6(Guid? id = default(Guid?), string name = default(string), string contentType = default(string), long? sizeInBytes = default(long?), Guid? fileId = default(Guid?), byte[] _file = default(byte[]), string driveName = default(string))
        {
            this.Id = id;
            this.Name = name;
            this.ContentType = contentType;
            this.SizeInBytes = sizeInBytes;
            this.FileId = fileId;
            this.File = _file;
            this.DriveName = driveName;
        }
        
        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        [DataMember(Name="Id", EmitDefaultValue=false)]
        public Guid? Id { get; set; }

        /// <summary>
        /// Gets or Sets Name
        /// </summary>
        [DataMember(Name="Name", EmitDefaultValue=false)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or Sets ContentType
        /// </summary>
        [DataMember(Name="ContentType", EmitDefaultValue=false)]
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or Sets SizeInBytes
        /// </summary>
        [DataMember(Name="SizeInBytes", EmitDefaultValue=false)]
        public long? SizeInBytes { get; set; }

        /// <summary>
        /// Gets or Sets FileId
        /// </summary>
        [DataMember(Name="FileId", EmitDefaultValue=false)]
        public Guid? FileId { get; set; }

        /// <summary>
        /// Gets or Sets File
        /// </summary>
        [DataMember(Name="File", EmitDefaultValue=false)]
        public byte[] File { get; set; }

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
            sb.Append("class Body6 {\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  ContentType: ").Append(ContentType).Append("\n");
            sb.Append("  SizeInBytes: ").Append(SizeInBytes).Append("\n");
            sb.Append("  FileId: ").Append(FileId).Append("\n");
            sb.Append("  File: ").Append(File).Append("\n");
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
            return this.Equals(input as Body6);
        }

        /// <summary>
        /// Returns true if Body6 instances are equal
        /// </summary>
        /// <param name="input">Instance of Body6 to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Body6 input)
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
                    this.ContentType == input.ContentType ||
                    (this.ContentType != null &&
                    this.ContentType.Equals(input.ContentType))
                ) && 
                (
                    this.SizeInBytes == input.SizeInBytes ||
                    (this.SizeInBytes != null &&
                    this.SizeInBytes.Equals(input.SizeInBytes))
                ) && 
                (
                    this.FileId == input.FileId ||
                    (this.FileId != null &&
                    this.FileId.Equals(input.FileId))
                ) && 
                (
                    this.File == input.File ||
                    (this.File != null &&
                    this.File.Equals(input.File))
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
                if (this.Id != null)
                    hashCode = hashCode * 59 + this.Id.GetHashCode();
                if (this.Name != null)
                    hashCode = hashCode * 59 + this.Name.GetHashCode();
                if (this.ContentType != null)
                    hashCode = hashCode * 59 + this.ContentType.GetHashCode();
                if (this.SizeInBytes != null)
                    hashCode = hashCode * 59 + this.SizeInBytes.GetHashCode();
                if (this.FileId != null)
                    hashCode = hashCode * 59 + this.FileId.GetHashCode();
                if (this.File != null)
                    hashCode = hashCode * 59 + this.File.GetHashCode();
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
