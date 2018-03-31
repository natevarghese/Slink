using System;
using System.Collections.Generic;

namespace TASMobile
{
	public static class ListUtils
	{
		public static IList<T> Swap<T>(this IList<T> list, int indexA, int indexB)
		{
			if (list == null || indexA < 0 || indexA > list.Count - 1 || indexB < 0 || indexB > list.Count - 1)
				return new List<T>();


			T tmp = list[indexA];
			list[indexA] = list[indexB];
			list[indexB] = tmp;
			return list;
		}
	}
}

