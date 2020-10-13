using Microsoft.AspNetCore.SignalR.Client;
using OpenBots.Service.Client.Server;
using System;

namespace OpenBots.Service.Client.Manager.Hub
{
    public class HubManager
    {
        private readonly HubConnection _connection;
        public event Action<string> JobNotificationReceived;
        public HubManager()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl($"{ConnectionSettingsManager.Instance.ConnectionSettings.ServerURL}/notification")
                .Build();
            _connection.On<string>("sendjobnotification", (message) => JobNotificationReceived?.Invoke(message));
        }

        public void Connect()
        {
            _connection.StartAsync();
        }

        public void Disconnect()
        {
            _connection.StopAsync();
        }
    }
}
