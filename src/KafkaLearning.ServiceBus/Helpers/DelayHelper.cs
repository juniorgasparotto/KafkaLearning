using System;
using System.Threading;

namespace KafkaLearning.ServiceBus.Helpers
{
    internal static class DelayHelper
    {
        public static void DelayMessage(long messageTimeMs, int delay, CancellationToken cancellationToken)
        {
            /* 
            Deve ler a mensagem após 10 segundos da sua produção. Exemplo

            -> Delay: 10 segundos
            -> Produzido: 1s 2s 3s 4s 5s 6s 7s 8s 9s 10s 11s 12s
            -> Consumido:           ^
            -> Após 10s :                                     ^

            1) Obtem a diferença do tempo de consumo menos o tempo de produção
                (4s - 2s) = 2 (o tempo de consumo será sempre maior, pois é o tempo atual)
            2) Descobre o tempo faltante para deixar a thead dormindo
                (10s - (4s - 2s)) = 8s (substrai o tempo do delay com o tempo gasto da msg para chegar até chegar aqui)
            */
            if (delay > 0)
            {
                var currentUnixTs = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                var sleep = (int)(delay - (currentUnixTs - messageTimeMs));
                if (sleep > 0)
                {
                    // Caso utilize cancelation token, utilize o WaitOne.
                    // garanta que saia do método após o cancelamento.
                    if (cancellationToken == default)
                        Thread.Sleep(sleep);
                    else if (cancellationToken.WaitHandle.WaitOne(sleep))
                        return;
                }
            }
        }
    }
}
