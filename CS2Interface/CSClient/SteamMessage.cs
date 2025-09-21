using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using SteamKit2;
using SteamKit2.GC.CSGO.Internal;
using SteamKit2.Internal;

namespace CS2Interface {
	public class SteamMessage {
		public class GCCraft : IGCSerializableMessage {
			public const ushort UnknownRecipe = 0xFFFF;

			public ushort Recipe;
			public ushort ItemCount;
			public List<ulong> ItemIDs;

			public GCCraft() {
				Recipe = 0;
				ItemCount = 0;
				ItemIDs = [];
			}

			public uint GetEMsg() {
				return (uint) EGCItemMsg.k_EMsgGCCraft;
			}

			public void Serialize(Stream stream) {
				BinaryWriter bw = new BinaryWriter(stream);

				bw.Write(Recipe);
				bw.Write(ItemCount);
				foreach(ulong ItemID in ItemIDs) {
					bw.Write(ItemID);
				}
			}

			public void Deserialize(Stream stream) {
				throw new NotImplementedException();
			}
		}

		public class GCCraftResponse : IGCSerializableMessage {
			[JsonInclude]
			[JsonPropertyName("recipe")]
			public ushort Recipe;

			[JsonInclude]
			[JsonPropertyName("unknown")]
			public uint Unknown;

			[JsonInclude]
			[JsonPropertyName("itemcount")]
			public ushort ItemCount;

			[JsonInclude]
			[JsonPropertyName("itemids")]
			public List<ulong> ItemIDs;

			public GCCraftResponse() {
				Recipe = 0;
				Unknown = 0;
				ItemCount = 0;
				ItemIDs = [];
			}

			public uint GetEMsg() {
				return (uint) EGCItemMsg.k_EMsgGCCraftResponse;
			}

			public void Serialize(Stream stream) {
				throw new NotImplementedException();
			}

			public void Deserialize(Stream stream) {
				BinaryReader br = new BinaryReader(stream);

				Recipe = br.ReadUInt16();
				Unknown = br.ReadUInt32();
				ItemCount = br.ReadUInt16();
				for (int i = 0; i < ItemCount; i++) {
					ItemIDs.Add(br.ReadUInt64());
				}
			}
		}

		public sealed class ClientMicroTxnAuthRequest : ISteamSerializableMessage {
			[JsonInclude]
			[JsonPropertyName("OrderDetails")]
			[JsonConverter(typeof(KVConverter))]
        	public KeyValue OrderDetails = new();

			[JsonInclude]
			[JsonPropertyName("WalletDetails")]
			[JsonConverter(typeof(KVConverter))]
			public KeyValue WalletDetails = new();

			[JsonInclude]
			[JsonPropertyName("PurchaseUrl")]
			public string? PurchaseUrl = null;

			public EMsg GetEMsg() {
				return EMsg.ClientMicroTxnAuthRequest;
			}

			public void Serialize(Stream stream) {
				throw new NotImplementedException();
			}

			public void Deserialize(Stream stream) {
            	stream.ReadByte(); // ignore the first byte (always 1)
				OrderDetails.TryReadAsBinary(stream);
				WalletDetails.TryReadAsBinary(stream);

				ulong OrderID = OrderDetails["orderid"].AsUnsignedLong();
				ulong TransID = OrderDetails["transid"].AsUnsignedLong();
				if (OrderID != 0 && TransID != 0) {
					PurchaseUrl = String.Format("https://checkout.steampowered.com/checkout/approvetxn/{0}/?returnurl=https%3A%2F%2Fstore.steampowered.com%2Fbuyitem%2F730%2Ffinalize%2F{1}%3Fcanceledurl%3Dhttps%253A%252F%252Fstore.steampowered.com%252F%26returnhost%3Dstore.steampowered.com&canceledurl=https%3A%2F%2Fstore.steampowered.com%2F", TransID, OrderID);
				}
			}
		}
	}
}