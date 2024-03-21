using BullPerks.Data.Dto;
using BullPerks.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BullPerks.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BLPTokenController : ControllerBase
    {
        private readonly IBLPTokenService _blpTokenService;

        public BLPTokenController(IBLPTokenService blpTokenService)
        {
            _blpTokenService = blpTokenService;
        }

        [Authorize]
        [HttpPost("calculate-supply")]
        public async Task<IActionResult> CalculateSupply([FromBody] SupplyCalculationDto dto)
        {
            try
            {
                return Ok(await _blpTokenService.CalculateSupply(dto.ContractAddress));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while calculating the supply.");
            }
        }
        // GET: api/BLPToken
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TokenDataDto>>> GetAllTokenData()
        {
            var tokenDataList = await _blpTokenService.GetAllTokenData();
            return Ok(tokenDataList);
        }

        // GET: api/BLPToken/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TokenDataDto>> GetTokenData(long id)
        {
            var tokenData = await _blpTokenService.GetTokenDataById(id);
            if (tokenData == null)
            {
                return NotFound();
            }

            return Ok(tokenData);
        }

        //// POST: api/BLPToken
        //[HttpPost]
        //public async Task<ActionResult<TokenDataDto>> PostTokenData(TokenDataDto tokenDataDto)
        //{
        //    var createdTokenData = await _blpTokenService.CreateTokenData(tokenDataDto);
        //    return CreatedAtAction(nameof(GetTokenData), new { id = createdTokenData.Id }, createdTokenData);
        //}

        //// PUT: api/BLPToken/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutTokenData(long id, TokenDataDto tokenDataDto)
        //{
        //    var result = await _blpTokenService.UpdateTokenData(id, tokenDataDto);
        //    if (!result)
        //    {
        //        return NotFound();
        //    }

        //    return NoContent();
        //}

        //// DELETE: api/BLPToken/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteTokenData(long id)
        //{
        //    var result = await _blpTokenService.DeleteTokenData(id);
        //    if (!result)
        //    {
        //        return NotFound();
        //    }

        //    return NoContent();
        //}
    
    }
}
