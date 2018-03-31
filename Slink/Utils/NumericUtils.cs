using System;

namespace Slink
{
	public static class NumericUtils
	{
		public static long CurrentDateTimeInMiliseconds(){
			return (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);
		}

	}
}

