using System.Collections.Generic;
using System.Linq;
using ValveKeyValue;

namespace CS2Interface {
	public static class UtilExtensions {
		public static KVObject Search(this IEnumerable<KVObject> obj, string value) {
			var objs = obj.Where(x => x.Name.ToUpper().Trim() == value.ToUpper().Trim()).SelectMany(x => x.Children);
			var result = new KVObject(value, objs);

			return result;
		}

		public static KVObject? SearchFirst(this IEnumerable<KVObject> obj, string value) {
			return obj.Where(x => x.Name.ToUpper().Trim() == value.ToUpper().Trim()).FirstOrDefault();
		}
	}
}