using System.Collections;

namespace Roar
{
	/**
   * \brief IConfig is an interface for setting roar client configuration.
   **/
	public interface IConfig
	{
		// gets/sets the roar client debug mode
		bool IsDebug { get; set; }
		// gets/sets the name of the game on the roar server
		string Game { get; set; }
		// gets/sets the game authorization token, automatically set after player login/create
		string AuthToken { get; set; }
		// gets/sets the url of the roar server, defaults to https
		string RoarAPIUrl { get; set; }
	}
}

