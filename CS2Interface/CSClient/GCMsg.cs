using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using SteamKit2.GC.CSGO.Internal;
using SteamKit2.Internal;

namespace CS2Interface {
	public class GCMsg {
		public class MsgCraft : IGCSerializableMessage {
			public const ushort UnknownRecipe = 0xFFFF;

			public ushort Recipe;
			public ushort ItemCount;
			public List<ulong> ItemIDs;

			public MsgCraft() {
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

		public class MsgCraftResponse : IGCSerializableMessage {
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

			public MsgCraftResponse() {
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
	}
}