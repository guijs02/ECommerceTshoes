﻿using EcommerceTShoes.Services.Interfaces;
using EcommerceTShoes.Services.Serialize;
using LoginAPI.Dto;
using System.Net.Http.Json;
using System.Net;
using EcommerceTShoes.Auth;

namespace EcommerceTShoes.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly HttpClient _http;
        private readonly TokenAuthenticationProvider _tokenProvider;
        private const string BASE_ADRESS = "https://localhost:7064";
        private const string API = $"{BASE_ADRESS}/api/Usuario";
        private const string ERROR_API = "Erro ao realizar a requisição API";

        public UsuarioService(HttpClient http, TokenAuthenticationProvider tokenProvider)
        {
            _http = http;
            _tokenProvider = tokenProvider;
        }

        public async Task<bool> Cadastro(CreateUsuarioDto dto)
        {
            var response = await _http.PostAsJsonAsync($"{API}/cadastro", dto);

            if (!response.IsSuccessStatusCode)
            {
                string erro = await TratarResponse(response);
                throw new Exception(erro);
            }
            return await SerializadorDeObjetos.Serializador<bool>(response);
        }

        public async Task Login(LoginUsuarioDto dto)
        {
            var response = await _http.PostAsJsonAsync($"{API}/login", dto);

            if (!response.IsSuccessStatusCode)
            {
                string erro = await TratarResponse(response);
                throw new Exception(erro);
            }
            var token = await response.Content.ReadAsStringAsync();
            await _tokenProvider.LoginTokenAction(token);
            await Task.CompletedTask;

        }
        public async Task Logout()
        {
            try
            {
                await _tokenProvider.Logout();
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<string> TratarResponse(HttpResponseMessage responseMessage)
        {
            string response;
            switch (responseMessage.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    throw new Exception("Erro na requisição");
                case HttpStatusCode.InternalServerError:
                    {
                        response = await responseMessage.Content.ReadAsStringAsync();
                    }
                    break;
                default:
                    {
                        response = ERROR_API;
                        break;
                    }

            }
            return response;
        }
    }
}