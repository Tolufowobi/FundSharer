using System;
using System.ComponentModel.DataAnnotations;

namespace FundSharer.Models
{
    public class BaseEntity
    {
        public BaseEntity()
        {
            Id = Guid.NewGuid().ToString();
        }

        [Key, Required]
        public string Id { get; set; }
    }
}