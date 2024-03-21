using BullPerks.Data.Dto;
using BullPerks.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BullPerks.Service
{
    public interface IBLPTokenService
    {
        Task<TokenDataDto> CalculateSupply(string contractAddress);
        Task<TokenDataDto> GetTokenDataById(long id);
        Task<IEnumerable<TokenData>> GetAllTokenData();
        //Task<TokenDataDto> CreateTokenData(TokenDataDto tokenDataDto);
        //Task<bool> UpdateTokenData(long id, TokenDataDto tokenDataDto);
        //Task<bool> DeleteTokenData(long id);

    }
}
