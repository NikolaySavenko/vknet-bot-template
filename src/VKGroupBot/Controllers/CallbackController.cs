﻿using System;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using VkNet.Abstractions;
using VkNet.Model;
using VkNet.Model.RequestParams;
using VkNet.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.GroupUpdate;

namespace VKGroupBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CallbackController : ControllerBase
    {
        /// <summary>
        /// Конфигурация приложения
        /// </summary>
        private readonly IConfiguration _configuration;

        private readonly IVkApi _vkApi;

        public CallbackController(IVkApi vkApi, IConfiguration configuration)
        {
            _vkApi = vkApi;
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult Callback([FromBody] JsonElement body)
        {
             string response = "ok";

             string type = body.GetProperty("type").GetString();
             if (type == "confirmation")
                 response = Environment.GetEnvironmentVariable("vk_response");
             else if (type == "message_new")
             {
                 try
                 {
                     Message message = JsonConvert.DeserializeObject<Message>(body.GetProperty("object")
                         .GetProperty("message")
                         .ToString());
                     string msg = message.Text;
                     if (msg == "headers")
                     {
                         foreach (var key in Request.Headers.Keys)
                         {
                             msg += $"{key}: {Request.Headers[key].ToString()}\n";
                         }
                         response = "not_ok(";
                     }

                     _vkApi.Messages.Send(new MessagesSendParams
                     {
                         RandomId = new DateTime().Millisecond,
                         PeerId = message.PeerId.Value,
                         Message = msg
                     });
                 }
                 finally
                 {
                     Console.WriteLine("looks like finally");
                 }
             }
             return Ok(response);
        }
    }
}