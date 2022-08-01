using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _photoService = photoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var users = await _userRepository.GetUsersAsync();

            var usersToReturn = _mapper.Map<IEnumerable<MemberDto>>(users);
            return Ok(usersToReturn);
        }


        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);
            return _mapper.Map<MemberDto>(user);

        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var username = User.GetUsername(); // get username from jtw
            var user = await _userRepository.GetUserByUsernameAsync(username); //get real user obj from db / repo

            _mapper.Map(memberUpdateDto, user);

            _userRepository.Update(user);

            //if (await _userRepository.SaveAllAsync()) return Ok(username);
            if (await _userRepository.SaveAllAsync()) return NoContent();
            //if (await _userRepository.SaveAllAsync()) return Ok(testo);

            return BadRequest("Failed to update user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var username = User.GetUsername(); // get username from jtw
            var user = await _userRepository.GetUserByUsernameAsync(username); //get real user obj from db / repo

            var result = await _photoService.AddPhotoAsync(file);

            //check for error
            if (result.Error != null) return BadRequest(result.Error.Message);

            //create new Photo based on Cloudinary API results (upon post request)
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            // make it Main Photo if there are no other Photos connected to the user
            if (user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }

            // add photo to db
            user.Photos.Add(photo);

            //save changes & map
            if (await _userRepository.SaveAllAsync())
            {
                //return _mapper.Map<PhotoDto>(photo);
                //return CreatedAtRoute("GetUser", _mapper.Map<PhotoDto>(photo));
                return CreatedAtRoute("GetUser", new { username = user.UserName }, _mapper.Map<PhotoDto>(photo));

            }

            return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMain(int photoId)
        {
            var username = User.GetUsername(); // get username from jtw
            var user = await _userRepository.GetUserByUsernameAsync(username); //get real user obj from db / repo

            //find given user photo in db
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            //check if that photo isMain, if yes return error
            if (photo.IsMain) return BadRequest("this is already your main photo");

            //find current Main photo inside user photos in db
            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            // make current Main photo NOT Main
            if (currentMain != null) currentMain.IsMain = false;

            //make given photo Main
            photo.IsMain = true;

            //Save changes in db
            if (await _userRepository.SaveAllAsync()) return NoContent();

            //in case everything went wrong
            return BadRequest("Failed to set main photo");

        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var username = User.GetUsername(); // get username from jtw
            var user = await _userRepository.GetUserByUsernameAsync(username); //get real user obj from db / repo

            //find given user photo in db
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null) return NotFound();
            if (photo.IsMain) return BadRequest("You cannot delete your main photo");
            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error);
            }

            user.Photos.Remove(photo);

            if (await _userRepository.SaveAllAsync()) return Ok();

            return BadRequest("Failed to delete the photo");
        }

    }
}