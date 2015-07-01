using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Core.Entity
{
    public class Publisher
    {
        [Key]
        public int PublisherID { get; set; }

        
        public string SubscriberEmail { get; set; }
        [ForeignKey("SubscriberEmail")]
        public Subscriber Subscriber { get; set; }
        
        public string Message { get; set; }
    }
}