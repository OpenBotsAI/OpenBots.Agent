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
    /// ServerDrive
    /// </summary>
    [DataContract]
        public partial class ServerDrive :  IEquatable<ServerDrive>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerDrive" /> class.
        /// </summary>
        /// <param name="id">id.</param>
        /// <param name="isDeleted">isDeleted (default to false).</param>
        /// <param name="createdBy">createdBy.</param>
        /// <param name="createdOn">createdOn.</param>
        /// <param name="deletedBy">deletedBy.</param>
        /// <param name="deleteOn">deleteOn.</param>
        /// <param name="timestamp">timestamp.</param>
        /// <param name="updatedOn">updatedOn.</param>
        /// <param name="updatedBy">updatedBy.</param>
        /// <param name="name">name (required).</param>
        /// <param name="fileStorageAdapterType">fileStorageAdapterType.</param>
        /// <param name="organizationId">organizationId.</param>
        /// <param name="storageSizeInBytes">storageSizeInBytes.</param>
        /// <param name="storagePath">storagePath.</param>
        public ServerDrive(Guid? id = default(Guid?), bool? isDeleted = false, string createdBy = default(string), DateTime? createdOn = default(DateTime?), string deletedBy = default(string), DateTime? deleteOn = default(DateTime?), byte[] timestamp = default(byte[]), DateTime? updatedOn = default(DateTime?), string updatedBy = default(string), string name = default(string), string fileStorageAdapterType = default(string), Guid? organizationId = default(Guid?), long? storageSizeInBytes = default(long?), string storagePath = default(string))
        {
            // to ensure "name" is required (not null)
            if (name == null)
            {
                throw new InvalidDataException("name is a required property for ServerDrive and cannot be null");
            }
            else
            {
                this.Name = name;
            }
            this.Id = id;
            // use default value if no "isDeleted" provided
            if (isDeleted == null)
            {
                this.IsDeleted = false;
            }
            else
            {
                this.IsDeleted = isDeleted;
            }
            this.CreatedBy = createdBy;
            this.CreatedOn = createdOn;
            this.DeletedBy = deletedBy;
            this.DeleteOn = deleteOn;
            this.Timestamp = timestamp;
            this.UpdatedOn = updatedOn;
            this.UpdatedBy = updatedBy;
            this.FileStorageAdapterType = fileStorageAdapterType;
            this.OrganizationId = organizationId;
            this.StorageSizeInBytes = storageSizeInBytes;
            this.StoragePath = storagePath;
        }
        
        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        [DataMember(Name="id", EmitDefaultValue=false)]
        public Guid? Id { get; set; }

        /// <summary>
        /// Gets or Sets IsDeleted
        /// </summary>
        [DataMember(Name="isDeleted", EmitDefaultValue=false)]
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// Gets or Sets CreatedBy
        /// </summary>
        [DataMember(Name="createdBy", EmitDefaultValue=false)]
        public string CreatedBy { get; set; }

        /// <summary>
        /// Gets or Sets CreatedOn
        /// </summary>
        [DataMember(Name="createdOn", EmitDefaultValue=false)]
        public DateTime? CreatedOn { get; set; }

        /// <summary>
        /// Gets or Sets DeletedBy
        /// </summary>
        [DataMember(Name="deletedBy", EmitDefaultValue=false)]
        public string DeletedBy { get; set; }

        /// <summary>
        /// Gets or Sets DeleteOn
        /// </summary>
        [DataMember(Name="deleteOn", EmitDefaultValue=false)]
        public DateTime? DeleteOn { get; set; }

        /// <summary>
        /// Gets or Sets Timestamp
        /// </summary>
        [DataMember(Name="timestamp", EmitDefaultValue=false)]
        public byte[] Timestamp { get; set; }

        /// <summary>
        /// Gets or Sets UpdatedOn
        /// </summary>
        [DataMember(Name="updatedOn", EmitDefaultValue=false)]
        public DateTime? UpdatedOn { get; set; }

        /// <summary>
        /// Gets or Sets UpdatedBy
        /// </summary>
        [DataMember(Name="updatedBy", EmitDefaultValue=false)]
        public string UpdatedBy { get; set; }

        /// <summary>
        /// Gets or Sets Name
        /// </summary>
        [DataMember(Name="name", EmitDefaultValue=false)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or Sets FileStorageAdapterType
        /// </summary>
        [DataMember(Name="fileStorageAdapterType", EmitDefaultValue=false)]
        public string FileStorageAdapterType { get; set; }

        /// <summary>
        /// Gets or Sets OrganizationId
        /// </summary>
        [DataMember(Name="organizationId", EmitDefaultValue=false)]
        public Guid? OrganizationId { get; set; }

        /// <summary>
        /// Gets or Sets StorageSizeInBytes
        /// </summary>
        [DataMember(Name="storageSizeInBytes", EmitDefaultValue=false)]
        public long? StorageSizeInBytes { get; set; }

        /// <summary>
        /// Gets or Sets StoragePath
        /// </summary>
        [DataMember(Name="storagePath", EmitDefaultValue=false)]
        public string StoragePath { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ServerDrive {\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  IsDeleted: ").Append(IsDeleted).Append("\n");
            sb.Append("  CreatedBy: ").Append(CreatedBy).Append("\n");
            sb.Append("  CreatedOn: ").Append(CreatedOn).Append("\n");
            sb.Append("  DeletedBy: ").Append(DeletedBy).Append("\n");
            sb.Append("  DeleteOn: ").Append(DeleteOn).Append("\n");
            sb.Append("  Timestamp: ").Append(Timestamp).Append("\n");
            sb.Append("  UpdatedOn: ").Append(UpdatedOn).Append("\n");
            sb.Append("  UpdatedBy: ").Append(UpdatedBy).Append("\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  FileStorageAdapterType: ").Append(FileStorageAdapterType).Append("\n");
            sb.Append("  OrganizationId: ").Append(OrganizationId).Append("\n");
            sb.Append("  StorageSizeInBytes: ").Append(StorageSizeInBytes).Append("\n");
            sb.Append("  StoragePath: ").Append(StoragePath).Append("\n");
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
            return this.Equals(input as ServerDrive);
        }

        /// <summary>
        /// Returns true if ServerDrive instances are equal
        /// </summary>
        /// <param name="input">Instance of ServerDrive to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ServerDrive input)
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
                    this.IsDeleted == input.IsDeleted ||
                    (this.IsDeleted != null &&
                    this.IsDeleted.Equals(input.IsDeleted))
                ) && 
                (
                    this.CreatedBy == input.CreatedBy ||
                    (this.CreatedBy != null &&
                    this.CreatedBy.Equals(input.CreatedBy))
                ) && 
                (
                    this.CreatedOn == input.CreatedOn ||
                    (this.CreatedOn != null &&
                    this.CreatedOn.Equals(input.CreatedOn))
                ) && 
                (
                    this.DeletedBy == input.DeletedBy ||
                    (this.DeletedBy != null &&
                    this.DeletedBy.Equals(input.DeletedBy))
                ) && 
                (
                    this.DeleteOn == input.DeleteOn ||
                    (this.DeleteOn != null &&
                    this.DeleteOn.Equals(input.DeleteOn))
                ) && 
                (
                    this.Timestamp == input.Timestamp ||
                    (this.Timestamp != null &&
                    this.Timestamp.Equals(input.Timestamp))
                ) && 
                (
                    this.UpdatedOn == input.UpdatedOn ||
                    (this.UpdatedOn != null &&
                    this.UpdatedOn.Equals(input.UpdatedOn))
                ) && 
                (
                    this.UpdatedBy == input.UpdatedBy ||
                    (this.UpdatedBy != null &&
                    this.UpdatedBy.Equals(input.UpdatedBy))
                ) && 
                (
                    this.Name == input.Name ||
                    (this.Name != null &&
                    this.Name.Equals(input.Name))
                ) && 
                (
                    this.FileStorageAdapterType == input.FileStorageAdapterType ||
                    (this.FileStorageAdapterType != null &&
                    this.FileStorageAdapterType.Equals(input.FileStorageAdapterType))
                ) && 
                (
                    this.OrganizationId == input.OrganizationId ||
                    (this.OrganizationId != null &&
                    this.OrganizationId.Equals(input.OrganizationId))
                ) && 
                (
                    this.StorageSizeInBytes == input.StorageSizeInBytes ||
                    (this.StorageSizeInBytes != null &&
                    this.StorageSizeInBytes.Equals(input.StorageSizeInBytes))
                ) && 
                (
                    this.StoragePath == input.StoragePath ||
                    (this.StoragePath != null &&
                    this.StoragePath.Equals(input.StoragePath))
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
                if (this.IsDeleted != null)
                    hashCode = hashCode * 59 + this.IsDeleted.GetHashCode();
                if (this.CreatedBy != null)
                    hashCode = hashCode * 59 + this.CreatedBy.GetHashCode();
                if (this.CreatedOn != null)
                    hashCode = hashCode * 59 + this.CreatedOn.GetHashCode();
                if (this.DeletedBy != null)
                    hashCode = hashCode * 59 + this.DeletedBy.GetHashCode();
                if (this.DeleteOn != null)
                    hashCode = hashCode * 59 + this.DeleteOn.GetHashCode();
                if (this.Timestamp != null)
                    hashCode = hashCode * 59 + this.Timestamp.GetHashCode();
                if (this.UpdatedOn != null)
                    hashCode = hashCode * 59 + this.UpdatedOn.GetHashCode();
                if (this.UpdatedBy != null)
                    hashCode = hashCode * 59 + this.UpdatedBy.GetHashCode();
                if (this.Name != null)
                    hashCode = hashCode * 59 + this.Name.GetHashCode();
                if (this.FileStorageAdapterType != null)
                    hashCode = hashCode * 59 + this.FileStorageAdapterType.GetHashCode();
                if (this.OrganizationId != null)
                    hashCode = hashCode * 59 + this.OrganizationId.GetHashCode();
                if (this.StorageSizeInBytes != null)
                    hashCode = hashCode * 59 + this.StorageSizeInBytes.GetHashCode();
                if (this.StoragePath != null)
                    hashCode = hashCode * 59 + this.StoragePath.GetHashCode();
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
