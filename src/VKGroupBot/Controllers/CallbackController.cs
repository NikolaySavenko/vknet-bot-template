using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using VkNet.Abstractions;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.Keyboard;
using VkNet.Model.RequestParams;
using VkNet.Model.Template;
using VkNet.Model.Template.Carousel;

namespace VKGroupBot.Controllers {
	[Route("api/[controller]")]
	[ApiController]
	public class CallbackController : ControllerBase {
		private readonly IConfiguration _configuration;
		private readonly IVkApi _vkApi;

		public CallbackController(IVkApi vkApi, IConfiguration configuration) {
			_vkApi = vkApi;
			_configuration = configuration;
		}

		[HttpPost]
		public IActionResult Callback([FromBody] JsonElement body) {
			var response = "ok";
			var type = body.GetProperty("type").GetString();

			switch (type) {
				case "confirmation":
					response = Environment.GetEnvironmentVariable("vk_response");
					break;
				case "message_new":
					var msgObject = body.GetProperty("object").GetProperty("message");
					var message = JsonConvert.DeserializeObject<Message>(msgObject.ToString());
					var tmpl = new MessageTemplate {
						Type = TemplateType.Carousel,
						Elements = new List<CarouselElement>() {
							new CarouselElement {
								Title = message.Text + "_1",
								Description =  "Description 1",
								Action = new CarouselElementAction {
									Type = CarouselElementActionType.OpenLink,
									Link = new Uri("https://vk.com")
								},
								PhotoId = "-109837093_457242809",
								Buttons = new List<MessageKeyboardButton>() {
									new MessageKeyboardButton() {
										Action = new MessageKeyboardButtonAction() {
											Type = KeyboardButtonActionType.Text,
											Label = "lABEL"
										}
									}
								}
							},
							new CarouselElement {
								Title = message.Text + "_1",
								Description =  "Description 2",
								Action = new CarouselElementAction {
									Type = CarouselElementActionType.OpenLink,
									Link = new Uri("https://vk.com")
								},
								PhotoId = "-109837093_457242809",
								Buttons = new List<MessageKeyboardButton>() {
									new MessageKeyboardButton() {
										Action = new MessageKeyboardButtonAction() {
											Type = KeyboardButtonActionType.Text,
											Label = "lABEL"
										}
									}
								}
							}
						}
					};
					if (!Request.Headers.Keys.Contains("X-Retry-Counter"))
						_vkApi.Messages.Send(new MessagesSendParams {
							RandomId = new DateTime().Millisecond,
							PeerId = message.PeerId.Value,
							Message = message.Text,
							Template = tmpl
						});

					break;
			}

			return Ok(response);
		}
	}
}