using BullPerks.Data.Context;
using BullPerks.Data.Dto;
using BullPerks.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nethereum.Web3;
using Nethereum.Contracts;
using System.Numerics;
using Nethereum.Contracts.Standards.ERC721.ContractDefinition;
using RestSharp;

namespace BullPerks.Service
{
    public class BLPTokenService : IBLPTokenService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly List<string> _nonCirculatingAddresses;
        private readonly string BCLURL;  //"https://api.bscscan.com";
        private readonly string API_KEY; // "6J9N6HP5YY8RQXM3IGGBIRIHHH7A3NEQU8";
        private readonly RestClient _client;

        public BLPTokenService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            
            BCLURL = _configuration["BCL:URL"] ?? string.Empty;
            API_KEY = _configuration["BCL:API_KEY"] ?? string.Empty;
            var nonCirculatingAddress = configuration.GetSection("NonCirculatingAddresses");
            if(nonCirculatingAddress != null)
            {
                _nonCirculatingAddresses = nonCirculatingAddress?.GetChildren()?.Select(x => x?.Value)!.ToList()!;
            }

            var options = new RestClientOptions(BCLURL)
            {
                MaxTimeout = -1,
            };
            _client = new RestClient(options);
        }
        public async Task<TokenDataDto> CalculateSupply(string contractAddress)
        {
            var totalSupply = await CalculateTotalSupply(contractAddress);
            decimal nonCirculatingSupply = 0;
            foreach(var address in _nonCirculatingAddresses)
            {
                nonCirculatingSupply +=  await CalculateCirculatingSupply(contractAddress, address);
            }

            decimal circulatingSupply = totalSupply - nonCirculatingSupply;
            
            var res = await CreateTokenData(new TokenDataDto()
            {
                CirculatingSupply = circulatingSupply,
                TotalSupply = totalSupply,
                Name = "Bull Perks",
            });
            return res;
        }
        public async Task<decimal> CalculateTotalSupply(string contractAddress)
        {
            
            var request = new RestRequest($"?module=stats&action=tokensupply&contractaddress={contractAddress}&apikey={API_KEY}");
            var response = await _client.ExecuteGetAsync<BalanceResponseDto>(request);
            if (!response.IsSuccessful) return 0;

            return response.Data!.Result;
        }
        public async Task<decimal> CalculateCirculatingSupply(string contractAddress,string address)
        {
            var request = new RestRequest($"?module=account&action=tokenbalance&contractaddress={contractAddress}&address={address}&tag=latest&apikey={API_KEY}");
            var response = await _client.ExecuteGetAsync<BalanceResponseDto>(request);
            if (!response.IsSuccessful) return 0;

            return response.Data!.Result;
        }

        public async Task<TokenDataDto> GetTokenDataById(long id)
        {
            var tokenData = await _context.TokenData.FindAsync(id);
            if (tokenData == null || tokenData.IsDeleted)
            {
                return null;
            }

            return MapTokenDataToDto(tokenData);
        }
        public async Task<IEnumerable<TokenData>> GetAllTokenData()
        {
            var tokenDataList = await _context.TokenData
                .Where(t => !t.IsDeleted)
                //.Select(t => MapTokenDataToDto(t))
                .ToListAsync();

            return tokenDataList;
        }
        public async Task<TokenDataDto> CreateTokenData(TokenDataDto tokenDataDto)
        {
            var tokenData = new TokenData
            {
                Name = tokenDataDto.Name,
                TotalSupply = tokenDataDto.TotalSupply,
                CirculatingSupply = tokenDataDto.CirculatingSupply,
                CreatedDate = DateTime.UtcNow,
                UpdatedData = null,
                Address = " ",
                IsDeleted = false
            };

            _context.TokenData.Add(tokenData);
            await _context.SaveChangesAsync();

            return MapTokenDataToDto(tokenData);
        }
        public async Task<bool> UpdateTokenData(long id, TokenDataDto tokenDataDto)
        {
            var tokenData = await _context.TokenData.FindAsync(id);
            if (tokenData == null || tokenData.IsDeleted)
            {
                return false;
            }

            tokenData.Name = tokenDataDto.Name;
            tokenData.TotalSupply = tokenDataDto.TotalSupply;
            tokenData.CirculatingSupply = tokenDataDto.CirculatingSupply;
            tokenData.UpdatedData = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                // Handle exception
                return false;
            }
        }
        public async Task<bool> DeleteTokenData(long id)
        {
            var tokenData = await _context.TokenData.FindAsync(id);
            if (tokenData == null || tokenData.IsDeleted)
            {
                return false;
            }

            tokenData.IsDeleted = true;
            tokenData.UpdatedData = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                // Handle exception
                return false;
            }
        }
        private TokenDataDto MapTokenDataToDto(TokenData tokenData)
        {
            return new TokenDataDto
            {
                Id = tokenData.Id,
                Name = tokenData.Name,
                TotalSupply = tokenData.TotalSupply,
                CirculatingSupply = tokenData.CirculatingSupply
            };
        }

    }
}
