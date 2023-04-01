using System;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.RoleAttributes;
using UnityEngine.Video;

namespace StatefulUI.Runtime.References
{
	[Serializable]
	public class VideoPlayerReference : BaseReference
	{
		[Role(typeof(VideoPlayerRoleAttribute))]
		public int Role;
    
		[ChildOnly]
		public VideoPlayer VideoPlayer;
	}	
}

