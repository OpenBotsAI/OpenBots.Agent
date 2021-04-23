using OpenBots.Agent.Core.Model;
using OpenBots.Agent.Core.Utilities;
using OpenBots.Service.Client.Manager.Logs;
using OpenBots.Service.Client.Manager.Win32;
using Serilog.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenBots.Service.Client.Manager.Execution
{
    /// <summary>
    /// Class that allows running applications with full admin rights. In
    /// addition the application launched will bypass the Vista UAC prompt.
    /// </summary>
    public class Executor
    {
        private Win32Utilities _win32Helper;
        private FileLogger _fileLogger;
        private int _rdpConnectionState = -1;

        private const int _disconnected = 0;
        private const int _connected = 1;
        private const int _errored = 2;
        public Executor(FileLogger fileLogger)
        {
            _win32Helper = new Win32Utilities();
            _fileLogger = fileLogger;
        }

        /// <summary>
        /// Launches the given application with full admin rights, and in addition bypasses the Vista UAC prompt
        /// </summary>
        /// <param name="commandLine">Command line containing automation info to be run by the executor</param>
        /// <param name="machineCredential">Machine credentials contaning the User Account info to run the job for.</param>
        /// <returns>Exit Code of the Process</returns>
        public void RunAutomation(String commandLine, MachineCredential machineCredential)
        {
            bool isRDPSession = false;
            IntPtr hPToken = IntPtr.Zero;
            RemoteDesktop rdpUtil = null;

            try
            {
                // Obtain the currently active session id for given User Credentials
                bool sessionFound = _win32Helper.GetUserSessionToken(machineCredential, ref hPToken);

                _fileLogger?.LogEvent("GetActiveUserSession", "Existing active user session " +
                    (!sessionFound ? "not found" : "found"), LogEventLevel.Information);

                // If unable to find an Active User Session (for given User Credentials)
                if (!sessionFound)
                {
                    _fileLogger?.LogEvent("ValidateUser", "Validate User Account Credentials", LogEventLevel.Information);

                    int errorCode;
                    if (!_win32Helper.ValidateUser(machineCredential, out errorCode))
                        throw new Exception($"Unable to Create an Active User Session " +
                            $"as the provided User Credentials are invalid.\n" +
                            (errorCode != 0 ? $"Error Code: {errorCode}" : string.Empty));

                    _fileLogger?.LogEvent("CreateRDPConnection", "Start RDP Connection", LogEventLevel.Information);

                    // Attempt to create a Remote Desktop Session
                    rdpUtil = new RemoteDesktop();
                    rdpUtil.ConnectionStateChangedEvent += OnConnectionStateChanged;
                    Task.Run(() => rdpUtil.CreateRdpConnection(
                        Environment.MachineName,
                        machineCredential.UserName,
                        machineCredential.Domain,
                        machineCredential.PasswordSecret));

                    _fileLogger?.LogEvent("WaitForRDPConnection", "Wait for RDP Connection", LogEventLevel.Information);

                    // Wait for RDP connection to be established within 60 sec
                    bool isConnected = WaitForRDPConnection();

                    if (!isConnected)
                    {
                        throw new Exception($"Unable to Create an Active User Session for provided Credential \"{machineCredential.Name}\" ");
                    }
                    else
                    {
                        _fileLogger?.LogEvent("CreateNewSession", $"New RDP Session Created.", LogEventLevel.Information);

                        // Obtain the id of Remote Desktop Session
                        sessionFound = _win32Helper.GetUserSessionToken(machineCredential, ref hPToken);

                        // If unable to find/create an Active User Session
                        if (!sessionFound)
                            throw new Exception($"Unable to Create an Active User Session for provided Credential \"{machineCredential.Name}\" ");

                        _fileLogger?.LogEvent("GetActiveUserSession", "RDP session " +
                            (!sessionFound ? "not found" : "found"), LogEventLevel.Information);

                        isRDPSession = true;
                    }
                }

                _fileLogger?.LogEvent("RunProcessAsCurrentUser", $"Start Automation in the RDP Session.", LogEventLevel.Information);

                // Start a new process in the current user's logon session
                _win32Helper.RunProcessAsCurrentUser(hPToken, commandLine);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (hPToken != IntPtr.Zero)
                    _win32Helper.ClosePtrHandle(hPToken);

                if (isRDPSession)
                    rdpUtil.DisconnectRDP();
            }
        }

        private bool WaitForRDPConnection(int seconds = 60)
        {
            bool isConnected = true;

            int sec = 0;
            while (sec < seconds)
            {
                if (_rdpConnectionState == _connected ||
                    _rdpConnectionState == _errored)
                    break;

                Thread.Sleep(1000);
                sec++;
            }

            if (sec == 60 ||
                _rdpConnectionState == _errored)
            {
                isConnected = false;
            }

            return isConnected;
        }

        private void OnConnectionStateChanged(object sender, int state)
        {
            _rdpConnectionState = state;
            _fileLogger?.LogEvent("OnConnectionStateChanged", $"RDP Connection State: {state}", LogEventLevel.Information);
        }
    }
}
