using System.ComponentModel.DataAnnotations;

namespace Real_chat.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; }

        public UserConnection UserConnections { get; set; }

        public ICollection<ChatMessage> ChatMessages { get; set; }
    }
}
