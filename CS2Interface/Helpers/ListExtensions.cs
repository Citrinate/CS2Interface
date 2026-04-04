using System;
using System.Collections.Generic;

namespace CS2Interface {
	public static class ListExtensions {
		public static void Shuffle<T>(this IList<T> list) {
			for (int i = list.Count - 1; i > 0; i--) {
				int j = Random.Shared.Next(i + 1);
				(list[i], list[j]) = (list[j], list[i]);
			}
		}
	}
}
