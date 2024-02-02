using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using DynamicData;
using EveSquadron.DataRepositories.Interfaces;
using EveSquadron.Models;
using EveSquadron.Models.Enums;
using EveSquadron.Models.Helper;
using EveSquadron.Models.Interfaces;
using EveSquadron.ViewModels.Interfaces;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace EveSquadron.ViewModels;

public class WhitelistManagementViewModel : ViewModelBase, IWhitelistManagementViewModel
{
    #region member fields

    private readonly IWhitelistManagementDataRepository _whitelistManagementDataRepository;
    private readonly IEqualityComparer<IWhitelistEntry> _whitelistEqualityComparer;
    private readonly IClipboardToWhitelistEntitiesParser<WhitelistEntry> _clipboardToWhitelistEntitiesParser;
    private readonly ILogger<IWhitelistManagementViewModel> _logger;
    private bool _isWindowVisible;

    #endregion

    #region constructor

    public WhitelistManagementViewModel(IWhitelistManagementDataRepository whitelistManagementDataRepository, IEqualityComparer<IWhitelistEntry> whitelistEqualityComparer,
        IClipboardToWhitelistEntitiesParser<WhitelistEntry> clipboardToWhitelistEntitiesParser, ILogger<IWhitelistManagementViewModel> logger)
    {
        _whitelistManagementDataRepository = whitelistManagementDataRepository;
        _whitelistEqualityComparer = whitelistEqualityComparer;
        _clipboardToWhitelistEntitiesParser = clipboardToWhitelistEntitiesParser;
        _logger = logger;

        CurrentWhitelistEntries = new ObservableCollection<IWhitelistEntry>();
        AvailableEntityTypes = Enum.GetValues<EntityTypeEnum>();

        SaveWhitelistCommand = ReactiveCommand.CreateFromTask(OnSaveWhitelistCommand);
        AddSingleItemWhitelistCommand = ReactiveCommand.Create(OnAddSingleItemWhitelistCommand);
        DeleteSelectedWhitelistCommand = ReactiveCommand.Create<IList>(OnDeleteSelectedWhitelistCommand);
        ImportClipboardWhitelistCommand = ReactiveCommand.Create(OnImportClipboardWhitelistCommand);
        
        LoadInitialWhitelistEntries();
    }
    
    private async Task OnImportClipboardWhitelistCommand()
    {
        var clipboardContent = await ApplicationHelper.GetMainWindow()!.Clipboard!.GetTextAsync();
        if (clipboardContent is null)
            return;
        
        var parsedEntities = _clipboardToWhitelistEntitiesParser.Parse(clipboardContent);
        CurrentWhitelistEntries.AddRange(parsedEntities);

        var uniqueEntries = CurrentWhitelistEntries.Distinct(_whitelistEqualityComparer).ToList();
        CurrentWhitelistEntries.Clear();
        CurrentWhitelistEntries.AddRange(uniqueEntries);
    }

    private async void LoadInitialWhitelistEntries()
    {
        CurrentWhitelistEntries.Clear();
        var whitelist = await _whitelistManagementDataRepository.LoadWhitelistedEntities();
        CurrentWhitelistEntries.AddRange(whitelist);
    }

    #endregion

    #region properties

    public ReactiveCommand<Unit, Unit> SaveWhitelistCommand { get; }

    public ReactiveCommand<Unit, Unit> AddSingleItemWhitelistCommand { get; }

    public ReactiveCommand<IList, Unit> DeleteSelectedWhitelistCommand { get; }

    public ReactiveCommand<Unit, Task> ImportClipboardWhitelistCommand { get; }

    public IEnumerable<EntityTypeEnum> AvailableEntityTypes { get; }

    public ObservableCollection<IWhitelistEntry> CurrentWhitelistEntries { get; }

    public bool IsWindowVisible {
        get => _isWindowVisible;
        set => SetProperty(ref _isWindowVisible, value);
    }

    #endregion

    #region helper methods

    private async Task OnSaveWhitelistCommand()
    {
        try //ReactiveCommand
        {
            var saveableEntities = CurrentWhitelistEntries
                .Where(x => !string.IsNullOrWhiteSpace(x.Name))
                .Select(x => new WhitelistEntry
                {
                    Name = x.Name.Trim(),
                    Type = x.Type
                })
                .Distinct(_whitelistEqualityComparer)
                .ToList();

            await _whitelistManagementDataRepository.SaveWhitelistedEntities(saveableEntities);
            CurrentWhitelistEntries.Clear();
            CurrentWhitelistEntries.AddRange(saveableEntities);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Could not save whitelist entities!");
            throw;
        }
    }

    private void OnAddSingleItemWhitelistCommand() => CurrentWhitelistEntries.Add(new WhitelistEntry());

    private void OnDeleteSelectedWhitelistCommand(IList selectedItems)
    {
        try //ReactiveCommand
        {
            var deleteableEntities = selectedItems.Cast<WhitelistEntry>();
            CurrentWhitelistEntries.RemoveMany(deleteableEntities);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Could not delete whitelist entities!");
            throw;
        }
    }

    #endregion
}