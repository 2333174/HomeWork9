using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Collections;
using System.Threading.Tasks;
using System.Linq;

namespace p1
{
    class Crawler
    {
        private Hashtable urls = new Hashtable();
        private int count = 0;
        static void Main(string[] args)
        {
            Crawler myCrawler = new Crawler();
            string startUrl = "http://www.cnblogs.com/dstang2000/";
            if (args.Length >= 1) startUrl = args[0];
            myCrawler.urls.Add(startUrl, false);
            new Thread(myCrawler.Crawl).Start();
        }
        private void Crawl()
        {
            Console.WriteLine("开始爬行了....");
            while (true)
            {
                string current = null;
                //Parallel.ForEach((ICollection<string>)urls.Keys,url=> {
                //    if (!(bool)urls[url])
                //    {
                //        current = url;
                //        Console.WriteLine("爬行" + current + "页面");
                //        string html = DownLoad(current);
                //        urls[current] = true;
                //        //count++;
                //        Parse(html);
                //    }
                //});
                IEnumerable<string> urlList = from string s in urls.Keys
                                              select s;

                Parallel.ForEach(urlList, (string url, ParallelLoopState state) =>
                {
                    if ((bool)urls[url])
                    {
                        state.Break();
                        return;
                    }
                    current = url;
                });
                if (current == null || count > 10) break;
                Console.WriteLine("爬行" + current + "页面");
                string html = DownLoad(current);
                urls[current] = true;
                count++;
                Parse(html);
            }
        Console.WriteLine("爬行结束");
        }
        public string DownLoad(string url)
        {
            try
            {
                WebClient webClient = new WebClient();
                webClient.Encoding = Encoding.UTF8;
                string html = webClient.DownloadString(url);
                string fileName = count.ToString();
                File.WriteAllText(fileName, html, Encoding.UTF8);
                return html;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
            }
        }
        public void Parse(string html)
        {
            string strRef = @"(href|HREF)[]*=[]*[""'][^""'#>]+[""']";
            MatchCollection matches = new Regex(strRef).Matches(html);
            Parallel.ForEach(matches, match => {
                strRef = match.Value.Substring(match.Value.IndexOf('=') + 1).Trim('"', '\'', '#', ' ', '>');
                if (strRef.Length != 0)
                {
                    if (urls[strRef] == null) urls[strRef] = false;
                }
            });
            //foreach(Match match in matches)
            //{
            //    strRef =match.Value.Substring(match.Value.IndexOf('=')+1).Trim('"','\'','#',' ','>');
            //    if (strRef.Length == 0) continue;
            //    if (urls[strRef] == null) urls[strRef] = false;
            //}
        }
    }
}
