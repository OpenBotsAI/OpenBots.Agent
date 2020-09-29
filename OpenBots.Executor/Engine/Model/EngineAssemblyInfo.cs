namespace OpenBots.Executor.Model
{
    public class EngineAssemblyInfo
    {
        /// <summary>
        /// Gets Assembly File Name
        /// </summary>
        public string FileName { get; } = "OpenBots.Engine.dll";

        /// <summary>
        /// Gets Class Name as Type
        /// </summary>
        public string ClassName { get; } = "OpenBots.Engine.AutomationEngineInstance";

        /// <summary>
        /// Gets Method Name to Execute Script
        /// </summary>
        public string MethodName { get; } = "ExecuteScriptAsync";
    }
}
