using Kanban.Models;
using Kanban.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Kanban.Services
{
    public class UserServices : IUserServices
    {
        public User GetUserByEmail(string userEmail)
        {
            User user = new User();

            var ctx = new MyContext();
            user = ctx.Users
                                   .Where(x => x.EmailAdress == userEmail)
                                   .FirstOrDefault<User>();

            return user;
        }

        public int AddUser(User user)
        {
            if (user.Password.Length > 7 && user.Password.Count(char.IsLetter) >= 1 && user.Password.Count(char.IsDigit) >= 1 && user.Password.Length - user.Password.Count(char.IsLetter) - user.Password.Count(char.IsDigit) >= 1)
            {
                var context = new MyContext();
                user.Password=EncodePasswordToBase64(user.Password);
                context.Add<User>(user);
                context.SaveChanges();
                return 0;
            }
            else
            {
                return 1;
            }

        }

        public string EncodePasswordToBase64(string password)
        {
            try
            {
                byte[] encData_byte = new byte[password.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(password);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);
            }
        }
    }
    }
