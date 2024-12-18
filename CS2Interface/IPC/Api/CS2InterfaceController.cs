using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ArchiSteamFarm.Core;
using ArchiSteamFarm.IPC.Controllers.Api;
using ArchiSteamFarm.IPC.Responses;
using ArchiSteamFarm.Steam;
using Microsoft.AspNetCore.Mvc;
using SteamKit2.GC.CSGO.Internal;
using Swashbuckle.AspNetCore.Annotations;

namespace CS2Interface {
	[Route("Api/CS2Interface")]
	public sealed class CS2InterfaceController : ArchiController {
		[HttpGet("{botNames:required}/Start")]
		[SwaggerOperation (Summary = "Starts the CS2 Interface")]
		[ProducesResponseType(typeof(GenericResponse), (int) HttpStatusCode.OK)]
		[ProducesResponseType(typeof(GenericResponse), (int) HttpStatusCode.BadRequest)]
		public async Task<ActionResult<GenericResponse>> Start(string botNames) {
			if (string.IsNullOrEmpty(botNames)) {
				throw new ArgumentNullException(nameof(botNames));
			}
			
			HashSet<Bot>? bots = Bot.GetBots(botNames);

			if ((bots == null) || (bots.Count == 0)) {
				return BadRequest(new GenericResponse(false, string.Format(ArchiSteamFarm.Localization.Strings.BotNotFound, botNames)));
			}

			IList<(bool Success, string Message)> results = await Utilities.InParallel(bots.Select(static bot => ClientHandler.ClientHandlers[bot.BotName].Run())).ConfigureAwait(false);

			return Ok(new GenericResponse(results.All(static result => result.Success), string.Join(Environment.NewLine, results.Select(static result => result.Message))));
		}

		[HttpGet("{botNames:required}/Stop")]
		[SwaggerOperation (Summary = "Stops the CS2 Interface")]
		[ProducesResponseType(typeof(GenericResponse), (int) HttpStatusCode.OK)]
		[ProducesResponseType(typeof(GenericResponse), (int) HttpStatusCode.BadRequest)]
		public ActionResult<GenericResponse> Stop(string botNames) {
			if (string.IsNullOrEmpty(botNames)) {
				throw new ArgumentNullException(nameof(botNames));
			}
			
			HashSet<Bot>? bots = Bot.GetBots(botNames);

			if ((bots == null) || (bots.Count == 0)) {
				return BadRequest(new GenericResponse(false, string.Format(ArchiSteamFarm.Localization.Strings.BotNotFound, botNames)));
			}

			IEnumerable<string> results = bots.Select(static bot => ClientHandler.ClientHandlers[bot.BotName].Stop());

			return Ok(new GenericResponse(true, string.Join(Environment.NewLine, results)));
		}

		[HttpGet("{botNames:required}/InspectItem")]
		[SwaggerOperation (Summary = "Inspect a CS2 Item")]
		[ProducesResponseType(typeof(GenericResponse<InspectItem>), (int) HttpStatusCode.OK)]
		[ProducesResponseType(typeof(GenericResponse), (int) HttpStatusCode.BadRequest)]
		[ProducesResponseType(typeof(GenericResponse), (int) HttpStatusCode.GatewayTimeout)]
		public async Task<ActionResult<GenericResponse>> InspectItem(
			string botNames, 
			[FromQuery]
			[SwaggerParameter(Description = "The item's inspect link", Required = false)] 
			string? url = null, 
			[FromQuery] 
			[SwaggerParameter(Description = "The S value from the item's inspect link (not needed if using the url parameter)", Required = false)] 
			ulong s = 0, 
			[FromQuery] 
			[SwaggerParameter(Description = "The A value from the item's inspect link (not needed if using the url parameter)", Required = false)] 
			ulong a = 0, 
			[FromQuery] 
			[SwaggerParameter(Description = "The D value from the item's inspect link (not needed if using the url parameter)", Required = false)] 
			ulong d = 0, 
			[FromQuery] 
			[SwaggerParameter(Description = "The M value from the item's inspect link (not needed if using the url parameter)", Required = false)] 
			ulong m = 0, 
			[FromQuery] 
			[SwaggerParameter(Description = "If true, only the data recieved from the CS2 client will be provided", Required = false)] 
			bool minimal = false, 
			[FromQuery] 
			[SwaggerParameter(Description = "If true, additional raw item information will be provided", Required = false)] 
			bool showDefs = false
		) {
			if (string.IsNullOrEmpty(botNames)) {
				throw new ArgumentNullException(nameof(botNames));
			}
			
			HashSet<Bot>? bots = Bot.GetBots(botNames);
			if ((bots == null) || (bots.Count == 0)) {
				return BadRequest(new GenericResponse(false, string.Format(ArchiSteamFarm.Localization.Strings.BotNotFound, botNames)));
			}

			(Bot? bot, Client? client, string status) = ClientHandler.GetAvailableClient(bots);
			if (bot == null || client == null) {
				return BadRequest(new GenericResponse(false, status));
			}

			if (url != null) {
				// Match examples: "S76561198044353454A15696840738D14620408789313147129" or "M4321812345280946805A30263694656D3318427899365708953"
				Regex reg = new Regex(@"(?<SMlabel>[SM])(?<SM>\d+)A(?<A>\d+)D(?<D>\d+)");
				Match match = reg.Match(url);
				if (!match.Success) {
					return BadRequest(new GenericResponse(false, "Invalid url"));
				}
				
				a = ulong.Parse(match.Groups["A"].Value);
				d = ulong.Parse(match.Groups["D"].Value);
				if (match.Groups["SMlabel"].Value == "S") {
					s = ulong.Parse(match.Groups["SM"].Value);
				} else if (match.Groups["SMlabel"].Value == "M") {
					m = ulong.Parse(match.Groups["SM"].Value);
				}
			}
			
			if ((s != 0 && m != 0) // Either s or m must be non-zero
				|| (s == 0 && m == 0) // But not both at the same time
			) {
				return BadRequest(new GenericResponse(false, "Missing inputs"));
			}

			CMsgGCCStrike15_v2_Client2GCEconPreviewDataBlockResponse inspect;
			try {
				inspect = await client.InspectItem(s, a, d, m).ConfigureAwait(false);
			} catch (ClientException e) {
				return await HandleClientException(bot, e).ConfigureAwait(false);
			}

			var item = new InspectItem(inspect, s, a, d, m);
			GameObject.SetSerializationProperties(!minimal, showDefs);

			return Ok(new GenericResponse<InspectItem>(true, item));
		}

		[HttpGet("{botName:required}/PlayerProfile/{steamID:required}")]
		[SwaggerOperation (Summary = "Get a friend's CS2 player profile")]
		[ProducesResponseType(typeof(GenericResponse<CMsgGCCStrike15_v2_PlayersProfile>), (int) HttpStatusCode.OK)]
		[ProducesResponseType(typeof(GenericResponse), (int) HttpStatusCode.BadRequest)]
		[ProducesResponseType(typeof(GenericResponse), (int) HttpStatusCode.GatewayTimeout)]
		public async Task<ActionResult<GenericResponse>> PlayerProfile(string botName, ulong steamID) {
			if (string.IsNullOrEmpty(botName)) {
				throw new ArgumentNullException(nameof(botName));
			}
			
			Bot? bot = Bot.GetBot(botName);
			if (bot == null) {
				return BadRequest(new GenericResponse(false, string.Format(ArchiSteamFarm.Localization.Strings.BotNotFound, botName)));
			}

			(Client? client, string client_status) = ClientHandler.ClientHandlers[bot.BotName].GetClient();
			if (client == null) {
				return BadRequest(new GenericResponse(false, client_status));
			}

			CMsgGCCStrike15_v2_PlayersProfile player;
			try {
				player = await client.RequestPlayerProfile(steamID).ConfigureAwait(false);
			} catch (ClientException e) {
				return await HandleClientException(bot, e).ConfigureAwait(false);
			}

			return Ok(new GenericResponse<CMsgGCCStrike15_v2_PlayersProfile>(true, player));
		}

		[HttpGet("{botName:required}/Inventory/")]
		[SwaggerOperation (Summary = "Get the given bot's CS2 inventory")]
		[ProducesResponseType(typeof(GenericResponse<List<InventoryItem>>), (int) HttpStatusCode.OK)]
		[ProducesResponseType(typeof(GenericResponse), (int) HttpStatusCode.BadRequest)]
		public ActionResult<GenericResponse> Inventory(
			string botName, 
			[FromQuery] 
			[SwaggerParameter(Description = "If true, only the data recieved from the CS2 client will be provided", Required = false)] 
			bool minimal = false, 
			[FromQuery] 
			[SwaggerParameter(Description = "If true, additional raw item information will be provided", Required = false)] 
			bool showDefs = false
		) {
			if (string.IsNullOrEmpty(botName)) {
				throw new ArgumentNullException(nameof(botName));
			}
			
			Bot? bot = Bot.GetBot(botName);
			if (bot == null) {
				return BadRequest(new GenericResponse(false, string.Format(ArchiSteamFarm.Localization.Strings.BotNotFound, botName)));
			}

			(Client? client, string status) = ClientHandler.ClientHandlers[bot.BotName].GetClient(EClientStatus.Connected);
			if (client == null) {
				return BadRequest(new GenericResponse(false, status));
			}

			if (client.Inventory == null) {
				return BadRequest(new GenericResponse(false, "Inventory not loaded yet"));
			}

			List<InventoryItem> inventory = client.Inventory.Values.Where(x => x.IsVisible()).ToList();
			GameObject.SetSerializationProperties(!minimal, showDefs);

			return Ok(new GenericResponse<List<InventoryItem>>(true, inventory));
		}

		[HttpGet("{botName:required}/GetCrateContents/{crateID:required}")]
		[SwaggerOperation (Summary = "Get the contents of the given bot's crate")]
		[ProducesResponseType(typeof(GenericResponse<List<InventoryItem>>), (int) HttpStatusCode.OK)]
		[ProducesResponseType(typeof(GenericResponse), (int) HttpStatusCode.BadRequest)]
		[ProducesResponseType(typeof(GenericResponse), (int) HttpStatusCode.GatewayTimeout)]
		public async Task<ActionResult<GenericResponse>> GetCrateContents(
			string botName,			
			ulong crateID, 
			[FromQuery] 
			[SwaggerParameter(Description = "If true, only the data recieved from the CS2 client will be provided", Required = false)] 
			bool minimal = false, 
			[FromQuery] 
			[SwaggerParameter(Description = "If true, additional raw item information will be provided", Required = false)] 
			bool showDefs = false
		) {
			if (string.IsNullOrEmpty(botName)) {
				throw new ArgumentNullException(nameof(botName));
			}
			
			Bot? bot = Bot.GetBot(botName);
			if (bot == null) {
				return BadRequest(new GenericResponse(false, string.Format(ArchiSteamFarm.Localization.Strings.BotNotFound, botName)));
			}
			
			(Client? client, string client_status) = ClientHandler.ClientHandlers[bot.BotName].GetClient();
			if (client == null) {
				return BadRequest(new GenericResponse(false, client_status));
			}

			List<InventoryItem> contents;
			try {
				contents = await client.GetCasketContents(crateID).ConfigureAwait(false);
			} catch (ClientException e) {
				return await HandleClientException(bot, e).ConfigureAwait(false);
			}
			
			GameObject.SetSerializationProperties(!minimal, showDefs);

			return Ok(new GenericResponse<List<InventoryItem>>(true, contents));
		}

		[HttpGet("{botName:required}/StoreItem/{crateID:required}/{itemID:required}")]
		[SwaggerOperation (Summary = "Stores an item into the specified crate")]
		[ProducesResponseType(typeof(GenericResponse), (int) HttpStatusCode.OK)]
		[ProducesResponseType(typeof(GenericResponse), (int) HttpStatusCode.BadRequest)]
		[ProducesResponseType(typeof(GenericResponse), (int) HttpStatusCode.GatewayTimeout)]
		public async Task<ActionResult<GenericResponse>> StoreItem(string botName, ulong crateID, ulong itemID) {
			if (string.IsNullOrEmpty(botName)) {
				throw new ArgumentNullException(nameof(botName));
			}
			
			Bot? bot = Bot.GetBot(botName);
			if (bot == null) {
				return BadRequest(new GenericResponse(false, string.Format(ArchiSteamFarm.Localization.Strings.BotNotFound, botName)));
			}

			(Client? client, string client_status) = ClientHandler.ClientHandlers[bot.BotName].GetClient();
			if (client == null) {
				return BadRequest(new GenericResponse(false, client_status));
			}

			try {
				await client.AddItemToCasket(crateID, itemID).ConfigureAwait(false);			
			} catch (ClientException e) {
				return await HandleClientException(bot, e).ConfigureAwait(false);
			}

			return Ok(new GenericResponse(true));	
		}

		[HttpGet("{botName:required}/RetrieveItem/{crateID:required}/{itemID:required}")]
		[SwaggerOperation (Summary = "Retrieves an item from the specified crate")]
		[ProducesResponseType(typeof(GenericResponse), (int) HttpStatusCode.OK)]
		[ProducesResponseType(typeof(GenericResponse), (int) HttpStatusCode.BadRequest)]
		[ProducesResponseType(typeof(GenericResponse), (int) HttpStatusCode.GatewayTimeout)]
		public async Task<ActionResult<GenericResponse>> RetrieveItem(string botName, ulong crateID, ulong itemID) {
			if (string.IsNullOrEmpty(botName)) {
				throw new ArgumentNullException(nameof(botName));
			}
			
			Bot? bot = Bot.GetBot(botName);
			if (bot == null) {
				return BadRequest(new GenericResponse(false, string.Format(ArchiSteamFarm.Localization.Strings.BotNotFound, botName)));
			}

			(Client? client, string client_status) = ClientHandler.ClientHandlers[bot.BotName].GetClient();
			if (client == null) {
				return BadRequest(new GenericResponse(false, client_status));
			}

			try {
				await client.RemoveItemFromCasket(crateID, itemID).ConfigureAwait(false);
			} catch (ClientException e) {
				return await HandleClientException(bot, e).ConfigureAwait(false);
			}

			return Ok(new GenericResponse(true));
		}

		[HttpGet("{botName:required}/CraftItem/{recipeID:required}")]
		[SwaggerOperation (Summary = "Crafts an item using the specified trade up recipe")]
		[ProducesResponseType(typeof(GenericResponse<GCMsg.MsgCraftResponse>), (int) HttpStatusCode.OK)]
		[ProducesResponseType(typeof(GenericResponse), (int) HttpStatusCode.BadRequest)]
		[ProducesResponseType(typeof(GenericResponse), (int) HttpStatusCode.GatewayTimeout)]
		public async Task<ActionResult<GenericResponse>> CraftItem(
			string botName, 
			ushort recipeID, 
			[FromQuery] 
			[SwaggerParameter(Description = "A comma separated list of item ids", Required = true)] 
			string itemIDs
		) {
			if (string.IsNullOrEmpty(botName)) {
				throw new ArgumentNullException(nameof(botName));
			}
			
			Bot? bot = Bot.GetBot(botName);
			if (bot == null) {
				return BadRequest(new GenericResponse(false, string.Format(ArchiSteamFarm.Localization.Strings.BotNotFound, botName)));
			}

			(Client? client, string client_status) = ClientHandler.ClientHandlers[bot.BotName].GetClient();
			if (client == null) {
				return BadRequest(new GenericResponse(false, client_status));
			}

			List<ulong> item_ids = new();
			foreach (string itemIDString in itemIDs.Split(",")) {
				if (!ulong.TryParse(itemIDString, out ulong item_id)) {
					return BadRequest(new GenericResponse(false, String.Format(ArchiSteamFarm.Localization.Strings.ErrorParsingObject, nameof(itemIDs))));
				}

				item_ids.Add(item_id);
			}

			GCMsg.MsgCraftResponse craftResponse;
			try {
				craftResponse = await client.Craft(recipeID, item_ids).ConfigureAwait(false);
			} catch (ClientException e) {
				return await HandleClientException(bot, e).ConfigureAwait(false);
			}

			return Ok(new GenericResponse<GCMsg.MsgCraftResponse>(true, craftResponse));
		}

		private async Task<ActionResult<GenericResponse>> HandleClientException(Bot bot, ClientException e) {
			bot.ArchiLogger.LogGenericError(e.Message);
			if (e.Type == EClientExceptionType.Timeout) {
				// On timeout, verify that the client is still connected
				(bool connected, string status, _) = await ClientHandler.ClientHandlers[bot.BotName].VerifyConnection().ConfigureAwait(false);
				if (!connected) {
					bot.ArchiLogger.LogGenericError(status);

					return BadRequest(new GenericResponse(false, status));
				}

				return StatusCode((int) HttpStatusCode.GatewayTimeout, new GenericResponse(false, e.Message));
			}

			return BadRequest(new GenericResponse(false, e.Message));
		}
	}
}