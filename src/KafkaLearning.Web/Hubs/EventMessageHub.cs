using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace KafkaLearning.Web.Hubs
{
    public class EventMessageHub : Hub
    {

        public EventMessageHub()
        {

        }

        public async Task AddToGroup(string appName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, appName);
            await Clients.Group(appName).SendAsync("Send", $"{Context.ConnectionId} has joined the group {appName}.");
        }

        public async Task RemoveFromGroup(string appName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, appName);
            await Clients.Group(appName).SendAsync("Send", $"{Context.ConnectionId} has left the group {appName}.");
        }
    }
}