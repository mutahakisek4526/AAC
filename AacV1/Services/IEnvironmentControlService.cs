using AacV1.Models;

namespace AacV1.Services;

public interface IEnvironmentControlService
{
    Task ExecuteAsync(AacEnvironmentAction environmentAction);
}
