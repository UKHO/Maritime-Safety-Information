﻿using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;

namespace UKHO.MaritimeSafetyInformation.Web.Services.Interfaces
{
    public interface IRNWService
    {
        Task<bool> CreateNewRadioNavigationWarningsRecord(RadioNavigationalWarning radioNavigationalWarning, string correlationId, bool skipDuplicateReferenceCheck, string userName);
        Task<List<WarningType>> GetWarningTypes();
        Task<RadioNavigationalWarningsAdminFilter> GetRadioNavigationWarningsForAdmin(int pageIndex, int? warningType, int? year, string correlationId);
        Task<List<RadioNavigationalWarningsData>> GetRadioNavigationalWarningsData(string correlationId);
        Task<string> GetRadioNavigationalWarningsLastModifiedDateTime(string correlationId);
        RadioNavigationalWarning GetRadioNavigationalWarningById(int id, string correlationId);
        Task<bool> EditRadioNavigationalWarningsRecord(RadioNavigationalWarning radioNavigationalWarning, string correlationId);
        Task<List<RadioNavigationalWarningsData>> GetSelectedRadioNavigationalWarningsData(int[] selectedIds, string correlationId);
    }
}
