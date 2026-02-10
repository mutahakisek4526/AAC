using System.Collections.ObjectModel;
using AacV1.Infrastructure;
using AacV1.Models;
using AacV1.Services;
using AacV1.ViewModels.Interfaces;

namespace AacV1.ViewModels;

public class HistoryViewModel : ObservableObject, INavigationAware, IInputTarget
{
    private readonly IStorageService _storageService;

    public HistoryViewModel(IStorageService storageService)
    {
        _storageService = storageService;
        HistoryItems = new ObservableCollection<AacHistoryItem>();
        ClearHistoryCommand = new RelayCommand(ClearHistory);
    }

    public ObservableCollection<AacHistoryItem> HistoryItems { get; }
    public RelayCommand ClearHistoryCommand { get; }

    public void OnEnter()
    {
        HistoryItems.Clear();
        foreach (var item in _storageService.LoadHistory().OrderByDescending(h => h.CreatedAt))
        {
            HistoryItems.Add(item);
        }
    }

    public void OnExit()
    {
    }

    public void OnSelect()
    {
    }

    public void OnBack()
    {
        ClearHistory();
    }

    private void ClearHistory()
    {
        HistoryItems.Clear();
        _storageService.SaveHistory(new List<AacHistoryItem>());
    }
}
