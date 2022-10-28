namespace TheMagic
{
    public class TestEvents
    {
        public event EventHandler DirectorySearched;

        public void DoAThing()
        {
            for (int i = 0; i < 5000; i++)
            {
                Thread.Sleep(1);
                DirectorySearched?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}