using System;
using System.Collections.Generic;

namespace CS2Interface {
	public static class ListExtensions {
		// Necessary for non-generic ASF support, which usually strips out Random.Shared.Shuffle
		public static void Shuffle<T>(this IList<T> list) {
			for (int i = list.Count - 1; i > 0; i--) {
				int j = Random.Shared.Next(i + 1);
				(list[i], list[j]) = (list[j], list[i]);
			}
		}
	}
}
