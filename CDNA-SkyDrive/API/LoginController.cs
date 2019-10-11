﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.IO;
using Newtonsoft.Json;
using CDNA_SkyDrive.Mode;
using CDNA_SkyDrive.Control;

namespace CDNA_SkyDrive.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        [HttpPost()]
        public string Post()
        {
            string Json = "";
            string a = new StreamReader(HttpContext.Request.Body).ReadToEnd();
            UserMode user = JsonConvert.DeserializeObject<UserMode>(a);
            if (!string.IsNullOrEmpty(user.Name) | !string.IsNullOrWhiteSpace(user.Name) | !string.IsNullOrEmpty(user.Pwds) | !string.IsNullOrWhiteSpace(user.Pwds))
            {
                MySqlConnection connection = new MySqlConnection(Resources.GetResources("ConnectionString"));
                connection.Open();
                MySqlCommand command = new MySqlCommand($"SELECT * FROM testbase.UserTable where  UserName='{user.Name}'and PassWord='{user.Pwds}';", connection);
                MySqlDataReader data = command.ExecuteReader();
                if (data.FieldCount != 0)
                {
                    data.Read();
                    string token = data[0].ToString() + DateTime.Now.ToString("yyyyMMddHH");
                    token += "-" + AES.EncodeAES(token);
                    data.Close();
                    connection.Close();
                    Json = JsonConvert.SerializeObject(new ReturnMode() { Data = token, Message = "OK" });
                }
                else
                    Json = JsonConvert.SerializeObject(new ReturnMode() { Data = "用户名密码错误", Message = "Error" });
                data.Close();
                connection.Close();
            }
            else
                Json = JsonConvert.SerializeObject(new ReturnMode() { Data = "空！", Message = "Error" });
            return Json;
        }
    }
}