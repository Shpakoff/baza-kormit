public sealed class ModalEventBus
{
    public delegate void ShowHandler(string message);
    public event ShowHandler NotifyShow;
    
    private static readonly ModalEventBus instance = new ModalEventBus();
    // Explicit static constructor to tell C# compiler
    // not to mark type as beforefieldinit
    static ModalEventBus()
    {
    }
    private ModalEventBus() { }
    public static ModalEventBus Instance
    {
        get { return instance; }
    }

    public void ShowNotification(string message)
    {
        NotifyShow?.Invoke(message);
    }

}
