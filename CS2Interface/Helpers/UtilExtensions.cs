using System.Collections.Generic;
using System.Linq;
using SteamKit2;

namespace CS2Interface {
	public static class UtilExtensions {
		public static List<KeyValue> Search(this KeyValue obj, string value) {
			return obj.Children.Where(x => x.Name == value).SelectMany(x => x.Children).ToList();
		}
	}
}