using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CS2Interface.Localization;
using SteamKit2;

namespace CS2Interface {
	public class Recipe : GameObject {
		[JsonInclude]
		[JsonPropertyName("id")]
		public ushort RecipeID { get; private init; }

		[JsonInclude]
		[JsonPropertyName("name")]
		public string? Name { get; private set; }

		[JsonInclude]
		[JsonPropertyName("inputs")]
		public string? InputDescription { get; private set; }		

		[JsonInclude]
		[JsonPropertyName("outputs")]
		public string? OutputDescription { get; private set; }
		
		[JsonInclude]
		[JsonPropertyName("quality")]
		public string? Quality { get; private set; }

		[JsonInclude]
		[JsonPropertyName("def")]
		[JsonConverter(typeof(KVConverter))]
		public KeyValue? RecipeData { get; private set; }

		public bool ShouldSerializeRecipeID() => true;
		public bool ShouldSerializeName() => Name != null && ShouldSerializeAdditionalProperties;
		public bool ShouldSerializeInputDescription() => InputDescription != null && ShouldSerializeAdditionalProperties;
		public bool ShouldSerializeOutputDescription() => OutputDescription != null && ShouldSerializeAdditionalProperties;
		public bool ShouldSerializeQuality() => Quality != null && ShouldSerializeAdditionalProperties;
		public bool ShouldSerializeRecipeData() => RecipeData != null && ShouldSerializeDefs;

		public Recipe(ushort recipeID) {
			RecipeID = recipeID;

			SetDefs();
			SetAdditionalProperties();
		}

		protected override bool SetDefs() {
			KeyValue? recipeDef = GameData.ItemsGame.GetDef("recipes", RecipeID.ToString());
			if (recipeDef == null) {
				return false;
			}

			RecipeData = recipeDef.Clone();

			return true;
		}

		protected override bool SetAdditionalProperties() {
			if (RecipeData == null) {
				return false;
			}
			
			Name = GameData.CsgoEnglish.Format(RecipeData["name"].Value?.Substring(1), GameData.CsgoEnglish[RecipeData["n_A"].Value?.Substring(1)]);
			InputDescription = GameData.CsgoEnglish.Format(RecipeData["desc_inputs"].Value?.Substring(1), RecipeData["di_A"].Value, GameData.CsgoEnglish[RecipeData["di_B"].Value?.Substring(1)]);
			OutputDescription = GameData.CsgoEnglish.Format(RecipeData["desc_outputs"].Value?.Substring(1), RecipeData["do_A"].Value, GameData.CsgoEnglish[RecipeData["do_B"].Value?.Substring(1)]);

			{
				List<KeyValue>? inputConditions = RecipeData["input_items"].Children.FirstOrDefault()?["conditions"].Children;
				if (inputConditions != null) {
					foreach (KeyValue condition in inputConditions) {
						if (condition["field"].Value == "*quality") {
							Quality = GameData.CsgoEnglish[condition["value"].Value];
							break;
						}
					}
				}
			}

			return true;
		}

		public static async Task<List<Recipe>> GetAll() {
			if (!await GameData.IsLoaded(update: false).ConfigureAwait(false)) {
				throw new ClientException(EClientExceptionType.Failed, Strings.GameDataLoadingFailed);
			}

			List<KeyValue>? kvs = GameData.ItemsGame["recipes"];
			if (kvs == null) {
				return [];
			}

			List<Recipe> recipes = [];
			foreach (KeyValue kv in kvs) {
				if (ushort.TryParse(kv.Name, out ushort recipeID)) {
					recipes.Add(new Recipe(recipeID));
				}
			}

			return recipes;
		}
	}
}
