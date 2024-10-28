using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NewsReader
{
    public class NntpClient : INntpClient
    {
        private TcpClient tcpClient;
        private NetworkStream networkStream;
        private StreamReader reader;
        private StreamWriter writer;

        public async Task ConnectAsync(string hostname, int port)
        {
            tcpClient = new TcpClient(hostname, port);
            networkStream = tcpClient.GetStream();

            //Set up stream reader and writer
            reader = new StreamReader(networkStream, Encoding.UTF8);
            writer = new StreamWriter(networkStream, Encoding.ASCII) { AutoFlush = true };

            //Checks response from server
            var response = await reader.ReadLineAsync();
            if (!(response.StartsWith("200") || response.StartsWith("201")))
            {
                throw new Exception($"Failed to connect to server: {response}");
            }
        }



        public async Task AuthenticateAsync(string username, string password)
        {
            //Sends AUTHINFO USER
            await writer.WriteLineAsync($"AUTHINFO USER {username}");
            var response = await reader.ReadLineAsync();

            //281 Meaning ok, no pass required
            if (response.StartsWith("281"))
                return;

            //381 Meaning ok, but send rest (password)
            //Sends AUTHINFO PASS
            if (response.StartsWith("381"))
            {
                await writer.WriteLineAsync($"AUTHINFO PASS {password}");
                response = await reader.ReadLineAsync();
                if (!response.StartsWith("281"))
                    throw new Exception("Authentication failed.");
            }
            else
            {
                throw new Exception("Authentication failed.");
            }
        }



        public async Task<List<string>> GetNewsgroupsAsync()
        {
            //Sends LIST command
            await writer.WriteLineAsync("LIST");
            var response = await reader.ReadLineAsync();

            //Throw exception if server doesnt return 215 
            if (!response.StartsWith("215"))
            {
                throw new Exception("Failed to retrieve newsgroups.");
            }

            var newsgroups = new List<string>();
            string line;

            //Reads all lines from server until a line with only "." is reached, indicating end
            //Adds each newsgroup first word in the line the list
            while ((line = await reader.ReadLineAsync()) != null && line != ".")
            {
                var newsgroup = line.Split(' ')[0];
                newsgroups.Add(newsgroup);
            }

            return newsgroups;
        }



        public async Task<string> GetArticleAsync(string articleNumber)
        {
            //Send command for ARTICLE with specified number
            await writer.WriteLineAsync($"ARTICLE {articleNumber}");
            var response = await reader.ReadLineAsync();


            if (!response.StartsWith("220"))
            {
                throw new Exception("Failed to retrieve article.");
            }

            //Stringbuilder for getting each line from the article 
            var articleContent = new StringBuilder();
            string line;

            //Reads until "."
            while ((line = await reader.ReadLineAsync()) != null && line != ".")
            {
                articleContent.AppendLine(line);
            }

            //Returns the full content as a string
            return articleContent.ToString();
        }



        public async Task<List<(int ArticleNumber, string Subject)>> GetArticleHeadersAsync(string newsgroupName)
        {
            // Sends GROUP command
            await writer.WriteLineAsync($"GROUP {newsgroupName}");
            var response = await reader.ReadLineAsync();

            // Check if the newsgroup selection was successful 
            if (!response.StartsWith("211"))
                throw new Exception($"Failed to select newsgroup: {response}");


            // Get the range of article numbers in the newsgroup
            var parts = response.Split(' ');
            int startArticle = int.Parse(parts[2]);
            int endArticle = int.Parse(parts[3]);

            var headers = new List<(int, string)>();


            // Sends XOVER for the article headers in the previously calculated range
            await writer.WriteLineAsync($"XOVER {startArticle}-{endArticle}");
            response = await reader.ReadLineAsync();


            // Check if the header retrieval was successful
            if (!response.StartsWith("224"))
                throw new Exception("Failed to retrieve article headers.");


            // Read each header line and extract article number and subject
            while ((response = await reader.ReadLineAsync()) != null && response != ".")
            {
                var headerParts = response.Split('\t');
                if (headerParts.Length >= 2)
                    headers.Add((int.Parse(headerParts[0]), headerParts[1]));
            }

            return headers;
        }


        //Closing streams 
        public void Disconnect()
        {
            writer?.WriteLine("QUIT");
            writer?.Close();
            reader?.Close();
            networkStream?.Close();
            tcpClient?.Close();

            Console.WriteLine("Disconnected.");
        }
    }
}
