using Microsoft.AspNetCore.SignalR.Client;
using System;

namespace OpenBots.Service.Client.Manager.Hub
{
    public class HubManager
    {
        private readonly HubConnection _connection;
        public event Action<Tuple<Guid, Guid, Guid>> JobNotificationReceived;
        public event Action<string> NewJobNotificationReceived;
        public HubManager(string ServerURL)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl($"{ServerURL}/notification")
                .Build();
            _connection.On<Tuple<Guid,Guid,Guid>>("broadcastnewjobs", (tuple) => JobNotificationReceived?.Invoke(tuple));
            _connection.On<string>("sendjobnotification", (message) => NewJobNotificationReceived?.Invoke(message));
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
