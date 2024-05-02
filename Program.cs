using System.Security.Cryptography;

namespace TeleprompterConsole;

public class Program
{
    public static async Task Main(string[] args)
    {
        await RunTeleprompter();
    }

    public static string[] giveSecretData (int randomNumber)
    {
        var rando = new Random();

        if (randomNumber > rando.Next(1, 10))
        {
            var creditCardNumber = "4000056655665556";
            var expDate = "0129";
            var cvc2 = "123";
            var billingAddress = "123 Easy St";
            var billingZip = "12345";

            return [creditCardNumber, cvc2, billingAddress, billingZip, expDate ];
        }

        return ["You", "picked", "wrong" ];
    }

    public static byte[] encryptString()
    {
        SymmetricAlgorithm serviceProvider = new DESCryptoServiceProvider();
        byte[] key = { 16, 22, 240, 11, 18, 150, 192, 21 };
        serviceProvider.Key = key;
        ICryptoTransform encryptor = serviceProvider.CreateEncryptor();

        String message = "Hello World";
        byte[] messageB = System.Text.Encoding.ASCII.GetBytes(message);
        return encryptor.TransformFinalBlock(messageB, 0, messageB.Length);
    }

    private static async Task RunTeleprompter()
    {
        var config = new TelePrompterConfig();
        var displayTask = ShowTeleprompter(config);

        var speedTask = GetInput(config);
        await Task.WhenAny(displayTask, speedTask);
    }

    private static async Task ShowTeleprompter(TelePrompterConfig config)
    {
        var words = ReadFrom("sampleQuotes.txt");
        foreach (var word in words)
        {
            Console.Write(word);
            if (!string.IsNullOrWhiteSpace(word))
            {
                await Task.Delay(config.DelayInMilliseconds);
            }
        }
        config.SetDone();
    }

    private static async Task GetInput(TelePrompterConfig config)
    {
        Action work = () =>
        {
            do
            {
                var key = Console.ReadKey(true);
                if (key.KeyChar == '>')
                    config.UpdateDelay(-10);
                else if (key.KeyChar == '<')
                    config.UpdateDelay(10);
                else if (key.KeyChar == 'X' || key.KeyChar == 'x')
                    config.SetDone();
            } while (!config.Done);
        };
        await Task.Run(work);
    }
    static IEnumerable<string> ReadFrom(string file)
    {
        string? line;
        using (var reader = File.OpenText(file))
        {
            while ((line = reader.ReadLine()) != null)
            {
                var words = line.Split(' ');
                var lineLength = 0;
                foreach (var word in words)
                {
                    yield return word + " ";
                    lineLength += word.Length + 1;
                    if (lineLength > 70)
                    {
                        yield return Environment.NewLine;
                        lineLength = 0;
                    }
                }
                yield return Environment.NewLine;
            }
        }
    }
}
