using System.Collections.Generic;
using System.Threading.Tasks;
using StudioX.Logging;
using StudioX.Threading;
#if NET46
using System.Configuration;
#endif

namespace StudioX.Configuration
{
    /// <summary>
    ///     Implements default behavior for ISettingStore.
    ///     Only <see cref="GetSettingOrNullAsync" /> method is implemented and it gets setting's value
    ///     from application's configuration file if exists, or returns null if not.
    /// </summary>
    public class DefaultConfigSettingStore : ISettingStore
    {
        /// <summary>
        ///     Gets singleton instance.
        /// </summary>
        public static DefaultConfigSettingStore Instance { get; } = new DefaultConfigSettingStore();

        private DefaultConfigSettingStore()
        {
        }

        public Task<SettingInfo> GetSettingOrNullAsync(int? tenantId, long? userId, string name)
        {
#if NET46
            var value = ConfigurationManager.AppSettings[name];

            if (value == null)
            {
                return Task.FromResult<SettingInfo>(null);
            }

            return Task.FromResult(new SettingInfo(tenantId, userId, name, value));
#else
            return Task.FromResult<SettingInfo>(null);
#endif
        }

        /// <inheritdoc />
        public Task DeleteAsync(SettingInfo setting)
        {
            LogHelper.Logger.Warn(
                "ISettingStore is not implemented, using DefaultConfigSettingStore which does not support DeleteAsync.");
            return StudioXTaskCache.CompletedTask;
        }

        /// <inheritdoc />
        public Task CreateAsync(SettingInfo setting)
        {
            LogHelper.Logger.Warn(
                "ISettingStore is not implemented, using DefaultConfigSettingStore which does not support CreateAsync.");
            return StudioXTaskCache.CompletedTask;
        }

        /// <inheritdoc />
        public Task UpdateAsync(SettingInfo setting)
        {
            LogHelper.Logger.Warn(
                "ISettingStore is not implemented, using DefaultConfigSettingStore which does not support UpdateAsync.");
            return StudioXTaskCache.CompletedTask;
        }

        /// <inheritdoc />
        public Task<List<SettingInfo>> GetAllListAsync(int? tenantId, long? userId)
        {
            LogHelper.Logger.Warn(
                "ISettingStore is not implemented, using DefaultConfigSettingStore which does not support GetAllListAsync.");
            return Task.FromResult(new List<SettingInfo>());
        }
    }
}