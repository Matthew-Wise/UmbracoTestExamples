using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Sync;
using Umbraco.Cms.Core;
using Umbraco.Cms.Infrastructure.Examine;

namespace TestExamples.IntegrationTests.Application.Examine
{
    public sealed class CustomRebuildOnStartupHandler
    : INotificationHandler<UmbracoRequestBeginNotification>
    {
        // The static private fields have been removed
        //    and replaced with the _state field,
        //    which is injected in the constructor
        private readonly ExamineIndexRebuilder _backgroundIndexRebuilder;
        private readonly IRuntimeState _runtimeState;
        private readonly CustomRebuildOnStartupHandlerState _state;
        private readonly ISyncBootStateAccessor _syncBootStateAccessor;

        public CustomRebuildOnStartupHandler(
            ISyncBootStateAccessor syncBootStateAccessor,
            ExamineIndexRebuilder backgroundIndexRebuilder,
            IRuntimeState runtimeState,
            CustomRebuildOnStartupHandlerState state)
        {
            _syncBootStateAccessor = syncBootStateAccessor;
            _backgroundIndexRebuilder = backgroundIndexRebuilder;
            _runtimeState = runtimeState;
            _state = state;
        }

        // This method should be a copy of the method in the original handler,
        //    except using the singleton state object instead of static fields
        public void Handle(UmbracoRequestBeginNotification notification)
        {
            if (_runtimeState.Level != RuntimeLevel.Run)
            {
                return;
            }

            // This method call now uses the fields in the state object,
            //    rather than the static fields
            LazyInitializer.EnsureInitialized(
                ref _state._isReady,
                ref _state._isReadSet,
                ref _state._isReadyLock,
                () =>
                {
                    SyncBootState bootState = _syncBootStateAccessor.GetSyncBootState();

                    // This method now passes TimeSpan.Zero to this method, so that we don't have to wait that 1 minute at the start.
                    _backgroundIndexRebuilder.RebuildIndexes(
                        bootState != SyncBootState.ColdBoot,
                        TimeSpan.Zero);

                    return true;
                });
        }
    }
}
