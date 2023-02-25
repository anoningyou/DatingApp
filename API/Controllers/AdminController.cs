using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _uow;
        public AdminController(UserManager<AppUser> userManager, IUnitOfWork uow)
        {
            _uow = uow;
            _userManager = userManager;
            
        }

        [Authorize(Policy = "RequiredAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            var users = await _userManager.Users
                .OrderBy(u=>u.UserName)
                .Select(u=> new 
                {
                    u.Id,
                    Username = u.UserName,
                    Roles = u.UserRoles.Select(r=> r.Role.Name).ToList()
                })
                .ToListAsync();

            return Ok(users);
        }

        [Authorize(Policy = "RequiredAdminRole")]
        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult>EditRoles(string username,[FromQuery]string roles)
        {
            if(string.IsNullOrEmpty(roles)) return BadRequest("You must select at least one role");

            var selectedRoles = roles.Split(",").ToArray();

            var user = await _userManager.FindByNameAsync(username);

            if(user == null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);

            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if(!result.Succeeded) return BadRequest("Failed to add to roles");

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if(!result.Succeeded) return BadRequest("Failed to remove from roles");

            return Ok(await _userManager.GetRolesAsync(user));
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public async Task<ActionResult> GetPhotosForModeration()
        {
            return Ok(await _uow.PhotoRepository.GetUnapprovedPhotos());
        }

        [Authorize(policy: "ModeratePhotoRole")]
        [HttpPut("approve-photo/{photoId}")]
        public async Task<ActionResult> ApprovePhoto(int photoId)
        {
            var user = await _uow.UserRepository.GetUsersByPhotoIdAsync(photoId);
            if(user==null) return NotFound();
            var photo = user.Photos.FirstOrDefault(p=>p.Id == photoId);
            photo.IsApproved = true;
            if(!user.Photos.Any(p=> p.IsApproved==true && p.IsMain))
                photo.IsMain = true;
            if(await _uow.Complete()) 
                return Ok();
            else
                return BadRequest("Failed to approve a photo");
        }

        [Authorize(policy: "ModeratePhotoRole")]
        [HttpPut("reject-photo/{photoId}")]
        public async Task<ActionResult> RejectPhoto(int photoId)
        {
            var photo = await _uow.PhotoRepository.GetPhotoById(photoId);
            if(photo==null) return NotFound();
            photo.IsApproved = false;
            if(await _uow.Complete()) 
                return Ok();
            else
                return BadRequest("Failed to reject a photo");
        }


    }
}