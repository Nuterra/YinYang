using System;

namespace YinYang
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			Server s = new Server();
			s.AddRoute(new HttpRoute("/api/mydata", HttpMethod.Get, HttpMethod.Post), new MyDataHandler());
			s.AddRoute(new HttpRoute("/", HttpMethod.Get), new StaticFileHandler());
			s.Start();
			s.Stop();
		}
	}
}