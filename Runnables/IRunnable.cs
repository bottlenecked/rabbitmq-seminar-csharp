namespace RabbitmqSeminar.Runnables
{
    public interface IRunnable
    {
        string Announcement { get; }
        void Run();
    }
}
