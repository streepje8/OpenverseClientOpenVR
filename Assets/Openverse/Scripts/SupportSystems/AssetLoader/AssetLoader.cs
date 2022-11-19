using System.Collections.Generic;

namespace Openverse.SupportSystems
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;
    
    public class AssetLoader
    {

        public static async Task DownloadFilesAsync(Dictionary<Uri, string> files, HttpClient client)
        {
            foreach (KeyValuePair<Uri,string> UriName in files)
            {
                await DownloadFileTaskAsync(UriName.Key, UriName.Value, client);
            }
        }
        
        public static async Task DownloadFileTaskAsync(Uri uri, string FileName, HttpClient client = null)
        {
            bool doDispose = client == null;
            if (doDispose) client = new HttpClient();
            using (var s = await client.GetStreamAsync(uri))
            {
                using (var fs = new FileStream(FileName, FileMode.CreateNew))
                {
                    await s.CopyToAsync(fs);
                }
            }
            if(doDispose) client.Dispose();
        }
    }
}
