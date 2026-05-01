using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace XMachine.Api.Hubs;

/// <summary>
/// Real-time monitoring hub; server pushes via <see cref="IXMachineHubClient"/> only.
/// </summary>
[Authorize]
public sealed class XMachineHub : Hub<IXMachineHubClient>
{
}
