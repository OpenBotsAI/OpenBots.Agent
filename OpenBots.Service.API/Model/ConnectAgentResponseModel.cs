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
    /// ConnectViewModel
    /// </summary>
    [DataContract]
    public partial class ConnectAgentResponseModel : IEquatable<ConnectAgentResponseModel>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectAgentResponseModel" /> class.
        /// </summary>
        /// <param name="agentId">agentId.</param>
        /// <param name="agentName">agentName.</param>

        public ConnectAgentResponseModel(Guid? agentId = default(Guid?), string agentName = default(string))
        {
            this.AgentId = agentId;
            this.AgentName = agentName;
        }

        /// <summary>
        /// Gets or Sets agentId
        /// </summary>
        [DataMember(Name = "agentId", EmitDefaultValue = false)]
        public Guid? AgentId { get; set; }

        /// <summary>
        /// Gets or Sets agentName
        /// </summary>
        [DataMember(Name = "agentName", EmitDefaultValue = false)]
        public string AgentName { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ConnectViewModel {\n");
            sb.Append("  Id: ").Append(AgentId).Append("\n");
            sb.Append("  Name: ").Append(AgentName).Append("\n");
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
            return this.Equals(input as ConnectAgentResponseModel);
        }

        /// <summary>
        /// Returns true if ConnectViewModel instances are equal
        /// </summary>
        /// <param name="input">Instance of ConnectViewModel to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ConnectAgentResponseModel input)
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
