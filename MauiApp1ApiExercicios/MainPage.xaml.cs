using System.Collections.ObjectModel;
using MauiApp1ApiExercicios.Models;
using MauiApp1ApiExercicios.Services;

namespace MauiApp1ApiExercicios;

public partial class MainPage : ContentPage
{
    // Serviço para buscar exercícios
    private readonly ExerciseService _exerciseService;

    // Lista dos exercícios selecionados pelo usuário
    private ObservableCollection<Exercise> _selectedExercises;

    // Timer para evitar muitas buscas enquanto o usuário está digitando
    private System.Timers.Timer _searchTimer;

    public MainPage()
    {
        InitializeComponent();

        // Inicializa o serviço
        _exerciseService = new ExerciseService();

        // Inicializa a lista de exercícios selecionados
        _selectedExercises = new ObservableCollection<Exercise>();
        listview_selecionados.ItemsSource = _selectedExercises;

        // Configura o timer de busca (espera 500ms depois que o usuário para de digitar)
        _searchTimer = new System.Timers.Timer(500);
        _searchTimer.Elapsed += async (sender, e) => await PerformSearch();
        _searchTimer.AutoReset = false; // Executa apenas uma vez por ativação
    }

    // Evento disparado quando o usuário digita no campo de busca
    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        // Para o timer anterior (se estiver rodando)
        _searchTimer.Stop();

        // Se o campo estiver vazio, esconde as sugestões
        if (string.IsNullOrWhiteSpace(e.NewTextValue))
        {
            listview_sugestoes.IsVisible = false;
            return;
        }

        // Reinicia o timer para esperar o usuário terminar de digitar
        _searchTimer.Start();
    }

    // Método que realmente faz a busca
    private async Task PerformSearch()
    {
        try
        {
            // Pega o texto digitado
            var searchTerm = "";

            // Precisamos acessar a UI na thread principal
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                searchTerm = entry_busca.Text;

                if (string.IsNullOrWhiteSpace(searchTerm))
                    return;

                // Mostra que está carregando
                listview_sugestoes.IsVisible = true;

                // Busca os exercícios
                var exercises = await _exerciseService.SearchExercisesByNameAsync(searchTerm);

                // Atualiza a lista de sugestões
                listview_sugestoes.ItemsSource = exercises;
            });
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro ao buscar exercícios: {ex.Message}", "OK");
        }
    }

    // Evento quando o usuário toca em uma sugestão
    private void OnSuggestionTapped(object sender, ItemTappedEventArgs e)
    {
        try
        {
            // Pega o exercício selecionado
            if (e.Item is Exercise selectedExercise)
            {
                // Verifica se já não foi adicionado antes
                if (!_selectedExercises.Any(x => x.Id == selectedExercise.Id))
                {
                    // Adiciona à lista de selecionados
                    _selectedExercises.Add(selectedExercise);
                }
                else
                {
                    DisplayAlert("Atenção", "Este exercício já foi adicionado!", "OK");
                }

                // Limpa o campo de busca e esconde sugestões
                entry_busca.Text = "";
                listview_sugestoes.IsVisible = false;
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Erro", $"Erro ao adicionar exercício: {ex.Message}", "OK");
        }
    }

    // Evento para remover exercício da lista
    private void OnRemoveExercise(object sender, EventArgs e)
    {
        try
        {
            // Pega o botão que foi clicado
            if (sender is Button button && button.BindingContext is Exercise exercise)
            {
                _selectedExercises.Remove(exercise);
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Erro", $"Erro ao remover exercício: {ex.Message}", "OK");
        }
    }

    // Evento do botão "Salvar Lista"
    private async void OnSaveListClicked(object sender, EventArgs e)
    {
        try
        {
            if (_selectedExercises.Count == 0)
            {
                await DisplayAlert("Atenção", "Adicione pelo menos um exercício à lista!", "OK");
                return;
            }

            // Navega para a nova tela passando a lista de exercícios
            var listaExerciciosPage = new ListaExerciciosPage(_selectedExercises.ToList());
            await Navigation.PushAsync(listaExerciciosPage);

            // Opcional: Limpa a lista atual para começar uma nova
            // _selectedExercises.Clear();
            // entry_busca.Text = "";
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro ao abrir lista: {ex.Message}", "OK");
        }
    }
}