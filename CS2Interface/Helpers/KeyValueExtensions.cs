using System.Linq;
using SteamKit2;

namespace CS2Interface {
	public static class KeyValueExtensions {
		public static void Merge(this KeyValue into, KeyValue from, int? mergeDepth = null) {
			if (mergeDepth != null) {
				mergeDepth--;
			}

			foreach (KeyValue child in from.Children) {
				if (child.Name == null) {
					continue;
				}

				KeyValue? matchingChild = into.Children.FirstOrDefault(c => c.Name == child.Name);
				if (matchingChild == null) {
					into[child.Name] = child.Clone(mergeDepth);
				} else if (mergeDepth == null || mergeDepth >= 0) {
					matchingChild.Merge(child, mergeDepth);
				} else {
					if (child.Children.Count == 0 && matchingChild.Value == child.Value) {
						// ignore duplicate error entries, ex: paint_kits_rarity.sp_palm_night = "uncommon" appears twice
						continue;
					}

					into.Children.Add(child.Clone(mergeDepth));
				}
			}
		}

		public static KeyValue Clone(this KeyValue original, int? mergeDepth = null) {
			KeyValue copy = new KeyValue(original.Name, original.Value);
			copy.Merge(original, mergeDepth);

			return copy;
		}
	}
}
