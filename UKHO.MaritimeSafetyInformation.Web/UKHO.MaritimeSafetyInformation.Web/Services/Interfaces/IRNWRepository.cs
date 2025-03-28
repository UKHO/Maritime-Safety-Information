﻿using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;

namespace UKHO.MaritimeSafetyInformation.Web.Services.Interfaces
{
    public interface IRNWRepository
    {
        Task AddRadioNavigationWarning(RadioNavigationalWarning radioNavigationalWarning);
        Task<List<RadioNavigationalWarningsAdmin>> GetRadioNavigationWarningsAdminList();        
        Task<List<WarningType>> GetWarningTypes();
        Task<List<RadioNavigationalWarningsData>> GetRadioNavigationalWarningsDataList();
        Task<List<string>> GetYears();
        Task<DateTime> GetRadioNavigationalWarningsLastModifiedDateTime();
        RadioNavigationalWarning GetRadioNavigationalWarningById(int id);
        Task UpdateRadioNavigationalWarning(RadioNavigationalWarning radioNavigationalWarning);
        Task<List<RadioNavigationalWarningsData>> GetSelectedRadioNavigationalWarningsDataList(int[] selectedIds);
        Task<bool> CheckReferenceNumberExistOrNot(int warningType, string referenceNumber);
    }
}
