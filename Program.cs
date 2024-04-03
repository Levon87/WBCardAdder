using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static AddProductToWB.Program;

namespace AddProductToWB
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // Ваша ссылка на карточку товара
            string productLink = "https://www.wildberries.ru/catalog/185967293/detail.aspx";      

            // Создаем HttpClientHandler с CookieContainer, чтобы передавать куки  в коммерческих проектах правильно получить HttpClient  через DI
            var handler = new HttpClientHandler();
            var cookieContainer = new CookieContainer();
            handler.CookieContainer = cookieContainer;

            // Добавляем необходимые куки
            var cookies = new CookieCollection();
            cookies.Add(new Cookie("BasketUID", "a3386b89e1ff4774a10a8e11d1fdb012", "/", "www.wildberries.ru"));
            cookies.Add(new Cookie("__wba_s", "1", "/", "www.wildberries.ru"));
            cookies.Add(new Cookie("_wbauid", "5202739581711636626", "/", "www.wildberries.ru"));
            cookies.Add(new Cookie("___wbu", "fdc00cae-a0e1-4c22-acdb-ed2e140614f5.1711636627", "/", "www.wildberries.ru"));
            cookies.Add(new Cookie("___wbs", "fb47e618-0b28-43a5-b8af-61c38269851c.1711726394", "/", "www.wildberries.ru"));
            cookies.Add(new Cookie("wbx-validation-key", "74cd0967-8bc1-4953-ac05-229f5ad6343e", "/", "www.wildberries.ru"));
            cookieContainer.Add(cookies);

            // Создаем HttpClient с настроенным обработчиком
            using (HttpClient client = new HttpClient(handler))
            {
                // Задаем заголовки, какие видим в запросах из браузера
                client.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");                
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br, zstd");
                client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.5");             
                client.DefaultRequestHeaders.Add("Origin", "https://www.wildberries.ru");
                client.DefaultRequestHeaders.Add("Referer", productLink);
                client.DefaultRequestHeaders.Add("X-Forwarded-For", "185.138.253.1:443");
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/97.0.4692.99 Safari/537.36");
                client.DefaultRequestHeaders.Add("Authorization", "Bearer  eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJpYXQiOjE3MTE2MzY3NDEsInZlcnNpb24iOjIsInVzZXIiOiI1MzUxNjg2OSIsInNoYXJkX2tleSI6IjEiLCJjbGllbnRfaWQiOiJ3YiIsInNlc3Npb25faWQiOiI3MjA2OWE1NjM5Mzg0NDkxYmQyNjNjNzgwZTUzMzU3NSIsInVzZXJfcmVnaXN0cmF0aW9uX2R0IjoxNjIxMDA0NjI1LCJ2YWxpZGF0aW9uX2tleSI6IjhjMDUxYzEzYzUwYzljYTgzNTNhOTk0ZjQ3OWIzNDkwZjFjMGYxNDNmM2RlNzM4ZGI0MTNjZDRkZTljNGE2ODkiLCJwaG9uZSI6InFjUk1lWlZlNWhkNkM2TGdDaDNmRGc9PSJ9.Q3-8TBZi7iaYfXXLyWa4fWe6EoOCEQWr3bn421gR0L5naWU_kvEHRWV1LyWSoZW27ehmOgya4Ntp_2FWZHqFHrxGhKDnNiT8DcGIx2nuJxvOdZ_-aBFO5Q9GIewf5tjBindgCpA2JWNhb40xwrzGRTDoIsh_dbS2KLixvAJszobUjMTuLDLlg_5OlV47C7rJwzD6cemGEcIxoNq25qwHAZTeL5F5Qi7l-7nMho4ns9uIwD0FZK_ZwBQn7JJT-3Wvs6BTlLophTWHs5Z08_R_b1zZxV5gz0iykK-6ESXiHYMweLWkPA-5PAOFJjYPvkxzrTsXPP8iv6jgeY2OnZ6A8Q");
                HttpResponseMessage cardResponse = await client.GetAsync(productLink);
                if (cardResponse.IsSuccessStatusCode)
                {
                    // Обработка успешного ответа
                    string responseBody = await cardResponse.Content.ReadAsStringAsync();
                    Console.WriteLine("Успешно получена карточка товара.");
                }
                else
                {
                    // Обработка ошибки
                    Console.WriteLine($"Ошибка при получении карточки товара: {cardResponse.StatusCode}");
                }                

                string json = "[{\"chrt_id\":305586549,\"quantity\":1,\"cod_1s\":185967293,\"client_ts\":1711731236,\"op_type\":1,\"target_url\":\"EX|1|MCS|IT|popular|||||\"}]";

                // Формируем данные для запроса
                var content = new StringContent(json, Encoding.UTF8, "application/json");                
                HttpResponseMessage response = await client.PostAsync("https://cart-storage-api.wildberries.ru/api/basket/sync?ts=1711729611898&device_id=site_b97cbd3373274b69b6699f4a71ceb68c", content);

                // Проверяем успешность запроса
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Товар успешно добавлен в корзину!");
                }
                else
                {
                    Console.WriteLine($"Ошибка при добавлении товара в корзину: {response.StatusCode}{response.RequestMessage}");
                }
            }
        }
    }
}
