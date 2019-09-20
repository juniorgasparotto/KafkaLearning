using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace KafkaLearning.Web.Hubs
{
    public class ChatHub : Hub
    {

        public ChatHub()
        {

        }

        public async Task AddToGroup(string rommId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, rommId);
            await Clients.Group(rommId).SendAsync("Send", $"{Context.ConnectionId} has joined the group {rommId}.");
        }

        public async Task RemoveFromGroup(string rommId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, rommId);
            await Clients.Group(rommId).SendAsync("Send", $"{Context.ConnectionId} has left the group {rommId}.");
        }

        //public async Task SendMessage(ChatMessage chat)
        //{
        //    //Ao usar o método Client(_connections.GetUserId(chat.destination)) eu estou enviando a mensagem apenas para o usuário destino, não realizando broadcast
        //    await Clients.Client(_connections.GetUserId(chat.Destination)).SendAsync("Receive", chat.User, chat.Message);
        //}
    }
}