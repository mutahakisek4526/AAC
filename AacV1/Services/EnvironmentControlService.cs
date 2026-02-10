using System.Diagnostics;
using System.Net.Http;
using AacV1.Models;

namespace AacV1.Services;

public class EnvironmentControlService : IEnvironmentControlService
{
    private readonly IComputerControlService _computerControlService;
    private readonly HttpClient _httpClient;

    public EnvironmentControlService(IComputerControlService computerControlService)
    {
        _computerControlService = computerControlService;
        _httpClient = new HttpClient();
    }

    public async Task ExecuteAsync(AacEnvironmentAction environmentAction)
    {
        try
        {
            if (environmentAction.ActionType.Equals("Keyboard", StringComparison.OrdinalIgnoreCase))
            {
                if (Enum.TryParse<AacKeyCode>(environmentAction.Argument, true, out var keyCode))
                {
                    _computerControlService.SendKey(keyCode);
                }

                return;
            }

            if (environmentAction.ActionType.Equals("Http", StringComparison.OrdinalIgnoreCase))
            {
                if (Uri.TryCreate(environmentAction.Argument, UriKind.Absolute, out var uri))
                {
                    await _httpClient.GetAsync(uri);
                }

                return;
            }

            if (environmentAction.ActionType.Equals("Exe", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrWhiteSpace(environmentAction.Argument))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = environmentAction.Argument,
                        UseShellExecute = true
                    });
                }
            }
        }
        catch
        {
        }
    }
}
