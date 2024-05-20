using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Http;
using System.Text.Json;
using System.IO;
using System.Windows.Media.Animation;
using System.Collections.Specialized;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string ApiKey = "";//paste your API key from Gemini here
        const string ModelName = "gemini-1.5-flash-latest";
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void myButton_Click(object sender, RoutedEventArgs e)
        {
            myWord.Visibility = Visibility.Hidden;
            myText.Text = "Processing... ";
            Random rnd = new Random();
            var lineCount = File.ReadLines(@"hsk/HSK 1.txt").Count();
            string word = File.ReadLines("hsk/HSK 1.txt").Skip(rnd.Next(1, lineCount-1)).Take(1).First();
            myWord.Text = word;
            // Create the request body
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
                                text = "Write a Chinese sentence with the word: " + word + " (No translation and don't isolate it with special characters). Provide the English meaning of the given word by prefixing it with \"Meaning: "
                            }
                        }
                    }
                }
            };

            myText.Text = await Generate(requestBody);
        }


        private async void myCheck_Click(object sender, RoutedEventArgs e)
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
                                text = "Translate the Chinese sentence: " + myText.Text + " \n (Add pinyin version of the sentence)"
                            }
                        }
                    }
                }
            };
            myWord.Visibility = Visibility.Visible;
            myReveal.Text = "Translation: \n" + await Generate(requestBody);

            
        }

        public async Task<string> Generate(object requestBody)
        {
            var requestUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{ModelName}:streamGenerateContent?alt=sse&key={ApiKey}";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var prev = DateTime.Now;// for testing the time delay
            using var response = await client.PostAsync(requestUrl, content);
            //myText.Text = (DateTime.Now - prev).TotalSeconds + "\n\n";
            string ret = "";
            if (response.IsSuccessStatusCode)
            {
                using var reader = new StreamReader(await response.Content.ReadAsStreamAsync());
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (line.Length > 5 && line.StartsWith("data:"))
                    {
                        line = line.Substring(5); // Remove "data:" prefix
                        var jsonBytes = Encoding.UTF8.GetBytes(line);
                        using var doc = JsonDocument.Parse(jsonBytes);
                        var root = doc.RootElement;
                        try
                        {
                            var generatedText = root.GetProperty("candidates")[0]
                                                .GetProperty("content")
                                                .GetProperty("parts")[0]
                                                .GetProperty("text").GetString();
                            ret += generatedText;
                        }catch (Exception ex)
                        {
                            ret += ex.Message;
                        }
                    }
                }
            }
            else
            {
                return $"Error: {response.StatusCode} - {response.ReasonPhrase}";
            }

            return ret;
        }
    }


}
