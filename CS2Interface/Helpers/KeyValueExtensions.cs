using System.Linq;
using SteamKit2;

namespace CS2Interface {
	public static class KeyValueExtensions {
		public static void Merge(this KeyValue into, KeyValue from) {
			foreach (KeyValue child in from.Children) {
				if (child.Name == null) {
					continue;
				}

				KeyValue? matchingChild = into.Children.FirstOrDefault(c => c.Name == child.Name);
				if (matchingChild == null) {
					into[child.Name] = child.Clone();
				} else {
					matchingChild.Merge(child);
				}
			}
		}

		public static KeyValue Clone(this KeyValue original) {
			KeyValue copy = new KeyValue(original.Name, original.Value);
			copy.Merge(original);

			return copy;
		}
	}
}
