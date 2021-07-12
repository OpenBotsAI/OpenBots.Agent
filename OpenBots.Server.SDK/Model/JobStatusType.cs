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
    /// Stores the current status of a job
    /// </summary>
    /// <value>Stores the current status of a job</value>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum JobStatusType
    {
        /// <summary>
        /// Enum Unknown for value: Unknown
        /// </summary>
        [EnumMember(Value = "Unknown")]
        Unknown = 0,
        /// <summary>
        /// Enum New for value: New
        /// </summary>
        [EnumMember(Value = "New")]
        New = 1,
        /// <summary>
        /// Enum Assigned for value: Assigned
        /// </summary>
        [EnumMember(Value = "Assigned")]
        Assigned = 2,
        /// <summary>
        /// Enum InProgress for value: InProgress
        /// </summary>
        [EnumMember(Value = "InProgress")]
        InProgress = 3,
        /// <summary>
        /// Enum Failed for value: Failed
        /// </summary>
        [EnumMember(Value = "Failed")]
        Failed = 4,
        /// <summary>
        /// Enum Completed for value: Completed
        /// </summary>
        [EnumMember(Value = "Completed")]
        Completed = 5,
        /// <summary>
        /// Enum Stopping for value: Stopping
        /// </summary>
        [EnumMember(Value = "Stopping")]
        Stopping = 6,
        /// <summary>
        /// Enum Abandoned for value: Abandoned
        /// </summary>
        [EnumMember(Value = "Abandoned")]
        Abandoned = 9
    }
}
