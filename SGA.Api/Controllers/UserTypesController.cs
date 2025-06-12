using Microsoft.AspNetCore.Mvc;
using SGA.Application.DTOs;
using SGA.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGA.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserTypesController : ControllerBase
    {
        private readonly IUserTypeRepository _userTypeRepository;

        public UserTypesController(IUserTypeRepository userTypeRepository)
        {
            _userTypeRepository = userTypeRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserTypeDto>>> GetAll()
        {
            var userTypes = await _userTypeRepository.GetAllAsync();
            var userTypeDtos = userTypes.Select(ut => new UserTypeDto
            {
                Id = ut.Id,
                Name = ut.Name,
                Description = ut.Description
            });

            return Ok(userTypeDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserTypeDto>> GetById(int id)
        {
            var userType = await _userTypeRepository.GetByIdAsync(id);
            if (userType == null)
            {
                return NotFound();
            }

            var userTypeDto = new UserTypeDto
            {
                Id = userType.Id,
                Name = userType.Name,
                Description = userType.Description
            };

            return Ok(userTypeDto);
        }
    }
}
