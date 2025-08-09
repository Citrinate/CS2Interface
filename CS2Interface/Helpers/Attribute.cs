using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchiSteamFarm.Core;
using CS2Interface.Localization;
using SteamKit2;
using SteamKit2.GC.CSGO.Internal;

namespace CS2Interface {
	public abstract partial class IAttribute {
		internal abstract string Name { get; }
		internal abstract Type Type { get; }

		internal abstract uint ToUInt32();

		internal abstract float ToSingle();

		public abstract override string ToString();
	}
	
	public sealed class Attribute<TObject> : IAttribute where TObject : notnull {
		internal override string Name { get; }
		internal override Type Type => typeof(TObject);
		internal TObject Value;

		public Attribute(string name, TObject value) {
			Name = name;
			Value = value;
		}

		internal override uint ToUInt32() => (uint) Convert.ChangeType(Value, typeof(uint));
		internal override float ToSingle() => (float) Convert.ChangeType(Value, typeof(float));
		public override string ToString() => (string) Convert.ChangeType(Value, typeof(string));
	}

	public static class AttributeParser {
		public static Dictionary<string, IAttribute>? Parse(List<CSOEconItemAttribute>? attributes) {
			Dictionary<string, IAttribute> parsedAttributes = new();

			if (attributes == null || attributes.Count == 0) {
				return parsedAttributes;
			}

			foreach (CSOEconItemAttribute attribute in attributes) {
				KeyValue attribute_def = GameData.ItemsGame["attributes"][attribute.def_index.ToString()];
				if (attribute_def == KeyValue.Invalid) {
					ASF.ArchiLogger.LogGenericError(String.Format("{0}: attributes[{1}]", Strings.GameDataDefinitionUndefined, attribute.def_index));

					return null;
				}

				string? attribute_name = attribute_def["name"].Value;
				if (attribute_name == null) {
					ASF.ArchiLogger.LogGenericError(String.Format("Missing name for attribute: {0}", attribute.def_index.ToString()));

					return null;
				}

				switch (attribute_def["attribute_type"].Value) {
					case "uint32":
					case null when attribute_def["stored_as_integer"].Value == "1":
						parsedAttributes.Add(attribute_name, new Attribute<uint>(attribute_name, BitConverter.ToUInt32(attribute.value_bytes.ToArray(), 0)));
						break;

					case "float":
					case null when attribute_def["stored_as_integer"].Value == "0" || attribute_def["stored_as_integer"].Value == "float_floor_to_integer":
						parsedAttributes.Add(attribute_name, new Attribute<float>(attribute_name, BitConverter.ToSingle(attribute.value_bytes.ToArray())));
						break;

					case "string":
						parsedAttributes.Add(attribute_name, new Attribute<string>(attribute_name, Encoding.UTF8.GetString(attribute.value_bytes, 2, attribute.value_bytes.Length - 2)));
						break;

					case "vector":
					default:
						ASF.ArchiLogger.LogGenericError(String.Format("Unknown attribute type: {0}, value: {1}", attribute_def["attribute_type"].Value, Convert.ToBase64String(attribute.value_bytes)));
						return null;
				}
			}

			return parsedAttributes;
		}
	}
}