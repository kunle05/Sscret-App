using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Secrets.Models;

namespace Secrets.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
        public HomeController(MyContext context)
        {
            dbContext = context;
        }


        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("/createUser")]
        public IActionResult CreateUser(User User)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(u => u.Email == User.Email))
                {
                    ModelState.AddModelError("User.Email", "Email already exists!");
                    return View("Index");
                }

                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                User.Password = Hasher.HashPassword(User, User.Password);

                dbContext.Add(User);
                dbContext.SaveChanges();

                User thisUser = dbContext.Users.Last();
                int id = thisUser.UserId;
                HttpContext.Session.SetInt32("UserId", id);
                HttpContext.Session.SetString("Username", thisUser.FirstName);

                return RedirectToAction("Secret");
            }
            else
            {
                return View("Index");
            }
        }

        [HttpPost("/signin")]
        public IActionResult UserLogin(myUser myUser)
        {
            if(ModelState.IsValid)
            {
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == myUser.Email);
                if(userInDb == null)
                {
                    ModelState.AddModelError("myUser.Email", "Invalid Email or Password");
                    return View("Index");
                }

                var hasher = new PasswordHasher<myUser>();
                var result = hasher.VerifyHashedPassword(myUser, userInDb.Password, myUser.Password);

                if(result == 0)
                {
                    ModelState.AddModelError("myUser.Email", "Invalid Email or Password");
                    return View("Index");
                }
                HttpContext.Session.SetInt32("UserId", userInDb.UserId);
                HttpContext.Session.SetString("Username", userInDb.FirstName);

                return RedirectToAction("Secret");
            }
            return View("Index");
        }

        [HttpGet("secret")]
        public IActionResult Secret()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if(userId == null)
            {
                ModelState.AddModelError("myUser.Email", "Login to continue");
                return View("Index");
            }
            int id = (int)userId;
            ViewBag.User = id;

            string Name = HttpContext.Session.GetString("Username");
            ViewBag.Name = Name;

            AllSecrets Secrets = new AllSecrets()
            {
                allSecrets = dbContext.Secrets.Include(sec => sec.Poster)
                .Include(sec => sec.Likes)
                .ThenInclude(Like => Like.User)
                .OrderByDescending(d => d.CreatedAt).Take(10)
                .ToList()
            };
            return View(Secrets);
        }

        [HttpPost("post/secret")]
        public IActionResult createPost(Secret newSecret)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            ViewBag.User = (int)userId;

            string Name = HttpContext.Session.GetString("Username");
            ViewBag.Name = Name;

            if(ModelState.IsValid)
            {
                if(newSecret.UserId == userId)
                {
                    dbContext.Add(newSecret);
                    dbContext.SaveChanges();
                }
                return RedirectToAction("Secret");
            }
            else
            {
                AllSecrets Secrets = new AllSecrets()
                {
                    allSecrets = dbContext.Secrets.Include(sec => sec.Poster)
                    .Include(sec => sec.Likes)
                    .ThenInclude(Like => Like.User)
                    .OrderByDescending(d => d.CreatedAt).Take(10)
                    .ToList()
                };
                return View("Secret", Secrets);
            }
        }

        [HttpGet("popular/secrets")]
        public IActionResult Popular()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if(userId == null)
            {
                ModelState.AddModelError("myUser.Email", "Login to continue");
                return View("Index");
            }
            int id = (int)userId;
            ViewBag.User = id;

            AllSecrets Secrets = new AllSecrets()
            {
                allSecrets = dbContext.Secrets.Include(sec => sec.Poster)
                .Include(sec => sec.Likes)
                .ThenInclude(Like => Like.User)
                .OrderByDescending(d => d.Likes.Count).Take(10)
                .ToList()
            };
            return View(Secrets);
        }

        [HttpGet("like/{id}")]
        public IActionResult likeSecret(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if(userId == null)
            {
                ModelState.AddModelError("myUser.Email", "Login to continue");
                return View("Index");
            }
            
            Like tolike = new Like()
            {
                UserId = (int)userId,
                SecretId = id
            };

            dbContext.Add(tolike);
            dbContext.SaveChanges();

            return RedirectToAction("Secret");
        }

        [HttpGet("destroy/{id}")]
        public IActionResult deleteSecret(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if(userId == null)
            {
                ModelState.AddModelError("myUser.Email", "Login to continue");
                return View("Index");
            }
            
            Secret todelete = dbContext.Secrets.FirstOrDefault(sec => sec.SecretId == id);
            dbContext.Remove(todelete);
            dbContext.SaveChanges();

            return RedirectToAction("Secret");
        }

        [HttpGet("/logout")]
        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return View("Index");
        }
    }
}
