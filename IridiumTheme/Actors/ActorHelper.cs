using System.Web;

namespace Iridium.Actors
{
    class ActorHelper
    {
        
        private static string UrlEncode(string name)
        {
            return HttpUtility.UrlEncode(name);
        }
    }
}
