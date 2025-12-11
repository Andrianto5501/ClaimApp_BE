using Microsoft.AspNetCore.Mvc;
using Project.Domain.DTOs.Requests;
using Project.Domain.Interfaces.Services;

namespace Project.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClaimsController : ControllerBase
    {
        private readonly IClaimService _service;

        public ClaimsController(IClaimService service)
        {
            _service = service;
        }

        /// <summary>
        /// List claims with filtering + paging
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] ClaimPagingRequestDto param)
        {
            var (items, total) = await _service.GetList(param);
            return Ok(new
            {
                Total = total,
                Items = items
            });
        }

        /// <summary>
        /// Get claim detail with 30s caching
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetDetail(Guid id)
        {
            var result = await _service.GetDetail(id);
            return Ok(result);
        }

        /// <summary>
        /// Create a new claim
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddClaimRequestDto param)
        {
            var id = await _service.Add(param);

            if (id == Guid.Empty)
                return BadRequest("Failed to create claim");

            return CreatedAtAction(nameof(GetDetail), new { id }, new { Id = id });
        }

        /// <summary>
        /// Update claim status
        /// </summary>
        [HttpPut("{id:guid}/status")]
        public async Task<IActionResult> Modify(Guid id, [FromBody] ClaimModifyStatusRequestDto param)
        {
            if (id != param.ClaimId)
                return BadRequest("Invalid ClaimId");

            var updated = await _service.ModifyStatus(param);

            if (updated == Guid.Empty)
                return BadRequest("Failed to update claim status");

            return Ok(new { Id = updated });
        }
    }
}
