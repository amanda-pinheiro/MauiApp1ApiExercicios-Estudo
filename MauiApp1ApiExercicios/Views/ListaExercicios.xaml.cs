using System.Collections.ObjectModel;
using MauiApp1ApiExercicios.Models;

namespace MauiApp1ApiExercicios;

public partial class ListaExerciciosPage : ContentPage
{
    // Lista dos exercícios salvos
    private ObservableCollection<Exercise> _exerciciosSalvos;

    // Construtor que recebe a lista de exercícios da tela anterior
    public ListaExerciciosPage(List<Exercise> exercicios)
    {
        InitializeComponent();

        // Converte a lista recebida para ObservableCollection
        _exerciciosSalvos = new ObservableCollection<Exercise>(exercicios ?? new List<Exercise>());

        // Conecta a lista com a ListView
        listview_exercicios_salvos.ItemsSource = _exerciciosSalvos;

        // Atualiza a interface
        AtualizarInterface();
    }

    // Construtor vazio (caso seja chamado sem parâmetros)
    public ListaExerciciosPage()
    {
        InitializeComponent();
        _exerciciosSalvos = new ObservableCollection<Exercise>();
        listview_exercicios_salvos.ItemsSource = _exerciciosSalvos;
        AtualizarInterface();
    }

    // Método para atualizar a interface baseado na quantidade de exercícios
    private void AtualizarInterface()
    {
        try
        {
            var quantidade = _exerciciosSalvos.Count;

            // Atualiza o contador
            lbl_contador.Text = quantidade == 0 ? "Nenhum exercício salvo" :
                               quantidade == 1 ? "1 exercício salvo" :
                               $"{quantidade} exercícios salvos";

            // Mostra/esconde elementos baseado se tem exercícios ou não
            if (quantidade == 0)
            {
                stack_lista_vazia.IsVisible = true;
                listview_exercicios_salvos.IsVisible = false;
            }
            else
            {
                stack_lista_vazia.IsVisible = false;
                listview_exercicios_salvos.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Erro", $"Erro ao atualizar interface: {ex.Message}", "OK");
        }
    }

    // Evento quando clica no botão de remover exercício específico
    private async void OnRemoveExerciseClicked(object sender, EventArgs e)
    {
        try
        {
            // Pega o botão que foi clicado e o exercício associado
            if (sender is Button button && button.BindingContext is Exercise exercicio)
            {
                // Pergunta se realmente quer remover
                bool resposta = await DisplayAlert(
                    "Confirmar Remoção",
                    $"Deseja remover '{exercicio.Name}' da lista?",
                    "Sim", "Não");

                if (resposta)
                {
                    // Remove da lista
                    _exerciciosSalvos.Remove(exercicio);

                    // Atualiza a interface
                    AtualizarInterface();

                    // Mostra mensagem de sucesso
                    await DisplayAlert("Sucesso", "Exercício removido!", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro ao remover exercício: {ex.Message}", "OK");
        }
    }

    // Evento quando clica no botão "Voltar"
    private async void OnVoltarClicked(object sender, EventArgs e)
    {
        try
        {
            // Se houve mudanças na lista, pergunta se quer salvar
            if (_exerciciosSalvos.Count > 0)
            {
                bool salvar = await DisplayAlert(
                    "Salvar Alterações?",
                    "Suas modificações foram salvas automaticamente.",
                    "OK", "Cancelar");
            }

            // Volta para a tela anterior
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro ao voltar: {ex.Message}", "OK");
        }
    }

    // Evento quando clica no botão "Limpar Tudo"
    private async void OnLimparTudoClicked(object sender, EventArgs e)
    {
        try
        {
            if (_exerciciosSalvos.Count == 0)
            {
                await DisplayAlert("Atenção", "A lista já está vazia!", "OK");
                return;
            }

            // Pergunta se realmente quer limpar tudo
            bool resposta = await DisplayAlert(
                "Confirmar Limpeza",
                $"Deseja remover TODOS os {_exerciciosSalvos.Count} exercícios da lista?\n\nEsta ação não pode ser desfeita!",
                "Sim, Limpar Tudo", "Cancelar");

            if (resposta)
            {
                // Remove todos os exercícios
                _exerciciosSalvos.Clear();

                // Atualiza a interface
                AtualizarInterface();

                // Mostra mensagem de sucesso
                await DisplayAlert("Sucesso", "Lista limpa com sucesso! 🗑️", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro ao limpar lista: {ex.Message}", "OK");
        }
    }

    // Método público para adicionar exercícios (caso precise no futuro)
    public void AdicionarExercicio(Exercise exercicio)
    {
        try
        {
            if (exercicio != null && !_exerciciosSalvos.Any(x => x.Id == exercicio.Id))
            {
                _exerciciosSalvos.Add(exercicio);
                AtualizarInterface();
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Erro", $"Erro ao adicionar exercício: {ex.Message}", "OK");
        }
    }

    // Método público para obter a lista atual (caso precise no futuro)
    public List<Exercise> ObterExerciciosSalvos()
    {
        return _exerciciosSalvos.ToList();
    }
}