﻿using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AdaSDK;
using Newtonsoft.Json;
using TokenManagerService = AdaW10.Models.AuthenticationServices.TokenManagerService;
using Microsoft.IdentityModel.Protocols;
using System.Configuration;
using AdaSDK.Models;

namespace AdaBot.Models
{
    public class DataService
    {
        public static readonly string ApiBasePath = $"{ ConfigurationManager.AppSettings["WebAppUrl"] }/api/person";
        public static readonly string PersonsRecognitionQuery = "recognizepersonsPicture";
        public static readonly string EmotionPicture = "EmotionPicture";

        private static string token;

        public static readonly string PersonUpdateInformationQuery = "updatepersoninformation";

        private HttpClient _httpClient;

        public DataService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<PersonDto[]> recognizepersonsPictureAsync(Stream picture)
        {
            using (var streamContent = new StreamContent(picture))
            using (var formData = new MultipartFormDataContent())
            {
                // Creates uri from configuration
                var uri = new Uri($"{ApiBasePath}/{PersonsRecognitionQuery}");

                // Adds content to multipart form data 
                formData.Add(streamContent, "file", $"{Guid.NewGuid()}.jpg");

                // Get authorization token 
                token = (await TokenManagerService.NewToken());

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage resp = await _httpClient.PostAsync(uri, formData);

                if (resp.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<PersonDto[]>(await resp.Content.ReadAsStringAsync());
                }

                var error = JsonConvert.DeserializeObject<WebServiceError>(await resp.Content.ReadAsStringAsync());
                Debug.WriteLine($"WebServiceError : {error.HttpStatus} - {error.ErrorCode} : {error.ErrorMessage}");
                return null;
            }
        }

        public async Task<EmotionDto> recognizeEmotion(Stream picture)
        {
            using (var streamContent = new StreamContent(picture))
            using (var formData = new MultipartFormDataContent())
            {
                // Creates uri from configuration
                var uri = new Uri($"{ApiBasePath}/{EmotionPicture}");

                // Adds content to multipart form data 
                formData.Add(streamContent, "file", $"{Guid.NewGuid()}.jpg");

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage resp = await _httpClient.PostAsync(uri, formData);

                if (resp.IsSuccessStatusCode)
                {
                    var test = JsonConvert.DeserializeObject<EmotionDto>(await resp.Content.ReadAsStringAsync());
                    return test;
                }

                var error = JsonConvert.DeserializeObject<WebServiceError>(await resp.Content.ReadAsStringAsync());
                Debug.WriteLine($"WebServiceError : {error.HttpStatus} - {error.ErrorCode} : {error.ErrorMessage}");
                return null;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _httpClient == null) return;

            _httpClient.Dispose();
            _httpClient = null;
        }
    }
}
