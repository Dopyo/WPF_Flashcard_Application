using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Text.Json;
using System.IO;
using Newtonsoft.Json;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient client;
        private readonly Random random;
        const string ApiKey = "";//paste your API key from Gemini here
        const string ModelName = "gemini-1.5-flash-latest";
        const string requestUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{ModelName}" +
            $":streamGenerateContent?alt=sse&key={ApiKey}";
        
        public MainWindow()
        {
            InitializeComponent();
            client = new HttpClient();
            random = new Random();
        }

        private async void myButton_Click(object sender, RoutedEventArgs e)
        {
            if (ApiKey.Equals(""))
            {
                myText.Text = "APIKEY EMPTY";
                return;
            }
            myText.Text = "";
            myText.Visibility = Visibility.Hidden;
            var lineCount = File.ReadLines(@"hsk/HSK 1.txt").Count();
            myWord.Text = File.ReadLines("hsk/HSK 1.txt").Skip(random.Next(1, lineCount-1)).Take(1).First();
            var requestBody = CreateRequestBody("Write a Chinese sentence with the word: " + myWord.Text + 
                " (No translation and don't isolate it with special characters)." +
                " Provide the English meaning of the given word by prefixing it with \"Meaning: ");
            await PostStream(requestUrl, requestBody, HandleStreamData, myText);
            //PostStream will replace content in the specified textbox with the response
        }


        private async void myCheck_Click(object sender, RoutedEventArgs e)
        {
            myWord.Visibility = Visibility.Visible;
            myReveal.Text = "Translation: \n";
            var requestBody = CreateRequestBody("Translate the Chinese sentence: " + myText.Text + 
                " \n (Add pinyin version of the sentence)");
            await PostStream(requestUrl, requestBody, HandleStreamData, myReveal);
        }

        private object CreateRequestBody(string prompt)
        {
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new
                            {
                                text = prompt
                            }
                        }
                    }
                }
            };
            return requestBody;
        }

        private void HandleStreamData(string s, TextBox t)
        {
            if (s.Length > 5 && s.StartsWith("data:"))
            {
                s = s.Substring(5); // Remove "data:" prefix
                var jsonBytes = Encoding.UTF8.GetBytes(s);
                using var doc = JsonDocument.Parse(jsonBytes);
                var root = doc.RootElement;
                try
                {
                    var generatedText = root.GetProperty("candidates")[0]
                                            .GetProperty("content")
                                            .GetProperty("parts")[0]
                                            .GetProperty("text").GetString();
                    t.Text += generatedText;
                }
                catch (Exception ex)
                {
                    t.Text += ex.Message;
                }
            }
        }




        private async Task PostStream(string url, object data, Action<string, TextBox> callback, TextBox t)
        {
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            using var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };
            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            if (response.IsSuccessStatusCode)
            {
                int bytesRead;
                var buffer = new byte[8192];
                using var responseStream = await response.Content.ReadAsStreamAsync();

                while ((bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    var chunk = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    callback(chunk, t);
                }
            }
            else
            {
                throw new Exception("Unhandled error from API " + response.Content.ReadAsStreamAsync().Result);
            }
        }
    }


}
