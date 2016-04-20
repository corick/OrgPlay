using System;

namespace OrgPlay
{
	class MainClass
	{
		public static void Main (string[] args)
		{
            var songName = "./Songs/Moonsong.org";

            if(args.Length != 0)
            {
                songName = args[0];
            }

            new PlayerGame(songName).Run ();
		}
	}
}
