using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Real_chat.Models
{
    public class UserConnection
    {
        [Key]
        public int Id { get; set; }
        public string ConnectionId { get; set; }
        public string ChatRoom {  get; set; }
        public string UserName {  get; set; }

        public int UserId { get; set; } 
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
