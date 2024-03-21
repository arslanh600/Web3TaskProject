using System.ComponentModel.DataAnnotations;

namespace BullPerks.Data.Entity
{
    public  class TokenData
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal TotalSupply { get; set; }
        public decimal CirculatingSupply { get; set; }
        public string Address { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedData { get; set; }
    }
}
