﻿namespace API.Entities
{
    public class Message
    {
        public int Id { get; set; }

        //relation
        public int SenderId { get; set; }
        public string SenderUsername { get; set; }
        public AppUser Sender { get; set; }


        public int RecipientId { get; set; }
        public string RecipientUsername { get; set; }
        public AppUser Recipient { get; set; }

        //message props
        public string Content { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; } = DateTime.Now;


        public bool SenderDeleted { get; set; }
        public bool RecipientDeleted { get; set; }
    }
}
