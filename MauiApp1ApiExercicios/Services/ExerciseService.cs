using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MauiApp1ApiExercicios.Models;

namespace MauiApp1ApiExercicios.Services
{
    public class ExerciseService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://exercisedb.p.rapidapi.com";

        // IMPORTANTE: SUBSTITUA pela sua chave real da RapidAPI
        private const string ApiKey = "COLE_SUA_CHAVE_AQUI";

        // Cache local dos exercícios para evitar muitas chamadas à API
        private List<Exercise> _cachedExercises = new List<Exercise>();
        private DateTime _lastCacheUpdate = DateTime.MinValue;

        public ExerciseService()
        {
            _httpClient = new HttpClient();

            // Configurando headers para a API
            _httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Host", "exercisedb.p.rapidapi.com");
            _httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Key", ApiKey);

            // Timeout de 30 segundos
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        // Buscar exercícios da API (com cache)
        private async Task<List<Exercise>> GetAllExercisesAsync()
        {
            try
            {
                // Se o cache é recente (menos de 10 minutos), usa ele
                if (_cachedExercises.Count > 0 &&
                    DateTime.Now.Subtract(_lastCacheUpdate).TotalMinutes < 10)
                {
                    return _cachedExercises;
                }

                // Verifica se a chave foi configurada
                if (ApiKey == "COLE_SUA_CHAVE_AQUI")
                {
                    // Retorna dados falsos se a chave não foi configurada
                    return GetFakeExercises();
                }

                System.Diagnostics.Debug.WriteLine("Buscando exercícios da API...");

                // Busca apenas 50 exercícios para economizar requests
                var response = await _httpClient.GetStringAsync($"{BaseUrl}/exercises?limit=50");

                var exercises = JsonConvert.DeserializeObject<List<Exercise>>(response);

                if (exercises != null && exercises.Count > 0)
                {
                    _cachedExercises = exercises;
                    _lastCacheUpdate = DateTime.Now;

                    System.Diagnostics.Debug.WriteLine($"✅ {exercises.Count} exercícios carregados da API!");
                    return exercises;
                }

                return GetFakeExercises();
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro de rede: {ex.Message}");
                return GetFakeExercises();
            }
            catch (TaskCanceledException ex)
            {
                System.Diagnostics.Debug.WriteLine($"⏰ Timeout na API: {ex.Message}");
                return GetFakeExercises();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro geral: {ex.Message}");
                return GetFakeExercises();
            }
        }

        // Buscar exercícios por nome
        public async Task<List<Exercise>> SearchExercisesByNameAsync(string searchTerm)
        {
            try
            {
                await Task.Delay(300); // Simula um pouco de delay para UX

                if (string.IsNullOrWhiteSpace(searchTerm))
                    return new List<Exercise>();

                // Pega todos os exercícios (do cache ou API)
                var allExercises = await GetAllExercisesAsync();

                // Filtra por nome
                var filteredExercises = allExercises
                    .Where(x => x.Name.ToLower().Contains(searchTerm.ToLower()) ||
                               x.Target.ToLower().Contains(searchTerm.ToLower()) ||
                               x.BodyPart.ToLower().Contains(searchTerm.ToLower()))
                    .Take(8) // Máximo 8 sugestões
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"🔍 Busca por '{searchTerm}': {filteredExercises.Count} resultados");

                return filteredExercises;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro na busca: {ex.Message}");
                return new List<Exercise>();
            }
        }

        // Dados falsos para fallback (caso API não funcione)
        // SUBSTITUA apenas o método GetFakeExercises() no seu ExerciseService.cs:

        private List<Exercise> GetFakeExercises()
        {
            return new List<Exercise>
    {
        new Exercise
        {
            Id = "1",
            Name = "Push Up",
            Target = "chest",
            BodyPart = "upper body",
            Equipment = "body weight",
            GifUrl = "https://media.giphy.com/media/l2JhpjWPccQhsAMfu/giphy.gif"
        },
        new Exercise
        {
            Id = "2",
            Name = "Squat",
            Target = "glutes",
            BodyPart = "lower body",
            Equipment = "body weight",
            GifUrl = "https://media.giphy.com/media/l0HlN5Y28D9MzzcRy/giphy.gif"
        },
        new Exercise
        {
            Id = "3",
            Name = "Plank",
            Target = "abs",
            BodyPart = "waist",
            Equipment = "body weight",
            GifUrl = "https://media.giphy.com/media/l0MYC0LajbaPoEADu/giphy.gif"
        },
        new Exercise
        {
            Id = "4",
            Name = "Jumping Jacks",
            Target = "cardiovascular",
            BodyPart = "cardio",
            Equipment = "body weight",
            GifUrl = "https://media.giphy.com/media/l0HlBO7eyXzSZkJri/giphy.gif"
        },
        new Exercise
        {
            Id = "5",
            Name = "Mountain Climbers",
            Target = "abs",
            BodyPart = "cardio",
            Equipment = "body weight",
            GifUrl = "https://media.giphy.com/media/l0MYGb1LuZ3n7dRnO/giphy.gif"
        }
    };
        }

        // Método para limpar cache (útil para debug)
        public void ClearCache()
        {
            _cachedExercises.Clear();
            _lastCacheUpdate = DateTime.MinValue;
        }

        // Método para verificar se está usando API real ou dados falsos
        public bool IsUsingRealApi()
        {
            return ApiKey != "3c2238986dmsh8ab3797b5d04079p11280bjsn5169ee680cb0";
        }
    }
}