using System;
using System.Collections.Generic;
using System.Linq;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq.Dynamic.Core;

namespace api.Controllers
{
    [ApiController]
    [Route("api")]
    public class UserController : ControllerBase
    {
        [HttpGet]
        [Route("users")]
        public IActionResult Get(int page = 1, int rows = 10, string sort = "LastName", string dir = "asc", string filter = "")
        {
            var users = GetUsers();
            if(!string.IsNullOrEmpty(filter))
            {
                var search = filter.ToLower();
                users = users.Where(g => 
                           (g.FirstName ?? string.Empty).ToLower().Contains(search)
                        || (g.LastName ?? string.Empty).ToLower().Contains(search)
                        || (g.Email ?? string.Empty).ToLower().Contains(search)
                        || (g.City ?? string.Empty).ToLower().Contains(search)
                        || (g.Address ?? string.Empty).ToLower().Contains(search)
                        || (g.Zip ?? string.Empty).ToLower().Contains(search)
                        || (g.Phone ?? string.Empty).ToLower().Contains(search))
                    .ToList();
            }

            return Ok(new UserGridModel
            {
                TotalRecords = users.Count,
                Users = users.AsQueryable().OrderBy($"{sort} {dir}").Skip(page * rows - rows).Take(rows).ToList()
            });
        }

        [HttpGet]
        [Route("user")]
        public IActionResult Get(int id)
        {
            var users = GetUsers();
            var target = users.FirstOrDefault(g => g.Id == id);
            if(target == null)            
            {
                return BadRequest("User not found.");
            }

            return Ok(target);    
        }

        [HttpPost]
        [Route("user")]
        public IActionResult Add([FromForm]User user)
        {
            if(string.IsNullOrWhiteSpace(user.LastName))
            {
                return BadRequest("Last Name is required.");
            }

            var users = GetUsers();
            users.Add(user);
            SaveUsers(users);

            return Ok();    
        }

        [HttpPut]
        [Route("user")]
        public IActionResult Edit([FromForm]User user)
        {
            var users = GetUsers();
            var target = users.FirstOrDefault(g => g.Id == user.Id);
            if(target == null)            
            {
                return BadRequest("User not found.");
            }

            if(string.IsNullOrWhiteSpace(user.LastName))
            {
                return BadRequest("Last Name is required.");
            }

            target.FirstName = user.FirstName;
            target.LastName = user.LastName;
            target.Email = user.Email;
            target.City = user.City;
            target.Address = user.Address;
            target.Zip = user.Zip;
            target.Phone = user.Phone;

            SaveUsers(users);

            return Ok();    
        }

        [HttpDelete]
        [Route("user")]
        public IActionResult Delete(int id)
        {
            var users = GetUsers();
            var target = users.FirstOrDefault(g => g.Id == id);
            if(target == null)            
            {
                return BadRequest("User not found.");
            }

            users.Remove(target);
            SaveUsers(users);

            return Ok();    
        }

        private List<User> GetUsers()
        {
            var json = System.IO.File.ReadAllText("database.json");
            return JsonConvert.DeserializeObject<List<User>>(json);
        }

        private void SaveUsers(IList<User> users)
        {
            var json = JsonConvert.SerializeObject(users);
            System.IO.File.WriteAllText("database.json", json);
        }
    }
}
