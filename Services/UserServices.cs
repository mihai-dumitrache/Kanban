using Kanban.Models;
using Kanban.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;
using Kanban.Models.Enums;

namespace Kanban.Services
{
    public class UserServices : IUserServices
    {

        private MyContext _context;

        public UserServices()
        {
            _context = new MyContext();
        }
        public User GetUserByEmail(string userEmail)
        {
            User user = new User();
            user = _context.Users
                                   .Where(x => x.EmailAdress == userEmail)
                                   .FirstOrDefault<User>();

            return user;
        }

        public int AddUser(User user)
        {
            if (CheckUserByEmail(user.EmailAdress)==false && CheckPassword(user.Password)==true)
            {
                user.Password = EncodePasswordToBase64(user.Password);
                _context.Add<User>(user);
                _context.SaveChanges();
                return (int)UserCreation.CorrectUser;
            }
            else if (CheckUserByEmail(user.EmailAdress) == true && CheckPassword(user.Password) == true)
            {
                return (int)UserCreation.UserAlreadyTaken;
            }
            else if (CheckUserByEmail(user.EmailAdress) == false && CheckPassword(user.Password) == false)
            {
                return (int)UserCreation.InvalidPassword;
            }
            else { return (int)UserCreation.UnknownError; }

        }

        public bool CheckUserByEmail(string userEmail)
        {
            if (GetUserByEmail(userEmail) != null) { return true; }
            return false; 
        }

        public bool CheckPassword(string userPassword)
        {
            if (userPassword.Length > 7 && userPassword.Count(char.IsLetter) >= 1 && userPassword.Count(char.IsDigit) >= 1 && userPassword.Length - userPassword.Count(char.IsLetter) - userPassword.Count(char.IsDigit) >= 1)
            {
                return true;
            }
                return false;            
        }

        public bool CheckPasswordFromLogin(User user)
        {
            User userToCheck = new User();
            userToCheck = _context.Users
                                   .Where(x => x.EmailAdress == user.EmailAdress)
                                   .FirstOrDefault<User>();
            if (user.Password==DecodeFrom64(userToCheck.Password))
                { return true; }
            return false;
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
        public string DecodeFrom64(string password)
        {
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            System.Text.Decoder utf8Decode = encoder.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(password);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            string result = new String(decoded_char);
            return result;
        }

        
    }
    }
