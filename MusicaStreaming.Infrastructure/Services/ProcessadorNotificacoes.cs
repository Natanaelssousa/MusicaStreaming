using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MusicaStreaming.Application.Abstractions;

namespace MusicaStreaming.Infrastructure.Services
{
    public class ProcessadorNotificacoes : BackgroundService
    {
        private static readonly TimeSpan Intervalo = TimeSpan.FromSeconds(10);

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ProcessadorNotificacoes> _logger;

        public ProcessadorNotificacoes(
            IServiceProvider serviceProvider,
            ILogger<ProcessadorNotificacoes> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessarPendentes(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar notificacoes pendentes");
                }

                await Task.Delay(Intervalo, stoppingToken);
            }
        }

        private async Task ProcessarPendentes(CancellationToken stoppingToken)
        {
            // BackgroundService e singleton; repositorio e scoped.
            // Por isso criamos um scope manual a cada ciclo.
            using var scope = _serviceProvider.CreateScope();
            var repositorio = scope.ServiceProvider.GetRequiredService<IRepositorioNotificacao>();

            var pendentes = await repositorio.ObterPendentesAsync();

            foreach (var notificacao in pendentes)
            {
                if (stoppingToken.IsCancellationRequested)
                    break;

                // "Envio" simulado: logamos. Em producao, aqui iria o email/SMS.
                _logger.LogInformation(
                    "Notificacao enviada para {Destinatario} ({Tipo}): {Conteudo}",
                    notificacao.Destinatario,
                    notificacao.Tipo,
                    notificacao.Conteudo);

                notificacao.MarcarComoEnviada();
                await repositorio.AtualizarAsync(notificacao);
            }

            await repositorio.SalvarAsync();
        }
    }
}
