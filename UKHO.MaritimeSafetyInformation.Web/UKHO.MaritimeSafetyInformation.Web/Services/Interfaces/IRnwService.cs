﻿using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;

namespace UKHO.MaritimeSafetyInformation.Web.Services.Interfaces
{
    public interface IRnwService
    {
        Task<RadioNavigationalWarningsAdminListFilter> GetRadioNavigationWarningsForAdmin(int pageIndex, int warningType, string year, bool reLoadData, string correlationId);
    }
}