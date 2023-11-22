using System;

namespace IReves.Network
{
	public sealed class NetFragmentationInfo
	{
		public int TotalFragmentCount;
		public bool[] Received;
		public int TotalReceived;
		public int FragmentSize;
	}
}
