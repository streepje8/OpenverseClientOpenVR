using System.Collections.Generic;

namespace Sly
{
    public class SlyFileTracker
    {
        public static List<SlyScript> currentlyBeeingEdited = new List<SlyScript>();
        public static List<string> currentlyBeeingEditedPaths = new List<string>();
    }
}