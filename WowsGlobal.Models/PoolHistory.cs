using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WowsGlobal.Models
{
    public class PoolHistory : Managed
    {
        [Key]
        public int Id { get; set; }
        public int poolId { get; set; }
        public int UserId { get; set; }
        public int poolChangeId { get; set; }

        [Required]
        public int PoolSize { get; set; } = 0;

        [Required]
        public int Action { get; set; } = 0;
        /*
         * 0: Unknown action
         * 1: Company increase or decrease pool size
         * 2: ESOPs exercised and deducted from pool size
         * 3: ESOPs bought back and added to the pool size
         */
    }
}
