using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WowsGlobal.Models
{
    public class Company : Managed
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int? AdminId { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }

        public int? PoolId { get; set; }

    }
}
