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
    /// FileFolderViewModel
    /// </summary>
    [DataContract]
        public partial class FileFolderViewModel :  IEquatable<FileFolderViewModel>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileFolderViewModel" /> class.
        /// </summary>
        /// <param name="id">id.</param>
        /// <param name="name">name.</param>
        /// <param name="size">size.</param>
        /// <param name="storagePath">storagePath.</param>
        /// <param name="fullStoragePath">fullStoragePath.</param>
        /// <param name="hasChild">hasChild.</param>
        /// <param name="contentType">contentType.</param>
        /// <param name="createdBy">createdBy.</param>
        /// <param name="createdOn">createdOn.</param>
        /// <param name="updatedOn">updatedOn.</param>
        /// <param name="isFile">isFile.</param>
        /// <param name="parentId">parentId.</param>
        /// <param name="storageDriveId">storageDriveId.</param>
        /// <param name="content">content.</param>
        /// <param name="files">files.</param>
        /// <param name="hash">hash.</param>
        public FileFolderViewModel(Guid? id = default(Guid?), string name = default(string), long? size = default(long?), string storagePath = default(string), string fullStoragePath = default(string), bool? hasChild = default(bool?), string contentType = default(string), string createdBy = default(string), DateTime? createdOn = default(DateTime?), DateTime? updatedOn = default(DateTime?), bool? isFile = default(bool?), Guid? parentId = default(Guid?), Guid? storageDriveId = default(Guid?), FileStream content = default(FileStream), List<byte[]> files = default(List<byte[]>), string hash = default(string))
        {
            this.Id = id;
            this.Name = name;
            this.Size = size;
            this.StoragePath = storagePath;
            this.FullStoragePath = fullStoragePath;
            this.HasChild = hasChild;
            this.ContentType = contentType;
            this.CreatedBy = createdBy;
            this.CreatedOn = createdOn;
            this.UpdatedOn = updatedOn;
            this.IsFile = isFile;
            this.ParentId = parentId;
            this.StorageDriveId = storageDriveId;
            this.Content = content;
            this.Files = files;
            this.Hash = hash;
        }
        
        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        [DataMember(Name="id", EmitDefaultValue=false)]
        public Guid? Id { get; set; }

        /// <summary>
        /// Gets or Sets Name
        /// </summary>
        [DataMember(Name="name", EmitDefaultValue=false)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or Sets Size
        /// </summary>
        [DataMember(Name="size", EmitDefaultValue=false)]
        public long? Size { get; set; }

        /// <summary>
        /// Gets or Sets StoragePath
        /// </summary>
        [DataMember(Name="storagePath", EmitDefaultValue=false)]
        public string StoragePath { get; set; }

        /// <summary>
        /// Gets or Sets FullStoragePath
        /// </summary>
        [DataMember(Name="fullStoragePath", EmitDefaultValue=false)]
        public string FullStoragePath { get; set; }

        /// <summary>
        /// Gets or Sets HasChild
        /// </summary>
        [DataMember(Name="hasChild", EmitDefaultValue=false)]
        public bool? HasChild { get; set; }

        /// <summary>
        /// Gets or Sets ContentType
        /// </summary>
        [DataMember(Name="contentType", EmitDefaultValue=false)]
        public string ContentType { get; set; }

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
        /// Gets or Sets UpdatedOn
        /// </summary>
        [DataMember(Name="updatedOn", EmitDefaultValue=false)]
        public DateTime? UpdatedOn { get; set; }

        /// <summary>
        /// Gets or Sets IsFile
        /// </summary>
        [DataMember(Name="isFile", EmitDefaultValue=false)]
        public bool? IsFile { get; set; }

        /// <summary>
        /// Gets or Sets ParentId
        /// </summary>
        [DataMember(Name="parentId", EmitDefaultValue=false)]
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Gets or Sets StorageDriveId
        /// </summary>
        [DataMember(Name="storageDriveId", EmitDefaultValue=false)]
        public Guid? StorageDriveId { get; set; }

        /// <summary>
        /// Gets or Sets Content
        /// </summary>
        [DataMember(Name="content", EmitDefaultValue=false)]
        public FileStream Content { get; set; }

        /// <summary>
        /// Gets or Sets Files
        /// </summary>
        [DataMember(Name="files", EmitDefaultValue=false)]
        public List<byte[]> Files { get; set; }

        /// <summary>
        /// Gets or Sets Hash
        /// </summary>
        [DataMember(Name="hash", EmitDefaultValue=false)]
        public string Hash { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class FileFolderViewModel {\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  Size: ").Append(Size).Append("\n");
            sb.Append("  StoragePath: ").Append(StoragePath).Append("\n");
            sb.Append("  FullStoragePath: ").Append(FullStoragePath).Append("\n");
            sb.Append("  HasChild: ").Append(HasChild).Append("\n");
            sb.Append("  ContentType: ").Append(ContentType).Append("\n");
            sb.Append("  CreatedBy: ").Append(CreatedBy).Append("\n");
            sb.Append("  CreatedOn: ").Append(CreatedOn).Append("\n");
            sb.Append("  UpdatedOn: ").Append(UpdatedOn).Append("\n");
            sb.Append("  IsFile: ").Append(IsFile).Append("\n");
            sb.Append("  ParentId: ").Append(ParentId).Append("\n");
            sb.Append("  StorageDriveId: ").Append(StorageDriveId).Append("\n");
            sb.Append("  Content: ").Append(Content).Append("\n");
            sb.Append("  Files: ").Append(Files).Append("\n");
            sb.Append("  Hash: ").Append(Hash).Append("\n");
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
            return this.Equals(input as FileFolderViewModel);
        }

        /// <summary>
        /// Returns true if FileFolderViewModel instances are equal
        /// </summary>
        /// <param name="input">Instance of FileFolderViewModel to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(FileFolderViewModel input)
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
                    this.Size == input.Size ||
                    (this.Size != null &&
                    this.Size.Equals(input.Size))
                ) && 
                (
                    this.StoragePath == input.StoragePath ||
                    (this.StoragePath != null &&
                    this.StoragePath.Equals(input.StoragePath))
                ) && 
                (
                    this.FullStoragePath == input.FullStoragePath ||
                    (this.FullStoragePath != null &&
                    this.FullStoragePath.Equals(input.FullStoragePath))
                ) && 
                (
                    this.HasChild == input.HasChild ||
                    (this.HasChild != null &&
                    this.HasChild.Equals(input.HasChild))
                ) && 
                (
                    this.ContentType == input.ContentType ||
                    (this.ContentType != null &&
                    this.ContentType.Equals(input.ContentType))
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
                    this.UpdatedOn == input.UpdatedOn ||
                    (this.UpdatedOn != null &&
                    this.UpdatedOn.Equals(input.UpdatedOn))
                ) && 
                (
                    this.IsFile == input.IsFile ||
                    (this.IsFile != null &&
                    this.IsFile.Equals(input.IsFile))
                ) && 
                (
                    this.ParentId == input.ParentId ||
                    (this.ParentId != null &&
                    this.ParentId.Equals(input.ParentId))
                ) && 
                (
                    this.StorageDriveId == input.StorageDriveId ||
                    (this.StorageDriveId != null &&
                    this.StorageDriveId.Equals(input.StorageDriveId))
                ) && 
                (
                    this.Content == input.Content ||
                    (this.Content != null &&
                    this.Content.Equals(input.Content))
                ) && 
                (
                    this.Files == input.Files ||
                    this.Files != null &&
                    input.Files != null &&
                    this.Files.SequenceEqual(input.Files)
                ) && 
                (
                    this.Hash == input.Hash ||
                    (this.Hash != null &&
                    this.Hash.Equals(input.Hash))
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
                if (this.Size != null)
                    hashCode = hashCode * 59 + this.Size.GetHashCode();
                if (this.StoragePath != null)
                    hashCode = hashCode * 59 + this.StoragePath.GetHashCode();
                if (this.FullStoragePath != null)
                    hashCode = hashCode * 59 + this.FullStoragePath.GetHashCode();
                if (this.HasChild != null)
                    hashCode = hashCode * 59 + this.HasChild.GetHashCode();
                if (this.ContentType != null)
                    hashCode = hashCode * 59 + this.ContentType.GetHashCode();
                if (this.CreatedBy != null)
                    hashCode = hashCode * 59 + this.CreatedBy.GetHashCode();
                if (this.CreatedOn != null)
                    hashCode = hashCode * 59 + this.CreatedOn.GetHashCode();
                if (this.UpdatedOn != null)
                    hashCode = hashCode * 59 + this.UpdatedOn.GetHashCode();
                if (this.IsFile != null)
                    hashCode = hashCode * 59 + this.IsFile.GetHashCode();
                if (this.ParentId != null)
                    hashCode = hashCode * 59 + this.ParentId.GetHashCode();
                if (this.StorageDriveId != null)
                    hashCode = hashCode * 59 + this.StorageDriveId.GetHashCode();
                if (this.Content != null)
                    hashCode = hashCode * 59 + this.Content.GetHashCode();
                if (this.Files != null)
                    hashCode = hashCode * 59 + this.Files.GetHashCode();
                if (this.Hash != null)
                    hashCode = hashCode * 59 + this.Hash.GetHashCode();
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
