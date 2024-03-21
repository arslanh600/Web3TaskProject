using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BullPerks.Data.Dto
{
    public class TokenDataDto
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public decimal TotalSupply { get; set; }
        public decimal CirculatingSupply { get; set; }
    }
}
