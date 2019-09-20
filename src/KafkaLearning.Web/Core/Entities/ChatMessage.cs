using System;

namespace KafkaLearning.Web.Core.Entities
{
    public class ChatMessage
    {
        public Guid Id { get; set; }
        public DateTime SendDate { get; set; }
        public DateTime ReceiveDate { get; set; }
        public string Message { get; set; }
    }
}