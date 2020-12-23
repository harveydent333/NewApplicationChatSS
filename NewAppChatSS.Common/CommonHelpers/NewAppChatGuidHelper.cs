using System;

namespace NewAppChatSS.Common.CommonHelpers
{
    public static class NewAppChatGuidHelper
    {
        public static string GetNewGuid()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
