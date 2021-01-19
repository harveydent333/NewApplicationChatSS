using System;

namespace NewAppChatSS.Common.CommonHelpers
{
    public static class GuidHelper
    {
        public static string GetNewGuid()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
