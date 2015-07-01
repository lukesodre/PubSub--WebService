using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Core.Entity
{
    public class Subscriber
    {        //public int SubscriberID { get; set; }
        [Key]
        [Required]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public bool IsActive { get; set; }
    }
}