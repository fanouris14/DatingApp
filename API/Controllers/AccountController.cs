﻿using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(DataContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }


        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            //check if username exists in db
            if (await UserExists(registerDto.Username))
            {
                return BadRequest("Username is taken");
            } 

            //generate hash for passwords
            using var hmac = new HMACSHA512();

            // create a new user object based on the info received
            var user = new AppUser
            {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            // add user to..
            _context.Users.Add(user); //add user to DbContext
            await _context.SaveChangesAsync(); //save user to db

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }


        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            //check if user exists in db based on username and return that user
            var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());

            if (user == null)
            {
                return Unauthorized("Invalid Username");
            }

            //generate hash for password -- but using the already saved passwordSalt key in order to decrypt user.passwordhash
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));



            for (int i = 0; i < computedHash.Length; i++)
            {
                //System.Diagnostics.Debug.WriteLine(" =======> " + computedHash[i] + " - " + user.PasswordHash[i]);
                if (computedHash[i] != user.PasswordHash[i])
                {                  
                    return Unauthorized("invalid password");
                }
            }

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        //method to check db tables for user exists
        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
                
        }
    }
}

