using System;

namespace YinYang
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			Server s = new Server("http://localhost/");
			s.AddRoute(new HttpRoute("/api/mydata", HttpMethod.Get, HttpMethod.Post), new MyDataHandler());
			s.AddRoute(new HttpRoute("/wait", HttpMethod.Get), new WaitHandler());
			s.AddRoute(new HttpRoute("/", HttpMethod.Get), new StaticFileHandler());
			s.Start();
			Console.WriteLine("Server Started");
			Console.ReadLine();
			s.Stop();
		}
	}
}