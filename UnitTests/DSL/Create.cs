namespace UnitTests.DSL;

public class Create
{
    public static KeyStoreBuilder Store()
    {
        return new KeyStoreBuilder();
    }

    public static TcpServerBuilder TcpServer()
    {
        return new TcpServerBuilder();
    }

    public static StoreReadersBuilder Readers()
    {
        return new StoreReadersBuilder();
    }
}