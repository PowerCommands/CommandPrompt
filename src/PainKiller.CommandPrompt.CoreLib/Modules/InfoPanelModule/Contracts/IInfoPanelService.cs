namespace PainKiller.CommandPrompt.CoreLib.Modules.InfoPanelModule.Contracts;
public interface IInfoPanelService
{
    void RegisterContent(IInfoPanel panel, bool autoUpdate);
    void Start();
    void Stop();
    void Update();
}