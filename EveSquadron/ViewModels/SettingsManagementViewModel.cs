using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia;
using DynamicData;
using EveSquadron.DataRepositories.Interfaces;
using EveSquadron.Models;
using EveSquadron.Models.EveSquadron;
using EveSquadron.Models.Interfaces;
using EveSquadron.ViewModels.Interfaces;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace EveSquadron.ViewModels;

public class SettingsManagementViewModel : ViewModelBase, ISettingsManagementViewModel
{
    #region member fields
    
    private readonly ILogger<IWhitelistManagementViewModel> _logger;
    
    #endregion

    #region constructor

    public SettingsManagementViewModel(ILogger<IWhitelistManagementViewModel> logger)
    {
        _logger = logger;
    }

    #endregion

    #region properties

    #endregion

    #region helper methods

    #endregion
}