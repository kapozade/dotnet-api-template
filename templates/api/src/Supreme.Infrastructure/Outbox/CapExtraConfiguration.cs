using DotNetCore.CAP.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Supreme.Infrastructure.Outbox;

public class CapExtraConfiguration
{
    public static void FailedThresholdCallback(FailedInfo failedInfo)
    {
        var logger = failedInfo.ServiceProvider.GetService<ILogger<CapExtraConfiguration>>()
                     ?? NullLogger<CapExtraConfiguration>.Instance;

        logger.LogError(
            @"A message of type {Type} failed after executing several times, requiring manual troubleshooting. Message name: {MessageName}, message id: {MessageId}",
            failedInfo.MessageType, failedInfo.Message.GetName(), failedInfo.Message.GetId());
    }
}
